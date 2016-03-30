TurretData TestTurret {
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
	shapeFile = "chainturret";
	shieldShapeName = "shield_medium";
	speed = 1.0;
	speedModifier = 1.0;
//	projectileType = RocketTurretRocket;
	damageSkinData = "objectDamageSkins";
	shadowDetailMask = 8;
	explosionId = LargeShockwave;
	description = "Rocket Turret";
};


function TestTurret::onFire(%this, %target) {
  echo("FIRING TURRET " @ %this @ ", and: " @ %target);
  //GameBase::playSequence(%this, 0, "fire");
}