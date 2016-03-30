$AIStateSystem::eventRedirect["GuardAI", "engage", "targetLOSLost"] = true;
$AIStateSystem::eventRedirect["GuardAI", "engage", "targetDied"] = true;
$AIStateSystem::eventRedirect["GuardAI", "patrol", "checktarget"] = true;
$AIStateSystem::eventRedirect["GuardAI", "patrol", "enemiesFound"] = true;
$AIStateSystem::eventRedirect["GuardAI", "patrol", "waypointReached"] = true;

function GuardAI::init(%aiName, %marker) {
  %this = SuperAI::getOwnedObject(%aiName);

  Player::setItemCount(%this, Challenger, 1);
  Player::setItemCount(%this, ChallengerAmmo, 1000);
  Player::mountItem(%this, Challenger, 0);

  SuperAI::setVar(%aiName, "position", GameBase::getPosition(%this));
  SuperAI::setVar(%aiName, "waypointGroup", nameToID("MissionGroup\\AITest"));

  SuperAI::changeState(%aiName, "patrol");
}

function GuardAI::onTargetDied() { echo("---DIED"); }
function GuardAI::onTargetLOSAcquired() { echo("---ACQ"); }
function GuardAI::onTargetLOSLost() { echo("---LOST"); }
function GuardAI::onKilled(%aiName) { SuperAI::respawn(%aiName, SuperAI::getVar(%aiName, "position"), 0, 10); }
function GuardAI::onDamage() {}
function GuardAI::onWaypointReached() {}




//
// Patrol state
//

function GuardAI::patrolState::enter(%aiName) {
  echo("Entering patrol state (" @ %aiName @ ")");

  AI::setVar(%aiName, "iq", 100);
  AI::setVar(%aiName, "spotDist", 1000);

  SuperAI::enemySearchMacro(%aiName);

  %wp = GuardAI::getNearestWaypoint(%aiName);
  if (%wp != -1)
    GuardAI::startPatrol(%aiName, %wp);
}

function GuardAI::patrolState::exit(%aiName) {
  echo("Exiting patrol state (" @ %aiName @ ")");
}

function GuardAI::getNearestWaypoint(%aiName) {
  %wps = SuperAI::getVar(%aiName, "waypointGroup");
  if (%wps <= 0) return -1;

  %n = Group::objectCount(%wps);
  %pos = SuperAI::getPosition(%aiName);
  %bestDistSq = 9999999;
  %bestDistI = -1;
  for (%i = 0; %i < %n; %i++) {
    %m = Group::getObject(%wps, %i);
    %distSq = Vector::lengthSquared(Vector::sub(GameBase::getPosition(%m), %pos));
    if (%distSq < %bestDistSq) {
      %bestDistSq = %distSq;
      %bestDistI = %i;
    }
  }
  return %bestDistI;
}

function GuardAI::getWaypointPos(%aiName, %wp) {
  %wps = SuperAI::getVar(%aiName, "waypointGroup");
  return GameBase::getPosition(Group::getObject(%wps, %wp));
}

function GuardAI::getWaypointCount(%aiName, %wp) {
  return Group::objectCount(SuperAI::getVar(%aiName, "waypointGroup"));
}

function GuardAI::startPatrol(%aiName, %wp) {
  if (%wp == -1) return;

  SuperAI::setStateVar(%aiName, "patrolForward", true);
  SuperAI::setStateVar(%aiName, "patrolWP", %wp);
  SuperAI::moveTo(%aiName, GuardAI::getWaypointPos(%aiName, %wp));
}

function GuardAI::patrolState::onCheckTarget(%aiName, %target) {
  if (getSimTime() < SuperAI::getVar(%aiName, "hidingUntil")) return false;
  return true;
}

function GuardAI::patrolState::onEnemiesFound(%aiName, %enemies) {
  SuperAI::changeState(%aiName, engage, 1, Group::getObject(%enemies, 0));
}

