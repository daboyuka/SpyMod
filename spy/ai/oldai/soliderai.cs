
//////
// Solider SuperAI scripting
//////

ItemImageData SelectedAIObjectImage {
	shapeFile = "grenade";
	mountPoint = 2;
	mountOffset = "0 0 1";
};

ItemData SelectedAIObject {
	shapeFile = "grenade";
	imageType = SelectedAIObjectImage;
};

$SoliderAI::LINE_FORMATION = 0;
$SoliderAI::BOX_FORMATION = 1;
$SoliderAI::RING_FORMATION = 2;

function Squad::spawnHelpers(%client, %num) {
  Squad::spawnSquad(GameBase::GetPosition(%client), %num, DMMale, GameBase::getTeam(%client), %client);
}

function Squad::spawnSquad(%pos, %num, %armor, %team, %commander) {
  %squad = newObject("", SimSet);
  %squad.pos = %pos;
  %squad.formation = $SoliderAI::RING_FORMATION;
  for (%i = 0; %i < %num; %i++) {
    %name = SoliderAI::spawn(%armor, 0, 0, %team, -1);
    %obj = SuperAI::getOwnedObject(%name);
    %obj.squad = %squad;
    %obj.selected = true;
    Player::mountItem(%obj, SelectedAIObject, 7);
    addToSet(%squad, %obj);
  }

  %squad.commander = %commander;
  %commander.squad = %squad;

  Player::setItemCount(%commander, SquadRadio, 1);

  addToSet(MissionCleanup, %squad);

  Squad::initFormation(%squad);
}

function Squad::initFormation(%squad) {
  %num = Group::objectCount(%squad);
  for (%i = 0; %i < %num; %i++) {
    GameBase::setPosition(Group::getObject(%squad, %i), Vector::add(%squad.pos, Squad::getOffset(%squad.formation, %i, %num)));
  }
}

function Squad::getOffset(%formation, %i, %num) {
  if (%formation == $SoliderAI::LINE_FORMATION) {
    return (%i * 4 - ((%num - 1) / 2 * 6)) @ " " @ (0.5 - (%i % 2)) @ " 0";
  }
  if (%formation == $SoliderAI::BOX_FORMATION) {
    %w = floor(sqrt(%num));
    %h = ceil(%num / %w);
    %x = %i % %h;
    %y = (%i - %x) / %h;
    return ((%x - (%w - 1) / 2) * 7) @ " " @ ((%y - (%h - 1) / 2) * 7) @ " 0";
  }
  if (%formation == $SoliderAI::RING_FORMATION) {
    %radius = floor(2 * sqrt(%num) + 2);
    %x = %radius * cos(%i / %num * 2 * $PI);
    %y = %radius * sin(%i / %num * 2 * $PI);
    return %x @ " " @ %y @ " 0";
  }
}

function Squad::follow(%squad) {
  %num = Group::objectCount(%squad);
  for (%i = 0; %i < %num; %i++) {
    %obj = Group::getObject(%squad, %i);
    if (!%obj.selected) continue;
    AI::directiveRemove(%obj.aiName, 100);
    AI::directiveFollow(%obj.aiName, %squad.commander);
  }
}
function SquadCommander::followSelected(%commander) {
  %aiName = %commander.selectedAI;
  if (%aiName == "") return;
  AI::directiveRemove(%aiName, 100);
  AI::directiveFollow(%aiName, %commander, 100);
}
function SquadCommander::follow(%commander) {
  Squad::follow(%commander.squad);
}

function Squad::formUp(%squad, %formation) {
  %squad.pos = GameBase::getPosition(%squad.commander);
  %num = Group::objectCount(%squad);
  %num2 = 0;
  for (%i = 0; %i < %num; %i++) {
    %obj = Group::getObject(%squad, %i);
    if (%obj.selected) %num2++;
  }
  for (%i = 0; %i < %num; %i++) {
    %obj = Group::getObject(%squad, %i);
    if (!%obj.selected) continue;
    AI::directiveRemove(%obj.aiName, 100);
    AI::directiveWaypoint(%obj.aiName, Vector::add(%squad.pos, Squad::getOffset(%formation, %i, %num2)));
  }
}
function SquadCommander::formUp(%commander, %formation) {
  Squad::formUp(%commander.squad, %formation);
}


