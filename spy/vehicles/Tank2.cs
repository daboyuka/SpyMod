exec("vehicles\\tank2Stuff.cs");

SoundData SoundFireTankCannon {
   wavFileName = "BXplo2.wav";
   profile = Profile3dFar;
};
SoundData SoundTankCannonShellExp {
   wavFileName = "shockExp.wav";
   profile = Profile3dFar;
};

ExplosionData TankCannonShellExp {
   shapeName = "mortarEx.dts";
   soundId   = SoundTankCannonShellExp;

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

GrenadeData TankCannonShell {
   bulletShapeName    = "mortar.dts";
   explosionTag       = TankCannonShellExp;
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
   maxLevelFlightDist = 800;
   totalTime          = 30.0;    // special meaning for grenades...
   liveTime           = 0;//1.0;
   projSpecialTime    = 0.05;

   inheritedVelocityScale = 0.5;

   smokeName              = "breath.dts";
};

GrenadeData Tank2Crash {
   bulletShapeName    = "hover_apc_sml.dts";
   explosionTag       = GrenadeShellExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.01;

   damageClass        = 1;       // 0 impact, 1, radius
   damageValue        = 140;
   damageType         = $ShrapnelDamageType;

   explosionRadius    = 10;
   kickBackStrength   = 150.0;
   maxLevelFlightDist = 0;
   totalTime          = 10.0;    // special meaning for grenades...
   liveTime           = 0;//1.0;
   projSpecialTime    = 0.05;

   inheritedVelocityScale = 0;

   smokeName              = "plasmatrail.dts";
};

FlierData Tank2 {
	explosionId = flashExpLarge;
	debrisId = flashDebrisLarge;
	className = "Vehicle";
   shapeFile = "hover_apc_sml";
   shieldShapeName = "shield_large";
   mass = 38.0;
   drag = 1.0;
   density = 1.2;
   maxBank = 0.15;
   maxPitch = 0.01;
   maxSpeed = 10;
   minSpeed = -5;
	lift = 0.1;
	maxAlt = 1500;
	maxVertical = 3;
	maxDamage = 500;
	damageLevel = {1.0, 1.0};
	maxEnergy = 100;
	accel = 0.08;

	groundDamageScale = 0;

	reloadDelay = 2.5;
	repairRate = 0;
	fireSound = SoundFireFlierRocket;
	damageSound = SoundFlierCrash;
	ramDamage = 0;
	ramDamageType = -1;
	mapFilter = 2;
	mapIcon = "M_vehicle";
	visibleToSensor = true;
	shadowDetailMask = 2;

	mountSound = SoundFlyerMount;
	dismountSound = SoundFlyerDismount;
	idleSound = SoundFlyerIdle;
	moveSound = SoundFlyerActive;

	visibleDriver = true;
	driverPose = 22;
	description = "Tank";
};

setVehicleAllowsArmorToPilot(Tank2, larmor);
setVehicleAllowsArmorToPilot(Tank2, lfemale);
setVehicleAllowsArmorToPilot(Tank2, SpyMale);
setVehicleAllowsArmorToPilot(Tank2, SpyFemale);
setVehicleAllowsArmorToPilot(Tank2, DMMale);
setVehicleAllowsArmorToPilot(Tank2, DMFemale);
setVehicleAllowsArmorToRide(Tank2, larmor);
setVehicleAllowsArmorToRide(Tank2, lfemale);
setVehicleAllowsArmorToRide(Tank2, marmor);
setVehicleAllowsArmorToRide(Tank2, mfemale);
setVehicleAllowsArmorToRide(Tank2, harmor);
setVehicleAllowsArmorToRide(Tank2, SpyMale);
setVehicleAllowsArmorToRide(Tank2, SpyFemale);
setVehicleAllowsArmorToRide(Tank2, DMMale);
setVehicleAllowsArmorToRide(Tank2, DMFemale);
addSpecialSeatToVehicle(Tank2, 2, Tank2Cannon);

$Vehicle::numSeats[Tank2] = 1;
$Vehicle::driverSeat[Tank2] = 1;
$Vehicle::crashProjectile[Tank2] = Tank2Crash;

$Vehicle::numWeps[Tank2] = 1;
$Vehicle::weaponDescription[Tank2, 0] = "<jc><f1>Tank Cannon: <f0>Fires massive explosive anti-ground vehicle shells";

$DamageScales[Tank2, $ImpactDamageType] = 0;

function Tank2::onNextWeapon(%this, %wep) {
  Vehicle::displayWeaponDescription(%this);
}

function Tank2::onPrevWeapon(%this, %wep) {
  Vehicle::displayWeaponDescription(%this);
}

function Tank2::onAdd(%this) {
  %objMounter = newObject("", Player, VehicleObjectMounter);
  Player::setMountObject(%objMounter, %this, 3);
  Player::mountItem(%objMounter, Tank2Object1, 3);
  Player::mountItem(%objMounter, Tank2Object2, 4);
  Player::mountItem(%objMounter, Tank2Object3, 5);


  %objMounter.damageVehicle = %this;

  %this.objMounter = %objMounter;

  Tank2::update(%this);
}

function Tank2::onFire(%this, %x) {
  if (%this.reloading) return;
  %this.reloading = true;
  Projectile::spawnProjectile(TankCannonShell, TransformMatrix::addToPos(GameBase::getMuzzleTransform(%this), Vector::add(Vector::getFromRot(GameBase::getRotation(%this), 2), "0 0 -0.5")),
                              %this, "0 0 25");

  GameBase::playSound(%this, SoundFireTankCannon, 0);

  schedule(%this@".reloading = false;", 3);
}

function Tank2::onDestroyed(%this) {
  schedule("deleteObject("@%this.objMounter@");", 0);
  Vehicle::onDestroyed(%this);
}

function Tank2::onDamage(%this,%type,%value,%pos,%vec,%mom,%object) {
  Vehicle::onDamage(%this, %type, %value, %pos, %vec, %mom, %object);
  if (GameBase::getDamageLevel(%this) / Tank2.maxDamage > 0.7) Effects::burn(%this, "-2 -2 1", "2 3 1", 5);
}

function Tank2::update(%this) {
  if (!isObject(%this)) return;

  if (GameBase::getLOSInfo(%this, 4, "-1 0 0") || GameBase::getLOSInfo(%this, 4, "-2 0 0") || GameBase::getLOSInfo(%this, 4, "-0.6 0 0")) {
    GameBase::addPosition(%this, "0 0 " @ ((1.2 - getWord($los::normal, 2)) * Vector::length(Item::getVelocity(%this)) / 2));
  } else if (!GameBase::getLOSInfo(%this, 5, "-1 0 0") && !GameBase::getLOSInfo(%this, 5, "-2 0 0")) {
    %pos = Vector::add(GameBase::getPosition(%this), "0 0 -0.25");
    GameBase::setPosition(%this, %pos);
  }
  schedule("Tank2::update("@%this@");", 0.0625);
}

function Tank2::onPlayerMount(%this, %player) {}