function GuardAI::patrolState::onWaypointReached(%aiName) {
  %wp = SuperAI::getStateVar(%aiName, "patrolWP");
  %fwd = SuperAI::getStateVar(%aiName, "patrolForward");
  %count = GuardAI::getWaypointCount(%aiName);

  if (%fwd && %wp == %count - 1)
    SuperAI::setStateVar(%aiName, "patrolForward", %fwd = "");
  else if (!%fwd && %wp == 0)
    SuperAI::setStateVar(%aiName, "patrolForward", %fwd = true);

  if (%fwd) SuperAI::setStateVar(%aiName, "patrolWP", %wp++);
  else      SuperAI::setStateVar(%aiName, "patrolWP", %wp--);

  SuperAI::moveTo(%aiName, GuardAI::getWaypointPos(%aiName, %wp));
}




function GuardAI::engageState::enter(%aiName, %target) {
  echo("Entering engage state (" @ %aiName @ ")");

  AI::setVar(%aiName, "attackMode", 1);
  SuperAI::targetPlayer(%aiName, Player::getClient(%target));

  SuperAI::setStateVar(%aiName, "target", %target);

//  SuperAI::statePeriodic(%aiName, "SuperAI::quickTurnToTarget", 0.125);
  SuperAI::statePeriodic(%aiName, "GuardAI::engageState::maneuver", 0, 2);
}

function GuardAI::engageState::exit(%aiName) {
  echo("Exiting engage state (" @ %aiName @ ")");
}

function GuardAI::engageState::onTargetLOSLost(%aiName, %target) {
  echo("onTargetLOSLost @ Patrol state");
  SuperAI::changeState(%aiName, "patrol");
}

function GuardAI::engageState::onTargetDied(%aiName, %target) {
  echo("onTargetDied @ Patrol state");
  SuperAI::changeState(%aiName, "patrol");
}



function GuardAI::engageState::maneuver(%aiName) {
  %los = newObject("", StaticShape, SpyTestage::CoffeeCup, true);

  %this = SuperAI::getOwnedObject(%aiName);
  %pos = getBoxCenter(%this);
  %cpos = getBoxCenter(%this);

  %targ = SuperAI::getStateVar(%aiName, "target");
  %targPos = getBoxCenter(%targ);//Matrix::subMatrix(GameBase::getMuzzleTransform(%targ), 3, 4, 3, 1, 0, 3);

  %dist = Vector::getDistance(%pos, %targPos);

  %vec = Vector::sub(%targPos, %pos);
  %perpVec = -getWord(%vec, 1) @ " " @ getWord(%vec, 0) @ " 0";
  %perpVec = Vector::resize(%perpVec, 3);

  %movePos = -1;
  if (GuardAI::engageState::maneuverCheckPos(%aiName, %this, Vector::add(%pos, %perpVec), Vector::add(%cpos, %perpVec), %targ, %targPos, %los)) {
    %movePos = Vector::add(%pos, %perpVec);
  }

  if (GuardAI::engageState::maneuverCheckPos(%aiName, %this, Vector::sub(%pos, %perpVec), Vector::sub(%cpos, %perpVec), %targ, %targPos, %los)) {
    if (%movePos == -1)
      %movePos = Vector::sub(%pos, %perpVec);
    else if (getRandom() > 0.5)
      %movePos = Vector::sub(%pos, %perpVec);
  }

  if (%movePos != -1)
    SuperAI::moveTo(%aiName, %movePos);

  //SuperAI::jump(%aiName);

  deleteObject(%los);

  return getRandom()*2 + 1;
}

function GuardAI::engageState::maneuverCheckPos(%aiName, %this, %pos2, %cpos2, %targ, %targPos, %los) {
  if (GameBase::testPosition(%this, %pos2)) {
    GameBase::setPosition(%los, %cpos2);
    if (GameBase::getLOSInfo(%los, 5, "-1.57 0 0")) {
      GameBase::setPosition(%los, "0 0 -100000");
      if (getLOSInfo(%cpos2, getBoxCenter(%this), ~0) && $los::object == %this) {
        return true;
      }
    }
  }
  return false;
}


function GuardAI::test(%name) {
  exec("ai\\aistatesystem2.cs");
  exec("ai\\aimacros.cs");
  exec("ai\\aigraph.cs");
  exec("ai\\aiutil.cs");
//  exec("ai\\GuardAI.cs");

  if (%name == "") %name = "Bob";

  focusserver();
  SuperAI::Spawn(%name, DMMale, GameBase::getPosition(Client::getObserverCamera(2049)), 0, GuardAI);
}