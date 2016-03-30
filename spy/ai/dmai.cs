$AIStateSystem::eventRedirect["DMAI", "engage", "targetLOSLost"] = true;
$AIStateSystem::eventRedirect["DMAI", "engage", "targetDied"] = true;
$AIStateSystem::eventRedirect["DMAI", "patrol", "checktarget"] = true;
$AIStateSystem::eventRedirect["DMAI", "patrol", "enemiesFound"] = true;

function DMAI::init(%aiName, %marker) {
  %this = SuperAI::getOwnedObject(%aiName);

  Player::setItemCount(%this, Challenger, 1);
  Player::setItemCount(%this, ChallengerAmmo, 1000);
  Player::mountItem(%this, Challenger, 0);

  SuperAI::setVar(%aiName, "position", GameBase::getPosition(%this));

  SuperAI::changeState(%aiName, "patrol");
}

function DMAI::onTargetDied() { echo("---DIED"); }
function DMAI::onTargetLOSAcquired() { echo("---ACQ");}
function DMAI::onTargetLOSLost() { echo("---LOST");}
function DMAI::onKilled(%aiName) { SuperAI::respawn(%aiName, SuperAI::getVar(%aiName, "position"), 0, 3); }
function DMAI::onDamage() {}





function DMAI::patrolState::enter(%aiName, %nextWaypoint) {
  echo("Entering patrol state (" @ %aiName @ ")");

  AI::setVar(%aiName, "iq", 10000);
  AI::setVar(%aiName, "spotDist", 1000);

  SuperAI::enemySearchMacro(%aiName);

  DMAI::roam(%aiName, %nextWaypoint);
}

function DMAI::roam(%aiName, %nextWaypoint) {

  if (%nextWaypoint == "") {
    %minDist = 100000000;
    %minDistMarker = -1;

    %g = nameToID("MissionGroup\\AIGraph");
    %pos = SuperAI::getPosition(%aiName);
    for (%i = 0; %i < Group::objectCount(%g); %i++) {
      %m = Group::getObject(%g, %i);
      %dist = Vector::getDistance(%pos, GameBase::getPosition(%m));
      if (%dist < %minDist) {
        if (SuperAI::canAISee(%aiName, %m, AI::getVar(%aiName, "spotDist"))) {
          %minDist = %dist;
          %minDistMarker = %m;
        }
      }
    }
    if (%minDistMarker == -1) { echo("CRYING NOW"); return; }
  } else {
    %minDistMarker = %nextWaypoint;
  }

  echo("- Roaming to " @ %minDistMarker @ " (" @ GameBase::getPosition(%minDistMarker) @ "), (" @ %aiName @ ")");

  %d = SuperAI::DirectiveWaypoint(%aiName, GameBase::getPosition(%minDistMarker));
  SuperAI::DirectiveCallback(%aiName, "DMAI::arrivedAtWaypoint", %d, 2, %minDistMarker, -1);
}

function DMAI::arrivedAtWaypoint(%aiName, %curWaypoint, %lastWaypoint) {
  SuperAI::cancelWaypoints(%aiName);

    %numDests = 0;
    for (%i = 0; %i < AIGraph::getNumEdges(%curWaypoint); %i++) {
      %d = AIGraph::getEdgeMarker(%curWaypoint, %i);
      if (%d == %lastWaypoint) {
        %lastWaypointEdge = %i;
      } else {
        if (getRandom() < 1/(%numDests++)) %destEdge = %i;
      }
    }

    if (%numDests == 0) %destEdge = %lastWaypointEdge;

    if (%destEdge == "" || %destEdge == -1) {
      echo("NO EDGES TO FOLLOW :(");
      return;
    }

    DMAI::followGraphEdge(%aiName, %curWaypoint, %destEdge);
}

function DMAI::followGraphEdge(%aiName, %marker, %edgeNum) {
  %edgeType = AIGraph::getEdgeType(%marker, %edgeNum);

  if (%edgeType == "grapple") {
    SuperAI::changeState(%aiName, "grapple", 2, %marker, %edgeNum);
  } else {
    %dest = AIGraph::getEdgeMarker(%marker, %edgeNum);

    echo("Heading to "@%dest@" (" @ GameBase::getPosition(%dest) @ ")");

    %d = SuperAI::DirectiveWaypoint(%aiName, GameBase::getPosition(%dest));
    SuperAI::DirectiveCallback(%aiName, "DMAI::arrivedAtWaypoint", %d, 2, %dest, %marker);
  }
}

