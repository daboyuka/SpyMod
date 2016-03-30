$AIStateSystem::eventRedirect["DMAI2", "engage", "targetLOSLost"] = true;
$AIStateSystem::eventRedirect["DMAI2", "engage", "targetDied"] = true;
$AIStateSystem::eventRedirect["DMAI2", "patrol", "checktarget"] = true;
$AIStateSystem::eventRedirect["DMAI2", "patrol", "enemiesFound"] = true;
$AIStateSystem::eventRedirect["DMAI2", "patrol", "waypointReached"] = true;
$AIStateSystem::eventRedirect["DMAI2", "elev", "waypointReached"] = true;

function DMAI2::init(%aiName, %marker) {
  %this = SuperAI::getOwnedObject(%aiName);

  Player::setItemCount(%this, Challenger, 1);
  Player::setItemCount(%this, ChallengerAmmo, 1000);
  Player::mountItem(%this, Challenger, 0);

  SuperAI::setVar(%aiName, "position", GameBase::getPosition(%this));

  SuperAI::changeState(%aiName, "patrol");
}

function DMAI2::onTargetDied() { echo("---DIED"); }
function DMAI2::onTargetLOSAcquired() { echo("---ACQ"); }
function DMAI2::onTargetLOSLost() { echo("---LOST"); }
function DMAI2::onKilled(%aiName) { SuperAI::respawn(%aiName, SuperAI::getVar(%aiName, "position"), 0, 10); }
function DMAI2::onDamage() {}
function DMAI2::onReachedWaypoint() {}


//
// Patrol state
//

function DMAI2::patrolState::enter(%aiName, %nextWaypoint, %lastWaypoint) {
  echo("Entering patrol state (" @ %aiName @ ")");

  AI::setVar(%aiName, "iq", 100);
  AI::setVar(%aiName, "spotDist", 1000);

  SuperAI::enemySearchMacro(%aiName);

  DMAI2::roam(%aiName, %nextWaypoint, %lastWaypoint);
}

function DMAI2::patrolState::exit(%aiName) {
  echo("Exiting patrol state (" @ %aiName @ ")");
}

// Actions

function DMAI2::roam(%aiName, %nextWaypoint) {

  if (%nextWaypoint == "") {
    %minDistMarker = AIGraph::getClosestMarkerToAI(%aiName, nameToID("MissionGroup\\AIGraph"));
  } else {
    %minDistMarker = %nextWaypoint;
  }

  echo("- Roaming to " @ %minDistMarker @ " (" @ GameBase::getPosition(%minDistMarker) @ "), (" @ %aiName @ ")");

  SuperAI::setStateVar(%aiName, "lastWaypoint", -1);
  SuperAI::setStateVar(%aiName, "nextWaypoint", %minDistMarker);
  SuperAI::moveTo(%aiName, GameBase::getPosition(%minDistMarker));
}

function DMAI2::pickNextEdge(%aiName, %curMarker, %prevMarker) {
  %numDests = 0;
  for (%i = 0; %i < AIGraph::getNumEdges(%curMarker); %i++) {
    %d = AIGraph::getEdgeMarker(%curMarker, %i);
    if (%d == %prevMarker) {
      %prevEdge = %i;
    } else {
      if (getRandom() < 1/(%numDests++)) %destEdge = %i;
    }
  }

  if (%numDests == 0)
    %destEdge = %prevEdge;

  if (%destEdge == "" || %destEdge == -1) {
    echo("NO EDGES TO FOLLOW :(");
    return -1;
  }

  return %destEdge;
}

function DMAI2::followGraphEdge(%aiName, %marker, %edgeNum) {
  %edgeType = AIGraph::getEdgeType(%marker, %edgeNum);

  if (%edgeType == "grapple") {
    DMAI2::followGrappleEdge(%aiName, %marker, %edgeNum);
  } else if (%edgeType == "elev") {
    DMAI2::followElevEdge(%aiName, %marker, %edgeNum);
  } else {
    %dest = AIGraph::getEdgeMarker(%marker, %edgeNum);

    echo("Heading to "@%dest@" (" @ GameBase::getPosition(%dest) @ ")");

    SuperAI::setStateVar(%aiName, "lastWaypoint", %marker);
    SuperAI::setStateVar(%aiName, "nextWaypoint", %dest);
    %d = SuperAI::moveTo(%aiName, GameBase::getPosition(%dest));
  }
}

// Events

