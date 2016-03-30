RocketData RocketTurretRocket {
   bulletShapeName  = "rocket.dts";
   explosionTag     = rocketExp;
   collisionRadius  = 0.0;
   mass             = 2.0;

   damageClass      = 1;       // 0 impact, 1, radius
   damageValue      = 150;
   damageType       = $MissileDamageType;

   explosionRadius  = 10;
   kickBackStrength = 500;
   muzzleVelocity   = 120;
   terminalVelocity = 120;
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

TurretData RocketTurret {
	maxDamage = 1000;
	maxEnergy = 1000;
	minGunEnergy = 0;
	maxGunEnergy = 0;
	reloadDelay = 0.75;
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
	projectileType = RocketTurretRocket;
	damageSkinData = "objectDamageSkins";
	shadowDetailMask = 8;
	explosionId = LargeShockwave;
	description = "Rocket Turret";
};


function RocketTurret::verifyTarget(%this, %target) {
  return true;
}