function DMAI::patrolState::exit(%aiName) {
  echo("Exiting patrol state (" @ %aiName @ ")");
}

function DMAI::patrolState::onCheckTarget(%aiName, %target) {
  if (getSimTime() < SuperAI::getVar(%aiName, "hidingUntil")) return false;
  return true;
}

function DMAI::patrolState::onEnemiesFound(%aiName, %enemies) {
  SuperAI::changeState(%aiName, engage, 1, Group::getObject(%enemies, 0));
}






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

function DMAI::grappleState::engageGrappleMarker(%aiName) {
  %curGrapPt = SuperAI::getStateVar(%aiName, "currentGrapplePoint");
  %grapplePoint = SuperAI::getStateVar(%aiName, "grapplePoint" @ %curGrapPt);

  DMAI::quickTurnTo(%aiName, %grapplePoint);
  AI::DirectiveTargetPoint(%aiName, %grapplePoint, 2000);

  SuperAI::stateSchedule(%aiName, "DMAI::grappleState::waitForAim(\"" @ %aiName @ "\");", 0.1);
}

function DMAI::grappleState::waitForAim(%aiName) {
  %this = SuperAI::getOwnedObject(%aiName);
  %curGrapPt = SuperAI::getStateVar(%aiName, "currentGrapplePoint");
  %grapplePoint = SuperAI::getStateVar(%aiName, "grapplePoint" @ %curGrapPt);
  if (GameBase::getLOSInfo(%this, $Grappler::maxRange)) {
    if (Vector::getDistance($los::position, %grapplePoint) < 3.0) {
      AI::DirectiveRemove(%aiName, 2000); // targetPoint
      SuperAI::DirectiveWaypoint(%aiName, %grapplePoint);
      Grappler::grapple(%this);
      SuperAI::stateSchedule(%aiName, "DMAI::grappleState::doSwing(\"" @ %aiName @ "\");", 0.05);
      return;
    }
  }

  SuperAI::stateSchedule(%aiName, "DMAI::grappleState::waitForAim(\"" @ %aiName @ "\");", 0.1);
}

function DMAI::grappleState::doSwing(%aiName) {
  %curGrapPt = SuperAI::getStateVar(%aiName, "currentGrapplePoint");
  %grapplePoint = SuperAI::getStateVar(%aiName, "grapplePoint" @ %curGrapPt);

  AI::DirectiveRemove(%aiName, "*");
  SuperAI::DirectiveWaypoint(%aiName, %grapplePoint);

  SuperAI::jump(%aiName);

  SuperAI::stateSchedule("SuperAI::cancelWaypoints(\""@%aiName@"\");SuperAI::DirectiveWaypoint(\""@%aiName@"\", \"" @ GameBase::getPosition(SuperAI::getStateVar(%aiName, "targetPlane" @ %curGrapPt)) @ "\");", 0.25);
  SuperAI::statePeriodic(%aiName, "DMAI::grappleState::controlSwing", 0.25);
}

function DMAI::grappleState::controlSwing(%aiName) {
  %curGrapPt = SuperAI::getStateVar(%aiName, "currentGrapplePoint");
  %startMarker = SuperAI::getStateVar(%aiName, "startMarker");
  %grapPos = SuperAI::getStateVar(%aiName, "grapplePoint" @ %curGrapPt);

  %this = SuperAI::getOwnedObject(%aiName);
  %pos = getBoxCenter(%this);//GameBase::getPosition(%this);
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
      AI::DirectiveRemove(%aiName, "*");
      DMAI::grappleState::engageGrappleMarker(%aiName);
    } else {
      SuperAI::changeState(%aiName, "patrol", 1, SuperAI::getStateVar(%aiName, "targetMarker"));
    }
  } else {
    if (Vector::dot(Item::getVelocity(%this), %normal) < 0) DMAI::Grappler::swing(%this, 0.25);
    %distDelta = Vector::getDistance(%pos, %grapPos) - Vector::getDistance(%targPos, %grapPos);

    if (%distDelta > 0) DMAI::Grappler::ascend(%this, 0.25);
    else if (%distDelta < -0.25) DMAI::Grappler::descend(%this, 0.25);
  }
}

function DMAI::Grappler::ascend(%this, %time) {
  %this.grappleDist = max(%this.grappleDist - $Grappler::ascendSpeed/$Grappler::callRate*%time, 0);
}
function DMAI::Grappler::descend(%this, %time) {
  %this.grappleDist = min(%this.grappleDist + $Grappler::descendSpeed/$Grappler::callRate*%time, $Grappler::maxRange);
}
function DMAI::Grappler::swing(%this, %time) {
  %vel = Item::getVelocity(%this);
  %vel = Vector::add(%vel, Vector::getFromRot(GameBase::getRotation(%this), $Grappler::swingStrength/$Grappler::callRate*%time));
  Item::setVelocity(%this, %vel);
}







