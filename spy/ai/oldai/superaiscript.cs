
//////
// Standard SuperAI scripting
//////

$SuperAI::defaultSightDist = 50;
$SuperAI::defaultIq = 60;
$SuperAI::defaultTriggerPct = 0.9;
$SuperAI::defaultAttackMode = 0;

function SuperAI::init(%aiName, %marker, %team) {
  %sightDist = tern(%marker != -1 && %marker.sightDist != "", %marker.sightDist, $SuperAI::defaultSightDist);
  %iq = tern(%marker != -1 && %marker.iq != "", %marker.iq, $SuperAI::defaultIq);
  %triggerPct = tern(%marker != -1 && %marker.triggerPct != "", %marker.triggerPct, $SuperAI::defaultTriggerPct);

  SuperAI::setVar(%aiName, "sightDist", %sightDist);
  AI::setVar(%aiName, "spotDist", %sightDist);

  AI::setVar(%aiName, "iq", %iq);
  AI::setVar(%aiName, "triggerPct", %triggerPct);

  if (%team != "") GameBase::setTeam(SuperAI::getOwnedObject(%aiName), %team);

  if (%marker != -1) {
    %player = SuperAI::getOwnedObject(%aiName);
    for (%i = 0; %marker.spawnItem[%i] != ""; %i++) {
      %str = %marker.spawnItem[%i];
      Player::setItemCount(%player, getWord(%str, 1), getWord(%str, 0));
    }
    if (%i > 0) {
      %mountWep = getWord(%marker.spawnItem[0], 1);
      Player::mountItem(%player, %mountWep, 0);
    }

    for (%i = 0; %marker.deadItem[%i] != ""; %i++) {
      %str = %marker.deadItem[%i];
      SuperAI::setVar(%aiName, "deadItem"@%i, %str);
    }

    AI::setVar(%aiName, "attackMode", tern(%marker.attackMode != "", %marker.attackMode, $SuperAI::defaultAttackMode));
    if (!%marker.dontAutoTarget) {
      if (%marker.useSuperAITargeting) SuperAI::checkForEnemies(%aiName);
      else AI::setAutomaticTargets(%aiName);
    }
  }
}

function SuperAI::checkTarget(%aiName, %target, %curTarget) {
  if (SuperAI::isDead(%aiName)) return;
  if (getNumTeams() > 1 && GameBase::getTeam(%target) == GameBase::getTeam(SuperAI::getOwnedObject(%aiName))) return;

  %curTarget = AI::getTarget(%aiName);
  if (%curTarget == %target) return false;
  if (%curTarget > 0) {
    %aiPos = SuperAI::getPosition(%aiName);
    if (Vector::getDistance(%aiPos, GameBase::getPosition(%target)) <
        Vector::getDistance(%aiPos, GameBase::getPosition(%curTarget))) {
      return true;
    }
  } else {
    return true;
  }
  return false;
}

function SuperAI::target(%aiName, %tclient) {
  if (AI::getTarget(%aiName) == %tclient) return;
  else if (AI::getTarget(%aiName) > 0) SuperAI::cancelTarget(%aiName);
  AI::directiveTarget(%aiName, GameBase::getOwnerClient(%tclient), 100);
}

function SuperAI::onTargetDied(%aiName, %target) {
  SuperAI::cancelTarget(%aiName);
}

function SuperAI::onTargetLOSAcquired(%aiName, %target) {}
function SuperAI::onTargetLOSLost(%aiName, %target) { SuperAI::cancelTarget(%aiName); }

function SuperAI::onDamage(%aiName, %shooter, %type, %value) {
  if (%value > 0) {
    if (getNumTeams() > 1 && GameBase::getTeam(%shooter) == GameBase::getTeam(SuperAI::getOwnedObject(%aiName))) {
      return;
    }
    SuperAI::cancelTarget(%aiName);
    AI::directiveTarget(%aiName, %shooter, 100);
  }
}

function SuperAI::onKilled(%aiName, %player) {
  for (%i = 0; (%str = SuperAI::getVar(%aiName, "deadItem"@%i)) != ""; %i++) {
    Player::setItemCount(%player, getWord(%str, 1), getWord(%str, 0));
  }
}

function SuperAI::targetFound(%aiName, %target) {
  SuperAI::target(%aiName, GameBase::getOwnerClient(%target));
}

function Vector::lerp(%v1, %v2, %p) {
  return Vector::add(Vector::mul(%v1, 1 - %p), Vector::mul(%v2, %p));
}
function SuperAI::checkForEnemies(%aiName) {
//  echo("Checking targets for "@%aiName@"...");
  if (SuperAI::isDead(%aiName)) return;
  if (SuperAI::getVar(%aiName, "fastCheck") && AI::getTarget(%aiName) != -1) { schedule("SuperAI::checkForEnemies("@%aiName@");", 0.5+getRandom()/5); return; }

//  echo(%ainame @ ": Not dead");

  %aiObject = SuperAI::getOwnedObject(%aiName);
  %pos = Vector::add(getBoxCenter(%aiObject), "0 0 1"); // head
  %range = SuperAI::getVar(%aiName, "sightDist");
  //%muzRot = Vector::getRot(Matrix::subMatrix(GameBase::getMuzzleTransform(%aiObject), 3, 4, 3, 1, 0, 1));

  %targ = -1;
  for (%c = BaseRep::getFirst(); %c != -1; %c = BaseRep::getNext(%c)) {
    if (getNumTeams() > 1 && GameBase::getTeam(%c) == GameBase::getTeam(%aiObject)) continue;

    %obj = Client::getOwnedObject(%c);
    if (%obj != -1) {
      if (Vector::getDistance(GameBase::getPosition(%obj), %pos) < %range) {
        //echo(%ainame @ ": " @ Client::getName(%c) @ " in range...");
        //%rot = Vector::getRot(Vector::sub(getBoxCenter(%obj), %pos));
        %objpos = Matrix::subMatrix(GameBase::getMuzzleTransform(%obj), 3, 4, 3, 1, 0, 3);
        %objpos = Vector::lerp(%objpos, getBoxCenter(%obj), 0.9);
        %delta = Vector::sub(%objpos, %pos);
        %offset = Vector::resize(%delta, 0.75);
        %hit = getLOSInfo(Vector::add(%offset, %pos), %objpos, -1);
        //echo(%pos @"/"@ %objpos @"/"@ %offset @"/"@ %hit @ "/" @ $los::object @ "/" @ $los::position);
        if (%hit && $los::object == %obj) { //GameBase::getLOSInfo(%aiObject, %range, Vector::sub(%rot, %muzRot))
          //echo(%ainame @ ": " @ Client::getName(%c) @ " visible...");
          if (SuperAI::callCustom(%aiName, "checkTarget", 2, %obj, %targ)) %targ = %obj;
        }
      }
    }
  }

  if (%targ != -1) SuperAI::callCustom(%aiName, "targetFound", 1, %targ);
  schedule("SuperAI::checkForEnemies("@%aiName@");", 0.5+getRandom()/5);
}