function DMAI2::patrolState::onWaypointReached(%aiName) {
  %prevWaypoint = SuperAI::getStateVar(%aiName, "lastWaypoint"); // Came from here
  %curWaypoint = SuperAI::getStateVar(%aiName, "nextWaypoint"); // Just arrived here

  %destEdge = DMAI2::pickNextEdge(%aiName, %curWaypoint, %prevWaypoint);

  DMAI2::followGraphEdge(%aiName, %curWaypoint, %destEdge);
}

function DMAI2::patrolState::onCheckTarget(%aiName, %target) {
  if (getSimTime() < SuperAI::getVar(%aiName, "hidingUntil")) return false;
  return true;
}

function DMAI2::patrolState::onEnemiesFound(%aiName, %enemies) {
  SuperAI::changeState(%aiName, engage, 1, Group::getObject(%enemies, 0));
}



function DMAI2::followGrappleEdge(%aiName, %startMarker, %edgeNum) {
  SuperAI::changeState(%aiName, grapple, 2, %startMarker, %edgeNum);
}

function DMAI2::followElevEdge(%aiName, %startMarker, %edgeNum) {
  SuperAI::changeState(%aiName, elev, 2, %startMarker, %edgeNum);
}

//
// Grapple state
//

function DMAI::grappleState::enter(%aiName, %startMarker, %edgeNum) {
  AI::setVar(%aiName, "attackMode", 1);

  SuperAI::setStateVar(%aiName, "startMarker", %startMarker);
  SuperAI::setStateVar(%aiName, "targetMarker", AIGraph::getEdgeMarker(%startMarker, %edgeNum));
  SuperAI::setStateVar(%aiName, "currentGrapplePoint", 0);

  for (%i = 0; AIGraph::getEdgeArg(%startMarker, %edgeNum, "grapplePoint" @ %i) != ""; %i++) {
    SuperAI::setStateVar(%aiName, "grapplePoint" @ %i, AIGraph::getEdgeArg(%startMarker, %edgeNum, "grapplePoint" @ %i));
    SuperAI::setStateVar(%aiName, "targetPlane" @ %i, AIGraph::getEdgeArg(%startMarker, %edgeNum, "targetPlane" @ %i));
  }

  DMAI::grappleState::engageGrappleMarker(%aiName);
}

function DMAI2::grappleState::engageGrappleMarker(%aiName) {
  %curGrapPt = SuperAI::getStateVar(%aiName, "currentGrapplePoint");
  %grapplePoint = SuperAI::getStateVar(%aiName, "grapplePoint" @ %curGrapPt);

  DMAI2::quickTurnTo(%aiName, %grapplePoint);
  SuperAI::targetPoint(%aiName, %grapplePoint);

  SuperAI::stateSchedule(%aiName, "DMAI2::grappleState::waitForAim(\"" @ %aiName @ "\");", 0.1);
}

function DMAI2::grappleState::waitForAim(%aiName) {
  %this = SuperAI::getOwnedObject(%aiName);
  %curGrapPt = SuperAI::getStateVar(%aiName, "currentGrapplePoint");
  %grapplePoint = SuperAI::getStateVar(%aiName, "grapplePoint" @ %curGrapPt);

  if (GameBase::getLOSInfo(%this, $Grappler::maxRange)) {
    if (Vector::getDistance($los::position, %grapplePoint) < 3.0) {
      SuperAI::cancelTarget(%aiName);
      SuperAI::moveTo(%aiName, %grapplePoint);

      Grappler::grapple(%this);
      SuperAI::stateSchedule(%aiName, "DMAI2::grappleState::doSwing(\"" @ %aiName @ "\");", 0.05);
      return;
    }
  } else {
    SuperAI::stateSchedule(%aiName, "DMAI2::grappleState::waitForAim(\"" @ %aiName @ "\");", 0.1);
  }
}

function DMAI2::grappleState::doSwing(%aiName) {
  %curGrapPt = SuperAI::getStateVar(%aiName, "currentGrapplePoint");
  %grapplePoint = SuperAI::getStateVar(%aiName, "grapplePoint" @ %curGrapPt);
  %targetPlanePoint = GameBase::getPosition(SuperAI::getStateVar(%aiName, "targetPlane" @ %curGrapPt));

  SuperAI::moveTo(%aiName, %grapplePoint);

  SuperAI::jump(%aiName);

  SuperAI::stateSchedule("SuperAI::moveTo(\""@%aiName@"\", \"" @ %targetPlanePoint @ "\");", 0.25);
  SuperAI::statePeriodic(%aiName, "DMAI2::grappleState::controlSwing", 0.25);
}

