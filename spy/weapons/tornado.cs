SoundData SoundFireTornado {
   wavFileName = "machinegun.wav";
   profile = Profile3dMediumLoop;
};

ItemData TornadoAmmo
{
	description = "Tornado Ammo";
   heading = "xAmmunition";
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

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 0.08;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 400.0;
   totalTime          = 0.25;
   liveTime           = 0.25;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0.3;
   isVisible          = False;

   aimDeflection      = 0.004;
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
	description = "Tornado MK III";
	className = "Weapon";
	shapeFile = "chaingun";
	hudIcon = "mortar";
   heading = "bWeapons";
	shadowDetailMask = 4;
	imageType = TornadoImage2;
	showWeaponBar = false;
};

ItemImageData TornadoImage3
{
	shapeFile = "chaingun";
	mountPoint = 0;

	weaponType = 1;
	spinUpTime = 0;
	reloadTime = 0.05; 
	fireTime = 0;
	spinDownTime = 4;

	accuFire = false;
};

ItemData Tornado3
{
	description = "Tornado MK III";
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

	ammoType = TornadoAmmo;

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
   heading = "bWeapons";
	shadowDetailMask = 4;
	imageType = TornadoImage;
	price = 125;
	showWeaponBar = true;
};

function TornadoImage::onFire(%player, %slot) {
  Weapon::fireProjectile(%player, TornadoBullet);
  Weapon::fireProjectile(%player, TornadoBullet);
  if (!Player::isTriggered(%player, 4) && !Player::isTriggered(%player, 5)) {
    Weapon::linkSlots(%player, 0.1, 4, 5);
  }
  Player::decItemCount(%player, TornadoAmmo, 2);
}

function Tornado::onMount(%player, %slot) {
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

//function Player::onNoAmmo(%player) { Player::trigger(%player, 0, false); }

registerWeapon(Tornado);
linkWeapon(Tornado);
setArmorAllowsItem(larmor, Tornado, 200);
setArmorAllowsItem(lfemale, Tornado, 200);