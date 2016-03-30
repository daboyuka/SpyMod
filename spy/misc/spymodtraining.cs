// ------ GENERAL STUFF ------

function Armor::onDamage(%this,%type,%value,%pos,%vec,%mom,%vertPos,%quadrant,%object) {
  if (Client::getOwnedObject(%object).region != "STUFF!!!") return;
  Player::onDamage(%this,%type,%value,%pos,%vec,%mom,%vertPos,%quadrant,%object);
}

// ------ WEAPONS TRAINING ------

function SpyModTraining::weapons::outOfBounds(%trigger, %object) {
  if (getObjectType(%object) == "Player") {
    if (Player::isAIControlled(%object)) return;
    %pos = GameBase::getPosition(%object);
    GameBase::setPosition(%object, getWord(%pos, 0) @ " 181.5 76");
    Client::sendMessage(Player::getClient(%object), 1, "Stay behind the wall please.~wLeftMissionArea.wav");
  }
}

function SpyModTraining::weapons::enter(%trigger, %object) {
  if (getObjectType(%object) != "Player") return;
  if (Player::isAIControlled(%object)) return;

  for (%i = 0; %i < $ITEM_SET[$ITEM_SET_ALL]; %i++) {
    %item = getWord($ITEM_SET[$ITEM_SET_ALL, %i], 1);
    if (%item.className == "Weapon") {
      Player::setItemCount(%object, %item, 1);
      if ($WeaponAmmo[%item] != "") Player::setItemCount(%object, $WeaponAmmo[%item], 10000);
    }
    if (%item.className == "Handgrenade") {
      Player::setItemCount(%object, %item, 1);
    }
  }
  selectValidWeapon(Player::getClient(%object), $FirstWeapon);
}

function SpyModTraining::weapons::leave(%trigger, %object) {
  if (getObjectType(%object) != "Player") return;
  if (Player::isAIControlled(%object)) return;

  for (%i = 0; %i < $ITEM_SET[$ITEM_SET_ALL]; %i++) {
    %item = getWord($ITEM_SET[$ITEM_SET_ALL, %i], 1);
    if (%item.className == Weapon) {
      Player::setItemCount(%object, %item, 0);
      if ($WeaponAmmo[%item] != "") Player::setItemCount(%object, $WeaponAmmo[%item], 0);
    }
  }
  
  Client::sendMessage(Player::getClient(%object), 1, "You've scored " @ Player::getClient(%object).weaponsTrainingPoints @ " points total!");
}

function Target::onDamage(%this,%type,%value,%pos,%vec,%mom,%object) {
  if (Client::getOwnedObject(%object).region != "WeaponsTrainingField") return;
  if (%this.poppedUp) {
    GameBase::playSound(%this, SoundHitTarget, 0);
    %this.popUpID++;
    %this.poppedUp = false;
    Moveable::moveBackward(%this);
    if (%this.basePoints || %this.damagePointsScale) {
      %pts = floor(%this.basePoints + %value*%this.damagePointsScale);
      %object.weaponsTrainingPoints += %pts;
      bottomprint(%object, "<jc>" @ %pts @ " points", 0.75);
    }
  }
}

function LightAIMale::onDamage(%this,%type,%value,%pos,%vec,%mom,%vertPos,%quadrant,%object) {
  if (Client::getOwnedObject(%object).region != "WeaponsTrainingField") return;
  %pts = %this.points;
  Player::onDamage(%this,%type,%value,%pos,%vec,%mom,%vertPos,%quadrant,%object);
  if (Player::isDead(%this)) {
    %object.weaponsTrainingPoints += %pts;
    bottomprint(%object, "<jc>"@%pts@" points", 0.75);
  }
}
function LightAIFemale::onDamage(%this,%type,%value,%pos,%vec,%mom,%vertPos,%quadrant,%object) {
  if (Client::getOwnedObject(%object).region != "WeaponsTrainingField") return;
  %pts = %this.points;
  Player::onDamage(%this,%type,%value,%pos,%vec,%mom,%vertPos,%quadrant,%object);
  if (Player::isDead(%this)) {
    %object.weaponsTrainingPoints += %pts;
    bottomprint(%object, "<jc>"@%pts@" points", 0.75);
  }
}

$SpyModTraining::weapons::numAIPaths[0] = 2;
$SpyModTraining::weapons::AILifetime[0] = 15;
$SpyModTraining::weapons::AIStart[0, 0] = "26 300 76";
$SpyModTraining::weapons::AIEnd[0, 0] = "-26 300 76";
$SpyModTraining::weapons::AIStart[0, 1] = "-26 300 76";
$SpyModTraining::weapons::AIEnd[0, 1] = "26 300 76";

$SpyModTraining::weapons::numAIPaths[1] = 2;
$SpyModTraining::weapons::AILifetime[1] = 15;
$SpyModTraining::weapons::AIStart[1, 0] = "26 275 76";
$SpyModTraining::weapons::AIEnd[1, 0] = "-26 275 76";
$SpyModTraining::weapons::AIStart[1, 1] = "-26 275 76";
$SpyModTraining::weapons::AIEnd[1, 1] = "26 275 76";

