exec("vehicles\\juggernaughtCannon.cs");
exec("vehicles\\juggernaughtBomber.cs");

GrenadeData JuggernaughtCrash {
   bulletShapeName    = "hover_apc.dts";
   explosionTag       = mortarExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.01;

   damageClass        = 1;       // 0 impact, 1, radius
   damageValue        = 100;
   damageType         = $ShrapnelDamageType;

   explosionRadius    = 25;
   kickBackStrength   = 150.0;
   maxLevelFlightDist = 0;
   totalTime          = 10.0;    // special meaning for grenades...
   liveTime           = 0;//1.0;
   projSpecialTime    = 0.05;

   inheritedVelocityScale = 0;

   smokeName              = "plasmatrail.dts";
};

GrenadeData JuggernaughtBomb {
   bulletShapeName    = "mortar.dts";
   explosionTag       = JuggernaughtBomberShellExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.01;

   damageClass        = 1;       // 0 impact, 1, radius
   damageValue        = 170;
   damageType         = $ExplosionDamageType;

   explosionRadius    = 25;
   kickBackStrength   = 250.0;
   maxLevelFlightDist = 0;
   totalTime          = 10;    // special meaning for grenades...
   liveTime           = 0;//1.0;
   projSpecialTime    = 0.05;

   inheritedVelocityScale = 1;

   smokeName              = "breath.dts";
};

FlierData Juggernaught {
	explosionId = LargeShockwave;
	debrisId = flashDebrisLarge;
	className = "Vehicle";
   shapeFile = "hover_apc";
   shieldShapeName = "shield_large";
   mass = 38.0;
   drag = 1.0;
   density = 1.2;
   maxBank = 0.2;
   maxPitch = 0.2;
   maxSpeed = 25;								   
   minSpeed = -5;
	lift = 0.35;
	maxAlt = 1500;
	maxVertical = 3;
	maxDamage = 500;
	damageLevel = {1.0, 1.0};
	maxEnergy = 100;
	accel = 0.08;

	groundDamageScale = 1.25;

	repairRate = 0;
	ramDamage = 100;
	ramDamageType = -1;
	mapFilter = 2;
	mapIcon = "M_vehicle";
//	fireSound = SoundFireFlierRocket;
//	reloadDelay = 3.0;
	damageSound = SoundTankCrash;
	visibleToSensor = true;
	shadowDetailMask = 1;

	mountSound = SoundFlyerMount;
	dismountSound = SoundFlyerDismount;
	idleSound = SoundFlyerIdle;
	moveSound = SoundFlyerActive;

	visibleDriver = true;//false;
	driverPose = 0;//23;
	description = "Juggernaught bomber";
};

setVehicleAllowsArmorToPilot(Juggernaught, larmor);
setVehicleAllowsArmorToPilot(Juggernaught, lfemale);
setVehicleAllowsArmorToPilot(Juggernaught, SpyMale);
setVehicleAllowsArmorToPilot(Juggernaught, SpyFemale);
setVehicleAllowsArmorToPilot(Juggernaught, DMMale);
setVehicleAllowsArmorToPilot(Juggernaught, DMFemale);
setVehicleAllowsArmorToRide(Juggernaught, larmor);
setVehicleAllowsArmorToRide(Juggernaught, lfemale);
setVehicleAllowsArmorToRide(Juggernaught, SpyMale);
setVehicleAllowsArmorToRide(Juggernaught, SpyFemale);
setVehicleAllowsArmorToRide(Juggernaught, DMMale);
setVehicleAllowsArmorToRide(Juggernaught, DMFemale);
setVehicleAllowsArmorToRide(Juggernaught, marmor);
setVehicleAllowsArmorToRide(Juggernaught, mfemale);
setVehicleAllowsArmorToRide(Juggernaught, harmor);
addSpecialSeatToVehicle(Juggernaught, 4, JuggernaughtCannon);
addSpecialSeatToVehicle(Juggernaught, 5, JuggernaughtCannon);
addSpecialSeatToVehicle(Juggernaught, 1, JuggernaughtCannon);
addSpecialSeatToVehicle(Juggernaught, 3, JuggernaughtBomber);

$Vehicle::numSeats[Juggernaught] = 4;
$Vehicle::driverSeat[Juggernaught] = 2;
$Vehicle::crashProjectile[Juggernaught] = JuggernaughtCrash;
$Vehicle::explodeDamage[Juggernaught] = 200;

$Vehicle::numWeps[Juggernaught] = 1;
$Vehicle::weaponDescription[Juggernaught, 0] = "<jc><f1>Bombs: <f0>Drops a three bomb salvo to annihilate anything below";

$DamageScales[Juggernaught, $BulletDamageType] = 0.15; // Like dude, bullets ain't gonna work...
$DamageScales[Juggernaught, $PlasmaDamageType] = 0.25;
$DamageScales[Juggernaught, $ImpactDamageType] = 0.1;

function Juggernaught::onAdd(%this) {
  Vehicle::onAdd(%this);
  Juggernaught::displayInfo(%this);
}

function Juggernaught::displayInfo(%this) {
  if (!isObject(%this) || GameBase::getDamageState(%this) == "Destroyed") return;
  if ((%client = GameBase::getControlClient(%this)) != -1) {
    if (GameBase::getLOSInfo(%this, 1000, Vector::add(GameBase::getRotation(%this), "-1.57 0 0")))
      %alt = getWord(Vector::sub(GameBase::getPosition(%this), $los::position), 2);
    else
      %alt = -1;

    %vel = Item::getVelocity(%this);
    %speed = Vector::length(%vel);
    %speed2 = Vector::length(getWord(%vel, 0) @ " " @ getWord(%vel, 1) @ " 0");

    %grav = 20;
    %estBombDist = %speed2 * sqrt(%alt / (%grav/2));

    bottomprint(%client, "<JC><F1>Altitude: <F0>" @ tern(%alt != -1, floor(%alt), "Indeterminable") @ "\n" @
                         "<F1>Speed: <F0>" @ floor(%speed) @ "\n" @
                         "<F1>Estmated bombing distance: <F0>" @ floor(%estBombDist), 1.2);
  }
  schedule("Juggernaught::displayInfo("@%this@");", 1);
}

function Juggernaught::onNextWeapon(%this, %wep) {
  Vehicle::displayWeaponDescription(%this);
}

function Juggernaught::onPrevWeapon(%this, %wep) {
  Vehicle::displayWeaponDescription(%this);
}

function Juggernaught::onFire(%this, %x) {
  if (%this.reloading) return;
  Juggernaught::dropBomb(%this);
  schedule("Juggernaught::dropBomb("@%this@");", 0.5);
  schedule("Juggernaught::dropBomb("@%this@");", 1);
  %this.reloading = true;
  schedule(%this @ ".reloading = false;", 3);
}

function Juggernaught::dropBomb(%this) {
  Weapon::fireOffsetProjectile(%this, JuggernaughtBomb, "0 2 -3");
}

function Juggernaught::onPlayerMount(%this, %player) {%player.damageVehicle = %this;}
function Juggernaught::onPlayerDismount(%this, %player) {if (%player.damageVehicle == %this) %player.damageVehicle = "";}