function DMAI2::grappleState::controlSwing(%aiName) {
  %this = SuperAI::getOwnedObject(%aiName);
  if (!%this.grappling) {
    SuperAI::changeState(%aiName, patrol); // Our hook has been shot or something, so fall (hopefully don't die), and search for enemies
    return;
  }

  %curGrapPt = SuperAI::getStateVar(%aiName, "currentGrapplePoint");
  %startMarker = SuperAI::getStateVar(%aiName, "startMarker");
  %grapPos = SuperAI::getStateVar(%aiName, "grapplePoint" @ %curGrapPt);

  %pos = getBoxCenter(%this);
  %targPlane = AIGraph::getHelperObject(%startMarker, SuperAI::getStateVar(%aiName, "targetPlane" @ %curGrapPt));
  %targPos = GameBase::getPosition(%targPlane);

  %normal = Vector::getFromRot(GameBase::getRotation(%targPlane));
  %sideaxis = Vector::cross(%normal, "0 0 1");
  %upaxis = Vector::cross(%sideaxis, %normal);

  %vec = Vector::sub(%pos, %targPos);
  %normalDist = Vector::dot(%vec, %normal);
  %sideDist = Vector::dot(%vec, %sideaxis);
  %upDist = Vector::dot(%vec, %upaxis);

  if (%normalDist <= 0 && abs(%sideDist) < %targPlane.width && abs(%upDist) < %targPlane.height) {
    Grappler::ungrapple(%this);

    if (SuperAI::getStateVar(%aiName, "grapplePoint" @ (%curGrapPt+1)) != "") {
      SuperAI::setStateVar(%aiName, "currentGrapplePoint", %curGrapPt + 1);

      DMAI2::grappleState::engageGrappleMarker(%aiName);
    } else {
      SuperAI::changeState(%aiName, "patrol", 1, SuperAI::getStateVar(%aiName, "targetMarker"));
    }
  } else {
    if (Vector::dot(Item::getVelocity(%this), %normal) < 0)
      DMAI2::Grappler::swing(%this, 0.25);

    %distDelta = Vector::getDistance(%pos, %grapPos) - Vector::getDistance(%targPos, %grapPos);

    if (%distDelta > 0) DMAI2::Grappler::ascend(%this, 0.25);
    else if (%distDelta < -0.25) DMAI2::Grappler::descend(%this, 0.25);
  }
}

function DMAI2::Grappler::ascend(%this, %time) {
  %this.grappleDist = max(%this.grappleDist - $Grappler::ascendSpeed/$Grappler::callRate*%time, 0);
}

function DMAI2::Grappler::descend(%this, %time) {
  %this.grappleDist = min(%this.grappleDist + $Grappler::descendSpeed/$Grappler::callRate*%time, $Grappler::maxRange);
}

function DMAI2::Grappler::swing(%this, %time) {
  %vel = Item::getVelocity(%this);
  %vel = Vector::add(%vel, Vector::getFromRot(GameBase::getRotation(%this), $Grappler::swingStrength/$Grappler::callRate*%time));
  Item::setVelocity(%this, %vel);
}



//
// Elevator state
//

function DMAI2::elevState::enter(%aiName, %startMarker, %edgeNum) {
  AI::setVar(%aiName, "attackMode", 1);

  SuperAI::setStateVar(%aiName, "startMarker", %startMarker);
  SuperAI::setStateVar(%aiName, "targetMarker", AIGraph::getEdgeMarker(%startMarker, %edgeNum));

  %elevButtonPoint = AIGraph::getEdgeArg(%startMarker, %edgeNum, "elevButtonPoint");
  %elevGroup = AIGraph::getEdgeArg(%startMarker, %edgeNum, "elevGroup");

  for (%i = 0; %i < Group::objectCount(%elevGroup); %i++) {
    %o = Group::getObject(%elevGroup, %i);
    if (getObjectType(%o) == "SimPath")
      %elevPath = %o;
    else if (getObjectType(%o) == "Moveable")
      %elevObj = %o;
  }

  SuperAI::setStateVar(%aiName, "elevPath",             %elevPath);
  SuperAI::setStateVar(%aiName, "elevObj",              %elevObj);
  SuperAI::setStateVar(%aiName, "startElevWaypointInd", AIGraph::getEdgeArg(%startMarker, %edgeNum, "startElevWaypointInd"));
  SuperAI::setStateVar(%aiName, "endElevWaypointInd",   AIGraph::getEdgeArg(%startMarker, %edgeNum, "endElevWaypointInd"));
  SuperAI::setStateVar(%aiName, "elevButtonPoint",      %elevButtonPoint);

  if (%elevButtonPoint)
    DMAI2::elevState::pushButton(%aiName);
  else
    DMAI2::elevState::waitForElevator(%aiName);
}

