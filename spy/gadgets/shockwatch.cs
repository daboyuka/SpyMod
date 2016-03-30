SoundData SoundShockWatchFire
{
	wavFileName = "elf_fire.wav";
	profile = Profile3dMediumLoop;
};

SoundData SoundShockWatchIdle
{
	wavFileName = "lightning_idle.wav";
	profile = Profile3dNearLoop;
};

ItemImageData ShockWatchImage
{
	shapeFile = "grenade";
   mountPoint = 0;

   weaponType = 0;  // Sustained
   projectileType = watchCharge;
   reloadTime = 2;
   minEnergy = 3;
   maxEnergy = 11;
                        
   lightType = 3;  // Weapon Fire
   lightRadius = 2;
   lightTime = 1;
   lightColor = { 0.25, 0.25, 0.85 };

   sfxActivate = SoundPickUpWeapon;
   sfxFire     = SoundShockWatchIdle;
};

ItemData ShockWatch
{
   description = "Shock Watch";
	shapeFile = "shotgunbolt";
	hudIcon = "energyRifle";
   className = "Weapon";
   heading = "bWeapons";
   shadowDetailMask = 4;
   imageType = ShockWatchImage;
	showWeaponBar = true;
   price = 200;
   validateShape = true;
};

LightningData watchCharge
{
   bitmapName       = "lightningNew.bmp";

   damageType       = $ElectricityDamageType;
   boltLength       = 5.0;
   coneAngle        = 10.0;
   damagePerSec      = 0.4;
   energyDrainPerSec = 0.0;
   segmentDivisions = 1;
   numSegments      = 1;
   beamWidth        = 0.025;//075;

   updateTime   = 120;
   skipPercent  = 0.5;
   displaceBias = 0.15;

   lightRange = 3.0;
   lightColor = { 0.25, 0.25, 0.85 };

   soundId = SoundShockWatchFire;
};

ItemData WatchAmmo
{
	description = "Watch Charge";
	className = "Ammo";
	shapeFile = "ammo1";
   heading = "xAmmunition";
	shadowDetailMask = 4;
	price = 1;
};

registerWeapon(ShockWatch);
linkWeapon(ShockWatch);
setArmorAllowsItem(larmor, ShockWatch, WatchAmmo, 5);
setArmorAllowsItem(lfemale, ShockWatch, WatchAmmo, 5);
setArmorAllowsItem(SpyMale, ShockWatch, WatchAmmo, 5);
setArmorAllowsItem(SpyFemale, ShockWatch, WatchAmmo, 5);
setArmorAllowsItem(DMMale, ShockWatch, WatchAmmo, 5);
setArmorAllowsItem(DMFemale, ShockWatch, WatchAmmo, 5);