function Squad::formAtLOS(%squad, %formation) {
  if (!GameBase::getLOSInfo(Client::getControlObject(%squad.commander), 300)) return;
  %squad.pos = $los::position;
  %num = Group::objectCount(%squad);
  %num2 = 0;
  for (%i = 0; %i < %num; %i++) {
    %obj = Group::getObject(%squad, %i);
    if (%obj.selected) %num2++;
  }
  for (%i = 0; %i < %num; %i++) {
    %obj = Group::getObject(%squad, %i);
    if (!%obj.selected) continue;
    AI::directiveRemove(%obj.aiName, 100);
    AI::directiveWaypoint(%obj.aiName, Vector::add(%squad.pos, Squad::getOffset(%formation, %i, %num2)), 100);
  }
}
function SquadCommander::formAtLOSSelected(%commander) {
  %aiName = %commander.selectedAI;
  if (%aiName == "") return;
  if (!GameBase::getLOSInfo(Client::getControlObject(%commander), 300)) return;
  %point = $los::position;
  AI::directiveRemove(%aiName, 100);
  AI::directiveWaypoint(%aiName, %point, 100);
}
function SquadCommander::formAtLOS(%commander, %formation) {
  Squad::formAtLOS(%commander.squad, %formation);
}

function Squad::fireOnLOS(%squad) {
  if (!GameBase::getLOSInfo(Client::getControlObject(%squad.commander), 300)) return;
  %point = $los::position;
  %num = Group::objectCount(%squad);
  for (%i = 0; %i < %num; %i++) {
    %obj = Group::getObject(%squad, %i);
    if (!%obj.selected) continue;
    AI::directiveRemove(%obj.aiName, 100);
    AI::directiveTargetPoint(%obj.aiName, %point, 100);
    SoliderAI::activateGun(%obj.aiName);
  }
}
function SquadCommander::fireOnLOSSelected(%commander) {
  %aiName = %commander.selectedAI;
  if (%aiName == "") return;
  if (!GameBase::getLOSInfo(Client::getControlObject(%commander), 300)) return;
  %point = $los::position;
  AI::directiveRemove(%aiName, 100);
  AI::directiveTargetPoint(%aiName, %point, 100);
  SoliderAI::activateGun(%aiName);
}
function SquadCommander::fireOnLOS(%commander) {
  Squad::fireOnLOS(%commander.squad);
}

function Squad::cease(%squad) {
  %num = Group::objectCount(%squad);
  for (%i = 0; %i < %num; %i++) {
    %obj = Group::getObject(%squad, %i);
    if (!%obj.selected) continue;
    AI::directiveRemove(%obj.aiName, 100);
  }
}
function SquadCommander::ceaseSelected(%commander) {
  %aiName = %commander.selectedAI;
  if (%aiName == "") return;
  AI::directiveRemove(%aiName, 100);
}
function SquadCommander::cease(%commander) {
  Squad::cease(%commander.squad);
}

function Squad::killSquad(%squad) {
  %squad.commander.squad = "";
  %num = Group::objectCount(%squad);
  for (%i = 0; %i < %num; %i++) {
    %obj = Group::getObject(%squad, %i);
    AI::Delete(%obj.aiName);
  }
  deleteObject(%squad);
}

function Squad::selectSwitchAll(%squad) {
  %num = Group::objectCount(%squad);
  for (%i = 0; %i < %num; %i++) {
    %obj = Group::getObject(%squad, %i);
    %obj.selected = !%obj.selected;
    if (%obj.selected) Player::mountItem(%obj, SelectedAIObject, 7);
    else               Player::unmountItem(%obj, 7);
  }
}

function Squad::deselectAll(%squad) {
  %num = Group::objectCount(%squad);
  for (%i = 0; %i < %num; %i++) {
    %obj = Group::getObject(%squad, %i);
    %obj.selected = false;
    Player::unmountItem(%obj, 7);
  }
}

function Squad::selectAll(%squad) {
  %num = Group::objectCount(%squad);
  for (%i = 0; %i < %num; %i++) {
    %obj = Group::getObject(%squad, %i);
    %obj.selected = true;
    Player::mountItem(%obj, SelectedAIObject, 7);
  }
}

function Squad::selectSwitch(%squad, %solider) {
  %solider.selected = !%solider.selected;
  if (%solider.selected) Player::mountItem(%solider, SelectedAIObject, 7);
  else                   Player::unmountItem(%solider, 7);
}

function Squad::select(%squad, %solider) {
  %solider.selected = true;
  Player::mountItem(%solider, SelectedAIObject, 7);
}

function Squad::deselect(%squad, %solider) {
  %solider.selected = false;
  Player::unmountItem(%solider, 7);
}

function Squad::incTarg(%squad, %target, %inc) {
  %num = Group::objectCount(%squad);
  for (%i = 0; %i < %num; %i++) {
    %obj = Group::getObject(%squad, %i);
    SoliderAI::incTarg(%obj.aiName, %target, %inc);
  }
}





