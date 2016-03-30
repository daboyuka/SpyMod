ItemImageData BombImage2 {
	shapeFile = "mortar";
	mountPoint = 0;
	mountOffset = "-0.15 0 0";
};
ItemData Bomb2 {
	shapeFile = "mortar";
	imageType = BombImage2;
};

ItemImageData BombImage3 {
	shapeFile = "sensor_small";
	mountPoint = 0;
	mountOffset = "0 0 0.05";
};
ItemData Bomb3 {
	shapeFile = "sensor_small";
	imageType = BombImage3;
};

ItemData Bomb;

ItemImageData BombImage {
   shapeFile  = "mortar";
	mountPoint = 0;
	mountOffset = "0.15 0 0";

	weaponType = 0; // Single Shot
	reloadTime = 0;
	fireTime = 0.25;

	ammoType = Bomb;

	sfxActivate = SoundPickUpWeapon;
};

ItemData Bomb {
  description = "Timed bomb";

  shapeFile = "mortar";
	shadowDetailMask = 4;

	lightType = 2;   // Pulsing
	lightRadius = 4;
	lightTime = 1.5;
	lightColor = { 0.3, 1, 0 };

	className = "Tool";
	imageType = BombImage;
	heading = $InvHeading[Gadget];
};

$ItemDescription[Bomb] = "<jc><f1>Timed bomb:<f0> A large demolition bomb on a 20 fuse";

function Bomb::onMount(%player, %item) {
  Weapon::displayDescription(%player, Bomb);
  Player::mountItem(%player, Bomb2, 3);
  Player::mountItem(%player, Bomb3, 4);
}
function Bomb::onUnmount(%player, %item) {
  Player::unmountItem(%player, 3);
  Player::unmountItem(%player, 4);
  Weapon::clearDescription(%player);
}

function BombImage::onFire(%player, %slot) {
  if (Player::getItemCount(%player, Bomb) == 0) return;
  if (Bomb::deploy(%player)) Player::decItemCount(%player, Bomb);
}

function Bomb::deploy(%player) {
  if (GameBase::getLOSInfo(%player, 3)) {
    %pos = $los::position;
    %normal = $los::normal;
    %rot = Vector::getRot(%normal);
    %team = GameBase::getTeam(%player);

    %axisa = Vector::getFromRot(Vector::add(%rot, Vector::getRot("0 0 1")), 0.15);
    %axis = Vector::cross(%axisa, %normal);

    %obj1 = newObject("", StaticShape, BombShape1, true);
    GameBase::setPosition(%obj1, Vector::add(%pos, %axis));
    GameBase::setRotation(%obj1, Vector::add(%rot, "1.57 0 0"));
    GameBase::setTeam(%obj1, %team);

    %obj2 = newObject("", StaticShape, BombShape1, true);
    GameBase::setPosition(%obj2, Vector::sub(%pos, %axis));
    GameBase::setRotation(%obj2, Vector::add(%rot, "1.57 0 0"));
    GameBase::setTeam(%obj2, %team);

    %obj3 = newObject("", StaticShape, BombShape2, true);
    GameBase::setPosition(%obj3, Vector::add(%pos, Vector::mul(%normal, 0.05)));
    GameBase::setRotation(%obj3, Vector::add(%rot, "-1.57 0 0"));
    GameBase::setTeam(%obj3, %team);
    GameBase::playSequence(%obj3, 0, "power");

    Bomb::startCountdown(%player, %obj1, %obj2, %obj3, Vector::add(%pos, Vector::mul(%normal, 0.4)));

    return true;
  } else return false;
}

function Bomb::startCountdown(%player, %obj1, %obj2, %obj3, %pos) {
  %client = Player::getClient(%player);
  Client::sendMessage(%client, 1, "Bomb deployed, 20 seconds until detonation");
  schedule("Client::sendMessage("@%client@", 1, \"10 seconds until detonation\");", 10);
  schedule("Client::sendMessage("@%client@", 1, \"5\");", 15);
  schedule("Client::sendMessage("@%client@", 1, \"4\");", 16);
  schedule("Client::sendMessage("@%client@", 1, \"3\");", 17);
  schedule("Client::sendMessage("@%client@", 1, \"2\");", 18);
  schedule("Client::sendMessage("@%client@", 1, \"1\");", 19);
  schedule("Bomb::detonate("@%player@","@%obj1@","@%obj2@","@%obj3@",\""@%pos@"\");", 20);
}

function Bomb::detonate(%player, %obj1, %obj2, %obj3, %pos) {
  GameBase::playSound(%obj1, shockExplosion, 0);
  deleteObject(%obj1);
  deleteObject(%obj2);
  deleteObject(%obj3);
  GameBase::applyRadiusDamage($BombDamageType, %pos, 50, 2000, 1000, %player);
}

//registerGadget(Bomb, Bomb, 1);
linkGadget(Bomb);
setArmorAllowsItem(larmor, Bomb, 1);
setArmorAllowsItem(lfemale, Bomb, 1);
setArmorAllowsItem(SpyMale, Bomb, 1);
setArmorAllowsItem(SpyFemale, Bomb, 1);
setArmorAllowsItem(DMMale, Bomb, 1);
setArmorAllowsItem(DMFemale, Bomb, 1);


StaticShapeData BombShape1 {
	shapeFile = "mortar";
	maxDamage = 100000.0;
    description = "Bomb";
};
StaticShapeData BombShape2 {
	shapeFile = "sensor_small";
	maxDamage = 100000.0;
    description = "Bomb";
};