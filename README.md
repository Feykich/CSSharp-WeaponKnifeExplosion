### [Weapon/Knife Explosion]
A simple plugin causes the player to explode (suicide + particle) by hit from weapons or knifes. Like a detonator.

#### Requirements
* Install <a href="https://github.com/roflmuffin/CounterStrikeSharp" target="_blank">[CounterStrikeSharp]</a> the lastest version.

#### Installion
* Path where to install the plugin: game/csgo/addons/counterstrikesharp/plugins/ drag and drop with "WeaponKnifeExplosion" folder.

#### Settings
Path to the settings: game/csgo/addons/counterstrikesharp/configs/plugins/WeaponKnifeExplosion/ There will be JSon file.

##### Start the plugin firstly to generate it.

  	{
		"TimerValue": 3, // Countdown timer to detonate a player. Value in seconds, max 300 seconds.
  		"OnlyKnifeExplosion": true, // true - only use knife to detonate a player. false - allow by using guns to detonate a player
		"ConfigVersion": 1
  	}