function SoliderAI::spawn(%armor, %pos, %rot, %team, %marker) {
  %aiName = SuperAI::Spawn(%armor, %pos, %rot, "soliderAI", %marker);
  GameBase::setTeam(AI::getID(%aiName), %team);
  return %aiName;
}

function SoliderAI::init(%aiName, %marker) {
  %id = AI::getID(%aiName);

  SuperAI::init(%aiName, -1);

  SuperAI::setVar(%aiName, "attackOnSight", true);
  SuperAI::setVar(%aiName, "attackOnDamage", true);
  SuperAI::setVar(%aiName, "fireOnTarget", true);
  SuperAI::setVar(%aiName, "chargeAtTarget", false);
  SuperAI::setVar(%aiName, "fastCheck", false);
  SuperAI::setVar(%aiName, "sightDist", 200);

  AI::setVar(%aiName, "iq", 100);
  AI::setVar(%aiName, "smartGuyMinAccuracy", 0.5);
  AI::setVar(%aiName, "spotDist", 200);
  AI::setVar(%aiName, "triggerWindup", -1);

  AI::CallbackPeriodic(%aiName, 1.5, "SoliderAI::checkLOSClear");

  Player::setItemCount(%id, Challenger, 1);
  Player::setItemCount(%id, ChallengerAmmo, 1000);

  if (SuperAI::getVar(%aiName, "chargeAtTarget")) AI::setVar(%aiName, "attackMode", 0);
  else                                            AI::setVar(%aiName, "attackMode", 1);

  AI::setScriptedTargets(%aiName);

  SuperAI::checkForEnemies(%aiName);
}

function SoliderAI::checkTarget(%aiName, %target) {
  %id = SuperAI::getOwnedObject(%aiName);
  if (AI::ActionResult(%aiName) != "Done") return false;
  if (%target != AI::getTarget(%aiName)) {
    if (AI::getTarget(%aiName) != -1 && (SoliderAI::getTarg(%aiName, AI::getTarget(%aiName)) > 0) || %id.targetSeen[AI::getTarget(%aiName)]) return false;
    if (%id.lastTargetTime + 0.5 > getSimTime()) return false;
  }

  %targPos = GameBase::getPosition(%target);
  %aiPos = GameBase::getPosition(%id);

  %speed = Vector::getDistance(0, Item::getVelocity(%target));
  %aiSpeed = Vector::getDistance(0, Item::getVelocity(%id));
  %dist = Vector::getDistance(%aiPos, %targPos);

  %targFactor = 10;

  %dot = Vector::dot(Vector::getFromRot(GameBase::getRotation(%id)), Vector::mul(Vector::sub(%targPos, %aiPos), 1 / %dist));
  %backTurned = %dot < 0;
  if (%backTurned && %dist > 15) %targFactor = 0;

  %targFactor *= tern(%speed < 1, 0.5, 1);   // If they aren't moving, they are harder to see
  %targFactor *= tern(%aiSpeed < 1, 3, 1);   // If we are standing still, we can see them better
  %targFactor *= 25 / %dist;                 // If over 25 meters, harder to see; if under, easier
  %targFactor *= tern(%backTurned, 0.2, 1);  // If our back is turned they can't see well
  %targFactor *= tern(!%backTurned, 0.2 + 12 * pow(%dot, 4), 1); // The closer to the enemy we are facing, the better we can see

  %targetHim = false;

  SoliderAI::incTarg(%aiName, %target, %targFactor);

  AI::setVar(%aiName, "triggerPct", AI::getVar(%aiName, "triggerPct"));

  if (SoliderAI::getTarg(%aiName, %target) >= 30) {
    %targetHim = true;//SuperAI::checkTarget(%aiName, %target);
    if (%targetHim && SuperAI::getVar(%aiName, "attackOnSight") && AI::getTarget(%aiName) != %target) {
      SoliderAI::attack(%aiName, %target);
      Squad::incTarg(%id.squad, %target, 50);
      if (%id.lastSquadEncounter[%target.squad] + 20 < getSimTime()) {
        AI::soundHelper(AI::getID(%aiName), %id.squad.commander, "incom2");
        %id.lastSquadEncounter[%target.squad] = getSimTime();
      } else %id.lastSquadEncounter[%target.squad] = getSimTime();
    }
  }
  return %targetHim;
}

function SoliderAI::incTarg(%aiName, %target, %amt) {
  %obj = SuperAI::getOwnedObject(%aiName);
  %obj.targ[%target] += %amt;
  schedule("if ("@%obj@".targ["@%target@"] == "@%obj.targ[%target]@")"@%obj@".targ["@%target@"]=0;",5);
}

