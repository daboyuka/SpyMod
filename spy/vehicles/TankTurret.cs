SoundData SoundFireTankTurret {
   wavFileName = "BXplo2.wav";
   profile = Profile3dFar;
};
SoundData SoundTankTurretShellExp {
   wavFileName = "shockExp.wav";
   profile = Profile3dFar;
};

ExplosionData TankTurretShellExp {
   shapeName = "mortarEx.dts";
   soundId   = SoundTankTurretShellExp;

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

GrenadeData TankTurretShell {
   bulletShapeName    = "mortar.dts";
   explosionTag       = TankTurretShellExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.01;

   damageClass        = 1;       // 0 impact, 1, radius
   damageValue        = 200;
   damageType         = $ShrapnelDamageType;

   explosionRadius    = 20;
   kickBackStrength   = 400.0;
   maxLevelFlightDist = 400;
   totalTime          = 30.0;    // special meaning for grenades...
   liveTime           = 0;//1.0;
   projSpecialTime    = 0.05;

   inheritedVelocityScale = 0.5;

   smokeName              = "breath.dts";
};

ItemImageData TankTurretImage2 {
	shapeFile = "gunturet";
	mountPoint = 4;
	mountOffset = "0 0.7 0";
	firstPerson = false;
};

ItemData TankTurret2 {
	shapeFile = "breath";
	imageType = TankTurretImage2;
};

ItemImageData TankTurretImage {
	shapeFile = "breath";
	mountPoint = 0;
	mountOffset = "-0.5 0.5 1.3";

	weaponType = 0;
	reloadTime = 1; 
	fireTime = 0;

	maxEnergy = 0;
	minEnergy = 0;

	accuFire = true;

	firstPerson = false;

	sfxFire = SoundFireTankTurret;
	sfxActivate = SoundPlasmaTurretOn;
};

ItemData TankTurret {
	description = "Tank Cannon";
	className = "Weapon";
	shapeFile = "mortar";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = TankTurretImage;
	price = 125;
	showWeaponBar = false;
};

$ItemDescription[TankTurret] = "<jc><f1>Tank Cannon:<f0> Fires large explosive shells";

function TankTurretImage::onFire(%player, %slot) {
  Weapon::fireProjectile(%player, TankTurretShell);
}

function TankTurret::onMount(%player, %slot) {
  Weapon::displayDescription(%player, TankTurret);
  Player::mountItem(%player, TankTurret2, 3);
  Weapon::standardMount(%player, TankTurret);
}
function TankTurret::onUnmount(%player, %slot) {
  Player::unmountItem(%player, 3);
  Weapon::standardUnmount(%player, TankTurret);
}

setArmorAllowsItem(larmor, TankTurret, "", 0);
setArmorAllowsItem(lfemale, TankTurret, "", 0);
setArmorAllowsItem(SpyMale, TankTurret, "", 0);
setArmorAllowsItem(SpyFemale, TankTurret, "", 0);
setArmorAllowsItem(DMMale, TankTurret, "", 0);
setArmorAllowsItem(DMFemale, TankTurret, "", 0);
