StaticShapeData MotionSensorDevice {
   description = "Motion Sensor";
   shapeFile = "shieldPack";
//   sfxAmbient = SoundGeneratorPower;
	debrisId = flashDebrisSmall;
	explosionId = flashExpSmall;
   maxDamage = 10;
	visibleToSensor = true;
	mapFilter = 4;
	mapIcon = "M_generator";
	damageSkinData = "objectDamageSkins";
	shadowDetailMask = 16;
	collisionRadius = 0.25;
};

function MotionSensorDevice::onAdd(%this) {
	schedule("MotionSensorDevice::deploy(" @ %this @ ");",1,%this);
	if (GameBase::getMapName(%this) == "") {
		GameBase::setMapName (%this, "Motion Sensor");
	}
}

function MotionSensorDevice::deploy(%this) {
//  GameBase::playSequence(%this,1,"activation");
  schedule("MotionSensorDevice::sense("@%this@");", 0);
}

function MotionSensorDevice::onDestroyed(%this) {
	StaticShape::onDestroyed(%this);
	$TeamItemCount[GameBase::getTeam(%this) @ "MotionSensor"]--;
}

$MotionSensorDevice::senseInterval = 0.25;
$MotionSensorDevice::senseRadius = 20;
$MotionSensorDevice::ignoreTime = 3.0;
$MotionSensorDevice::speedThreshold = 3.0;
function MotionSensorDevice::sense(%this) {
  if (!isObject(%this) || GameBase::getDamageState(%this) == "Destroyed") return;

  if (%this.noMessagesUntil != "" && getSimTime() < %this.noMessagesUntil) {
    schedule("MotionSensorDevice::sense("@%this@");", %this.noMessagesUntil - getSimTime());
    return;
  }

  %pos = GameBase::getPosition(%this);
  %rad = $MotionSensorDevice::senseRadius;

  %set = newObject("MotionSet", SimSet);
  containerBoxFillSet(%set, $SimPlayerObjectType, %pos, 2*%rad, 2*%rad, 2*%rad, 0);

  %thisTeam = GameBase::getTeam(%this);
  for (%i = 0; %i < Group::objectCount(%set); %i++) {
    %obj = Group::getObject(%set, %i);
    %objTeam = GameBase::getApparentTeam(%obj);
    %objPos = getBoxCenter(%obj);

    if (getNumTeams() == 1 || %objTeam != %thisTeam) {
      if (Vector::length(Item::getVelocity(%obj)) >= $MotionSensorDevice::speedThreshold) {
        if (Vector::getDistance(%objPos, %pos) < $MotionSensorDevice::senseRadius) {
          if (getLOSInfo(%pos, %objPos, ~0) && $los::object == %obj) {
            teamMessages(1, %thisTeam, "Motion Sensor: Enemy detected!~wfloat_target.wav");
            %this.noMessagesUntil = getSimTime() + $MotionSensorDevice::ignoreTime;
            break;
          }
        }
      }
    }
  }

  deleteObject(%set);

  schedule("MotionSensorDevice::sense("@%this@");", $MotionSensorDevice::senseInterval);
}



ItemData MotionSensor;

ItemImageData MotionSensorImage {
   shapeFile  = "shieldPack";
	mountPoint = 0;

	weaponType = 0; // Single Shot
	reloadTime = 0;
	fireTime = 0.25;

	ammoType = MotionSensor;

	sfxActivate = SoundPickUpWeapon;
};

ItemData MotionSensor {
  description = "Motion Sensor";

  shapeFile = "shieldPack";
	shadowDetailMask = 4;

	lightType = 2;   // Pulsing
	lightRadius = 4;
	lightTime = 1.5;
	lightColor = { 0.3, 1, 0 };

	className = "Tool";
	imageType = MotionSensorImage;
	heading = $InvHeading[Gadget];

	price = 300;
};

$ItemDescription[MotionSensor] = "<jc><f1>Motion Sensor:<f0> A deployable device that alarm when it detects an enemy.";//"<jc><f1>MiniCam, Unnamed Mode:<f0> An anonymous deployable surveilance MotionSensor that can stick to walls.";

function MotionSensor::onMount(%player, %item) {
  Weapon::displayDescription(%player, MotionSensor);
}
function MotionSensor::onUnmount(%player, %item) {
  Weapon::clearDescription(%player);
}

function MotionSensorImage::onFire(%player, %slot) {
  if (Player::getItemCount(%player, MotionSensor) == 0) return;
  Player::deployItem(%player, MotionSensor);
}

function MotionSensor::onDeploy(%player,%item,%pos) {
	if (MotionSensor::deploy(%player,%item)) {
		Player::decItemCount(%player,%item);
	}
}

function MotionSensor::deploy(%player, %item) {
 	%client = Player::getClient(%player);
	if ($TeamItemCount[GameBase::getApparentTeam(%player) @ %item] < $TeamItemMax[%item]) {
		if (GameBase::getLOSInfo(%player,3)) {
			%obj = getObjectType($los::object);
			if (%obj == "SimTerrain" || %obj == "InteriorShape") {
				if (checkDeployArea(%client,$los::position)) {
					%rot = Vector::add(Vector::getRot($los::normal), $PI @ " " @ $PI @ " 0");
					%msensor = newObject("MotionSensor","StaticShape",MotionSensorDevice,true);

					addToSet(MissionCleanup, %msensor);

					GameBase::setTeam(%msensor,GameBase::getApparentTeam(%player));
					GameBase::setRotation(%msensor,%rot);
					GameBase::setPosition(%msensor,Vector::add($los::position, Vector::mul($los::normal, 0.25)));
					GameBase::setMapName(%msensor,"MotionSensor#"@ $totalNumMotionSensors++ @ " " @ Client::getName(%client));
					Client::sendMessage(%client,0,"Motion Sensor deployed");
					playSound(SoundPickupBackpack,$los::position);
					$TeamItemCount[GameBase::getTeam(%msensor) @ "MotionSensor"]++;

					echo("MSG: ",%client," deployed a MotionSensor");
					return true;
				}
			} else {
				Client::sendMessage(%client, 0, "Can only deploy on terrain or buildings");
			}
		} else {
			Client::sendMessage(%client, 0, "Deploy position out of range");		
		}
	} else {
		Client::sendMessage(%client, 0, "Deployable Item limit reached for " @ %item.description @ "s");
	}	
	return false;
}

$TeamItemMax[MotionSensor] = 3;

registerGadget(MotionSensor, MotionSensor, 1);
linkGadget(MotionSensor);
setArmorAllowsItem(larmor, MotionSensor, 1);
setArmorAllowsItem(lfemale, MotionSensor, 1);
setArmorAllowsItem(SpyMale, MotionSensor, 1);
setArmorAllowsItem(SpyFemale, MotionSensor, 1);
setArmorAllowsItem(DMMale, MotionSensor, 1);
setArmorAllowsItem(DMFemale, MotionSensor, 1);