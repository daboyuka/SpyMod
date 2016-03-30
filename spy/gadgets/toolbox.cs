function comment() {
ItemImageData ToolboxImage2 {
	shapeFile = "mortar";
	mountPoint = 0;
	mountOffset = "-0.15 0 0";
};
ItemData Toolbox2 {
	shapeFile = "mortar";
	imageType = ToolboxImage2;
};

ItemImageData ToolboxImage3 {
	shapeFile = "sensor_small";
	mountPoint = 0;
	mountOffset = "0 0 0.05";
};
ItemData Toolbox3 {
	shapeFile = "sensor_small";
	imageType = ToolboxImage3;
};

ItemData Toolbox;
}

RepairEffectData ToolboxTargetBolt {
   bitmapName       = "bar00.bmp";
   boltLength       = 3;
   segmentDivisions = 1;
   beamWidth        = 0;

   updateTime   = 1000;
   skipPercent  = 0.6;
   displaceBias = 0.15;
};

ItemImageData ToolboxImage {
   shapeFile  = "ammopack";
	mountPoint = 0;
	mountOffset = "0 0 0";
	mountRotation = "-1.57 0 0";

	weaponType = 2; // Sustained
	reloadTime = 0;
	fireTime = 1;

	projectileType = ToolboxTargetBolt;

	minEnergy = 0;
	maxEnergy = 0;

	sfxActivate = SoundPickUpWeapon;
};

ItemData Toolbox {
  description = "Toolbox";

  shapeFile = "ammopack";
	shadowDetailMask = 4;

	lightType = 2;   // Pulsing
	lightRadius = 4;
	lightTime = 1.5;
	lightColor = { 0.3, 1, 0 };

	className = "Tool";
	imageType = ToolboxImage;
	heading = $InvHeading[Gadget];

	price = 400;
};

$ItemDescription[Toolbox] = "<jc><f1>Toolbox, Repair Mode:<f0> Fire at an object to repair it.";

$Toolbox::repairRate = 20;

function ToolboxTargetBolt::onAcquire(%blah, %this, %target) {
  if (%this.repairTarget != "") return;
  if (%target == %this || %target == -1) return;

  echo("Target: " @ %target);

  %client = Player::getClient(%this);
  %type = getObjectType(%target);
  %name = GameBase::getDataName(%target);
  %desc = tern(%type != "Player", %name.description, Client::getName(Player::getClient(%target)));

  if (getObjectType(%target) != "StaticShape" &&getObjectType(%target) != "Turret" &&
      getObjectType(%target) != "Sensor" && getObjectType(%target) != "Moveable" &&
      getObjectType(%target) != "Flier" && !%target.notRepairable) {
    bottomprint(%client, "<jc><f0>Cannot repair " @ %desc, 3);
    echo(getObjectType(%target));
    return;
  }

  if (GameBase::getDamageLevel(%target) == 0) {
    bottomprint(%client, "<jc><f0>" @ %desc @ " is already fully repaired", 3);
    return;
  }

  bottomprint(%client, "<jc><f0>Repairing " @ %desc @ "...", 2);

  GameBase::setAutoRepairRate(%target, GameBase::getAutoRepairRate(%target)+$Toolbox::repairRate);
  %this.repairTarget = %target;

  Player::setAnimation(%this, 48);
}

function ToolboxTargetBolt::onRelease(%blah, %this, %target) {
  %target = %this.repairTarget;
  if (%target == "") return;

  %client = Player::getClient(%this);
  %name = GameBase::getDataName(%target);
  %desc = %name.description;

  bottomprint(%client, "<jc><f0>Stopped repairing " @ %desc, 2);

  GameBase::setAutoRepairRate(%target, GameBase::getAutoRepairRate(%target)-$Toolbox::repairRate);

  %this.repairTarget = "";

  Player::setAnimation(%this, 0);
}

function ToolboxTargetBolt::checkDone(%blah, %this) {
  %target = %this.repairTarget;
  if (%target == "") return;

  %client = Player::getClient(%this);
  %name = GameBase::getDataName(%target);
  %desc = %name.description;
  %maxD = %name.maxDamage;
  %pct = floor((%maxD - GameBase::getDamageLevel(%target)) / %maxD * 100);
  bottomprint(%client, "<jc><f0>Repair on " @ %desc @ " " @ %pct @ "% complete", 3);

  if (%pct == 100) Player::trigger(%this, 0, false);
}

function Toolbox::onMount(%player, %item) {
  Weapon::displayDescription(%player, Toolbox);
//  Player::mountItem(%player, Toolbox2, 3);
//  Player::mountItem(%player, Toolbox3, 4);
}
function Toolbox::onUnmount(%player, %item) {
//  Player::unmountItem(%player, 3);
//  Player::unmountItem(%player, 4);
  Weapon::clearDescription(%player);
}

registerGadget(Toolbox);
linkGadget(Toolbox);
setArmorAllowsItem(larmor, Toolbox);
setArmorAllowsItem(lfemale, Toolbox);
setArmorAllowsItem(SpyMale, Toolbox);
setArmorAllowsItem(SpyFemale, Toolbox);
setArmorAllowsItem(DMMale, Toolbox);
setArmorAllowsItem(DMFemale, Toolbox);