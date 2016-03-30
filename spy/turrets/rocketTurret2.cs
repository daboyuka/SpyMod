RocketData RocketTurret2Rocket {
   bulletShapeName  = "rocket.dts";
   explosionTag     = flashExpSmall;
   collisionRadius  = 0.0;
   mass             = 2.0;

   damageClass      = 1;       // 0 impact, 1, radius
   damageValue      = 150;
   damageType       = $MissileDamageType;

   explosionRadius  = 10;
   kickBackStrength = 500;
   muzzleVelocity   = 40;
   terminalVelocity = 40;
   acceleration     = 0;
   totalTime        = 0.25;
   liveTime         = 0.25;
   lightRange       = 5.0;
   lightColor       = { 1.0, 0.7, 0.5 };
   inheritedVelocityScale = 0;

   // rocket specific
   trailType   = 2;
   trailString = "breath.dts";
   smokeDist   = 1.8;

   soundId = SoundJetHeavy;
};

RocketData RocketTurret2Rocket2 {
   bulletShapeName  = "rocket.dts";
   explosionTag     = rocketExp;
   collisionRadius  = 0.0;
   mass             = 2.0;

   damageClass      = 1;       // 0 impact, 1, radius
   damageValue      = 45;
   damageType       = $MissileDamageType;

   explosionRadius  = 10;
   kickBackStrength = 100;
   muzzleVelocity   = 80;
   terminalVelocity = 80;
   acceleration     = 0;
   totalTime        = 10;
   liveTime         = 10;
   lightRange       = 5.0;
   lightColor       = { 1.0, 0.7, 0.5 };
   inheritedVelocityScale = 0;

   // rocket specific
   trailType   = 2;
   trailString = "breath.dts";
   smokeDist   = 1.8;

   soundId = SoundJetHeavy;
};

TurretData RocketTurret2 {
	maxDamage = 1000;
	maxEnergy = 1000;
	minGunEnergy = 0;
	maxGunEnergy = 0;
	reloadDelay = 1.5;
	fireSound = SoundPlasmaTurretFire;
	activationSound = SoundPlasmaTurretOn;
	deactivateSound = SoundPlasmaTurretOff;
	whirSound = SoundPlasmaTurretTurn;
	range = 1000;
	dopplerVelocity = 0;
	castLOS = true;
	supression = false;
	mapFilter = 2;
	mapIcon = "M_turret";
	visibleToSensor = true;
	debrisId = defaultDebrisMedium;
	className = "MountableTurret";
	shapeFile = "hellfiregun";
	shieldShapeName = "shield_medium";
	speed = 1.0;
	speedModifier = 1.0;
	projectileType = RocketTurret2Rocket;
	damageSkinData = "objectDamageSkins";
	shadowDetailMask = 8;
	explosionId = LargeShockwave;
	description = "Rocket Turret";
};

function RocketTurret2Rocket::onAdd(%this) {
  schedule("RocketTurret2Rocket::split("@%this@");", 0.25);
}

function RocketTurret2Rocket::split(%this) {
  if (!isObject(%this)) return;

  %pos = GameBase::getPosition(%this);
  %vec = Vector::getFromRot(GameBase::getRotation(%this), 15);
  for (%i = 0; %i < 5; %i++) {
    %vec2 = Vector::add(%vec, Vector::randomVec(-1,1,-1,1,-1,1));
    %rot = Vector::add(Vector::getRotation(%vec2),"1.57 0 0");
    %trans = Matrix::rotXYZ(getWord(%rot, 0), getWord(%rot, 1), getWord(%rot, 2)) @ " " @ %pos;
    %doom = Projectile::spawnProjectile(RocketTurret2Rocket2, %trans, -1, 0);//RocketTurret2Rocket2, %trans, -1, 0);
//    Doom::onU(%doom);
  }
}

function Doom::onU(%this) {
  if (!isObject(%this)) return;
  %vel = Vector::add(Item::getVelocity(%this), "0 0 "@(6/8));
  echo(%vel);
  %len = Vector::length(%vel);
  %dir = Vector::resize(Vector::sub(GameBase::getPosition(2049), GameBase::getPosition(%this)), 40);
  %vel = Vector::resize(Vector::add(%vel, %dir), %len);
  Item::setVelocity(%this, %vel);
  schedule("Doom::onU("@%this@");", 0.25);
}

function RocketTurret2::verifyTarget(%this, %target) {
  return true;
}