BulletData MachineGunTurretBullet {
   bulletShapeName    = "bullet.dts";
   explosionTag       = bulletExp0;
   expRandCycle       = 3;
   bulletHoleIndex    = 0;

   damageClass        = 0;                 // 0 = impact, 1 = radius
   damageValue        = 20;
   damageType         = $BulletDamageType;

   muzzleVelocity     = 1000.0;
   totalTime          = 4.5;
   liveTime           = 4.5;
   lightRange         = 1.0;
   lightColor         = { 1, 1, 0 };
   inheritedVelocityScale = 0.3;
   isVisible          = False;

   aimDeflection      = 0.005;
   tracerPercentage   = 1.0;
   tracerLength       = 15;

   soundId = SoundJetLight;
};

TurretData MachineGunTurret {
	maxDamage = 500;
	maxEnergy = 0;
	minGunEnergy = 0;
	maxGunEnergy = 0;
	reloadDelay = 0.05;
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
	shapeFile = "hellfiregun";//"hellfiregun";
	shieldShapeName = "shield_medium";
	speed = 1.0;
	speedModifier = 1.0;
	projectileType = MachineGunTurretBullet;
	damageSkinData = "objectDamageSkins";
	shadowDetailMask = 8;
	explosionId = LargeShockwave;
	description = "Machine Gun Turret";
};