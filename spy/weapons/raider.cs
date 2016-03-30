SoundData SoundFireRaider {
   wavFileName = "shockexp.wav";
   profile = Profile3dMedium;
};

SoundData SoundReloadRaider {
   wavFileName = "mortar_reload.wav";
   profile = Profile3dNear;
};

ItemData RaiderAmmo
{
	description = "Raider Ammo";
	heading = $InvHeading[Ammo];
	className = "Ammo";
	shapeFile = "plasammo";
	shadowDetailMask = 4;
	price = 2;
};

BulletData RaiderBullet1 {
   bulletShapeName    = "bullet.dts";
   explosionTag       = bulletExp0;
   expRandCycle       = 3;
   bulletHoleIndex    = 0;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 30;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 800.0;
   totalTime          = 0.75;//0.25;
   liveTime           = 0.75;//0.25;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0;
   isVisible          = False;

   aimDeflection      = 0.0005;
   tracerPercentage   = 1.0;
   tracerLength       = 30;

   soundId = SoundJetLight;
};

BulletData RaiderBullet2 {
   bulletShapeName    = "bullet.dts";
   explosionTag       = bulletExp0;
   expRandCycle       = 3;
   bulletHoleIndex    = 0;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 17;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 800.0;
   totalTime          = 0.25;
   liveTime           = 0.25;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0;
   isVisible          = False;

   aimDeflection      = 0.009;
   tracerPercentage   = 1.0;
   tracerLength       = 30;

   soundId = SoundJetLight;
};

RepairEffectData RaiderTriggerBolt {
   bitmapName       = "bar00.bmp";
   boltLength       = 0;
   segmentDivisions = 1;
   beamWidth        = 0;

   updateTime   = 1000;
   skipPercent  = 0.6;
   displaceBias = 0.15;
};

ItemImageData RaiderImage2 {
	shapeFile = "paintgun";
	mountPoint = 0;
	mountOffset = "0 0.2 -0.02";
};

ItemData Raider2 {
	shapeFile = "paintgun";
	imageType = RaiderImage2;
};

ItemImageData RaiderImage3 {
	shapeFile = "paintgun";
	mountPoint = 0;
	mountOffset = "-0.1 0.3 0.12";
	mountRotation = "0 1.57 0";
};

ItemData Raider3 {
	shapeFile = "paintgun";
	imageType = RaiderImage3;
};

ItemImageData RaiderImage4 {
	shapeFile = "paintgun";
	mountPoint = 0;
	mountOffset = "0.1 0.3 0.12";
	mountRotation = "0 -1.57 0";
};

ItemData Raider4 {
	shapeFile = "paintgun";
	imageType = RaiderImage4;
};

ItemImageData RaiderImage5 {
	shapeFile = "sniper";
	mountPoint = 0;
	mountOffset = "0 0 0";
	mountRotation = "0 0 0";
};

ItemData Raider5 {
	shapeFile = "sniper";
	imageType = RaiderImage5;
};

ItemImageData RaiderM1Image {
	shapeFile = "breath";
	mountPoint = 0;

	weaponType = 0;
	reloadTime = 0.45; 
	fireTime = 0;

	ammoType = LoadedAmmo;

	accuFire = false;

	firstPerson = false;

	lightType = 3;  // 1 = sustained, 2 = pulsing, 3 = weapon fire
	lightRadius = 5;
	lightTime = 0.25;
	lightColor = { 1, 1, 0.8 };

	sfxFire = SoundFireRaider;
	sfxActivate = SoundPickUpWeapon;
};

ItemData RaiderM1 {
	description = "";
	className = "Weapon";
	shapeFile = "plasma";
	shadowDetailMask = 4;
	imageType = RaiderM1Image;
	price = 125;
	showWeaponBar = false;
};

ItemImageData RaiderM2Image {
	shapeFile = "breath";
	mountPoint = 0;

	weaponType = 0;
	reloadTime = 0.125; 
	fireTime = 0;

	ammoType = LoadedAmmo;

	accuFire = true;

	firstPerson = false;

	lightType = 3;  // 1 = sustained, 2 = pulsing, 3 = weapon fire
	lightRadius = 5;
	lightTime = 0.25;
	lightColor = { 1, 1, 0.8 };

	sfxFire = SoundFireRaider;
	sfxActivate = SoundPickUpWeapon;
};

ItemData RaiderM2 {
	description = "";
	className = "Weapon";
	shapeFile = "plasma";
	shadowDetailMask = 4;
	imageType = RaiderM2Image;
	price = 125;
	showWeaponBar = false;
};

ItemImageData RaiderImage {
	shapeFile = "breath";
	mountPoint = 0;

	weaponType = 2;
	reloadTime = 0;
	fireTime = 0;
	projectileType = RaiderTriggerBolt;

        minEnergy = 0;
        maxEnergy = 0;

	firstPerson = false;

	accuFire = true;
};

