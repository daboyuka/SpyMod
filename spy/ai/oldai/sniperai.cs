
//////
// Sniper SuperAI scripting
//////

// Directive number table:
//   Target player = 100
//   Target point = 1

function SniperAI::init(%aiName, %marker, %team) {
  %id = AI::getID(%aiName);
  Player::setItemCount(%id, UberSniper, 1);
  Player::mountItem(%id, UberSniper, 0);
  AI::setVar(%aiName, "attackMode", 1);
  AI::setVar(%aiName, "smartGuyMinAccuracy", 1);
  AI::setVar(%aiName, "triggerPct", 0.3);
  AI::setVar(%aiName, "iq", 1000);

  %sightDist = tern(%marker != -1, tern(%marker.sightDist != "", %marker.sightDist, 200), 200);
  SuperAI::setVar(%aiName, "sightDist", %sightDist);
  AI::setVar(%aiName, "spotDist", %sightDist);

  SuperAI::setVar(%aiName, "startingRot", SuperAI::getRotation(%aiName));

  if (%team != "") GameBase::setTeam(SuperAI::getOwnedObject(%aiName), %team);

  SuperAI::checkForEnemies(%aiName);
  SniperAI::startIdleTargeting(%aiName);
}

function SniperAI::checkTarget(%aiName, %target) {
  %targPos = GameBase::getPosition(%target);
  %aiPos = SuperAI::getPosition(%target);

  %speed = Vector::getDistance(0, Item::getVelocity(%target));
  %dist = Vector::getDistance(%aiPos, %targPos);
  %rand = getRandom();
  %targetHim = false;
  %backTurned = Vector::dot(Vector::getFromRot(SuperAI::getRotation(%aiName)), Vector::sub(%targPos, %aiPos)) < 0;
  %targFactor = tern(%backTurned, 0.5, 1);
  %targFactor *= tern(%dist < 10, 2, 1);
  //echo(%aiName @ ":" @ %speed @ "," @ %dist @ "," @ %backTurned @ "," @ %targFactor);

  if (%speed < 1) {
    SniperAI::incSniperTarg(%aiName, %target, 0.25 * %targFactor);
  } else if (%speed < 4) {
    SniperAI::incSniperTarg(%aiName, %target, 1.5 * %targFactor);
  } else {
    SniperAI::incSniperTarg(%aiName, %target, 7.5 * %targFactor);
  }
  if (SniperAI::getSniperTarg(%aiName, %target) >= 20) {
    AI::setVar(%aiName, "triggerPct", 0.3);
    SniperAI::stopIdleTargeting(%aiName);
    %targetHim = SuperAI::checkTarget(%aiName, %target);
  }
  //echo(SniperAI::getSniperTarg(%aiName, %target) @ "," @ %targetHim);
  return %targetHim;
}

function SniperAI::incSniperTarg(%aiName, %target, %amt) {
  %obj = SuperAI::getOwnedObject(%aiName);
  %obj.sniperTarg[%target] += %amt;
  schedule("if ("@%obj@".sniperTarg["@%target@"] == "@%obj.sniperTarg[%target]@")"@%obj@".sniperTarg["@%target@"]=0;",5);//-= "@%amt@";", 5);
}

function SniperAI::getSniperTarg(%aiName, %target) { return SuperAI::getOwnedObject(%aiName).sniperTarg[%target]; }

function SniperAI::targetFound(%aiName, %target) {
  SuperAI::targetFound(%aiName, %target);
}

function SniperAI::onTargetDied(%aiName, %target) {
  $SuperAI::AIData[%aiName, "targetLostTime"] = getSimTime();
  SniperAI::targetLost(%aiName, %target);
}

function SniperAI::onTargetLOSAcquired(%aiName, %target) {
  SuperAI::onTargetLOSAcquired(%aiName, %target);
}

function SniperAI::onTargetLOSLost(%aiName, %target) {
  %time = getSimTime();
  $SuperAI::AIData[%aiName, "targetLostTime"] = %time;
  schedule("if (AI::getTarget(\""@%aiName@"\") == -1 && $SuperAI::AIData[\""@%aiName@"\", \"targetLostTime\"] == "@%time@")" @
           "SniperAI::targetLost(\""@%aiName@"\","@%target@");", 5);
}

function SniperAI::onDamage(%aiName, %shooter, %type, %value) {
  AI::setVar(%aiName, "triggerPct", 0.1);
  SniperAI::stopIdleTargeting(%aiName);
  SuperAI::onDamage(%aiName, %shooter, %type, %value);
}

function SniperAI::onKilled(%aiName, %this) {
  Player::setItemCount(%this, UberSniper, 0);
  Player::setItemCount(%this, Sniper, 1);
  Player::setItemCount(%this, SniperAmmo, 3);
}

function SniperAI::targetLost(%aiName, %target) {
  %target.sniperTarg = 0;
  SuperAI::cancelTarget(%aiName);
  schedule("SniperAI::startIdleTargeting("@%aiName@");", 2);
}

function SniperAI::startIdleTargeting(%aiName) {
  $SuperAI::AIData[%aiName, "idleTargetingID"]++;
  SniperAI::idleTarget(%aiName, $SuperAI::AIData[%aiName, "idleTargetingID"]);
}
function SniperAI::stopIdleTargeting(%aiName) {
  $SuperAI::AIData[%aiName, "idleTargetingID"]++;
  AI::directiveRemove(%aiName, 1);
}

function SniperAI::idleTarget(%aiName, %id) {
  if (SuperAI::isDead(%aiName) || $SuperAI::AIData[%aiName, "idleTargetingID"] != %id)
    return;

  %pos = SuperAI::getPosition(%aiName);
  %rot = SuperAI::getVar(%aiName, "startingRot");//GameBase::getRotation(%aiObj);

  %randRot = Vector::add(%rot, "0 0 " @ (getRandom() * ($PI / 6) - ($PI / 12)));
  %targPos = Vector::add(%pos, Vector::getFromRot(%randRot, getRandom() * 150 + 70));
  %targPos = getWord(%targPos, 0) @ " " @ getWord(%targPos, 1) @ " " @ (getWord(%targPos, 2) - 20);

  AI::directiveTargetPoint(%aiName, %targPos, 1);
  AI::setVar(%aiName, "triggerPct", 0);

  schedule("SniperAI::idleTarget("@%aiName@", "@%id@");", 5);
}