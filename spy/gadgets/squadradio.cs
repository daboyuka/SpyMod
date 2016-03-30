ItemImageData SquadRadioImage {
	shapeFile = "grenade";
	mountPoint = 0;

	weaponType = 0;
	reloadTime = 0.3;
	fireTime = 0;
	minEnergy = 0;
	maxEnergy = 0;
	
	accuFire = false;

//	sfxFire = SoundFireGrappler;
//	sfxReload = SoundLoadGrappler;
	sfxActivate = SoundPickUpWeapon;
};

ItemData SquadRadio {
	description = "Suqad Command Radio";
	className = "Tool";
	shapeFile = "grenade";
	heading = $InvHeading[Gadget];
	shadowDetailMask = 4;
	imageType = SquadRadioImage;
	price = 125;
	showWeaponBar = false;
};

$ItemDescription[SquadRadio, 0] = "<jc><f1>Squad Radio, Follow Mode:<f0> Fire to command squad to follow you.";
$ItemDescription[SquadRadio, 1] = "<jc><f1>Squad Radio, Form Mode:<f0> Fire to command squad to form up where you are looking.";
$ItemDescription[SquadRadio, 2] = "<jc><f1>Squad Radio, Fire Mode:<f0> Fire to command squad to fire on your target.";
$ItemDescription[SquadRadio, 3] = "<jc><f1>Squad Radio, Cease Mode:<f0> Fire to command squad to cease action.";
$ItemDescription[SquadRadio, 4] = "<jc><f1>Squad Radio, Select Mode:<f0> Fire to select a solider for commands (miss to select squad).";
$NumModes[SquadRadio] = 5;

function SquadRadio::onMount(%player, %slot) {
  Weapon::displayDescription(%player, SquadRadio);
}
function SquadRadio::onUnmount(%player, %slot) {
  Weapon::clearDescription(%player);
}

function SquadRadioImage::onFire(%player, %slot) {
  %client = Player::getClient(%player);
  if (%client.squad == "") {
    Client::sendMessage(%client, 1, "You have no squad to command");
    return;
  }

  %mode = Weapon::getMode(%player, SquadRadio);
  if (%mode == 0) {
    remotePlayAnimWav(%client, 0, "regroup");
    SquadCommander::follow(%client);
//    if (%client.selectedAI == "") SquadCommander::follow(%client);
//    else                          SquadCommander::followSelected(%client);
  }
  if (%mode == 1) {
    remotePlayAnimWav(%client, 1, "moveout");
    %client.formCommand++;
    %client.formCommand %= 3;
    SquadCommander::formAtLOS(%client, %client.formCommand);
//    if (%client.selectedAI == "") SquadCommander::formAtLOS(%client, %client.formCommand);
//    else                          SquadCommander::formAtLOSSelected(%client);
  }
  if (%mode == 2) {
    remotePlayAnimWav(%client, 1, "firetgt");
    SquadCommander::fireOnLOS(%client);
//    if (%client.selectedAI == "") SquadCommander::fireOnLOS(%client);
//    else                          SquadCommander::fireOnLOSSelected(%client);
  }
  if (%mode == 3) {
    remotePlayAnimWav(%client, 3, "cease");
    SquadCommander::cease(%client);
//    if (%client.selectedAI == "") SquadCommander::cease(%client);
//    else                          SquadCommander::ceaseSelected(%client);
  }
  if (%mode == 4) {
    if (Player::isJetting(%player)) {
      if (GameBase::getLOSInfo(Client::getControlObject(%client), 300) && $los::object.aiName != "") {
        Squad::deselectAll(%client.squad);
        Squad::select(%client.squad, $los::object);
      } else {
        Squad::selectAll(%client.squad);
      }
    } else {
      if (GameBase::getLOSInfo(Client::getControlObject(%client), 300) && $los::object.aiName != "") {
        Squad::selectSwitch(%client.squad, $los::object);
      } else {
        Squad::selectSwitchAll(%client.squad);
      }
    }
  }
}

function SquadRadio::onModeChanged(%player) {
  Weapon::displayDescription(%player, SquadRadio);
}

//registerGadget(SquadRadio);
linkGadget(SquadRadio);
setArmorAllowsItem(larmor, SquadRadio);
setArmorAllowsItem(lfemale, SquadRadio);
setArmorAllowsItem(SpyMale, SquadRadio);
setArmorAllowsItem(SpyFemale, SquadRadio);
setArmorAllowsItem(DMMale, SquadRadio);
setArmorAllowsItem(DMFemale, SquadRadio);