$SpyModTraining::weapons::AIPoints[0] = 150;
$SpyModTraining::weapons::AIPoints[1] = 100;

$AISpawner::lastAISpawn = -100;
$AISpawner::lastAIDelete = -100;
function AISpawner::spawnAI(%set) {
  if ($AISpawner::lastAIDelete + 0.5 > getSimTime()) { schedule("AISpawner::spawnAI("@%set@");", 0.5); return; }

  %index = floor($SpyModTraining::weapons::numAIPaths[%set] * getRandom());
  %name = "AI" @ $SpyModTraining::weapons::numAIPathsAINum++;

    $time = "SPAWNING " @ %name @ " AT " @ getSimTime();
    export("$time", "config\\aideletesux.txt", true);

  $AISpawner::lastAISpawn = getSimTime();
  AI::Spawn(%name, radnomItems(2, LightAIMale, LightAIFemale),
            $SpyModTraining::weapons::AIStart[%set, %index], 0, "AI Target");

  AI::DirectiveWaypoint(%name, $SpyModTraining::weapons::AIEnd[%set, %index]);
  AI::setVar(%name, "iq", 30);
  GameBase::setEnergy(Client::getOwnedObject(AI::getID(%name)), 0);
  Client::getOwnedObject(AI::getID(%name)).points = $SpyModTraining::weapons::AIPoints[%set];
  schedule("AISpawner::killAI(\""@%name@"\");", $SpyModTraining::weapons::AILifetime[%set]);
}

function AISpawner::killAI(%name) {
  %index = String::getSubStr(%name, 2, 1000);
  $AISpawner::AISToDelete[%index] = 1;
  if ($AISpawner::AIIndexMax < %index) $AISpawner::AIIndexMax = %index;

//  AI::Delete(%name);
}
//2285
$AISpawner::AIIndex = 1;
$AISpawner::AIIndexMax = 0;
function AISpawner::cleanUp() {
  while (($AISpawner::AISToDelete[$AISpawner::AIIndex] == "" || $AISpawner::AISToDelete[$AISpawner::AIIndex] == 0) &&
         $AISpawner::AIIndex < $AISpawner::AIIndexMax) {

    $AISpawner::AIIndex++;
  }
  if ($AISpawner::lastAISpawn + 0.5 > getSimTime()) { schedule("AISpawner::cleanUp();", 0.5); return; }
  if ($AISpawner::AISToDelete[$AISpawner::AIIndex] == 1) {
    $time = "DELETING AI" @ $AISpawner::AIIndex @ " AT " @ getSimTime();
    export("$time", "config\\aideletesux.txt", true);
    $AISpawner::lastAIDelete = getSimTime();
    AI::delete("AI"@$AISpawner::AIIndex);
    $AISpawner::AISToDelete[$AISpawner::AIIndex] = 0;
    $AISpawner::AIIndex++;
    schedule("AISpawner::cleanUp();", 3);
  } else {
    schedule("AISpawner::cleanUp();", 0.5);
  }
}

function AISpawner::spawnRepeat(%set, %interval, %intervalRand) {
  AISpawner::spawnAI(%set);
  schedule("AISpawner::spawnRepeat("@%set@","@%interval@","@%intervalRand@");", %interval + %intervalRand*getRandom());
}

AISpawner::spawnRepeat(0, 10, 10);
AISpawner::spawnRepeat(1, 10, 10);

AISpawner::cleanUp();

// ------ PARADROP TRAINING ------

function SpyModTraining::paradrop::enter(%trigger, %object) {
  if (getObjectType(%object) == "Player") {
    Player::setItemCount(%object, ParachutePack, 1);
    Player::mountItem(%object, ParachutePack, $BackpackSlot);
  }
}

function SpyModTraining::paradrop::contact(%trigger, %object) {
  if (getObjectType(%object) == "Player") {
    if (Player::getItemCount(%object, ParachutePack) == 0 || Player::getMountedItem(%object, $BackpackSlot) != ParachutePack) {
      Player::setItemCount(%object, ParachutePack, 1);
      Player::mountItem(%object, ParachutePack, $BackpackSlot);
    }
  }
}

function SpyModTraining::paradrop::leave(%trigger, %object) {
  if (getObjectType(%object) == "Player") {
    if (%object.parachuting) {
      %object.parachuting = false;
      bottomprint(Player::getClient(%object), "<jc>You missed the landing field. Try again.");
      Item::setVelocity(%object, 0);
      GameBase::setPosition(%object, "203.5 -3.5 119");
      GameBase::setRotation(%object, "0 0 -1.57");
    } else {
      Player::setItemCount(%object, ParachutePack, 0);
      Client::sendMessage(Player::getClient(%object), 1, "Your paradrop record is " @ Player::getClient(%object).paradropTrainingPoints @ "!");
    }
  }
}

