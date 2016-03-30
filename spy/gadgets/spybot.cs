ItemData SpyBotItem;

ItemImageData SpyBotItemImage {
   shapeFile  = "mortar";
	mountPoint = 0;

	weaponType = 0; // Single Shot
	reloadTime = 1;
	fireTime = 1;

	ammoType = SpyBotItem;

	accuFire = true;

	sfxFire = SoundThrowItem;
	sfxActivate = SoundPickUpWeapon;
};

ItemData SpyBotItem {
   heading = $InvHeading[Gadget];
	description = "SpyBot";
	className = "Tool";
   shapeFile  = "mortar";
	hudIcon = "blaster";
	shadowDetailMask = 4;
	imageType = SpyBotItemImage;
	price = 50;
	showWeaponBar = false;//true;
};

$ItemDescription[SpyBotItem] = "<jc><f1>SpyBot:<f0> A compact, hovering robot that can be remotely controlled";

function SpyBotItem::onMount(%player, %slot) { Weapon::displayDescription(%player, SpyBotItem); }

function SpyBotItemImage::onFire(%player, %slot) {
  %dir = Matrix::subMatrix(GameBase::getMuzzleTransform(%player), 4, 3, 1, 3, 1, 0);
  echo(%dir);
  echo(gameBase::getMuzzleTransform(%player));//Vector::getFromRot(GameBase::getRotation(%player), 1);
  %pos = Vector::add(GameBase::getPosition(%player), %dir);

  %client = Player::getClient(%player);
  if (SpyBot::deploy(%player, %pos)) {
    Client::sendMessage(%client, 1, "SpyBot deployed");
    Player::decItemCount(%player, SpyBotItem);
  } else {
    Client::sendMessage(%client, 1, "Cannot deploy a SpyBot there");
  }
}

//registerGadget(SpyBotItem);
//linkGadget(SpyBotItem);
//setArmorAllowsItem(larmor, SpyBotItem, SpyBotItem, 1);
//setArmorAllowsItem(lfemale, SpyBotItem, SpyBotItem, 1);
//setArmorAllowsItem(SpyMale, SpyBotItem, SpyBotItem, 1);
//setArmorAllowsItem(SpyFemale, SpyBotItem, SpyBotItem, 1);
//setArmorAllowsItem(DMMale, SpyBotItem, SpyBotItem, 1);
//setArmorAllowsItem(DMFemale, SpyBotItem, SpyBotItem, 1);

function SpyBot::deploy(%player, %pos) {
  %client = GameBase::getControlClient(%player);
  %obj = newObject("", Flier, SpyBot, true);
  if (!GameBase::testPosition(%obj, %pos)) {
    deleteObject(%obj);
    return false;
  }

  %team = GameBase::getTeam(%player);

  GameBase::setTeam(%obj, %team);
  GameBase::setPosition(%obj, %pos);
  GameBase::setRotation(%obj, GameBase::getRotation(%player));

  %p2 = newObject("", Player, larmor, false);
  GameBase::setTeam(%p2, %team);
  Player::setMountObject(%p2, %obj, 1);
  %p2.dummyMount = %obj;
  echo(%p2);

  %obj.controller = %client;
  %obj.controllerObj = %player;
  %obj.dummyGuy = %p2;

  Client::setControlObject(%client, %obj);
  Client::setOwnedObject(%client, %p2);

  return true;
}

FlierData SpyBot {
	explosionId = flashExpSmall;
	debrisId = flashDebrisSmall;
	className = "Vehicle";
   shapeFile = "camera";
   shieldShapeName = "shield";
   mass = 9.0;
   drag = 1.0;
   density = 1.2;
   maxBank = 0.5;
   maxPitch = 0.5;
   maxSpeed = 5;
   minSpeed = -2;
	lift = 0.75;
	maxAlt = 25;
	maxVertical = 2;
	maxDamage = 0.2;
	damageLevel = {1,1};
	maxEnergy = 100;
	accel = 0.4;

	groundDamageScale = 1.0;

//	projectileType = FlierRocket;
	reloadDelay = 2.0;
	repairRate = 0;
	fireSound = SoundFireFlierRocket;
	damageSound = SoundFlierCrash;
	ramDamage = 0.001;
	ramDamageType = -1;
	mapFilter = 2;
	mapIcon = "M_vehicle";
	visibleToSensor = true;
	shadowDetailMask = 2;

//	mountSound = SoundFlyerMount;
//	dismountSound = SoundFlyerDismount;
//	idleSound = SoundFlyerIdle;
//	moveSound = SoundFlyerActive;

	visibleDriver = false;
	driverPose = 22;
	description = "SpyBot";
};

function SpyBot::returnController(%this) {
  Client::setControlObject(%this.controller, %this.controllerObj);
  Client::setOwnedObject(%this.controller, %this.controllerObj);
}

function SpyBot::onCollision(%this, %object) {
  if (%this.controller != -1) return;

  if (getObjectType(%object) == "Player") {
    if (Item::giveItem(%object, SpyBotItem, 1)) {
      if (%this.dummyGuy) schedule("deleteObject("@%this.dummyGuy@");", 0.01);
      deleteObject(%this);
    }
  }
}

function SpyBot::jump(%this,%mom) {
  if (%this.controller == -1) return;

  SpyBot::returnController(%this);
  %this.controller = -1;
}

function SpyBot::onDestroyed(%this) {
  if (%this.controller != -1) {
    SpyBot::returnController(%this);
    %this.controller = -1;
  }
  if (%this.dummyGuy) schedule("deleteObject("@%this.dummyGuy@");", 0.01);
  echo("Ow.");
}

function SpyBot::onDamage(%this,%type,%value,%pos,%vec,%mom,%object) {
  StaticShape::onDamage(%this,%type,%value,%pos,%vec,%mom,%object);
}