function DMAI::engageState::enter(%aiName, %target) {
  echo("Entering engage state (" @ %aiName @ ")");


  AI::setVar(%aiName, "attackMode", 1);
  AI::DirectiveTarget(%aiName, GameBase::getOwnerClient(%target), 1000);

  SuperAI::statePeriodic(%aiName, "DMAI::quickTurn", 0.125);

  SuperAI::setStateVar(%aiName, "target", %target);

  SuperAI::stateSchedule(%aiName, "DMAI::engageState::maneuver(\"" @ %aiName @ "\");", 2);
}

function DMAI::engageState::exit(%aiName) {
  echo("Exiting engage state (" @ %aiName @ ")");
}

function DMAI::engageState::onTargetLOSLost(%aiName, %target) {
  echo("onTargetLOSLost @ Patrol state");
  SuperAI::changeState(%aiName, "patrol");
}

function DMAI::engageState::onTargetDied(%aiName, %target) {
  echo("onTargetDied @ Patrol state");
  SuperAI::changeState(%aiName, "patrol");
}



function DMAI::engageState::maneuver(%aiName) {
  %los = newObject("", StaticShape, SpyTestage::CoffeeCup, true);

  %this = SuperAI::getOwnedObject(%aiName);
  %pos = GameBase::getPosition(%this);
  %cpos = getBoxCenter(%this);

  %targ = SuperAI::getStateVar(%aiName, "target");
  %targPos = getBoxCenter(%targ);//Matrix::subMatrix(GameBase::getMuzzleTransform(%targ), 3, 4, 3, 1, 0, 3);

  %dist = Vector::getDistance(%pos, %targPos);

  for (%d = 3; %d < 12; %d += 3) {
  for (%i = 0; %i < 8; %i++) {
    %offset = Vector::getFromRot("0 0 " @ (%i*$PI/4), %d);
    %pos2 = Vector::add(%pos, %offset);
    %cpos2 = Vector::add(%cpos, %offset);
  }
  }

  deleteObject(%los);
}

function DMAI::engageState::maneuverCheckPos(%this, %pos2, %cpos2, %targ, %targPos, %los) {
    if (GameBase::testPosition(%this, %pos2)) {
      GameBase::setPosition(%los, %cpos2);
      echo("1: " @ %cpos2);
      if (GameBase::getLOSInfo(%los, 5, "-1.57 0 0")) {
      echo("2");
        GameBase::setPosition(%los, %cpos2);
        echo(%targPos, ",", %cpos2, ",", %dist);

        if (GameBase::getLOSInfo(%los, %dist+%d+2, Vector::getRot(Vector::sub(%targPos, %cpos2))) && $los::object != %targ && $los::object != %this) {
      echo("3: ", $los::object, ",", %targ);
          schedule("SuperAI::DirectiveWaypoint(\""@%aiName@"\", \""@%pos2@"\");", 0);
          
SuperAI::setVar(%aiName, "hidingUntil", getSimTime() + 1);
  deleteObject(%los);
return;
        }
      }
    }
}



function DMAI::quickTurn(%aiName) {
  %pos1 = SuperAI::getPosition(%aiName);
  %pos2 = GameBase::getPosition(AI::getTarget(%aiName));


  %dirRot = getWord(Vector::getRot(Vector::sub(%pos2, %pos1)), 2);
  %aiRot = getWord(SuperAI::getRotation(%aiName), 2);

  if (abs(%dirRot - %aiRot) > $PI/24) SuperAI::setRotation(%aiName, "0 0 " @ %dirRot);
}

function DMAI::quickTurnTo(%aiName, %pos2) {
  %pos1 = SuperAI::getPosition(%aiName);

  %dirRot = getWord(Vector::getRot(Vector::sub(%pos2, %pos1)), 2);
  %aiRot = getWord(SuperAI::getRotation(%aiName), 2);

  if (abs(%dirRot - %aiRot) > $PI/24) SuperAI::setRotation(%aiName, "0 0 " @ %dirRot);
}





function DMAI::test() {
  exec("ai\\dmai.cs");
  exec("ai\\aistatesystem.cs");

  focusserver();
  SuperAI::Spawn("BOB", DMMale, GameBase::getPosition(Client::getObserverCamera(2049)), 0, DMAI);
}