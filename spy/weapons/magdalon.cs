SoundData SoundFireMagdalon {
   wavFileName = "debris_medium.wav";//"ricoche2.wav";
   profile = Profile3dMedium;
};
SoundData SoundLoadMagdalon {
   wavFileName = "mortar_reload.wav";
   profile = Profile3dNear;
};

ItemData MagdalonAmmo
{
	description = "Magdalon Ammo";
	heading = $InvHeading[Ammo];
	className = "Ammo";
	shapeFile = "plasammo";
	shadowDetailMask = 4;
	price = 2;
};

BulletData MagdalonBullet {
   bulletShapeName    = "bullet.dts";
   explosionTag       = bulletExp0;
   expRandCycle       = 3;
   bulletHoleIndex    = 0;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 29;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 600.0;
   totalTime          = 0.25;
   liveTime           = 0.25;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0;
   isVisible          = False;

   aimDeflection      = 0.004;
   tracerPercentage   = 1.0;
   tracerLength       = 7;

   soundId = SoundJetLight;
};

ItemImageData MagdalonImage2 {
	shapeFile = "plasma";
	mountPoint = 0;
	mountRotation = "0 3.14 0";
};

ItemData Magdalon2 {
	shapeFile = "plasma";
	imageType = MagdalonImage2;
};

ItemImageData MagdalonImage3 {
	shapeFile = "paintgun";
	mountPoint = 0;
	mountOffset = "0 0.4 0";
};

ItemData Magdalon3 {
	shapeFile = "paintgun";
	imageType = MagdalonImage3;
};

ItemImageData MagdalonImage {
	shapeFile = "breath";
	mountPoint = 0;

	weaponType = 0;
	reloadTime = 0; 
	fireTime = 0.35;

	ammoType = LoadedAmmo;

	accuFire = false;

	firstPerson = false;

	lightType = 3;  // 1 = sustained, 2 = pulsing, 3 = weapon fire
	lightRadius = 5;
	lightTime = 0.25;
	lightColor = { 1, 1, 0.8 };

	sfxFire = SoundFireMagdalon;
	sfxActivate = SoundPickUpWeapon;
};

ItemData Magdalon {
	description = "Magdalon";
	className = "Weapon";
	shapeFile = "paintgun";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = MagdalonImage;
	price = 125;
	showWeaponBar = false;
};



ItemImageData MagdalonDisplayImage {
	shapeFile = "breath";
	firstPerson = false;
	ammoType = MagdalonAmmo;
};

ItemData MagdalonDisplay {
	description = "";
	className = "Tool";
	shapeFile = "breath";
	hudIcon = "ammopack";
	shadowDetailMask = 4;
	imageType = MagdalonDisplayImage;
	showWeaponBar = true;
	showInventory = false;
};



$ClipSize[Magdalon] = 12;
$AmmoDisplay[Magdalon] = MagdalonDisplay;
$ItemDescription[Magdalon] = "<jc><f1>Magdalon:<f0> A medium-caliber rifle, with an average reload rate";

function MagdalonImage::onFire(%player, %slot) {
  if (Player::getItemCount(%player, LoadedAmmo) == 0) return;
  Weapon::fireProjectile(%player, MagdalonBullet);
  Player::decItemCount(%player, LoadedAmmo);
}

function Magdalon::onMount(%player, %slot) {
  Weapon::displayDescription(%player, Magdalon);
  Player::mountItem(%player, Magdalon2, 3);
  Player::mountItem(%player, Magdalon3, 4);
  Weapon::standardMount(%player, Magdalon);
}
function Magdalon::onUnmount(%player, %slot) {
  Player::unmountItem(%player, 3);
  Player::unmountItem(%player, 4);
  Weapon::standardUnmount(%player, Magdalon);
}

function Magdalon::onNoAmmo(%player) {
  Magdalon::reload(%player);
}

function Magdalon::reload(%player) {
  Weapon::standardReload(%player, Magdalon, SoundLoadMagdalon, 1.75);
}

registerWeapon(Magdalon, MagdalonAmmo, 10);
linkWeapon(Magdalon);
setArmorAllowsItem(larmor, Magdalon, 50);
setArmorAllowsItem(lfemale, Magdalon, 50);
setArmorAllowsItem(SpyMale, Magdalon, 50);
setArmorAllowsItem(SpyFemale, Magdalon, 50);
setArmorAllowsItem(DMMale, Magdalon, 50);
setArmorAllowsItem(DMFemale, Magdalon, 50);