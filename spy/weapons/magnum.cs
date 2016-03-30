SoundData SoundFireMagnum
{
   wavFileName = "mine_exp.wav";
   profile = Profile3dNear;
};

ItemData MagnumAmmo
{
	description = "Magnum Ammo";
   heading = "xAmmunition";
	className = "Ammo";
	shapeFile = "plasammo";
	shadowDetailMask = 4;
	price = 2;
};

BulletData MagnumBullet
{
   bulletShapeName    = "bullet.dts";
   explosionTag       = bulletExp0;
   expRandCycle       = 3;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 0.55;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 400.0;
   totalTime          = 0.5;
   liveTime           = 0.5;
   lightRange         = 3.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0.3;
   isVisible          = False;

   aimDeflection      = 0.003;
   tracerPercentage   = 1.0;
   tracerLength       = 30;

   soundId = SoundJetLight;
};

ItemImageData MagnumImage
{
	shapeFile = "energygun";
	mountPoint = 0;

	weaponType = 0;
	reloadTime = 0.5; 
	fireTime = 0;

	ammoType = LoadedAmmo;//MagnumAmmo;

//	projectileType = MagnumBullet;
	accuFire = false;

	lightType = 3;  // 1 = sustained, 2 = pulsing, 3 = weapon fire
	lightRadius = 0;
	lightTime = 1;
	lightColor = { 1, 1, 0.8 };

	sfxFire = SoundFireMagnum;
	sfxActivate = SoundPickUpWeapon;
};

ItemData Magnum
{
	description = "Magnum";
	className = "Weapon";
	shapeFile = "energygun";
	hudIcon = "mortar";
   heading = "bWeapons";
	shadowDetailMask = 4;
	imageType = MagnumImage;
	price = 125;
	showWeaponBar = true;
};

$ClipSize[Magnum] = 6;

function MagnumImage::onFire(%player, %slot) {
  Weapon::fireProjectile(%player, MagnumBullet);
  Player::decItemCount(%player, LoadedAmmo);//MagnumAmmo);
}

function Magnum::onMount(%player, %slot) {
  Magnum::loadClip(%player);
}
function Magnum::onUnmount(%player, %slot) {
  Player::incItemCount(%player, MagnumAmmo, Player::getItemCount(%player, LoadedAmmo));
  //Player::setItemCount(%player, LoadedAmmo, 0);
  $lastUnmount[Magnum] = getSimTime();
}

$lastUnmount[Magnum] = 0;
function Player::onNoAmmo(%player) {
  Player::trigger(%player, 0, false);
  schedule("if (getSimTime() - $lastUnmount[Magnum] > 2 && !Player::isDead("@%player@")) Magnum::loadClip("@%player@");", 2);
}

function Magnum::loadClip(%player) {
  %num = Player::getItemCount(%player, MagnumAmmo);
  if (%num > $ClipSize[Magnum]) %num = $ClipSize[Magnum];
  Player::decItemCount(%player, MagnumAmmo, %num);
  Player::setItemCount(%player, LoadedAmmo, %num);
}

registerWeapon(Magnum);
linkWeapon(Magnum);
setArmorAllowsItem(larmor, Magnum, 30);
setArmorAllowsItem(lfemale, Magnum, 30);