function SoliderAI::getTarg(%aiName, %target) { return SuperAI::getOwnedObject(%aiName).targ[%target]; }

function SoliderAI::onTargetDied(%aiName, %target) {
  $SuperAI::AIData[%aiName, "targetLostTime"] = getSimTime();
  SoliderAI::targetLost(%aiName, %target);
}

function SoliderAI::onTargetLOSAcquired(%aiName, %target) {
  SuperAI::onTargetLOSAcquired(%aiName, %target);
  SuperAI::getOwnedObject(%aiName).targetSeen[%target] = true;
}

function SoliderAI::onTargetLOSLost(%aiName, %target) {
  %time = getSimTime();
  $SuperAI::AIData[%aiName, "targetLostTime"] = %time;
  schedule("if (AI::getTarget(\""@%aiName@"\") == -1 && $SuperAI::AIData[\""@%aiName@"\", \"targetLostTime\"] == "@%time@")" @
           "SoliderAI::targetLost(\""@%aiName@"\","@%target@");", 1);
}

function SoliderAI::onDamage(%aiName, %shooter, %type, %value, %pos, %vec) {
  %id = SuperAI::getOwnedObject(%aiName);
  if (%type == $SuperAI::CheckDamageType || %type == $PoisonGasDamageType || %type == $LandingDamageType) return;
  if (%id.squad.commander && Player::getClient(%shooter) == %id.squad.commander) return;
  if (AI::getTarget(%aiName) != -1 && SoliderAI::getTarg(%aiName, AI::getTarget(%aiName)) > 0) return;

  if (%value > 0.01 && SuperAI::getVar(%aiName, "attackOnDamage")) {
    if (%shooter > 0) {
      SoliderAI::attack(%aiName, %shooter);
    } else {
      AI::directiveRemove(%aiName, 100);
      %tPos = Vector::add(%pos, Vector::mul(%vec, -20));
      AI::directiveTargetPoint(%aiName, %tPos);
      SoliderAI::deactivateGun(%aiName);
    }
  }
}

function SoliderAI::onKilled(%aiName, %this) {
  Player::unmountItem(%this, 7);
  if (%this.squad) {
    removeFromSet(%this.squad, %this);
    if (Group::objectCount(%this.squad) == 0) {
      Squad::killSquad(%this.squad);
    }
  }
}

function SoliderAI::targetLost(%aiName, %target) {
  SuperAI::getOwnedObject(%aiName).targ[%target] = 0;
  SuperAI::getOwnedObject(%aiName).targetSeen[%target] = false;
  AI::DirectiveRemove(%aiName, 100);
  SoliderAI::deactivateGun(%aiName);
}

function SoliderAI::attack(%aiName, %target) {
  if (SuperAI::isDead(%aiName)) return;

  %curTarget = AI::getTarget(%aiName);
  if (%curTarget == GameBase::getOwnerClient(%target)) return;

  AI::directiveRemove(%aiName, 100);
  AI::directiveTarget(%aiName, Player::getClient(%target), 100);

  SuperAI::getOwnedObject(%aiName).lastTargetTime = getSimTime();

  SoliderAI::activateGun(%aiName);
}

function SoliderAI::activateGun(%aiName) {
  AI::setVar(%aiName, "triggerPct", 0.7);
  Player::useItem(SuperAI::getOwnedObject(%aiName), Challenger);
  //Challenger::reload(SuperAI::getOwnedObject(%aiName));
}

function SoliderAI::deactivateGun(%aiName) {
  AI::setVar(%aiName, "triggerPct", 0);
  Player::unmountItem(SuperAI::getOwnedObject(%aiName), 0);
}

function SoliderAI::checkLOSClear(%aiName) {
  if (AI::getTarget(%aiName) != -1) {
    %id = SuperAI::getOwnedObject(%aiName);
    if (GameBase::getLOSInfo(%id, SuperAI::getVar(%aiName, "sightDist"))) {
      echo(AI::getVar(%aiName, "triggerPct"));
      if ($los::object.aiName != "" && GameBase::getTeam(SuperAI::getOwnedObject($los::object.aiName)) == GameBase::getTeam(%id) &&
          AI::getVar(%aiName, "triggerPct") != 0) { SoliderAI::deactivateGun(%aiName); %id.dudeInWay = true; return false; }
      else if (%id.dudeInWay) { SoliderAI::activateGun(%aiName); %id.dudeInWay = false; return true; }
    } else if (%id.dudeInWay) { SoliderAI::activateGun(%aiName); %id.dudeInWay = false; return true; }
  }
  return true;
}