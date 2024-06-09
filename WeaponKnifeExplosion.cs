using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace WeaponKnifeExplosion;

public class Settings : BasePluginConfig
{
    [JsonPropertyName("TimerValue")] public float TimerValue { get; set; } = 3;
    [JsonPropertyName("OnlyKnifeExplosion")] public bool KnifeEnable { get; set; } = true;
}

public class WeaponKnifeExplosion : BasePlugin, IPluginConfig<Settings>
{
    public override string ModuleName => "[ANY] Weapon/Knife Explosion";
    public override string ModuleVersion => "1.0.1";
    public override string ModuleAuthor => "Feykich";

    public Settings Config { get; set; } = null!;

    public void OnConfigParsed(Settings config)
    {
        if(config.TimerValue > 300)
        {
            config.TimerValue = 300;
        }
        Config = config;
    }

    public override void Load(bool hotReload)
    {    
        Console.WriteLine("[Weapon/Knife Explosion] Plugin is Loaded!");
  
        RegisterEventHandler<EventPlayerHurt>(OnEventPlayerHurt);
    }

    private HookResult OnEventPlayerHurt(EventPlayerHurt @event, GameEventInfo _)
    {
        var client =    @event.Userid;
        var attacker =  @event.Attacker;
        var weapon =    @event.Attacker.PlayerPawn.Value!.WeaponServices!.ActiveWeapon.Value!.DesignerName;

        if(!client.IsValid || !attacker.IsValid) 
        {
            Console.WriteLine("[Weapon/Knife Explosion] Attacker/Victim is not found.");
            return 0;
        }

        client.PrintToChat($"[Weapon/Knife Explosion] You're gonna explode in {Config.TimerValue} second(s)!");

        AddTimer(Config.TimerValue, () =>
        {
            OnKillPlayer(client, weapon);
        });
        return HookResult.Continue;
    }

    private void OnKillPlayer(CCSPlayerController? client, string weapon)
    {
        CParticleSystem? entity = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system");

        var pOrigin =   client!.PlayerPawn.Value!.AbsOrigin!;
        var vAngle =    client!.PlayerPawn.Value!.AbsRotation!;
        var sVec =      client!.PlayerPawn.Value!.AbsVelocity!;

        if(!client.IsValid)
        {
            Console.WriteLine("[Weapon/Knife Explosion] Client not found.");
            return;
        }
        
        if(entity != null)
        {      
            entity.EffectName = "particles/explosions_fx/explosion_basic.vpcf"; // HE Grenade particle
            entity.DispatchSpawn();
            entity.Teleport(pOrigin, vAngle, sVec);
            entity.AcceptInput("Start");
            AddTimer(0.2f, () =>
            {
                entity.AcceptInput("Kill");
            });
        }
        if(Config.KnifeEnable == true && weapon == "weapon_knife" && client.PawnIsAlive)
        {
            client.CommitSuicide(true, true);
            return;
        }
        if(client.PawnIsAlive)
        {
            client.CommitSuicide(true, true); // Allows weapons detonate a player by hit. If boolean "KnifeEnable" is false
        }
        return;
    }
}
