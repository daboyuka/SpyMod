SoundData SoundDragonFire
{
   wavFileName = "DryFire1.wav";
   profile = Profile3dNear;
};

ItemData DragonAmmo
{
	description = "Dragon Ammo";
	heading = $InvHeading[Ammo];
	className = "Ammo";
	shapeFile = "plasammo";
	shadowDetailMask = 4;
	price = 2;
};

BulletData DragonBullet
{
   bulletShapeName    = "bullet.dts";
   explosionTag       = bulletExp0;
   expRandCycle       = 3;
   bulletHoleIndex    = 0;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 13;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 400.0;
   totalTime          = 5.25;
   liveTime           = 5.25;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0.3;
   isVisible          = False;

   aimDeflection      = 0.007;
   tracerPercentage   = 1.0;
   tracerLength       = 30;

   soundId = SoundJetLight;
};

ItemImageData DragonImage2 {
	shapeFile = "plasma";
	mountPoint = 0;
	mountOffset = "0 0.4 0";
};

ItemData Dragon2 {
	shapeFile = "plasma";
	imageType = DragonImage2;
};

ItemImageData DragonImage3 {
	shapeFile = "plasma";
	mountPoint = 0;
	mountOffset = "0 0 0";
};

ItemData Dragon3 {
	shapeFile = "plasma";
	imageType = DragonImage3;
};

ItemImageData DragonImage {
	shapeFile = "breath";
	mountPoint = 0;

	weaponType = 0;
	reloadTime = 0.07; 
	fireTime = 0;

	ammoType = LoadedAmmo;//DragonAmmo;

	accuFire = true;

	firstPerson = false;

	lightType = 3;  // 1 = sustained, 2 = pulsing, 3 = weapon fire
	lightRadius = 5;
	lightTime = 0.25;
	lightColor = { 1, 1, 0.8 };

	sfxFire = SoundDragonFire;
	sfxActivate = SoundPickUpWeapon;
};

ItemData Dragon {
	description = "Dragon";
	className = "Weapon";
	shapeFile = "plasma";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = DragonImage;
	price = 125;
	showWeaponBar = false;//true;
};



ItemImageData DragonDisplayImage {
	shapeFile = "breath";
	firstPerson = false;
	ammoType = DragonAmmo;
};

ItemData DragonDisplay {
	description = "";
	className = "Tool";
	shapeFile = "breath";
	hudIcon = "ammopack";
	shadowDetailMask = 4;
	imageType = DragonDisplayImage;
	showWeaponBar = true;
	showInventory = false;
};

$ClipSize[Dragon] = 40;
$AmmoDisplay[Dragon] = DragonDisplay;
$ItemDescription[Dragon] = "<jc><f1>Dragon:<f0> A fairly accurate machine gun";

function DragonImage::onFire(%player, %slot) {
  if (Player::getItemCount(%player, LoadedAmmo) == 0) return;
  Weapon::fireProjectile(%player, DragonBullet);
  Player::decItemCount(%player, LoadedAmmo);//DragonAmmo);
}

function Dragon::onMount(%player, %slot) {
  Weapon::displayDescription(%player, Dragon);
  Player::mountItem(%player, Dragon2, 4);
  Player::mountItem(%player, Dragon3, 5);
  Weapon::standardMount(%player, Dragon);
}
function Dragon::onUnmount(%player, %slot) {
  Player::unmountItem(%player, 4);
  Player::unmountItem(%player, 5);
  Weapon::standardUnmount(%player, Dragon);
}

function Dragon::onNoAmmo(%player) {
  Dragon::reload(%player);
}

function Dragon::reload(%player) {
  if (%player.reloading[Dragon]) return;
  %player.reloading[Dragon] = true;

  Weapon::returnClipAmmo(%player, Dragon);

  schedule(%player @ ".reloading[Dragon] = false;" @
           "if (getSimTime() - "@%player@".lastUnmount[Dragon] > 2 && !Player::isDead("@%player@")) Weapon::loadClip("@%player@", Dragon);", 2);
}

registerWeapon(Dragon, DragonAmmo, 40);
linkWeapon(Dragon);
setArmorAllowsItem(larmor, Dragon, 160);
setArmorAllowsItem(lfemale, Dragon, 160);
setArmorAllowsItem(SpyMale, Dragon, 160);
setArmorAllowsItem(SpyFemale, Dragon, 160);
setArmorAllowsItem(DMMale, Dragon, 160);
setArmorAllowsItem(DMFemale, Dragon, 160);
