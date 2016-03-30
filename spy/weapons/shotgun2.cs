SoundData SoundFireShotgun2 {
   wavFileName = "debris_medium.wav";//"ricoche2.wav";
   profile = Profile3dMedium;
};
SoundData SoundLoadShotgun2 {
   wavFileName = "Pku_weap.wav";
   profile = Profile3dNear;
};

ItemData Shotgun2Ammo
{
	description = "Shotgun M2 Ammo";
	heading = $InvHeading[Ammo];
	className = "Ammo";
	shapeFile = "plasammo";
	shadowDetailMask = 4;
	price = 2;
};

BulletData Shotgun2Bullet {
   bulletShapeName    = "bullet.dts";
   explosionTag       = bulletExp0;
   expRandCycle       = 3;
   bulletHoleIndex    = 0;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 12;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 400.0;
   totalTime          = 0.25;
   liveTime           = 0.25;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0;
   isVisible          = False;

   aimDeflection      = 0.008;
   tracerPercentage   = 1.0;
   tracerLength       = 30;

   soundId = SoundJetLight;
};

ItemImageData Shotgun2Image3 {
	shapeFile = "energygun";
	mountPoint = 0;
};

ItemData Shotgun23 {
	shapeFile = "energygun";
	imageType = Shotgun2Image3;
};

ItemImageData Shotgun2Image4 {
	shapeFile = "energygun";
	mountPoint = 0;
	mountOffset = "-0.2 0 0";
};

ItemData Shotgun24 {
	shapeFile = "energygun";
	imageType = Shotgun2Image4;
};

ItemImageData Shotgun2Image5 {
	shapeFile = "grenammo";
	mountPoint = 0;
	mountOffset = "-0.1 0 0";
	mountRotation = "0 0 3.14";
};

ItemData Shotgun25 {
	shapeFile = "grenammo";
	imageType = Shotgun2Image5;
};

ItemImageData Shotgun2Image {
	shapeFile = "breath";
	mountPoint = 0;

	weaponType = 0;
	reloadTime = 0.85; 
	fireTime = 0;

	ammoType = LoadedAmmo;

	accuFire = true;

	firstPerson = false;

	lightType = 3;  // 1 = sustained, 2 = pulsing, 3 = weapon fire
	lightRadius = 5;
	lightTime = 0.25;
	lightColor = { 1, 1, 0.8 };

	sfxFire = SoundFireShotgun2;
	sfxActivate = SoundPickUpWeapon;
};

ItemData Shotgun2 {
	description = "Shotgun M2";
	className = "Weapon";
	shapeFile = "energygun";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = Shotgun2Image;
	price = 125;
	showWeaponBar = false;
};



ItemImageData Shotgun2DisplayImage {
	shapeFile = "breath";
	firstPerson = false;
	ammoType = Shotgun2Ammo;
};

ItemData Shotgun2Display {
	description = "";
	className = "Tool";
	shapeFile = "breath";
	hudIcon = "ammopack";
	shadowDetailMask = 4;
	imageType = Shotgun2DisplayImage;
	showWeaponBar = true;
	showInventory = false;
};



$ClipSize[Shotgun2] = 4;
$AmmoDisplay[Shotgun2] = Shotgun2Display;
$ItemDescription[Shotgun2] = "<jc><f1>Shotgun M2:<f0> A larger model of the Shotgun M1; it can hold 4 shells in one clip";

function Shotgun2Image::onFire(%player, %slot) {
  if (Player::getItemCount(%player, LoadedAmmo) == 0) return;

  Weapon::fireProjectile(%player, Shotgun2Bullet);
  Weapon::fireProjectile(%player, Shotgun2Bullet);
  Weapon::fireProjectile(%player, Shotgun2Bullet);
  Weapon::fireProjectile(%player, Shotgun2Bullet);
  Weapon::fireProjectile(%player, Shotgun2Bullet);
  Weapon::fireProjectile(%player, Shotgun2Bullet);
  Player::decItemCount(%player, LoadedAmmo);
}

function Shotgun2::onMount(%player, %slot) {
  Weapon::displayDescription(%player, Shotgun2);
  Player::mountItem(%player, Shotgun23, 4);
  Player::mountItem(%player, Shotgun24, 5);
  Player::mountItem(%player, Shotgun25, 6);
  Weapon::standardMount(%player, Shotgun2);
}
function Shotgun2::onUnmount(%player, %slot) {
  Player::unmountItem(%player, 4);
  Player::unmountItem(%player, 5);
  Player::unmountItem(%player, 6);
  Weapon::standardUnmount(%player, Shotgun2);
}

function Shotgun2::onNoAmmo(%player) {
  Shotgun2::reload(%player);
}

function Shotgun2::reload(%player) {
  if (%player.reloading[Shotgun2]) return;

  %num = min(Player::getItemCount(%player, Shotgun2Ammo), $ClipSize[Shotgun2] - Player::getItemCount(%player, LoadedAmmo));
  for (%i = 1; %i <= %num; %i++)
    schedule("if (getSimTime() - "@%player@".lastUnmount[Shotgun2] > "@(%i * 0.5)@" && !Player::isDead("@%player@"))" @
             "GameBase::playSound("@%player@", SoundLoadShotgun2, 0);", %i * 0.5);

  Weapon::standardReload(%player, Shotgun2, "", %num * 0.5);
}

registerWeapon(Shotgun2, Shotgun2Ammo, 4);
linkWeapon(Shotgun2);
setArmorAllowsItem(larmor, Shotgun2, 16);
setArmorAllowsItem(lfemale, Shotgun2, 16);
setArmorAllowsItem(SpyMale, Shotgun2, 16);
setArmorAllowsItem(SpyFemale, Shotgun2, 16);
setArmorAllowsItem(DMMale, Shotgun2, 16);
setArmorAllowsItem(DMFemale, Shotgun2, 16);