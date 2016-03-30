SoundData SoundFireTornado {
   wavFileName = "machinegun.wav";
   profile = Profile3dMediumLoop;
};
SoundData SoundLoadTornado {
   wavFileName = "mortar_reload.wav";
   profile = Profile3dNear;
};

ItemData TornadoAmmo
{
	description = "Tornado Ammo";
	heading = $InvHeading[Ammo];
	className = "Ammo";
	shapeFile = "plasammo";
	shadowDetailMask = 4;
	price = 2;
};

BulletData TornadoBullet
{
   bulletShapeName    = "bullet.dts";
   explosionTag       = bulletExp0;
   expRandCycle       = 3;
   bulletHoleIndex    = 0;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 8.5;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 400.0;
   totalTime          = 5.25;
   liveTime           = 5.25;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0.3;
   isVisible          = False;

   aimDeflection      = 0.01;
   tracerPercentage   = 1.0;
   tracerLength       = 15;

   soundId = SoundJetLight;
};

ItemImageData TornadoImage2 {
	shapeFile = "chaingun";
	mountPoint = 0;
	mountRotation = "0 3.14 0";
	mountOffset = "0 0 0.2";

	weaponType = 1;
	spinUpTime = 0;
	reloadTime = 0.05; 
	fireTime = 0;
	spinDownTime = 4;

	accuFire = false;
};

ItemData Tornado2 {
	className = "Weapon";
	shapeFile = "chaingun";
	hudIcon = "mortar";
   heading = "bWeapons";
	shadowDetailMask = 4;
	imageType = TornadoImage2;
	showWeaponBar = false;
};

ItemImageData TornadoImage3 {
	shapeFile = "chaingun";
	mountPoint = 0;

	weaponType = 1;
	spinUpTime = 0;
	reloadTime = 0.05; 
	fireTime = 0;
	spinDownTime = 4;

	accuFire = false;
};

ItemData Tornado3 {
	className = "Weapon";
	shapeFile = "chaingun";
	hudIcon = "mortar";
   heading = "bWeapons";
	shadowDetailMask = 4;
	imageType = TornadoImage3;
	showWeaponBar = false;
};



ItemImageData TornadoImage
{
	shapeFile = "breath";
	mountPoint = 0;

	weaponType = 1;
	spinUpTime = 0;
	reloadTime = 0;
	fireTime = 0.13;
	spinDownTime = 0;

	ammoType = LoadedAmmo;//TornadoAmmo;

	lightType = 3;  // 1 = sustained, 2 = pulsing, 3 = weapon fire
	lightRadius = 5;
	lightTime = 0.25;
	lightColor = { 1, 1, 0.8 };

	accuFire = false;
	sfxFire = SoundFireTornado;
	sfxActivate = SoundPickUpWeapon;

	firstPerson = false;
};

ItemData Tornado
{
	description = "Tornado MK III";
	className = "Weapon";
	shapeFile = "chaingun";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = TornadoImage;
	price = 125;
	showWeaponBar = false;//true;
};

ItemImageData TornadoDisplayImage {
	shapeFile = "breath";
	firstPerson = false;
	ammoType = TornadoAmmo;
};

ItemData TornadoDisplay {
	description = "";
	className = "Tool";
	shapeFile = "breath";
	hudIcon = "ammopack";
	shadowDetailMask = 4;
	imageType = TornadoDisplayImage;
	showWeaponBar = true;
	showInventory = false;
};

$ClipSize[Tornado] = 80;
$AmmoDisplay[Tornado] = TornadoDisplay;
$ItemDescription[Tornado] = "<jc><f1>Tornado:<f0> A twin machine gun that can empty its large magazine in seconds";

function TornadoImage::onFire(%player, %slot) {
  if (Player::getItemCount(%player, LoadedAmmo) == 0) return;

  Weapon::fireProjectile(%player, TornadoBullet);
  Weapon::fireProjectile(%player, TornadoBullet);

  if (!Player::isTriggered(%player, 4) && !Player::isTriggered(%player, 5)) {
    Weapon::linkSlots(%player, 0.1, 4, 5);
  }
  Player::decItemCount(%player, LoadedAmmo, 2);//TornadoAmmo, 2);
}

function Tornado::onMount(%player, %slot) {
  Weapon::displayDescription(%player, Tornado);
  Player::mountItem(%player, Tornado2, 4);
  Player::mountItem(%player, Tornado3, 5);

  Weapon::standardMount(%player, Tornado);
}

function Tornado::onUnmount(%player, %slot) {
  Player::trigger(%player, %slot, false);
  Player::trigger(%player, 4, false);
  Player::trigger(%player, 5, false);
  Player::unmountItem(%player, 4);
  Player::unmountItem(%player, 5);

  Weapon::standardUnmount(%player, Tornado);
}

function Tornado::onNoAmmo(%player) {
  Tornado::reload(%player);
}

function Tornado::reload(%player) {
  Weapon::standardReload(%player, Tornado, SoundLoadTornado, 2);
}

registerWeapon(Tornado, TornadoAmmo, 60);
linkWeapon(Tornado);
setArmorAllowsItem(larmor, Tornado, 320);
setArmorAllowsItem(lfemale, Tornado, 320);
setArmorAllowsItem(SpyMale, Tornado, 320);
setArmorAllowsItem(SpyFemale, Tornado, 320);
setArmorAllowsItem(DMMale, Tornado, 320);
setArmorAllowsItem(DMFemale, Tornado, 320);