function DMAI2::elevState::exit(%aiName) {}

function DMAI2::elevState::pushButton(%aiName) {
  %elevButtonPoint = SuperAI::getStateVar(%aiName, "elevButtonPoint");

  %vec = Vector::sub(%elevButtonPoint, SuperAI::getPosition(%aiName));
  %elevButtonPoint = Vector::add(%elevButtonPoint, Vector::resize(%vec, 0.5));

  %d = SuperAI::moveTo(%aiName, %elevButtonPoint);
  SuperAI::setStateVar(%aiName, "elevButtonPushDirective", %d);
  SuperAI::setStateVar(%aiName, "waitingForPush", true);

  SuperAI::stateSchedule(%aiName, "if (SuperAI::getStateVar(\""@%aiName@"\", waitingForPush)) DMAI2::elevState::walkBackAndWaitForElevator(\""@%aiName@"\");", 2);
}

function DMAI2::elevState::walkBackAndWaitForElevator(%aiName) {
  SuperAI::setStateVar(%aiName, "waitingForPush", "");

  %startMarker = SuperAI::getStateVar(%aiName, "startMarker");

  %d = SuperAI::moveTo(%aiName, GameBase::getPosition(%startMarker));
  SuperAI::setStateVar(%aiName, "waitingForElevDirective", %d);

  DMAI2::elevState::waitForElevator(%aiName);
}

function DMAI2::elevState::onWaypointReached(%aiName, %directive) {
  if (%directive == SuperAI::getStateVar(%aiName, "elevButtonPushDirective")) {
    DMAI2::elevState::walkBackAndWaitForElevator(%aiName);
  } else if (%directive == SuperAI::getStateVar(%aiName, "waitingForElevDirective")) {
    %startElevMarker = DMAI2::elevState::getElevatorMarker(%aiName, SuperAI::getStateVar(%aiName, "startElevWaypointInd"));

    DMAI2::quickTurnTo(%aiName, GameBase::getPosition(%startElevMarker));
  } else if (%directive == SuperAI::getStateVar(%aiName, "elevGetOnDirective")) {
    %targetMarker = SuperAI::getStateVar(%aiName, "targetMarker");

    SuperAI::cancelMove(%aiName);
    DMAI2::quickTurnTo(%aiName, GameBase::getPosition(%targetMarker));

    DMAI2::elevState::waitForElevatorDone(%aiName);
  } else if (%directive == SuperAI::getStateVar(%aiName, "doneDirective")) {
    SuperAI::changeState(%aiName, "patrol", 1, SuperAI::getStateVar(%aiName, "targetMarker"), SuperAI::getStateVar(%aiName, "startMarker"));
  }
}

function DMAI2::elevState::getElevatorMarker(%aiName, %index) {
  return Group::getObject(SuperAI::getStateVar(%aiName, "elevPath"), %index);
}

function DMAI2::elevState::waitForElevator(%aiName, %waitsLeft) {
  if (String::ICompare(%waitsLeft, "") == 0 || %waitsLeft < 0)
    %waitsLeft = 12;

  %elevObj = SuperAI::getStateVar(%aiName, "elevObj");
  %startElevWaypointInd = SuperAI::getStateVar(%aiName, "startElevWaypointInd");

  if (Moveable::getPosition(%elevObj) == %startElevWaypointInd) {
    DMAI2::elevState::getOn(%aiName);
  } else if (%waitsLeft == 0) {
    DMAI2::elevState::pushButton(%aiName);
  } else {
    SuperAI::stateSchedule(%aiName, "DMAI2::elevState::waitForElevator(\"" @ %aiName @ "\", " @ (%waitsLeft - 1) @ ");", 0.25);
  }
}

function DMAI2::elevState::getOn(%aiName) {
  %startElevMarker = DMAI2::elevState::getElevatorMarker(%aiName, %startElevWaypointInd);

  %d = SuperAI::moveTo(%aiName, GameBase::getPosition(%startElevMarker));
  SuperAI::setStateVar(%aiName, "elevGetOnDirective", %d);
}

