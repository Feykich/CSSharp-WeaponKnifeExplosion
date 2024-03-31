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
    public override string ModuleVersion => "1.0.0";

    public Settings Config { get; set; } = null!;

    public void OnConfigParsed(Settings config)
    {
        if(config.TimerValue > 300)
        {
            config.TimerValue = 300;
        }
        Config = config;
    }

    //private readonly Dictionary<CCSPlayerController, bool> bParticleCheck = [];

    public override void Load(bool hotReload)
    {    
        Console.WriteLine("[Weapon/Knife Explosion] Plugin is Loaded!");

        /*
        RegisterEventHandler<EventPlayerSpawn>((@event, info) => 
        {
            bParticleCheck[@event.Userid] = false;
            return HookResult.Continue;
        });
        */
  
        RegisterEventHandler<EventPlayerHurt>(OnEventPlayerHurt);
    }

    private HookResult OnEventPlayerHurt(EventPlayerHurt @event, GameEventInfo _)
    {
        var client = @event.Userid;
        var attacker = @event.Attacker;
        var weapon = @event.Attacker.PlayerPawn.Value!.WeaponServices!.ActiveWeapon.Value!.DesignerName;

        if(!client.IsValid || !attacker.IsValid) 
        {
            return 0;
        }

        client.PrintToChat($"[Weapon/Knife Explosion] You're gonna explode in {Config.TimerValue} seconds!");
        AddTimer(Config.TimerValue, () =>
        {
            if(Config.KnifeEnable == true && weapon == "weapon_knife" && client.PawnIsAlive)
            {
                OnKillPlayer(client);
                OnParticleExplosion(client);
            }
            if(Config.KnifeEnable == false && client.PawnIsAlive)
            {
                OnKillPlayer(client);
                OnParticleExplosion(client);
            } 
        });
        return HookResult.Continue;
    }

    private static void OnKillPlayer(CCSPlayerController? client)
    {
        if(client == null)
        {
            Console.WriteLine("[Knife Explosion] Null");
            return;
        }
        client.CommitSuicide(true, true);
    }

    private void OnParticleExplosion(CCSPlayerController? client)
    {
        CParticleSystem? entity = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system");

        var pOrigin =   client!.PlayerPawn.Value!.AbsOrigin!;
        var vAngle =    client!.PlayerPawn.Value!.AbsRotation!;
        var sVec =      client!.PlayerPawn.Value!.AbsVelocity!;

        if(entity != null)
        {      
            entity.EffectName = "particles/explosions_fx/explosion_basic.vpcf";
            entity.DispatchSpawn();
            entity.Teleport(pOrigin, vAngle, sVec);
            entity.AcceptInput("Start");
            AddTimer(0.2f, () =>
            {
                entity.AcceptInput("Kill");
            });
        }
    }
}