function SpyModTraining::paradrop::playerHitGround(%player) {
  %client = Player::getClient(%player);
  if (%player.region != "XParadropField") {
    %pos = GameBase::getPosition(%player);
    if (getWord(%pos, 0) > 224 && getWord(%pos, 1) <= 32 && getWord(%pos, 1) >= -32) {
      %dist = floor(getWord(%pos, 0) - 224);
      if (%client.paradropTrainingPoints == "") %client.paradropTrainingPoints = 0;
      if (%dist > %client.paradropTrainingPoints) %client.paradropTrainingPoints = %dist;
      bottomprint(%client, "<jc>You flew " @ %dist @ " meters!");
    } else {
      bottomprint(%client, "<jc>You missed the landing field. Try again.");
    }
  } else {
    bottomprint(%client, "<jc>You missed the landing field. Try again.");
  }
  schedule("Item::setVelocity("@%player@", 0); GameBase::setPosition("@%player@", \"203.5 -3.5 119\"); GameBase::setRotation("@%player@", \"0 0 -1.57\");", 3);
}

function ParachutePack::checkParachute(%player) {
  if (!%player.parachuting) { ParachutePack::unparachute(%player); return; }
  if (Player::getLastContactCount(%player) <= 4) {
    ParachutePack::unparachute(%player);
    SpyModTraining::paradrop::playerHitGround(%player);
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
    echo(%move);
    Item::setVelocity(%player, Vector::add(Item::getVelocity(%player), %move));
  }

  schedule("ParachutePack::checkParachute("@%player@");", 0.125);
}

// ------ GRAPPLER TRAINING ------

function SpyModTraining::grappler::enter(%trigger, %object) {
  if (getObjectType(%object) == "Player") {
    Player::setItemCount(%object, Grappler, 1);
    Player::mountItem(%object, Grappler, $WeaponSlot);
  }
}

function SpyModTraining::grappler::leave(%trigger, %object) {
  if (getObjectType(%object) == "Player") {
    schedule("if ("@%object@".region != \"GrapplerObstacleCourse\") SpyModTraining::grappler::leave2("@%trigger@","@%object@");", 0.5);
  }
}

function SpyModTraining::grappler::leave2(%trigger, %object) {
  if (%object.grapplerTrainingStartTime != "") {
    %client = Player::getClient(%object);
    bottomprint(%client, "<jc>You left the course.  Try again.", 2);
    SpyModTraining::grappler::resetPlayer(%object);
  } else {
    Player::setItemCount(%object, Grappler, 0);
    %client = Player::getClient(%object);
    Client::sendMessage(%client, 1, "Your best time was " @ Time::getMinutes(%client.grapplerTrainingScore) @ ":" @ Time::getSeconds(%client.grapplerTrainingScore) @ "!");
    SpyModTraining::grappler::resetClientTime(%client);
  }
}

function SpyModTraining::grappler::playerHitGround(%trigger, %object) {
  if (getObjectType(%object) == "Player") {
    %client = Player::getClient(%object);
    bottomprint(%client, "<jc>You touched the ground.  Try again.", 2);
    schedule("SpyModTraining::grappler::resetPlayer("@%object@");", 3);
  }
}

function SpyModTraining::grappler::resetPlayer(%player) {
  if (%player.grappling) Grappler::ungrapple(%player, false);
  Item::setVelocity(%player, 0);
  GameBase::setPosition(%player, "-174 0 89");
  GameBase::setRotation(%player, 0);
  SpyModTraining::grappler::resetClientTime(Player::getClient(%player));
}

function SpyModTraining::grappler::start(%trigger, %object) {
  if (getObjectType(%object) == "Player") {
    %object.grapplerTrainingStartTime = getSimTime();
    %client = Player::getClient(%object);
    remoteEval(%client, "setTime", 0);
    bottomprint(%client, "<jc>Time started", 1);
  }
}

function SpyModTraining::grappler::finish(%trigger, %object) {
  if (getObjectType(%object) == "Player") {
    %client = Player::getClient(%object);

    %time = getSimTime() - %object.grapplerTrainingStartTime;
    if (%time < %client.grapplerTrainingScore || %client.grapplerTrainingScore == "") %client.grapplerTrainingScore = %time;

    bottomprint(%client, "<jc>Your time was " @ Time::getMinutes(%time) @ ":" @ Time::getSeconds(%time), 3);
    schedule("SpyModTraining::grappler::resetPlayer("@%object@");", 3);
    %object.grapplerTrainingStartTime = "";
  }
}

function SpyModTraining::grappler::resetClientTime(%client) {
  %curTimeLeft = ($Server::timeLimit * 60) + $missionStartTime - getSimTime();
  if (!$Server::timeLimit) %curTimeLeft *= -1;
  remoteEval(%client, "setTime", %curTimeLeft);
}