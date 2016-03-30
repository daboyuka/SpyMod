exec("vehicles\\tankTurret.cs");

GrenadeData TankCrash {
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

FlierData Tank {
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
	ramDamage = 300;
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
	description = "SpyScout";
};

function blah() {

FlierData Tank {
   mass = 38.0;
   drag = 1.0;
   density = 10000.2;
   maxBank = 0.15;
   maxPitch = 0.01;
   maxSpeed = 10;
   minSpeed = 3;
	lift = 1.45;
	maxAlt = 150;
	maxVertical = 3;
	maxDamage = 500;
	damageLevel = {1.0, 1.0};
	maxEnergy = 100;
	accel = 0.03;
	};
}

setVehicleAllowsArmorToPilot(Tank, larmor);
setVehicleAllowsArmorToPilot(Tank, lfemale);
setVehicleAllowsArmorToPilot(Tank, SpyMale);
setVehicleAllowsArmorToPilot(Tank, SpyFemale);
setVehicleAllowsArmorToPilot(Tank, DMMale);
setVehicleAllowsArmorToPilot(Tank, DMFemale);
setVehicleAllowsArmorToRide(Tank, larmor);
setVehicleAllowsArmorToRide(Tank, lfemale);
setVehicleAllowsArmorToRide(Tank, marmor);
setVehicleAllowsArmorToRide(Tank, mfemale);
setVehicleAllowsArmorToRide(Tank, harmor);
setVehicleAllowsArmorToRide(Tank, SpyMale);
setVehicleAllowsArmorToRide(Tank, SpyFemale);
setVehicleAllowsArmorToRide(Tank, DMMale);
setVehicleAllowsArmorToRide(Tank, DMFemale);

$Vehicle::numSeats[Tank] = 1;
$Vehicle::driverSeat[Tank] = 1;
$Vehicle::crashProjectile[Tank] = TankCrash;

$Vehicle::numWeps[Tank] = 0;

$DamageScales[Tank, $ImpactDamageType] = 0;

function Tank::onNextWeapon(%this, %wep) {
  Vehicle::displayWeaponDescription(%this);
}

function Tank::onPrevWeapon(%this, %wep) {
  Vehicle::displayWeaponDescription(%this);
}

function Tank::onAdd(%this) {
  %turret = newObject("", Player, LightAIMale);
  Player::setMountObject(%turret, %this, 3);
  Player::mountItem(%turret, TankTurret, 0);


  %turret.damageVehicle = %this;
  %turret.tankTurret = true;
  %turret.weaponLock = true;

  %this.turret = %turret;

  Tank::update(%this);
}

function Tank::onCollision(%this, %object) {
	%name = GameBase::getDataName(%this);
	if (GameBase::getDamageLevel(%this) < %name.maxDamage) {
		if (getObjectType (%object) == "Player" && %object.vehicle == "" &&
                    (getSimTime() > %object.newMountTime || %object.lastMount != %this) && %this.fading == "") {

	        	if (Player::isAiControlled(%object)) return;

			%armor = Player::getArmor(%object);
			%client = Player::getClient(%object);


			%mountSlot = Vehicle::findEmptySeat(%this, %client, Vehicle::canArmorRide(%name, %armor),
                                                            Vehicle::canArmorPilot(%name, %armor) && Vehicle::canMount(%this, %object));

			if (%mountSlot == 1) {
				%weapon = Player::getMountedItem(%object,$WeaponSlot);
				if (%weapon != -1) {
					if (%weapon.className == "Weapon") %object.lastWeapon = %weapon;
					else if (%weapon.className == "Tool") %object.lastGadget = %weapon;
					Player::unMountItem(%object,$WeaponSlot);
				}
				Player::setMountObject(%object, %this, 1);
				Client::setControlObject(%client, %this);
				playSound(%name.mountSound, GameBase::getPosition(%this));
				%object.driver = 1;
				%object.vehicle = %this;
				%this.clLastMount = %client;
			} else if (%mountSlot > 1) {
				if (%mountSlot) {
					%object.vehicleSlot = %mountSlot;
					%object.vehicle = %this;
					Player::setMountObject(%object, %this, %mountSlot);
					playSound(%name.mountSound, GameBase::getPosition(%this));
					if (%mountSlot == 2) {
						%object.damageVehicle = %this;
						%this.turret.turretClient = Player::getClient(%object);
						%this.turret.turretObject = %object;
						echo(Player::getClient(%object), %this.turret);
						Client::setControlObject(Player::getClient(%object), %this.turret);
					}
					//if ($Vehicle::specialSeats[%name, %mountSlot] != "") 
					//	Vehicle::giveSpecialWeapon(%object, %this, %mountSlot);
				}
			} else if (GameBase::getControlClient(%this) == -1)
				Client::sendMessage(Player::getClient(%object),0,"You cannot pilot a "@%name@" while wearing "@%armor@" armor.~wError_Message.wav");
		}
	}
}

function Tank::passengerJump(%this,%passenger,%mom) {
	if (%passenger.tankTurret) {
		Client::setControlObject(%passenger.turretClient, %passenger.turretObject);
		Vehicle::passengerJump(%this, %passenger.turretObject, %mom, true);
		%passenger.turretObject.damageVehicle = "";
		%passenger.turretClient = 0;
		%passenger.turretObject = 0;
		return;
	} else {
          Vehicle::passengerJump(%this, %passenger, %mom, true);
          %passenger.damageVehicle = "";
        }
}

function Tank::onFire(%this, %x) {
  if (%this.reloading) return;
  %this.reloading = true;
  schedule(%this@".reloading = false;", 5);
}

function Tank::onDestroyed(%this) {
  if (%this.turret.turretObject && !Player::isDead(%this.turret.turretObject) && isObject(%this.turret.turretObject)) {
    Client::setControlObject(%this.turret.turretClient, %this.turret.turretObject);
    %this.turret.turretObject.damageVehicle = "";
    %this.turret.turretClient = 0;
    %this.turret.turretObject = 0;
    Vehicle::passengerJump(%this, %this.turret.turretObject, %mom, true);
  }

  deleteObject(%this.turret);
  Vehicle::onDestroyed(%this);
}

function Tank::update(%this) {
  if (!isObject(%this)) return;

  if (GameBase::getLOSInfo(%this, 4, "-1 0 0") || GameBase::getLOSInfo(%this, 4, "-2 0 0") || GameBase::getLOSInfo(%this, 4, "-0.6 0 0")) {
    GameBase::addPosition(%this, "0 0 " @ ((1.2 - getWord($los::normal, 2)) * Vector::length(Item::getVelocity(%this)) / 2));
  } else if (!GameBase::getLOSInfo(%this, 5, "-1 0 0") && !GameBase::getLOSInfo(%this, 5, "-2 0 0")) {
    %pos = Vector::add(GameBase::getPosition(%this), "0 0 -0.25");
    GameBase::setPosition(%this, %pos);
  }
  schedule("Tank::update("@%this@");", 0.0625);
}
function Tank::onPlayerMount(%this, %player) {}
