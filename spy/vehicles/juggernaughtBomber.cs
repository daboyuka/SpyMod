SoundData SoundJuggernaughtBomberFire {
   wavFileName = "BXplo2.wav";
   profile = Profile3dMedium;
};

SoundData SoundJuggernaughtBomberShellExp {
   wavFileName = "shockexp.wav";
   profile = Profile3dLudicrouslyFar;
};

ExplosionData JuggernaughtBomberShellExp {
   shapeName = "mortarex.dts";
   soundId   = SoundJuggernaughtBomberShellExp;

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

GrenadeData JuggernaughtBomberShell {
   bulletShapeName    = "mortar.dts";
   explosionTag       = JuggernaughtBomberShellExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.01;

   damageClass        = 1;       // 0 impact, 1, radius
   damageValue        = 80;
   damageType         = $ShrapnelDamageType;

   explosionRadius    = 25;
   kickBackStrength   = 250.0;
   maxLevelFlightDist = 100;
   totalTime          = 30.0;    // special meaning for grenades...
   liveTime           = 0;//1.0;
   projSpecialTime    = 0.05;

   inheritedVelocityScale = 0.5;

   smokeName              = "breath.dts";
};

ItemImageData JuggernaughtBomberImage2 {
	shapeFile = "mortargun";
	mountPoint = 0;
	mountOffset = "-0.55 -0.2 -0.1";
	mountRotation = "0 -1.57 0";
	weaponType = 0;
	reloadTime = 1.5; 
	fireTime = 0;
};

ItemData JuggernaughtBomber2 {
	shapeFile = "mortargun";
	imageType = JuggernaughtBomberImage2;
};

ItemImageData JuggernaughtBomberImage {
	shapeFile = "mortargun";
	mountPoint = 0;
	mountOffset = "-0.35 -0.2 -0.1";
	mountRotation = "0 1.57 0";

	weaponType = 0;
	reloadTime = 1.5; 
	fireTime = 0;

	maxEnergy = 0;
	minEnergy = 0;

	accuFire = true;

	sfxFire = SoundJuggernaughtBomberFire;
	sfxActivate = SoundPickUpWeapon;
};

ItemData JuggernaughtBomber {
	description = "Juggernaught bomber";
	className = "Weapon";
	shapeFile = "mortar";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = JuggernaughtBomberImage;
	price = 125;
	showWeaponBar = false;
};

$ItemDescription[JuggernaughtBomber] = "<jc><f1>Juggernaught bomber:<f0> A twin bomb launcher";

function JuggernaughtBomberImage::onFire(%player, %slot) {
  Weapon::fireProjectile(%player, JuggernaughtBomberShell);
  Weapon::fireOffsetProjectile(%player, JuggernaughtBomberShell, "0 0 -1.2", "", true);
  if (!Player::isTriggered(%player, 3)) {
    Weapon::linkSlots(%player, 0.1, 3);
  }
}

function JuggernaughtBomber::onMount(%player, %slot) {
  Weapon::displayDescription(%player, JuggernaughtBomber);
  Player::mountItem(%player, JuggernaughtBomber2, 3);
  Weapon::standardMount(%player, JuggernaughtBomber);
}
function JuggernaughtBomber::onUnmount(%player, %slot) {
  Player::unmountItem(%player, 3);
  Weapon::standardUnmount(%player, JuggernaughtBomber);
}

setArmorAllowsItem(larmor, JuggernaughtBomber, "", 0);
setArmorAllowsItem(lfemale, JuggernaughtBomber, "", 0);
setArmorAllowsItem(SpyMale, JuggernaughtBomber, "", 0);
setArmorAllowsItem(SpyFemale, JuggernaughtBomber, "", 0);
setArmorAllowsItem(DMMale, JuggernaughtBomber, "", 0);
setArmorAllowsItem(DMFemale, JuggernaughtBomber, "", 0);
