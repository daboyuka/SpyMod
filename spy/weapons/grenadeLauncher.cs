SoundData SoundFireGrenadeLauncher {
   wavFileName = "Grenade.wav";
   profile = Profile3dNear;
};
SoundData SoundGrenadeShellExp {
   wavFileName = "BXplo1.wav";
   profile = Profile3dFar;
};

ExplosionData GrenadeShellExp {
   shapeName = "fiery.dts";
   soundId   = SoundGrenadeShellExp;

   faceCamera = true;
   randomSpin = true;
   hasLight   = true;
   lightRange = 10.0;

   timeScale = 2;//1.5;

   timeZero = 0.150;
   timeOne  = 0.500;

   colors[0]  = { 0.0, 0.0,  0.0 };
   colors[1]  = { 1.0, 0.63, 0.0 };
   colors[2]  = { 1.0, 0.63, 0.0 };
   radFactors = { 0.0, 1.0, 0.9 };
};

GrenadeData GrenadeLauncherShell {
   bulletShapeName    = "grenade.dts";
   explosionTag       = GrenadeShellExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.01;

   damageClass        = 1;       // 0 impact, 1, radius
   damageValue        = 140;
   damageType         = $ShrapnelDamageType;

   explosionRadius    = 10;
   kickBackStrength   = 150.0;
   maxLevelFlightDist = 50;
   totalTime          = 30.0;    // special meaning for grenades...
   liveTime           = 0;//1.0;
   projSpecialTime    = 0.05;

   inheritedVelocityScale = 0.5;

   smokeName              = "breath.dts";
};



ItemData GrenadeLauncherAmmo {
	description = "Grenade Ammo";
	heading = $InvHeading[Ammo];
	className = "Ammo";
	shapeFile = "plasammo";
	shadowDetailMask = 4;
	price = 2;
};

ItemImageData GrenadeLauncherImage {
	shapeFile = "grenadeL";
	mountPoint = 0;

	weaponType = 0;
	reloadTime = 0; 
	fireTime = 1.5;

	ammoType = LoadedAmmo;//MagnumAmmo;

	accuFire = false;

	sfxFire = SoundFireGrenadeLauncher;
	sfxActivate = SoundPickUpWeapon;
};

ItemData GrenadeLauncher {
	description = "Grenade Launcher";
	className = "Weapon";
	shapeFile = "grenadeL";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = GrenadeLauncherImage;
	price = 125;
	showWeaponBar = false;
};

ItemImageData GrenadeLauncherDisplayImage {
	shapeFile = "breath";
	firstPerson = false;
	ammoType = GrenadeLauncherAmmo;
};

ItemData GrenadeLauncherDisplay {
	description = "";
	className = "Tool";
	shapeFile = "breath";
	hudIcon = "ammopack";
	shadowDetailMask = 4;
	imageType = GrenadeLauncherDisplayImage;
	showWeaponBar = true;
	showInventory = false;
};

$ClipSize[GrenadeLauncher] = 1;
$AmmoDisplay[GrenadeLauncher] = GrenadeLauncherDisplay;
$ItemDescription[GrenadeLauncher] = "<jc><f1>Grenade Launcher:<f0> Lobs explosive grenades at enemies";

function GrenadeLauncherImage::onFire(%player, %slot) {
  if (Player::getItemCount(%player, LoadedAmmo) == 0) return;
  Weapon::fireProjectile(%player, GrenadeLauncherShell);
  Player::decItemCount(%player, LoadedAmmo);
}

function GrenadeLauncher::onMount(%player, %slot) {
  Weapon::displayDescription(%player, GrenadeLauncher);
  Weapon::standardMount(%player, GrenadeLauncher);
}
function GrenadeLauncher::onUnmount(%player, %slot) {
  Weapon::standardUnmount(%player, GrenadeLauncher);
}

function GrenadeLauncher::onNoAmmo(%player) {
  GrenadeLauncher::reload(%player);
}

function GrenadeLauncher::reload(%player) {
  Weapon::standardReload(%player, GrenadeLauncher, "", 3);
}

registerWeapon(GrenadeLauncher, GrenadeLauncherAmmo, 1);
linkWeapon(GrenadeLauncher);
setArmorAllowsItem(larmor, GrenadeLauncher, 4);
setArmorAllowsItem(lfemale, GrenadeLauncher, 4);
setArmorAllowsItem(SpyMale, GrenadeLauncher, 4);
setArmorAllowsItem(SpyFemale, GrenadeLauncher, 4);
setArmorAllowsItem(DMMale, GrenadeLauncher, 4);
setArmorAllowsItem(DMFemale, GrenadeLauncher, 4);

// All weapons have LoadedAmmo as their ammo type, but is not shown on the weapon list
// All weapons have an accompanying item which has the weapon's normal ammo as its ammo type
// So the weapon relies on LoadedAmmo, but the weapons real ammo type is displayed on the GUI