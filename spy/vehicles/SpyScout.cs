SoundData SoundFireSpyScoutBullet {
   wavFileName = "debris_medium.wav";
   profile = Profile3dMedium;
};
SoundData SoundFireSpyScoutRocket {
   wavFileName = "turretfire1.wav";
   profile = Profile3dMedium;
};
SoundData SoundFireSpyScoutFlares {
   wavFileName = "shockexp.wav";
   profile = Profile3dMedium;
};

GrenadeData SpyScoutCrash {
   bulletShapeName    = "flyer.dts";
   explosionTag       = GrenadeShellExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.01;

   damageClass        = 1;       // 0 impact, 1, radius
   damageValue        = 80;
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

GrenadeData SpyScoutSmoke {
   bulletShapeName    = "breath.dts";
   explosionTag       = noExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.45;

   damageClass        = 0;       // 0 impact, 1, radius
   damageValue        = 0;
   damageType         = 0;

   maxLevelFlightDist = 0;
   totalTime          = 0.01;    // special meaning for grenades...
   liveTime           = 0.01;
   projSpecialTime    = 0.05;

   inheritedVelocityScale = 0;

   smokeName              = "rsmoke.dts";
};

BulletData SpyScoutBullet {
   bulletShapeName    = "bullet.dts";
   explosionTag       = bulletExp0;
   expRandCycle       = 3;
   bulletHoleIndex    = 0;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 50;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 3000.0;
   totalTime          = 0.75;//0.25;
   liveTime           = 0.75;//0.25;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0;
   isVisible          = False;

   aimDefelction = 0.03;
   tracerPercentage   = 1.0;
   tracerLength       = 30;

   soundId = SoundJetLight;
};

SeekingMissileData SpyScoutMissile {
   bulletShapeName = "rocket.dts";
   explosionTag    = RocketLauncherRocketExp;
   collisionRadius = 0.0;
   mass            = 2.0;

   damageClass      = 1;       // 0 impact, 1, radius
   damageValue      = 100;
   damageType       = $MissileDamageType;
   explosionRadius  = 12;
   kickBackStrength = 200.0;

   muzzleVelocity    = 100.0;
   totalTime         = 5;
   liveTime          = 5;
   seekingTurningRadius    = 9;
   nonSeekingTurningRadius = 75.0;
   proximityDist     = 1.0;
   smokeDist         = 1000;

   lightRange       = 5.0;
   lightColor       = { 1, 0.8, 0 };

   inheritedVelocityScale = 0;

   soundId = SoundJetHeavy;
};

function SpyScoutMissile::updateTargetPercentage(%target) {
   return tern(%target.lockDiffusion != "", 1-%target.lockDiffusion, 0.9);
}

GrenadeData SpyScoutBomb {
   bulletShapeName    = "mortar.dts";
   explosionTag       = GrenadeShellExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.01;

   damageClass        = 1;       // 0 impact, 1, radius
   damageValue        = 120;
   damageType         = $ExplosionDamageType;

   explosionRadius    = 15;
   kickBackStrength   = 250.0;
   maxLevelFlightDist = 0;
   totalTime          = 5;    // special meaning for grenades...
   liveTime           = 0;//1.0;
   projSpecialTime    = 0.05;

   inheritedVelocityScale = 1;

   smokeName              = "breath.dts";
};

GrenadeData SpyScoutFlare {
   bulletShapeName    = "shotgunex.dts";
   explosionTag       = noExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.01;

   damageClass        = 0;       // 0 impact, 1, radius
   damageValue        = 0;
   damageType         = 0;

   maxLevelFlightDist = 0;
   totalTime          = 2;    // special meaning for grenades...
   liveTime           = 0;//1.0;
   projSpecialTime    = 0.05;

   inheritedVelocityScale = 1;

   smokeName              = "tumult_small.dts";
};

FlierData SpyScout {
	explosionId = flashExpLarge;
	debrisId = flashDebrisLarge;
	className = "Vehicle";
   shapeFile = "flyer";
   shieldShapeName = "shield_medium";
   mass = 9.0;
   drag = 1.0;
   density = 1.2;
   maxBank = 0.6;
   maxPitch = 0.6;
   maxSpeed = 40;
   minSpeed = -10;
	lift = 1;
	maxAlt = 2500;
	maxVertical = 10;
	maxDamage = 150;
	damageLevel = {1.0, 1.0};
	maxEnergy = 100;
	accel = 0.3;

	groundDamageScale = 10.0;

	reloadDelay = 2.5;
	repairRate = 0;
	fireSound = SoundFireFlierRocket;
	damageSound = SoundFlierCrash;
	ramDamage = 30;
	ramDamageType = -1;
	mapFilter = 2;
	mapIcon = "M_vehicle";
	visibleToSensor = true;
	shadowDetailMask = 2;

	mountSound = SoundFlyerMount;
	dismountSound = SoundFlyerDismount;
	idleSound = SoundFlyerIdle;
	moveSound = SoundFlyerActive;

	visibleDriver = false;
	driverPose = 22;
	description = "SpyScout";
};

setVehicleAllowsArmorToPilot(SpyScout, larmor);
setVehicleAllowsArmorToPilot(SpyScout, lfemale);
setVehicleAllowsArmorToPilot(SpyScout, SpyMale);
setVehicleAllowsArmorToPilot(SpyScout, SpyFemale);
setVehicleAllowsArmorToPilot(SpyScout, DMMale);
setVehicleAllowsArmorToPilot(SpyScout, DMFemale);

$Vehicle::numSeats[SpyScout] = 0;
$Vehicle::driverSeat[SpyScout] = 1;
$Vehicle::crashProjectile[SpyScout] = SpyScoutCrash;
$Vehicle::explodeDamage[SpyScout] = 120;

$Vehicle::numWeps[SpyScout] = 4;
$Vehicle::weaponDescription[SpyScout, 0] = "<jc><f1>Machine gun: <f0>Rapidly fires high caliber bullets";
$Vehicle::weaponDescription[SpyScout, 1] = "<jc><f1>Seeking missiles: <f0> Fires wing-mounted homing missiles";
$Vehicle::weaponDescription[SpyScout, 2] = "<jc><f1>Bombs: <f0> Drops explosive bombs that detonate on impact";
$Vehicle::weaponDescription[SpyScout, 3] = "<jc><f1>Flares: <f0> Deploys flares to confuse seeking missiles";

$LockOn::lockonTime[SpyScout] = 1;
$LockOn::lockExpireTime[SpyScout] = 1;

$DamageScales[SpyScout, $ExplosionDamageType] = 2.0;
$DamageScales[SpyScout, $MissileDamageType] = 2.0;
$DamageScales[SpyScout, $ShrapnelDamageType] = 2.0;
$DamageScales[SpyScout, $BulletDamageType] = 1;
$DamageScales[SpyScout, $ImpactDamageType] = 0.1;

function SpyScout::onNextWeapon(%this, %wep) {
  if (%wep == 1) LockOn::enableLockon(%this, SpyScout);
  else           LockOn::disableLockon(%this, SpyScout);
  Vehicle::displayWeaponDescription(%this);
}

function SpyScout::onPrevWeapon(%this, %wep) {
  if (%wep == 1) LockOn::enableLockon(%this, SpyScout);
  else           LockOn::disableLockon(%this, SpyScout);
  Vehicle::displayWeaponDescription(%this);
}

function SpyScout::onTargetLockAcquired(%this, %target) {
  %client = GameBase::getControlClient(%this);
  %name = GameBase::getDataName(%target);
  %type = getObjectType(%target);

  %yourTeam = GameBase::getTeam(%client);
  %theirTeam = GameBase::getTeam(%target);
  if (getObjectType(%target) == "Player") %theirTeam = Client::getApparentTeam(Player::getClient(%target));

  %allegence = tern(%yourTeam == %theirTeam && getNumTeams() > 1, "Friendly", "Enemy");

  if (%type == "Flier") {
    bottomprint(%client, "<jc>Target acquired: " @ %name.description @
                         " (" @ %allegence @ ")", 2);
  } else if (%type == "Player") {
    bottomprint(%client, "<jc>Target acquired: " @ Client::getName(Player::getClient(%target)), 2);
  } else {
    bottomprint(%client, "<jc>Target acquired: " @ %name.description, 2);
  }
}

function SpyScout::onTargetLockLost(%this, %target) {
  %client = GameBase::getControlClient(%this);
  bottomprint(%client, "<jc>Target lost", 2);
}

function SpyScout::onTargetLockLocked(%this, %target) {
  %client = GameBase::getControlClient(%this);
  %name = GameBase::getDataName(%target);
  %type = getObjectType(%target);

  %yourTeam = GameBase::getTeam(%client);
  %theirTeam = GameBase::getTeam(%target);
  if (getObjectType(%target) == "Player") %theirTeam = Client::getApparentTeam(Player::getClient(%target));

  %allegence = tern(%yourTeam == %theirTeam && getNumTeams() > 1, "Friendly", "Enemy");

  if (%type == "Flier" && GameBase::getControlClient(%target) != -1) {
    bottomprint(%client, "<jc>Target lock acquired: " @ %name.description @
                         " (" @ %allegence @ ")", 2);
  } else if (%type == "Player") {
    bottomprint(%client, "<jc>Target lock acquired: " @ Client::getName(Player::getClient(%target)), 2);
  } else {
    bottomprint(%client, "<jc>Target lock acquired: " @ %name.description, 2);
  }

  %client2 = GameBase::getControlClient(%target);
  if (%client2 != -1) {
    bottomprint(%client2, "<JC><F1>WARNING! You have been locked onto!", 3);
    Client::sendMessage(%client2, 1, "~wfloat_target.wav");
  }
}

function SpyScout::verifyTarget(%this, %target) {
  if (%this == %target || %target == Client::getOwnedObject(GameBase::getControlClient(%this)))
    return false; // lol, ya this might be good to have here...

  if (getObjectType(%target) != "Flier") return false;

  if (GameBase::getDamageState(%target) == "Destroyed") return false;

//  if (getObjectType(%target) == "Player")
//    if (Client::getControlObject(Player::getClient(%target)) != %target || getObjectType(Player::getMountObject(%target)) == "Flier") return false;

  if (Vector::getDistance(GameBase::getPosition(%this), GameBase::getPosition(%target)) > 200) return false;

  return true;
}

function SpyScout::onFire(%this, %x) {
  if (%this.reloading) return;
  %wep = Vehicle::getCurrentWeapon(%this);
  %vel = Item::getVelocity(%this);
  if (%wep == 0) {
    Projectile::spawnProjectile(ChallengerBullet1, GameBase::getMuzzleTransform(%this), %this, %vel);
    GameBase::playSound(%this, SoundFireSpyScoutBullet, 0);
    %this.reloading = true;
    schedule(%this @ ".reloading = false;", 0.1);
  } else if (%wep == 1) {
    %rot = GameBase::getRotation(%this);
    %xaxis = Vector::getFromRot(Vector::add(%rot, "0 0 -1.57"));
    %yaxis = Vector::getFromRot(%rot);
    %zaxis = Vector::getFromRot(Vector::add(%rot, "-1.57 0 0"));
    %matrix = %xaxis @ " " @ %yaxis @ " " @ %zaxis @ " " @
              Vector::add(Vector::add(GameBase::getPosition(%this), Vector::mul(%xaxis, tern(%this.rocketSide, -1.2, 1.4))),
                          Vector::mul(%zaxis, 0.4));

    %this.rocketSide++;
    %this.rocketSide %= 2;

    if (%this.lockonTargetLocked && %this.lockonTarget)
      Projectile::spawnProjectile(SpyScoutMissile, %matrix, %this, %vel, %this.lockonTarget);
    else
      Projectile::spawnProjectile(SpyScoutMissile, %matrix, %this, %vel, -1);

    GameBase::playSound(%this, SoundFireSpyScoutRocket, 0);
    %this.reloading = true;
    schedule(%this @ ".reloading = false;", 5);
  } else if (%wep == 2) {
    %rot = GameBase::getRotation(%this);
    %xaxis = Vector::getFromRot(Vector::add(%rot, "0 0 -1.57"));
    %yaxis = Vector::getFromRot(%rot);
    %zaxis = Vector::getFromRot(Vector::add(%rot, "-1.57 0 0"));
    %matrix = %xaxis @ " " @ %yaxis @ " " @ %zaxis @ " " @
              Vector::add(GameBase::getPosition(%this), Vector::mul(%xaxis, 0.1));

    Projectile::spawnProjectile(SpyScoutBomb, %matrix, %this, %vel);
    %this.reloading = true;
    schedule(%this @ ".reloading = false;", 3);
  } else if (%wep == 3) {
    %yaxis = Matrix::subMatrix(GameBase::getMuzzleTransform(%this), 3, 4, 3, 1, 0, 1);
    %offset = Vector::mul(%yaxis, -3);
    %vb = Vector::add("0 0 0", Vector::mul(%yaxis, -15));
    %v1 = Vector::add(%vb, Vector::randomVec2("-10 -10 -10", "10 10 10"));
    %v2 = Vector::add(%vb, Vector::randomVec2("-10 -10 -10", "10 10 10"));
    %v3 = Vector::add(%vb, Vector::randomVec2("-10 -10 -10", "10 10 10"));
    Projectile::spawnProjectile(SpyScoutFlare, "1 0 0 0 1 0 0 0 1 " @ Vector::add(getBoxCenter(%this), %offset), %this, %v1);
    Projectile::spawnProjectile(SpyScoutFlare, "1 0 0 0 1 0 0 0 1 " @ Vector::add(getBoxCenter(%this), %offset), %this, %v2);
    Projectile::spawnProjectile(SpyScoutFlare, "1 0 0 0 1 0 0 0 1 " @ Vector::add(getBoxCenter(%this), %offset), %this, %v3);
    GameBase::playSound(%this, SoundFireSpyScoutFlares, 0);

    %this.reloading = true;
    %this.lockDiffusion = 0.9;
    schedule(%this @ ".lockDiffusion = \"\";", 3);
    schedule(%this @ ".reloading = false;", 4);
  }
}

function SpyScout::onAdd(%this) {
  SpyScout::doSmoke(%this);
}

function SpyScout::doSmoke(%this) {
  if (!isObject(%this)) return;
  if (Vector::lengthSquared(Item::getVelocity(%this)) > 100) {
    %rot = GameBase::getRotation(%this);
    %trans = GameBase::getMuzzleTransform(%this);
    %xaxis = Matrix::subMatrix(%trans, 3, 4, 3, 1, 0, 0);//Vector::getFromRot(Vector::add(%rot, "0 0 -1.57"));
    %yaxis = Matrix::subMatrix(%trans, 3, 4, 3, 1, 0, 1);//Vector::getFromRot(%rot);
    %zaxis = Matrix::subMatrix(%trans, 3, 4, 3, 1, 0, 2);//Vector::getFromRot(Vector::add(%rot, "-1.57 0 0"));
    %matrix1 = %xaxis @ " " @ %yaxis @ " " @ %zaxis @ " " @
               Vector::add(Vector::add(GameBase::getPosition(%this), Vector::mul(%xaxis, -1.2)),
                           Vector::mul(%zaxis, 0.4));
    %matrix2 = %xaxis @ " " @ %yaxis @ " " @ %zaxis @ " " @
               Vector::add(Vector::add(GameBase::getPosition(%this), Vector::mul(%xaxis, 1.4)),
                           Vector::mul(%zaxis, 0.4));

    Projectile::spawnProjectile(SpyScoutSmoke, %matrix1,-1,0);
    Projectile::spawnProjectile(SpyScoutSmoke, %matrix2,-1,0);
  }
  schedule("SpyScout::doSmoke("@%this@");", 0.125);
}

function SpyScout::onPlayerMount(%this, %player) {}
//function SpyScout::onAdd(%this) {
//  GameBase::setRechargeRate(%this, 2);
//}