ItemData Raider {
	description = "Raider";
	className = "Weapon";
	shapeFile = "plasma";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = RaiderImage;
	price = 125;
	showWeaponBar = false;
};



ItemImageData RaiderDisplayImage {
	shapeFile = "breath";
	firstPerson = false;
	ammoType = RaiderAmmo;
};

ItemData RaiderDisplay {
	description = "";
	className = "Tool";
	shapeFile = "breath";
	hudIcon = "ammopack";
	shadowDetailMask = 4;
	imageType = RaiderDisplayImage;
	showWeaponBar = true;
	showInventory = false;
};

$ItemDescription[Raider, 0] = "<jc><f1>Raider, Single Mode:<f0> Powerful with high accuracy, with a slow reload rate";
$ItemDescription[Raider, 1] = "<jc><f1>Raider, Auto Mode:<f0> Rains bullets down on your enemies, but with low accuracy";

$ClipSize[Raider] = 20;
$AmmoDisplay[Raider] = RaiderDisplay;
$NumModes[Raider] = 2;

function RaiderTriggerBolt::onAcquire(%this, %player, %target) {
  Player::trigger(%player, 3, true);
}
function RaiderTriggerBolt::onRelease(%this, %player) {
  Player::trigger(%player, 3, false);
}
function RaiderTriggerBolt::checkDone(%this, %player) {}



function RaiderM1Image::onFire(%player, %slot) {
  if (Player::getItemCount(%player, LoadedAmmo) == 0) return;
  Weapon::fireProjectile(%player, RaiderBullet1);
  GameBase::playSound(%player, SoundFireRaider, 0);
  GameBase::playSound(%player, SoundFireRaider, 1);
  Player::decItemCount(%player, LoadedAmmo);
}
function RaiderM2Image::onFire(%player, %slot) {
  if (Player::getItemCount(%player, LoadedAmmo) == 0) return;
  Weapon::fireProjectile(%player, RaiderBullet2);
  GameBase::playSound(%player, SoundFireRaider, 0);
  GameBase::playSound(%player, SoundFireRaider, 1);
  Player::decItemCount(%player, LoadedAmmo);
}

function Raider::onMount(%player, %slot) {
  Weapon::displayDescription(%player, Raider);

  if (Weapon::getMode(%player, Raider) == 0)
    Player::mountItem(%player, RaiderM1, 3);
  else 
    Player::mountItem(%player, RaiderM2, 3);

  Player::mountItem(%player, Raider2, 4);
  Player::mountItem(%player, Raider3, 5);
  Player::mountItem(%player, Raider4, 6);
  Player::mountItem(%player, Raider5, 7);
  Weapon::standardMount(%player, Raider);
}
function Raider::onUnmount(%player, %slot) {
  Player::unmountItem(%player, 3);
  Player::unmountItem(%player, 4);
  Player::unmountItem(%player, 5);
  Player::unmountItem(%player, 6);
  Player::unmountItem(%player, 7);
  Weapon::standardUnmount(%player, Raider);
}

function Raider::onNoAmmo(%player) {
  Raider::reload(%player);
}

function Raider::reload(%player) {
  if (%player.reloading[Raider]) return;
  %player.reloading[Raider] = true;

  Weapon::returnClipAmmo(%player, Raider);

  %seconds = 3;

  %command = %player @ ".reloading[Raider] = false;" @
             "if (getSimTime() - "@%player@".lastUnmount[Raider] > "@%seconds@" && !Player::isDead("@%player@"))" @
             "{";
  %command = %command @ "GameBase::playSound("@%player@",SoundReloadRaider,0);";
  %command = %command @ "GameBase::playSound("@%player@",SoundReloadRaider,1);";
  %command = %command @ "Weapon::loadClip("@%player@", Raider); }";

  %command2 = "if (getSimTime() - "@%player@".lastUnmount[Raider] > "@(%seconds+0.2)@" && !Player::isDead("@%player@"))" @
              "{";
  %command2 = %command2 @ "GameBase::playSound("@%player@",SoundReloadRaider,0);"
                        @ "GameBase::playSound("@%player@",SoundReloadRaider,1); }";


  schedule(%command, %seconds, %player);
  schedule(%command2, %seconds + 0.2, %player);
}

function Raider::onModeChanged(%player, %mode) {
  Weapon::displayDescription(%player, Raider);
  if (%mode == 0) Player::mountItem(%player, RaiderM1, 3);
  if (%mode == 1) Player::mountItem(%player, RaiderM2, 3);
}

registerWeapon(Raider, RaiderAmmo, 20);
linkWeapon(Raider);
setArmorAllowsItem(larmor, Raider, 80);
setArmorAllowsItem(lfemale, Raider, 80);
setArmorAllowsItem(SpyMale, Raider, 80);
setArmorAllowsItem(SpyFemale, Raider, 80);
setArmorAllowsItem(DMMale, Raider, 80);
setArmorAllowsItem(DMFemale, Raider, 80);