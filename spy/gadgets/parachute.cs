ItemData ParachutePack;

ItemImageData ParachutePackImage {
	shapeFile = "ammopack";
	mountPoint = 2;
	weaponType = 0;
	ammoType = ParachutePack;
  	mountOffset = { 0, -0.03, 0 };
  	mountRotation = { 0, 0, 0 };
	firstPerson = false;
};

ItemData ParachutePack {
	description = "Parachute";
	shapeFile = "ammopack";
	className = "Backpack";
   heading = "cBackpacks";
	shadowDetailMask = 4;
	imageType = ParachutePackImage;
	price = 150;
	hudIcon = "energypack";
	showWeaponBar = false;
};

ItemImageData ParachuteImage1 {
	shapeFile = "elevator_8x4";
	mountPoint = 4;
	mountOffset = { 0, 0, 15 };
};

ItemData Parachute1 {
	shapeFile = "breath";
	imageType = ParachuteImage1;
};

ItemImageData ParachuteImage2 {
	shapeFile = "elevator_8x4";
	mountPoint = 4;
	mountOffset = { 7, 0, 13.5 };
  	mountRotation = { 0, 0.4, 0 };
};

ItemData Parachute2 {
	shapeFile = "breath";
	imageType = ParachuteImage2;
};

ItemImageData ParachuteImage3 {
	shapeFile = "elevator_8x4";
	mountPoint = 4;
	mountOffset = { -7, 0, 13.5 };
  	mountRotation = { 0, -0.4, 0 };
};

ItemData Parachute3 {
	shapeFile = "breath";
	imageType = ParachuteImage3;
};


$Parachute::slowFactor = 4.5;
$Parachute::moveSpeed = 2;
$ItemDescription[ParachutePack] = "<jc><f1>Parachute:<f0> Slows your descent to earth";

function ParachutePack::onUse(%player, %slot) {
  if (Player::getMountedItem(%player, $BackpackSlot) == ParachutePack) {
    if (!%player.parachuting) ParachutePack::parachute(%player);
  } else Player::mountItem(%player, ParachutePack, $BackpackSlot);
}

function ParachutePack::parachute(%player) {
  if (%player.parachuting || Player::getLastContactCount(%player) <= 4 ||
      Player::getItemCount(%player, ParachutePack) == 0) return;

  %player.parachuting = true;
  %player.weaponLock = true;

  if (%player.grappling) Grappler::ungrapple(%player, false);

  Player::trigger(%player, 0, false);
  Player::unmountItem(%player, 0);

  Player::mountItem(%player, Parachute1, 3);
  Player::mountItem(%player, Parachute2, 4);
  Player::mountItem(%player, Parachute3, 5);

  Player::decItemCount(%player, ParachutePack);

  ParachutePack::checkParachute(%player);
}

function ParachutePack::unparachute(%player) {
  if (!%player.parachuting) return;

  Player::unmountItem(%player, 3);
  Player::unmountItem(%player, 4);
  Player::unmountItem(%player, 5);

  %player.parachuting = false;
  %player.weaponLock = false;

  if (Player::getClient(%player).lastWeapon != "")
    Player::useItem(%player, Player::getClient(%player).lastWeapon);
  else if (Player::getClient(%player).lastGadget != "")
    Player::useItem(%player, Player::getClient(%player).lastGadget);
}

function ParachutePack::checkParachute(%player) {
  if (!%player.parachuting || Player::getMountObject(%player) != -1 || Player::getLastContactCount(%player) <= 4) {
    ParachutePack::unparachute(%player);
    return;
  }

  %vel = Item::getVelocity(%player);
  if (Vector::dot(%vel, "0 0 -1") > 0) {
    %speed = Vector::length(%vel);
    %x = pow(%speed, 0.6) * -0.125 * $Parachute::slowFactor;
    %vel = Vector::add(%vel, Vector::resize(Vector::add(%vel, "0 0 "@getWord(%vel, 2)*20), %x));
    Item::setVelocity(%player, %vel);
  }
  if (Player::isJetting(%player)) {
    %move = Vector::mul(Vector::getFromRot(GameBase::getRotation(%player)), $Parachute::moveSpeed * 0.125);
    Item::setVelocity(%player, Vector::add(Item::getVelocity(%player), %move));
  }

  schedule("ParachutePack::checkParachute("@%player@");", 0.125);
}

function phun2(%x) { Player::setItemCount(%x, ParachutePack, 1); Player::mountItem(%x, ParachutePack, $BackpackSlot); focusServer(); %obj = newObject("", Flier, SpyScout, true); GameBase::setPosition(%obj, GameBase::getPosition(%x)); focusClient(); }
function phun3(%x) { Player::setItemCount(%x, ParachutePack, 1); Player::mountItem(%x, ParachutePack, $BackpackSlot); focusServer(); %obj = newObject("", Flier, Juggernaught, true); GameBase::setPosition(%obj, GameBase::getPosition(%x)); focusClient(); }
function phun4(%x) { Player::setItemCount(%x, ParachutePack, 1); Player::mountItem(%x, ParachutePack, $BackpackSlot); focusServer(); %obj = newObject("", Flier, Tank, true); GameBase::setPosition(%obj, GameBase::getPosition(%x)); focusClient(); }
function phun5(%x) { focusServer(); %obj = newObject("", Flier, Transport, true); GameBase::setPosition(%obj, GameBase::getPosition(%x)); focusClient(); }
function phun6(%x) { focusServer(); %obj = newObject("", Flier, Tank2, true); GameBase::setPosition(%obj, GameBase::getPosition(%x)); focusClient(); }
function phun7(%x) { focusServer(); %obj = newObject("", Flier, Tank3, true); GameBase::setPosition(%obj, GameBase::getPosition(%x)); focusClient(); }

registerGadget(ParachutePack, ParachutePack, 1);
setArmorAllowsItem(larmor, ParachutePack, 1);
setArmorAllowsItem(lfemale, ParachutePack, 1);
setArmorAllowsItem(SpyMale, ParachutePack, 1);
setArmorAllowsItem(SpyFemale, ParachutePack, 1);
setArmorAllowsItem(DMMale, ParachutePack, 1);
setArmorAllowsItem(DMFemale, ParachutePack, 1);