function DMAI2::elevState::waitForElevatorDone(%aiName) {
  %elevObj = SuperAI::getStateVar(%aiName, "elevObj");
  %endElevWaypointInd = SuperAI::getStateVar(%aiName, "endElevWaypointInd");

  if (Moveable::getPosition(%elevObj) == %endElevWaypointInd) {
    %endMarker = SuperAI::getStateVar(%aiName, "targetMarker");

    %d = SuperAI::moveTo(%aiName, GameBase::getPosition(%endMarker));
    SuperAI::setStateVar(%aiName, "doneDirective", %d);
  } else {
    SuperAI::stateSchedule(%aiName, "DMAI2::elevState::waitForElevatorDone(\"" @ %aiName @ "\");", 0.25);
  }
}



//
// Engage state
//

function DMAI2::engageState::enter(%aiName, %target) {
  echo("Entering engage state (" @ %aiName @ ")");

  AI::setVar(%aiName, "attackMode", 1);
  SuperAI::targetPlayer(%aiName, Player::getClient(%target));

  SuperAI::setStateVar(%aiName, "target", %target);

  SuperAI::statePeriodic(%aiName, "DMAI2::quickTurn", 0.125);
  SuperAI::stateSchedule(%aiName, "DMAI2::engageState::maneuver(\"" @ %aiName @ "\");", 2);
}

function DMAI2::engageState::exit(%aiName) {
  echo("Exiting engage state (" @ %aiName @ ")");
}

function DMAI2::engageState::onTargetLOSLost(%aiName, %target) {
  echo("onTargetLOSLost @ Patrol state");
  SuperAI::changeState(%aiName, "patrol");
}

function DMAI2::engageState::onTargetDied(%aiName, %target) {
  echo("onTargetDied @ Patrol state");
  SuperAI::changeState(%aiName, "patrol");
}



function DMAI2::engageState::maneuver(%aiName) {
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
  if (DMAI2::engageState::maneuverCheckPos(%aiName, %this, Vector::add(%pos, %perpVec), Vector::add(%cpos, %perpVec), %targ, %targPos, %los)) {
    %movePos = Vector::add(%pos, %perpVec);
  }

  if (DMAI2::engageState::maneuverCheckPos(%aiName, %this, Vector::sub(%pos, %perpVec), Vector::sub(%cpos, %perpVec), %targ, %targPos, %los)) {
    if (%movePos == -1)
      %movePos = Vector::sub(%pos, %perpVec);
    else if (getRandom() > 0.5)
      %movePos = Vector::sub(%pos, %perpVec);
  }

  if (%movePos != -1)
    SuperAI::moveTo(%aiName, %movePos);

  SuperAI::jump(%aiName);

  deleteObject(%los);
}

function DMAI2::engageState::maneuverCheckPos(%aiName, %this, %pos2, %cpos2, %targ, %targPos, %los) {
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

function comment() {
      if (GameBase::getLOSInfo(%los, AI::getVar(%aiName, spotDist), Vector::getRot(Vector::sub(%targPos, %cpos2))) && $los::object != %targ && $los::object != %this) {
        echo("3: ", $los::object, ",", %targ);
        schedule("SuperAI::DirectiveWaypoint(\""@%aiName@"\", \""@%pos2@"\");", 0);

        SuperAI::setVar(%aiName, "hidingUntil", getSimTime() + 1);
        deleteObject(%los);
        return;
      }
}

function DMAI2::quickTurn(%aiName) {
  DMAI2::quickTurnTo(%aiName, GameBase::getPosition(AI::getTarget(%aiName)));
}

function DMAI2::quickTurnTo(%aiName, %pos2) {
  %pos1 = SuperAI::getPosition(%aiName);

  %dirRot = getWord(Vector::getRot(Vector::sub(%pos2, %pos1)), 2);
  %aiRot = getWord(SuperAI::getRotation(%aiName), 2);

  if (abs(%dirRot - %aiRot) > $PI/24) SuperAI::setRotation(%aiName, "0 0 " @ %dirRot);
}





function DMAI2::test(%name) {
//  exec("ai\\aistatesystem.cs");
//  exec("ai\\aigraph.cs");
//  exec("ai\\aiutil.cs");
//  exec("ai\\dmai2.cs");

  if (%name == "") %name = "Bob";

  focusserver();
  SuperAI::Spawn(%name, DMMale, GameBase::getPosition(Client::getObserverCamera(2049)), 0, DMAI2);
}