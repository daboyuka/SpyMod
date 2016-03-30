SoundData SoundFireChallenger {
   wavFileName = "debris_small.wav";
   profile = Profile3dMedium;
};

ItemData ChallengerAmmo {
	description = "X-9 Challenger Ammo";
	heading = $InvHeading[Ammo];
	className = "Ammo";
	shapeFile = "plasammo";
	shadowDetailMask = 4;
	price = 2;
};

BulletData ChallengerBullet1 {
   bulletShapeName    = "bullet.dts";
   explosionTag       = bulletExp0;
   expRandCycle       = 3;
   bulletHoleIndex    = 0;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 20;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 800.0;
   totalTime          = 0.75;//0.25;
   liveTime           = 0.75;//0.25;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0;
   isVisible          = False;

   aimDeflection      = 0.0001;
   tracerPercentage   = 1.0;
   tracerLength       = 30;

   soundId = SoundJetLight;
};

BulletData ChallengerBullet2 {
   bulletShapeName    = "bullet.dts";
   explosionTag       = bulletExp0;
   expRandCycle       = 3;
   bulletHoleIndex    = 0;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 21;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 800.0;
   totalTime          = 0.25;
   liveTime           = 0.25;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0;
   isVisible          = False;

   aimDeflection      = 0.005;
   tracerPercentage   = 1.0;
   tracerLength       = 30;

   soundId = SoundJetLight;
};

RepairEffectData ChallengerTriggerBolt {
   bitmapName       = "bar00.bmp";
   boltLength       = 0;
   segmentDivisions = 1;
   beamWidth        = 0;

   updateTime   = 1000;
   skipPercent  = 0.6;
   displaceBias = 0.15;
};

ItemImageData ChallengerImage2 {
	shapeFile = "plasma";
	mountPoint = 0;
	mountOffset = "0 -0.3 -0.05";
};

ItemData Challenger2 {
	shapeFile = "plasma";
	imageType = ChallengerImage2;
};

ItemImageData ChallengerImage3 {
	shapeFile = "sniper";
	mountPoint = 0;
};

ItemData Challenger3 {
	shapeFile = "sniper";
	imageType = ChallengerImage3;
};

ItemImageData ChallengerM1Image {
	shapeFile = "breath";
	mountPoint = 0;

	weaponType = 0;
	reloadTime = 0.2; 
	fireTime = 0;

	ammoType = LoadedAmmo;//DragonAmmo;

	accuFire = false;

	firstPerson = false;

	lightType = 3;  // 1 = sustained, 2 = pulsing, 3 = weapon fire
	lightRadius = 5;
	lightTime = 0.25;
	lightColor = { 1, 1, 0.8 };

	sfxFire = SoundFireChallenger;
	sfxActivate = SoundPickUpWeapon;
};

ItemData ChallengerM1 {
	className = "Weapon";
	shapeFile = "plasma";
	shadowDetailMask = 4;
	imageType = ChallengerM1Image;
	showWeaponBar = false;//true;
};

ItemImageData ChallengerM2Image {
	shapeFile = "breath";
	mountPoint = 0;

	weaponType = 0;
	reloadTime = 0.85; 
	fireTime = 0;

	ammoType = LoadedAmmo;//DragonAmmo;

	accuFire = false;

	firstPerson = false;

	lightType = 3;  // 1 = sustained, 2 = pulsing, 3 = weapon fire
	lightRadius = 5;
	lightTime = 0.25;
	lightColor = { 1, 1, 0.8 };

	sfxFire = SoundFireChallenger;
	sfxActivate = SoundPickUpWeapon;
};

ItemData ChallengerM2 {
	className = "Weapon";
	shapeFile = "plasma";
	shadowDetailMask = 4;
	imageType = ChallengerM2Image;
	showWeaponBar = false;//true;
};

ItemImageData ChallengerImage {
	shapeFile = "breath";
	mountPoint = 0;

	weaponType = 2;
	reloadTime = 0;
	fireTime = 0;
	projectileType = ChallengerTriggerBolt;

        minEnergy = 0;
        maxEnergy = 0;

	accuFire = false;

	firstPerson = false;
};

