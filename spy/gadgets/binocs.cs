ItemImageData BinocularsImage
{
   shapeFile  = "breath";
	mountPoint = 0;
	mountOffset = "0 -0.2 0";

	weaponType = 0; // Single Shot
	reloadTime = 0;
	fireTime = 0.125;

	firstPerson = false;

	sfxActivate = SoundPickUpWeapon;
};

ItemData Binoculars {
  description = "Binoculars";

  shapeFile = "sensor_small";
	shadowDetailMask = 4;

	lightType = 2;   // Pulsing
	lightRadius = 4;
	lightTime = 1.5;
	lightColor = { 0.3, 1, 0 };

	className = "Tool";
	imageType = BinocularsImage;
	heading = $InvHeading[Gadget];

	price = 150;
};

$ItemDescription[Binoculars] = "<jc><f1>Binoculars:<f0> Allows long range reconnaissance";

function Binoculars::onMount(%player, %item) {
  Weapon::displayDescription(%player, Binoculars);
  if (!%player.usingBinocs) Binoculars::activate(%player);
}
function Binoculars::onUnmount(%player, %item) {
  if (%player.usingBinocs) Binoculars::deactivate(%player);
}

function Binoculars::activate(%player) {
  %player.usingBinocs = true;
  Binoculars::setInfoIdle(%player, -1);
}
function Binoculars::deactivate(%player) {
  %player.usingBinocs = false;
  centerprint(GameBase::getOwnerClient(%player), "", 0.01);
}



function Binoculars::setInfoIdle(%player, %targetId) {
  if (!%player.usingBinocs) return;
  %client = GameBase::getOwnerClient(%player);
  centerprint(%client, "<jc>Binoculars\n\x97\x97\x97\x97\x97\x97   \x97\x97\x97\x97\x97\x97\n ", 0);
}

function Binoculars::setInfoPlayerOld(%player, %targetId) {
  %client = GameBase::getOwnerClient(%player);

  %health = "<f2>";
  %damage = GameBase::getDamageLevel(%targetId);
  %maxDamage = GameBase::getDataName(%targetId).maxDamage;
  for (%i = 15; %i >= 0; %i--) {
    if (%damage <= %maxDamage * (%i/15)) { %health = %health @ "1"; }
    else break;
  }

  %wep = Player::getMountedItem(%targetId, 0);
  %wepDesc = %wep.description;

  centerprint(%client, "<jl>      " @ Client::getName(GameBase::getOwnerClient(%targetId)) @
                       "<jr><f0>1111111111111111      " @ %health @ "<f0>" @
                       "\n<jc>\x97\x97\x97\x97\x97\x97   \x97\x97\x97\x97\x97\x97\n" @
                       "<jl>      Weapon: " @ %wepDesc, 0);
}

function Binoculars::setInfoPlayer(%player, %targetId) {
  %client = GameBase::getOwnerClient(%player);

  %damage = GameBase::getDamageLevel(%targetId);
  %maxDamage = GameBase::getDataName(%targetId).maxDamage;
  %healthPct = floor((1 - (%damage / %maxDamage)) * 100);

  %armor = GameBase::getEnergy(%targetId) - 5;
  %maxArmor = GameBase::getDataName(%targetId).maxEnergy - 5;
  %armorPct = floor((%armor / %maxArmor) * 100);

  %wep = Player::getMountedItem(%targetId, 0);
  %wepDesc = tern(%wep != -1, %wep.description, "Unarmed");

  %yourTeam = Client::getTeam(%client);
  %hisTeam = GameBase::getApparentTeam(%targetId);
  if (getNumTeams() == 1) {
    %teamStr = "<f1>Enemy<f0>";
  } else {
    if (%yourTeam == %hisTeam) {
      %teamStr = "<f2>Friendly<f0>";
    } else {
      %teamStr = "<f2>Enemy<f0> " @ tern(%hisTeam != -1, "("@$teamName[%hisTeam]@")", "");
    }
  }

  %hisName = Client::getName(GameBase::getOwnerClient(%targetId));

  centerprint(%client, "<jc><f1>Targeting \"" @ %hisName @ "\"<f0>" @

                       "\n" @

                       "<jc>\x97\x97\x97\x97\x97\x97   \x97\x97\x97\x97\x97\x97" @

                       "\n" @

                       "<jc><f0>" @ %teamStr @
                       "<jl><f0>Health: " @ %healthPct @ "%, Armor: " @ %armorPct @ "%" @ "      " @
                       "<jr><f0>Weapon: " @ %wepDesc @ "      ", 0);
}

function Binoculars::setInfoVehicle(%player, %targetId) {
  %client = GameBase::getOwnerClient(%player);

  %damage = GameBase::getDamageLevel(%targetId);
  %maxDamage = GameBase::getDataName(%targetId).maxDamage;
  %healthPct = floor((1 - (%damage / %maxDamage)) * 100);

  %vehName = GameBase::getDataName(%targetId);
  %vehDesc = aOrAn(%vehName.description, false) @ " " @ %vehName.description;

  %yourTeam = Client::getTeam(%client);
  %hisTeam = GameBase::getApparentTeam(%targetId);

  echo(%client, ",", %targetId, ",", %yourTeam, ",", %hisTeam);

  if (getNumTeams() == 1) {
    %teamStr = "<f1>Enemy<f0>";
  } else {
    if (%yourTeam == %hisTeam) {
      %teamStr = "<f2>Friendly<f0>";
    } else {
      %teamStr = "<f2>Enemy<f0> " @ tern(%hisTeam != -1, "("@$teamName[%hisTeam]@")", "");
    }
  }

  %riders = Vehicle::countRiders(%targetId, true);
  %ridersStr = tern(%riders > 0, %riders @ " personel onboard", "Unoccupied");

  %hisName = Client::getName(GameBase::getOwnerClient(%targetId));

  centerprint(%client, "<jc><f1>Targeting " @ %vehDesc @ "<f0>" @

                       "\n" @

                       "<jc>\x97\x97\x97\x97\x97\x97   \x97\x97\x97\x97\x97\x97" @

                       "\n" @

                       "<jc><f0>" @ %teamStr @
                       "<jl><f0>      Health: " @ %healthPct @ "%" @
                       "<jr><f0>" @ %ridersStr @ "      ", 0);
}

function BinocularsImage::onFire(%player, %slot) {
  if (GameBase::getLOSInfo(%player, 1000)) {
    %target = $los::object;

    if (getObjectType(%target) == "Flier")
      Binoculars::setInfoVehicle(%player, %target);

    else if (getObjectType(%target) == "Player" && Player::getArmor(%target) != VehicleObjectMounter)
      Binoculars::setInfoPlayer(%player, %target);

    else
      Binoculars::setInfoIdle(%player, -1);
  }
}

registerGadget(Binoculars);
linkGadget(Binoculars);
setArmorAllowsItem(larmor, Binoculars, 0);
setArmorAllowsItem(lfemale, Binoculars, 0);
setArmorAllowsItem(SpyMale, Binoculars, 0);
setArmorAllowsItem(SpyFemale, Binoculars, 0);
setArmorAllowsItem(DMMale, Binoculars, 0);
setArmorAllowsItem(DMFemale, Binoculars, 0);