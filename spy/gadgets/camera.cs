TurretData CameraTurret {
	className = "Turret";
	shapeFile = "camera";
	maxDamage = 25;
	maxEnergy = 1000;
	speed = 20;
	speedModifier = 1.0;
	range = 50;
	sequenceSound[0] = { "deploy", SoundActivateMotionSensor };
	visibleToSensor = true;
	shadowDetailMask = 4;
	castLOS = true;
	supression = false;
	supressable = false;
	mapFilter = 2;
	mapIcon = "M_camera";
	debrisId = defaultDebrisSmall;
	FOV = 0.707;
	pinger = false;
	explosionId = debrisExpMedium;
	description = "Camera";
};

$Turret::catchWepChanges[CameraTurret] = true;

function CameraTurret::onAdd(%this) {
	schedule("CameraTurret::deploy(" @ %this @ ");",1,%this);
	if (GameBase::getMapName(%this) == "") {
		GameBase::setMapName (%this, "Camera");
	}
}

function CameraTurret::deploy(%this) {
	GameBase::playSequence(%this,1,"deploy");
}

function CameraTurret::onEndSequence(%this,%thread) {
	GameBase::setActive(%this,true);
}

function CameraTurret::onDestroyed(%this) {
	Turret::onDestroyed(%this);
	$TeamItemCount[GameBase::getTeam(%this) @ "Camera"]--;
}

function CameraTurret::onPrevWeapon(%this) {
  %g = nameToID("MissionCleanup\\Cameras");
  %n = Group::objectCount(%g);
  %beforeGood = -1;
  %afterGood = -1;
  %client = GameBase::getControlClient(%this);
  for (%i = 0; %i < %n; %i++) {
    %o = Group::getObject(%g, %i);
    if (Client::isObjectControllable(%client, %o)) {
      %beforeGood = %o;
    }
    if (Group::getObject(%g, %i) == %this) break;
  }
  if (%beforeGood == -1) {
    for (%i = %i+1; %i < %n; %i++) {
      %o = Group::getObject(%g, %i);
      if (Client::isObjectControllable(%client, %o)) {
        %afterGood = %o;
      }
    }
  }

  if (%beforeGood != -1) Client::takeControl(GameBase::getControlClient(%this), %beforeGood);
  else if (%afterGood != -1) Client::takeControl(GameBase::getControlClient(%this), %afterGood);
  else return;
}

function CameraTurret::onNextWeapon(%this) {
  %g = nameToID("MissionCleanup\\Cameras");
  %n = Group::objectCount(%g);
  %beforeGood = -1;
  %afterGood = -1;
  %client = GameBase::getControlClient(%this);
  for (%i = 0; %i < %n; %i++) {
    %o = Group::getObject(%g, %i);
    if (%beforeGood == -1) {
      if (Client::isObjectControllable(%client, %o)) %beforeGood = %o;
    }
    if (Group::getObject(%g, %i) == %this) break;
  }
  for (%i = %i+1; %i < %n; %i++) {
    %o = Group::getObject(%g, %i);
    if (Client::isObjectControllable(%client, %o)) {
      %afterGood = %o;
      break;
    }
  }

  if (%afterGood != -1) Client::takeControl(GameBase::getControlClient(%this), %afterGood);
  else if (%beforeGood != -1) Client::takeControl(GameBase::getControlClient(%this), %beforeGood);
  else return;
}

function CameraTurret::jump(%this) {
  Turret::checkOperator(%this);
}


ItemData Camera;

ItemImageData CameraImage {
   shapeFile  = "camera";
	mountPoint = 0;

	weaponType = 0; // Single Shot
	reloadTime = 0;
	fireTime = 0.25;

	ammoType = Camera;

	sfxActivate = SoundPickUpWeapon;
};

ItemData Camera {
  description = "Deployable MiniCam";

  shapeFile = "camera";
	shadowDetailMask = 4;

	lightType = 2;   // Pulsing
	lightRadius = 4;
	lightTime = 1.5;
	lightColor = { 0.3, 1, 0 };

	className = "Tool";
	imageType = CameraImage;
	heading = $InvHeading[Gadget];

	price = 300;
};

//$NumModes[Camera] = 1;
$ItemDescription[Camera] = "<jc><f1>MiniCam:<f0> A deployable surveilance camera that can stick to walls.";//"<jc><f1>MiniCam, Unnamed Mode:<f0> An anonymous deployable surveilance camera that can stick to walls.";
//$ItemDescription[Camera, 1] = "<jc><f1>MiniCam, Named Mode:<f0> A named deployable surveilance camera that can stick to walls.";

function Camera::onMount(%player, %item) {
  Weapon::displayDescription(%player, Camera);
}
function Camera::onUnmount(%player, %item) {
  Weapon::clearDescription(%player);
}

function CameraImage::onFire(%player, %slot) {
  if (Player::getItemCount(%player, Camera) == 0) return;
  Player::deployItem(%player, Camera);
}

function Camera::onDeploy(%player,%item,%pos) {
	if (Camera::deploy(%player,%item)) {
		Player::decItemCount(%player,%item);
	}
}

function Camera::deploy(%player, %item) {
 	%client = Player::getClient(%player);
	if ($TeamItemCount[GameBase::getApparentTeam(%player) @ %item] < $TeamItemMax[%item]) {
		if (GameBase::getLOSInfo(%player,3)) {
			%obj = getObjectType($los::object);
			if (%obj == "SimTerrain" || %obj == "InteriorShape") {
				if (checkDeployArea(%client,$los::position)) {
					%rot = Vector::getRotation($los::normal);
					%camera = newObject("Camera","Turret",CameraTurret,true);
					%camera.deployerClient = %client;
					addToSet("MissionCleanup", %camera);

					%cgroup = nameToID("MissionCleanup\\Cameras");
					addToSet(%cgroup, %camera);

					GameBase::setTeam(%camera,GameBase::getApparentTeam(%player));
					GameBase::setRotation(%camera,%rot);
					GameBase::setPosition(%camera,$los::position);
					GameBase::setMapName(%camera,"Camera#"@ $totalNumCameras++ @ " " @ Client::getName(%client));
					Client::sendMessage(%client,0,"Camera deployed");
					playSound(SoundPickupBackpack,$los::position);
					$TeamItemCount[GameBase::getTeam(%camera) @ "Camera"]++;

					if (Weapon::getMode(%player, Camera) == 1) {
						bottomprint(%client, "<JC><F1>Say what you want to name this camera (your chat is blocked)");
						%client.redirectChat = "Camera::setName";
						%client.namingCamera = %camera;
					}

					echo("MSG: ",%client," deployed a Camera");
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

function Camera::setName(%client, %name) {
  %name = String::getSubStr(%name, 0, 32);
  %camera = %client.namingCamera;
  GameBase::setMapName(%camera, %name);
  bottomprint(%client, "<JC><F1>Camera named " @ %name @ ". Your chat is now unblocked");
  %client.redirectChat = "";
}

$TeamItemMax[Camera] = 10;

registerGadget(Camera, Camera, 1);
linkGadget(Camera);
setArmorAllowsItem(larmor, Camera, 3);
setArmorAllowsItem(lfemale, Camera, 3);
setArmorAllowsItem(SpyMale, Camera, 3);
setArmorAllowsItem(SpyFemale, Camera, 3);
setArmorAllowsItem(DMMale, Camera, 3);
setArmorAllowsItem(DMFemale, Camera, 3);