ItemData Challenger {
	description = "X-9 Challenger";
	className = "Weapon";
	shapeFile = "plasma";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = ChallengerImage;
	price = 125;
	showWeaponBar = false;//true;
};



ItemImageData ChallengerDisplayImage {
	shapeFile = "breath";
	firstPerson = false;
	ammoType = ChallengerAmmo;
};

ItemData ChallengerDisplay {
	description = "";
	className = "Tool";
	shapeFile = "breath";
	hudIcon = "ammopack";
	shadowDetailMask = 4;
	imageType = ChallengerDisplayImage;
	showWeaponBar = true;
	showInventory = false;
};

$ItemDescription[Challenger, 0] = "<jc><f1>Challenger, Single Mode:<f0> High rate of fire and sniper-like accuracy";
$ItemDescription[Challenger, 1] = "<jc><f1>Challenger, Burst Mode:<f0> Fires 3 bullets at a time, but with less accuracy";



$ClipSize[Challenger] = 15;
$AmmoDisplay[Challenger] = ChallengerDisplay;
$NumModes[Challenger] = 2;

function ChallengerTriggerBolt::onAcquire(%this, %player, %target) {
  Player::trigger(%player, 4, true);
}
function ChallengerTriggerBolt::onRelease(%this, %player) {
  Player::trigger(%player, 4, false);
}
function ChallengerTriggerBolt::checkDone(%this, %player) {}



function ChallengerM1Image::onFire(%player, %slot) {
  if (Player::getItemCount(%player, LoadedAmmo) == 0) return;
  Weapon::fireProjectile(%player, ChallengerBullet1);
  Player::decItemCount(%player, LoadedAmmo);
}
function ChallengerM2Image::onFire(%player, %slot) {
  %count = Player::getItemCount(%player, LoadedAmmo);
  if (%count == 0) return;
  Weapon::fireProjectile(%player, ChallengerBullet2);
  if (%count >= 2) schedule("Weapon::fireProjectile("@%player@", ChallengerBullet2); GameBase::playSound("@%player@",SoundFireChallenger,0);", 0.07);
  if (%count >= 3) schedule("Weapon::fireProjectile("@%player@", ChallengerBullet2); GameBase::playSound("@%player@",SoundFireChallenger,0);", 0.14);
  Player::decItemCount(%player, LoadedAmmo, min(3, %count));
}

function Challenger::onMount(%player, %slot) {
  Weapon::displayDescription(%player, Challenger);

  if (Weapon::getMode(%player, Challenger) == 0)
    Player::mountItem(%player, ChallengerM1, 4);
  else 
    Player::mountItem(%player, ChallengerM2, 4);

  Player::mountItem(%player, Challenger2, 5);
  Player::mountItem(%player, Challenger3, 6);
  Weapon::standardMount(%player, Challenger);
}
function Challenger::onUnmount(%player, %slot) {
  Player::unmountItem(%player, 4);
  Player::unmountItem(%player, 5);
  Player::unmountItem(%player, 6);
  Weapon::standardUnmount(%player, Challenger);
}

function Challenger::onNoAmmo(%player) {
  Challenger::reload(%player);
}

function Challenger::reload(%player) {
  Weapon::standardReload(%player, Challenger, "", 3);
}

function Challenger::onModeChanged(%player, %mode) {
  Weapon::displayDescription(%player, Challenger);
  if (%mode == 0) Player::mountItem(%player, ChallengerM1, 4);
  if (%mode == 1) Player::mountItem(%player, ChallengerM2, 4);
}

registerWeapon(Challenger, ChallengerAmmo, 15);
linkWeapon(Challenger);
setArmorAllowsItem(larmor, Challenger, 60);
setArmorAllowsItem(lfemale, Challenger, 60);
setArmorAllowsItem(SpyMale, Challenger, 60);
setArmorAllowsItem(SpyFemale, Challenger, 60);
setArmorAllowsItem(DMMale, Challenger, 60);
setArmorAllowsItem(DMFemale, Challenger, 60);
