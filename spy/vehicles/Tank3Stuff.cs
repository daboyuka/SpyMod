ItemImageData Tank3Object1Image;
//	shapeFile = "gunturet";
//	mountPoint = 4;
//	mountOffset = "-2.5 -2 0";
//};

ItemImageData Tank3Object1Image {
	shapeFile = "gunturet";
	mountPoint = 4;
	mountOffset = "-2.5 -2.4 -0.25";
};

ItemData Tank3Object1 {
	shapeFile = "breath";
	imageType = Tank3Object1Image;
};

ItemImageData Tank3Object2Image {
	shapeFile = "magcargo";
	mountPoint = 4;
	mountOffset = "-2.5 -2.8 0.1";
	mountRotation = "0 0 1.57";
};

ItemData Tank3Object2 {
	shapeFile = "breath";
	imageType = Tank3Object2Image;
};










SoundData SoundFireTank3Turret {
   wavFileName = "BXplo2.wav";
   profile = Profile3dFar;
};
SoundData SoundTank3TurretShellExp {
   wavFileName = "shockExp.wav";
   profile = Profile3dFar;
};

ExplosionData Tank3TurretShellExp {
   shapeName = "mortarEx.dts";
   soundId   = SoundTank3TurretShellExp;

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

GrenadeData Tank3TurretShell {
   bulletShapeName    = "mortar.dts";
   explosionTag       = Tank3TurretShellExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.01;

   damageClass        = 1;       // 0 impact, 1, radius
   damageValue        = 200;
   damageType         = $ShrapnelDamageType;

   explosionRadius    = 20;
   kickBackStrength   = 400.0;
   maxLevelFlightDist = 200;
   totalTime          = 30.0;    // special meaning for grenades...
   liveTime           = 0;//1.0;
   projSpecialTime    = 0.05;

   inheritedVelocityScale = 0.5;

   smokeName              = "breath.dts";
};

ItemImageData Tank3TurretImage2 {
	shapeFile = "gunturet";
	mountPoint = 4;
	mountOffset = "0 0.5 -0.5";
	firstPerson = false;
};

ItemData Tank3Turret2 {
	shapeFile = "breath";
	imageType = Tank3TurretImage2;
};

ItemImageData Tank3TurretImage3 {
	shapeFile = "magcargo";
	mountPoint = 4;
	mountOffset = "0 -0.3 0";
	mountRotation = "0 0 1.57";
	firstPerson = false;
};

ItemData Tank3Turret3 {
	shapeFile = "breath";
	imageType = Tank3TurretImage3;
};

function mortarStuff() {

ItemImageData Tank3TurretImage2 {
	shapeFile = "mortargun";
	mountPoint = 0;
	mountOffset = "-0.4 0 0.25";
	firstPerson = false;
};

ItemData Tank3Turret2 {
	shapeFile = "breath";
	imageType = Tank3TurretImage2;
};

ItemImageData Tank3TurretImage3 {
	shapeFile = "mortargun";
	mountPoint = 0;
	mountOffset = "-0.4 0.5 0.25";
	firstPerson = false;
};

ItemData Tank3Turret3 {
	shapeFile = "breath";
	imageType = Tank3TurretImage3;
};

ItemImageData Tank3TurretImage4 {
	shapeFile = "mortargun";
	mountPoint = 0;
	mountOffset = "-0.4 1 0.25";
	firstPerson = false;
};

ItemData Tank3Turret4 {
	shapeFile = "breath";
	imageType = Tank3TurretImage4;
};

ItemImageData Tank3TurretImage5 {
	shapeFile = "mortargun";
	mountPoint = 0;
	mountOffset = "-0.4 1.5 0.25";
	firstPerson = false;
};

ItemData Tank3Turret5 {
	shapeFile = "breath";
	imageType = Tank3TurretImage5;
};

ItemImageData Tank3TurretImage6 {
	shapeFile = "magcargo";
	mountPoint = 4;
	mountOffset = "0 0 0";
	firstPerson = false;
};

ItemData Tank3Turret6 {
	shapeFile = "breath";
	imageType = Tank3TurretImage6;
};

}

ItemImageData Tank3TurretImage {
	shapeFile = "breath";
	mountPoint = 0;
	mountOffset = "-0.4 0 0.25";

	weaponType = 0;
	reloadTime = 2; 
	fireTime = 0;

	maxEnergy = 0;
	minEnergy = 0;

	accuFire = true;

	firstPerson = false;

	sfxFire = SoundFireTank3Turret;
	sfxActivate = SoundPlasmaTurretOn;
};

ItemData Tank3Turret {
	description = "Tank Cannon";
	className = "Weapon";
	shapeFile = "mortar";
	hudIcon = "mortar";
	heading = $InvHeading[Weapon];
	shadowDetailMask = 4;
	imageType = Tank3TurretImage;
	price = 125;
	showWeaponBar = false;
};

$ItemDescription[Tank3Turret] = "<jc><f1>Tank Cannon:<f0> Fires large explosive shells";

$Tank3::maxZ = 0.6;
$Tank3::minZ = -0.1;
$Tank3::precalc = sqrt(1-$Tank3::maxZ*$Tank3::maxZ);
$Tank3::precalc2 = sqrt(1-$Tank3::minZ*$Tank3::minZ);
function Tank3TurretImage::onFire(%player, %slot) {
  %trans = GameBase::getMuzzleTransform(%player);
  %y = Matrix::subMatrix(%trans, 3, 4, 3, 1, 0, 1);
  if (getWord(%y, 2) > $Tank3::maxZ) {
    %y = Vector::add(Vector::resize(getWord(%y, 0) @ " " @ getWord(%y, 1) @ " 0", $Tank3::precalc), "0 0 " @ $Tank3::maxZ);
    %x = Vector::cross(%y, "0 0 1");
    %z = Vector::cross(%x, %y);
    %pos = Matrix::subMatrix(%trans, 3, 4, 3, 1, 0, 3);
    %trans = %x @ " " @ %y @ " " @ %z @ " " @ %pos;
  }
  if (getWord(%y, 2) < $Tank3::minZ) {
    %y = Vector::add(Vector::resize(getWord(%y, 0) @ " " @ getWord(%y, 1) @ " 0", $Tank3::precalc2), "0 0 " @ $Tank3::minZ);
    %x = Vector::cross(%y, "0 0 1");
    %z = Vector::cross(%x, %y);
    %pos = Matrix::subMatrix(%trans, 3, 4, 3, 1, 0, 3);
    %trans = %x @ " " @ %y @ " " @ %z @ " " @ %pos;
  }
  Projectile::spawnProjectile(Tank3TurretShell, %trans, %player, 0);//Weapon::fireProjectile(%player, Tank3TurretShell);
}

function Tank3Turret::onMount(%player, %slot) {
  Weapon::displayDescription(%player, TankTurret);
  Player::mountItem(%player, Tank3Turret2, 3);
  Player::mountItem(%player, Tank3Turret3, 4);
//  Player::mountItem(%player, Tank3Turret4, 5);
//  Player::mountItem(%player, Tank3Turret5, 6);
//  Player::mountItem(%player, Tank3Turret6, 7);

  Weapon::standardMount(%player, Tank3Turret);
}
function Tank3Turret::onUnmount(%player, %slot) {
  Player::unmountItem(%player, 3);
  Player::unmountItem(%player, 4);
//  Player::unmountItem(%player, 5);
//  Player::unmountItem(%player, 6);
//  Player::unmountItem(%player, 7);
  Weapon::standardUnmount(%player, Tank3Turret);
}

setArmorAllowsItem(larmor, Tank3Turret, "", 0);
setArmorAllowsItem(lfemale, Tank3Turret, "", 0);
setArmorAllowsItem(SpyMale, Tank3Turret, "", 0);
setArmorAllowsItem(SpyFemale, Tank3Turret, "", 0);
setArmorAllowsItem(DMMale, Tank3Turret, "", 0);
setArmorAllowsItem(DMFemale, Tank3Turret, "", 0);
