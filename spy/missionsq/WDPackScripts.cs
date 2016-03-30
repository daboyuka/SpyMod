MoveableData fastelevator16x16Octa
{
	shapeFile = "elevator16x16_octo";
	className = "Elevator";
	maxDamage = 200.0;
	speed = 100;
	debrisId = defaultDebrisLarge;
	sfxStart = SoundElevatorStart;
	sfxStop = SoundElevatorStop;
	sfxRun = SoundElevatorRun;
	sfxBlocked = SoundElevatorBlocked;
	triggerRadius = 0;
	explosionId = debrisExpLarge;
   isPerspective = true;
	description = "Elevator";
};


MoveableData fastelevator6x6
{
	shapeFile = "elevator_6x6";
	className = "Elevator";
	destroyable = false;
	maxDamage = 200.0;
	speed = 100;
	debrisId = defaultDebrisLarge;
	explosionId = debrisExpLarge;
	sfxStart = SoundElevatorStart;
	sfxStop = SoundElevatorStop;
	sfxRun = SoundElevatorRun;
	sfxBlocked = SoundElevatorBlocked;
	triggerRadius = 0;
   isPerspective = true;
	description = "Elevator";
};

StaticShapeData BriefcasePart1 {
	shapeFile = "ammo1";
	debrisId = defaultDebrisSmall;
	explosionId = flashExpSmall;
	maxDamage = 1000.0;
   description = "Briefcase Part 1";
};

ItemImageData BriefcaseItemImage {
	shapeFile = "ammo1";
	mountPoint = 2;
	mountOffset = { 0, 0, 0 };
	mountRotation = { 1.57, 0, 0 };

	lightType = 2;   // Pulsing
	lightRadius = 4;
	lightTime = 1.5;
	lightColor = { 1, 1, 1};
};

ItemData BriefcaseItem {
	description = "Briefcase";
	shapeFile = "ammo1";
	className = "Flag";
	imageType = BriefcaseItemImage;
	showInventory = false;
	shadowDetailMask = 4;
   validateShape = true;

	lightType = 2;   // Pulsing
	lightRadius = 4;
	lightTime = 1.5;
	lightColor = { 1, 1, 1 };
};

RocketData FlareRocket {
   bulletShapeName = "plasmatrail.dts";
   explosionTag    = noExp;

   collisionRadius = 0.0;
   mass            = 2.0;

   damageClass      = 0;
   damageValue      = 0;
   damageType       = 0;

   muzzleVelocity   = 7.0;
   terminalVelocity = 7.0;
   acceleration     = 0;

   totalTime        = 5;
   liveTime         = 5;

   lightRange       = 5.0;
   lightColor       = { 1,0.5,0 };

   inheritedVelocityScale = 0;

   // rocket specific
   trailType   = 2;
   trailString = "plasmatrail.dts";
   smokeDist   = 1;

	timescale = 4;
};

GrenadeData FlareRocket2 {
   bulletShapeName    = "plasmatrail.dts";
   explosionTag       = noExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0;
   mass               = 1.0;
   elasticity         = 0.45;

   damageClass        = 1;       // 0 impact, 1, radius
   damageValue        = 0;
   damageType         = 0;

   explosionRadius    = 0;
   kickBackStrength   = 0;
   maxLevelFlightDist = 200;
   totalTime          = 4;    // special meaning for grenades...
   liveTime           = 0;
   projSpecialTime    = 0.05;

   inheritedVelocityScale = 0;

   smokeName              = "plasmaex.dts";
};

StaticShapeData BurningHPC {
	shapeFile = "hover_apc";
	debrisId = defaultDebrisSmall;
	maxDamage = 100000;
   description = "Burning Wreakage";
};

StaticShapeData BurningFlyer {
	shapeFile = "flyer";
	debrisId = defaultDebrisSmall;
	maxDamage = 100000;
   description = "Burning Wreakage";
};

SoundProfileData Profile3dOverThere {
   baseVolume = 0;
   minDistance = 0.01;
   maxDistance = 100000;
   flags = SFX_IS_HARDWARE_3D;
};

SoundData SoundWind2 {
   wavFileName = "wind1.wav";
   profile = Profile3dOverThere;
};

SoundData SoundWind3 {
   wavFileName = "wind2.wav";
   profile = Profile3dOverThere;
};