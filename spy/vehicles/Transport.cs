GrenadeData TransportCrash {
   bulletShapeName    = "hover_apc.dts";
   explosionTag       = mortarExp;
   collideWithOwner   = True;
   ownerGraceMS       = 250;
   collisionRadius    = 0.2;
   mass               = 1.0;
   elasticity         = 0.01;

   damageClass        = 1;       // 0 impact, 1, radius
   damageValue        = 240;
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

FlierData Transport {
	explosionId = LargeShockwave;
	debrisId = flashDebrisLarge;
	className = "Vehicle";
   shapeFile = "hover_apc";
   shieldShapeName = "shield_large";
   mass = 38.0;
   drag = 1.0;
   density = 1.2;
   maxBank = 0.3;
   maxPitch = 0.3;
   maxSpeed = 35;								   
   minSpeed = -10;
	lift = 0.35;
	maxAlt = 1500;
	maxVertical = 5;
	maxDamage = 250;
	damageLevel = {1.0, 1.0};
	maxEnergy = 100;
	accel = 0.4;

	groundDamageScale = 0.125;

	repairRate = 0;
	ramDamage = 100;
	ramDamageType = -1;
	mapFilter = 2;
	mapIcon = "M_vehicle";
	damageSound = SoundTankCrash;
	visibleToSensor = true;
	shadowDetailMask = 2;

	mountSound = SoundFlyerMount;
	dismountSound = SoundFlyerDismount;
	idleSound = SoundFlyerIdle;
	moveSound = SoundFlyerActive;

	visibleDriver = false;
	driverPose = 23;
	description = "Infiltration Transport";
};

setVehicleAllowsArmorToPilot(Transport, larmor);
setVehicleAllowsArmorToPilot(Transport, lfemale);
setVehicleAllowsArmorToPilot(Transport, SpyMale);
setVehicleAllowsArmorToPilot(Transport, SpyFemale);
setVehicleAllowsArmorToPilot(Transport, DMMale);
setVehicleAllowsArmorToPilot(Transport, DMFemale);
setVehicleAllowsArmorToRide(Transport, larmor);
setVehicleAllowsArmorToRide(Transport, lfemale);
setVehicleAllowsArmorToRide(Transport, SpyMale);
setVehicleAllowsArmorToRide(Transport, SpyFemale);
setVehicleAllowsArmorToRide(Transport, DMMale);
setVehicleAllowsArmorToRide(Transport, DMFemale);
setVehicleAllowsArmorToRide(Transport, marmor);
setVehicleAllowsArmorToRide(Transport, mfemale);
setVehicleAllowsArmorToRide(Transport, harmor);

$Vehicle::numSeats[Transport] = 4;
$Vehicle::driverSeat[Transport] = 1;
$Vehicle::crashProjectile[Transport] = TransportCrash;

$Vehicle::numWeps[Transport] = 1;
$Vehicle::weaponDescription[Transport, 0] = "<jc><f1>Supply Drop: <f0> Drops any currently held supplies";

$Vehicle::customCollisionScript[Transport] = true;

$DamageScales[Transport, $BulletDamageType] = 0.3;

function Transport::onNextWeapon(%this, %wep) {
  Vehicle::displayWeaponDescription(%this);
}

function Transport::onPrevWeapon(%this, %wep) {
  Vehicle::displayWeaponDescription(%this);
}

function Transport::onPlayerMount(%this, %player) {
  Player::setItemCount(%player, ParachutePack, 1);
  Player::mountItem(%player, ParachutePack, $BackpackSlot);
}

function Transport::onFire(%this, %x) {
  if (%this.reloading) return;
  Transport::dropItems(%this);
  schedule(%this @ ".reloading = false;", 3);
}

function Transport::addDropItem(%this, %object) {
  if (%this.numDropItems == "") %this.numDropItems = 0;
  %this.dropItems[%this.numDropItems] = %object;
  %this.numDropItems++;
  %object.lastPickupTime = getSimTime();
  Item::hide(%object, true);
  GameBase::setPosition(%object, "0 0 1000000");
}

function Transport::onCollision(%this, %object) {
  echo(getObjectType(%object));
  if (getObjectType(%object) == "Item") { Transport::addDropItem(%this, %object); }
  else Vehicle::onCollision(%this, %object, true);
}

function Transport::dropItems(%this) {
  for (%i = 0; %i < %this.numDropItems; %i++) {
    %obj = %this.dropItems[%i];
    echo(%obj);
    Item::hide(%obj, false);
    GameBase::setPosition(%obj, Vector::add(GameBase::getPosition(%this), "0 0 -3"));
    Item::setVelocity(%obj, Item::getVelocity(%this));
    schedule("Item::pop("@%obj@");", $ItemPopTime);
  }  
  %this.numDropItems = 0;
}
