$Cinematic::CAMERA_REFRESH_RATE = 0.0625;

$Cinematic::NO_CAMERA_MODE = 0;
$Cinematic::FIXED_CAMERA = -1;
$Cinematic::FIXED_CAMERA_LOOK_AT = -2;
$Cinematic::FIXED_CAMERA_FOLLOW = -3;
$Cinematic::MOVING_CAMERA_PATH = -4;
$Cinematic::MOVING_CAMERA_ORBIT = -5;

$Cinematic::START_FUNCTION[$Cinematic::FIXED_CAMERA] = "Cinematic::startFixedCamera";
$Cinematic::STOP_FUNCTION[$Cinematic::FIXED_CAMERA] = "Cinematic::stopFixedCamera";

$Cinematic::START_FUNCTION[$Cinematic::FIXED_CAMERA_LOOK_AT] = "Cinematic::startFixedCameraLookAt";
$Cinematic::STOP_FUNCTION[$Cinematic::FIXED_CAMERA_LOOK_AT] = "Cinematic::stopFixedCameraLookAt";

$Cinematic::START_FUNCTION[$Cinematic::FIXED_CAMERA_FOLLOW] = "Cinematic::startFixedCameraFollow";
$Cinematic::STOP_FUNCTION[$Cinematic::FIXED_CAMERA_FOLLOW] = "Cinematic::stopFixedCameraFollow";

$Cinematic::START_FUNCTION[$Cinematic::MOVING_CAMERA_PATH] = "Cinematic::startMovingCameraPath";
$Cinematic::STOP_FUNCTION[$Cinematic::MOVING_CAMERA_PATH] = "Cinematic::stopMovingCameraPath";

$Cinematic::START_FUNCTION[$Cinematic::MOVING_CAMERA_ORBIT] = "Cinematic::startMovingCameraOrbit";
$Cinematic::STOP_FUNCTION[$Cinematic::MOVING_CAMERA_ORBIT] = "Cinematic::stopMovingCameraOrbit";

//$Cinematic::viewers[movieid, index] = client id
//$Cinematic::viewerObject[movieid, index] = control object of viewer before controlling camera
//$Cinematic::numViewers[movieid] = num viewers

//$Cinematic::movieMode[movieid] = movie type
//$Cinematic::movieArgs[movieid, index] = args to movie mode
//$Cinematic::movieModeCounter[movieid] = each time the mode is set it gets a number

//$Cinematic::movieStartTime[movieid] = getSimTime() at movie start
//$Cinematic::movieRunning[movieid] = is movie running

//$Cinematic::objectSet[movieid] = SimSet of movie objects

$Cinematic::currentMovieID = 0;
function Cinematic::generateMovieID() {
  %id = $Cinematic::currentMovieID;
  $Cinematic::currentMovieID++;
  return %id;
}

function Cinematic::setMovieMode(%movieid, %mode, %args0, %args1, %args2, %args3, %args4, %args5, %args6, %args7) {
  %oldMode = $Cinematic::movieMode[%movieid];
  $Cinematic::movieMode[%movieid] = %mode;
  $Cinematic::movieModeCounter[%movieid]++;
  for (%i = 0; %i < 8 && %args[%i] != ""; %i++) $Cinematic::movieArgs[%movieid, %i] = %args[%i];
  for (%i = %i; %i < 8; %i++) $Cinematic::movieArgs[%movieid, %i] = "";

  if ($Cinematic::movieRunning[%movieid]) {
    if (%oldMode != $Cinematic::NO_CAMERA_MODE)
      eval($Cinematic::STOP_FUNCTION[%oldMode] @ "("@%movieid@");");
    if ($Cinematic::movieMode[%movieid] != $Cinematic::NO_CAMERA_MODE)
      eval($Cinematic::START_FUNCTION[$Cinematic::movieMode[%movieid]] @ "("@%movieid@");");
  }
}

function Cinematic::addViewer(%movieid, %client) {
  if ($Cinematic::numViewers[%movieid] == "") $Cinematic::numViewers[%movieid] = 0;
  $Cinematic::viewers[%movieid, $Cinematic::numViewers[%movieid]] = %client;

  %v = $Cinematic::numViewers[%movieid];
  if ($Cinematic::movieRunning[%movieid]) {
    $Cinematic::viewerObject[%movieid, %v] = Client::getControlObject($Cinematic::viewers[%movieid, %v]);
    Client::setControlObject($Cinematic::viewers[%movieid, %v], Client::getObserverCamera($Cinematic::viewers[%movieid, %v]));
  }

  $Cinematic::numViewers[%movieid]++;
}

function Cinematic::clearViewers() {
  $Cinematic::numViewers[%movieid] = 0;
}

function Cinematic::newMovie() {
  %movieid = Cinematic::generateMovieID();
  return %movieid;
}

function Cinematic::start(%movieid) {
  echo("MOVIE STARTED, ID = " @ %movieid);

  $Cinematic::movieStartTime[%movieid] = getSimTime();
  $Cinematic::movieRunning[%movieid] = true;
  $Cinematic::movieModeCounter[%movieid] = 0;
  $Cinematic::numAIs[%movieid] = 0;

  $Cinematic::objectSet[%movieid] = newObject("CinematicSet"@%movieid, SimSet);
  $Cinematic::objectSet[%movieid].isObjectSet = true;
  addToSet(MissionCleanup, $Cinematic::objectSet[%movieid]);

  if ($Cinematic::movieMode[%movieid] == "")
    $Cinematic::movieMode[%movieid] = $Cinematic::NO_CAMERA_MODE;

  for (%i = 0; %i < $Cinematic::numViewers[%movieid]; %i++) {
    $Cinematic::viewerObject[%movieid, %i] = Client::getControlObject($Cinematic::viewers[%movieid, %i]);
    Client::setControlObject($Cinematic::viewers[%movieid, %i], Client::getObserverCamera($Cinematic::viewers[%movieid, %i]));
  }

  if ($Cinematic::movieMode[%movieid] != $Cinematic::NO_CAMERA_MODE)
    eval($Cinematic::START_FUNCTION[$Cinematic::movieMode[%movieid]] @ "("@%movieid@");");

  return %movieid;
}

function Cinematic::stop(%movieid) {
  echo("MOVIE STOPPED, ID = " @ %movieid @ ", ELAPSED TIME = " @ Cinematic::getMovieTime(%movieid));

  $Cinematic::movieRunning[%movieid] = false;

  eval($Cinematic::STOP_FUNCTION[$Cinematic::movieMode[%movieid]] @ "("@%movieid@");");

  for (%i = 0; %i < $Cinematic::numAIs[%movieid]; %i++) {
    schedule("AI::Delete(\""@$Cinematic::ai[%movieid, %i]@"\");",%i*0.25);
  }

  for (%i = 0; %i < $Cinematic::numViewers[%movieid]; %i++) {
    %v = $Cinematic::viewers[%movieid, %i];
    %vc = Client::getObserverCamera(%v);
    Observer::setFlyMode(%v, GameBase::getPosition(%vc), GameBase::getRotation(%vc), true, true);
    Client::setControlObject(%v, $Cinematic::viewerObject[%movieid, %i]);
  }



  for (%i = 0; %i < $Cinematic::numViewers[%movieid]; %i++) {
    $Cinematic::viewers[%movieid, %i] = "";
    $Cinematic::viewerObject[%movieid, %i] = "";
  }
  $Cinematic::numViewers[%movieid] = "";

  $Cinematic::movieMode[%movieid] = "";
  for (%i = 0; %i < 6; %i++) $Cinematic::movieArgs[%movieid, %i] = "";
  $Cinematic::movieModeCounter[%movieid] = "";

  $Cinematic::movieStartTime[%movieid] = "";
  $Cinematic::movieRunning[%movieid] = "";

  if ($Cinematic::objectSet[%movieid].isObjectSet) deleteObject($Cinematic::objectSet[%movieid]);
  $Cinematic::objectSet[%movieid] = "";

  $Cinematic::timelineMarker = "";
  $Cinematic::timelineName = "";

  for (%i = 0; %i < $Cinematic::numAIs[%movieid]; %i++) {
    %name = $Cinematic::ai[%movieid, %i];
    $Cinematic::AIOrder[%movieid, %name] = "";
    $Cinematic::ais[%movieid, %i] = "";
  }

  $Cinematic::numAIs[%movieid] = "";
}

function Cinematic::addAllViewers(%movieid) {
  for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c)) {
    Cinematic::addViewer(%movieid, %c);    
  }
}

function Cinematic::addAllViewersFromTeam(%movieid, %team) {
  for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c)) {
    if (Client::getTeam(%c) == %team) Cinematic::addViewer(%movieid, %c);    
  }
}

function Cinematic::getMovieTime(%movieid) {
  return getSimTime() - $Cinematic::movieStartTime[%movieid];
}

// --- Camera mode functions ---

// Arguments:
// 0: Camera position
// 1: Camera rotation
function Cinematic::startFixedCamera(%movieid) {
  for (%i = 0; %i < $Cinematic::numViewers[%movieid]; %i++) {
    Observer::setFlyMode($Cinematic::viewers[%movieid, %i], $Cinematic::movieArgs[%movieid, 0], $Cinematic::movieArgs[%movieid, 1], false, false);
  }
}

function Cinematic::stopFixedCamera(%movieid) {
  return;
}


// Arguments:
// 0: Camera position
// 1: Look-at position
function Cinematic::startFixedCameraLookAt(%movieid) {
  %camPos = $Cinematic::movieArgs[%movieid, 0];
  %targPos = $Cinematic::movieArgs[%movieid, 1];
  %rot = Vector::add(Vector::getRotation(Vector::sub(%targPos, %camPos)), "1.57 0 0");
  for (%i = 0; %i < $Cinematic::numViewers[%movieid]; %i++) {
    Observer::setFlyMode($Cinematic::viewers[%movieid, %i], %camPos, %rot, false, false);
  }
}

function Cinematic::stopFixedCameraLookAt(%movieid) {
  return;
}



// Arguments:
// 0: Camera position
// 1: Target object id
function Cinematic::startFixedCameraFollow(%movieid) {
  for (%i = 0; %i < $Cinematic::numViewers[%movieid]; %i++) {
    Observer::setFlyMode($Cinematic::viewers[%movieid, %i], $Cinematic::movieArgs[%movieid, 0], "0 0 0", false, false);
  }
  Cinematic::updateFixedCameraFollow(%movieid, $Cinematic::movieModeCounter[%movieid]);
}

function Cinematic::updateFixedCameraFollow(%movieid, %counter) {
  if (!$Cinematic::movieRunning[%movieid] || $Cinematic::movieModeCounter[%movieid] != %counter) return;

  %camPos = $Cinematic::movieArgs[%movieid, 0];
  %targPos = GameBase::getPosition($Cinematic::movieArgs[%movieid, 1]);
  %rot = Vector::add(Vector::getRotation(Vector::sub(%targPos, %camPos)), "1.57 0 0");

  for (%i = 0; %i < $Cinematic::numViewers[%movieid]; %i++) {
    Observer::setFlyMode($Cinematic::viewers[%movieid, %i], %camPos, %rot, false, false);
    if (Client::getControlObject($Cinematic::viewers[%movieid, %i]) != Client::getObserverCamera($Cinematic::viewers[%movieid, %i]))
      Client::setControlObject($Cinematic::viewers[%movieid, %i], Client::getObserverCamera($Cinematic::viewers[%movieid, %i]));
  }

  schedule("Cinematic::updateFixedCameraFollow("@%movieid@","@%counter@");", $Cinematic::CAMERA_REFRESH_RATE);
}

function Cinematic::stopFixedCameraFollow(%movieid) {
  return;
}



// Arguments:
// 0: Camera start position
// 1: Camera start rotation
// 2: Camera end position
// 3: Camera end rotation
// 4: Move time
function Cinematic::startMovingCameraPath(%movieid) {
  for (%i = 0; %i < $Cinematic::numViewers[%movieid]; %i++) {
    Observer::setFlyMode($Cinematic::viewers[%movieid, %i], $Cinematic::movieArgs[%movieid, 0], $Cinematic::movieArgs[%movieid, 1], false, false);
  }
  $Cinematic::movieArgs[%movieid, 5] = Cinematic::getMovieTime(%movieid);
  Cinematic::updateMovingCameraPath(%movieid, $Cinematic::movieModeCounter[%movieid]);
}

function Cinematic::updateMovingCameraPath(%movieid, %counter) {
  if (!$Cinematic::movieRunning[%movieid] || $Cinematic::movieModeCounter[%movieid] != %counter) return;

  %percent = min((Cinematic::getMovieTime(%movieid) - $Cinematic::movieArgs[%movieid, 5]) / $Cinematic::movieArgs[%movieid, 4], 1);
  %camPos = Vector::add(Vector::mul($Cinematic::movieArgs[%movieid, 0], 1 - %percent), Vector::mul($Cinematic::movieArgs[%movieid, 2], %percent));
  %camRot = Vector::add(Vector::mul($Cinematic::movieArgs[%movieid, 1], 1 - %percent), Vector::mul($Cinematic::movieArgs[%movieid, 3], %percent));

  for (%i = 0; %i < $Cinematic::numViewers[%movieid]; %i++) {
    Observer::setFlyMode($Cinematic::viewers[%movieid, %i], %camPos, %camRot, false, false);
    if (Client::getControlObject($Cinematic::viewers[%movieid, %i]) != Client::getObserverCamera($Cinematic::viewers[%movieid, %i]))
      Client::setControlObject($Cinematic::viewers[%movieid, %i], Client::getObserverCamera($Cinematic::viewers[%movieid, %i]));
  }

  if (%percent == 1) return;
  schedule("Cinematic::updateMovingCameraPath("@%movieid@","@%counter@");", $Cinematic::CAMERA_REFRESH_RATE);
}

function Cinematic::stopMovingCameraPath(%movieid) {
  return;
}


// Arguments:
// 0: Orbit position
// 1: Start radius rotation
// 2: End radius rotation
// 3: Radius
// 4: Move time
function Cinematic::startMovingCameraOrbit(%movieid) {
  $Cinematic::movieArgs[%movieid, 5] = Cinematic::getMovieTime(%movieid);
  Cinematic::updateMovingCameraOrbit(%movieid, $Cinematic::movieModeCounter[%movieid]);
}

function Cinematic::updateMovingCameraOrbit(%movieid, %counter) {
  if (!$Cinematic::movieRunning[%movieid] || $Cinematic::movieModeCounter[%movieid] != %counter) return;

  %percent = min((Cinematic::getMovieTime(%movieid) - $Cinematic::movieArgs[%movieid, 5]) / $Cinematic::movieArgs[%movieid, 4], 1);
  %radiusRot = Vector::add(Vector::mul($Cinematic::movieArgs[%movieid, 1], 1 - %percent), Vector::mul($Cinematic::movieArgs[%movieid, 2], %percent));
  %camPos = Vector::add(Vector::getFromRot(%radiusRot, $Cinematic::movieArgs[%movieid, 3]), $Cinematic::movieArgs[%movieid, 0]);
  %camRot = -getWord(%radiusRot, 0) @ " " @ -getWord(%radiusRot, 1) @ " " @ (getWord(%radiusRot, 2) + $PI);

  for (%i = 0; %i < $Cinematic::numViewers[%movieid]; %i++) {
    Observer::setFlyMode($Cinematic::viewers[%movieid, %i], %camPos, %camRot, false, false);
    if (Client::getControlObject($Cinematic::viewers[%movieid, %i]) != Client::getObserverCamera($Cinematic::viewers[%movieid, %i]))
      Client::setControlObject($Cinematic::viewers[%movieid, %i], Client::getObserverCamera($Cinematic::viewers[%movieid, %i]));
  }

  if (%percent == 1) return;
  schedule("Cinematic::updateMovingCameraOrbit("@%movieid@","@%counter@");", $Cinematic::CAMERA_REFRESH_RATE);
}

function Cinematic::stopMovingCameraOrbit(%movieid) {
  return;
}

// --- Timeline parser thingy ---

function Cinematic::getNamedTimelineElement(%timelineName, %index, %var, %index2) {
  if (%timelineName == "") %timelineName = "timeline";
  if (%index2 != "") return eval("%x = $" @ %timelineName @ "[" @ %index @ ",\"" @ %var @ "\", " @ %index2 @ "];");
  else return eval("%x = $" @ %timelineName @ "[" @ %index @ ",\"" @ %var @ "\"];");
}

function Cinematic::getTimelineElement(%movieid, %index, %var, %index2) {
  return Cinematic::getNamedTimelineElement($Cinematic::timelineName[%movieid], %index, %var, %index2);
}

function Cinematic::parseTimeline(%movieid, %timelineName) {
  $Cinematic::timelineMarker[%movieid] = 0;
  $Cinematic::timelineName[%movieid] = %timelineName;
  %time = getWord(Cinematic::getTimelineElement(%movieid, 0, "action"), 0) - Cinematic::getMovieTime(%movieid);
  schedule("Cinematic::timelineNext("@%movieid@");", tern(%time >= 0, %time, 0));
}

//$Cinematic::AIOrder[%movieid, %aiName] = current order level
function Cinematic::timelineNext(%movieid) {
  %i = $Cinematic::timelineMarker[%movieid];
  %action = Cinematic::getTimelineElement(%movieid, %i, "action");
  if (%action == "") return;
  %actionTime = getWord(%action, 0);
  %actionCommand = getWord(%action, 1);

  for (%j = 0; %j < 7; %j++) { %e = Cinematic::getTimelineElement(%movieid, %i, "args", %j); %args[%j] = %e; echo("ARGS["@%j@"]="@%e); }

  // --- CAMERA MODE COMMANDS ---
  if (%actionCommand == "setCamModeLookAtAI") {
    Cinematic::setMovieMode(%movieid, $Cinematic::FIXED_CAMERA_FOLLOW, %args[0],
                            Client::getOwnedObject(AI::getID(%args[1])));
  }
  if (%actionCommand == "setCamModeLookAtPoint") {
    Cinematic::setMovieMode(%movieid, $Cinematic::FIXED_CAMERA_LOOK_AT, %args[0], %args[1]);
  }
  if (%actionCommand == "setCamModeMoveAlongPath") {
    Cinematic::setMovieMode(%movieid, $Cinematic::MOVING_CAMERA_PATH, %args[0], %args[1], %args[2], %args[3], %args[4]);
  }
  if (%actionCommand == "setCamModeOrbitPoint") {
    Cinematic::setMovieMode(%movieid, $Cinematic::MOVING_CAMERA_ORBIT, %args[0], %args[1], %args[2], %args[3], %args[4]);
  }

  // --- AI CONTROL COMMANDS ---
  if (%actionCommand == "spawnAI") {
    AI::Spawn(%args[0], %args[2], %args[3], %args[4], %args[1]);
    AI::SetVar(%args[0], "pathType", 1);

    $Cinematic::AIOrder[%movieid, %args[0]] = 10;
    $Cinematic::ai[%movieid, $Cinematic::numAIs[%movieid]] = %args[0];
    $Cinematic::numAIs[%movieid]++;
  }
  if (%actionCommand == "moveAI") {
    AI::setVar(%args[0], "iq", %args[2]);
    AI::directiveWaypoint(%args[0], %args[1], $Cinematic::AIOrder[%movieid, %args[0]]++);
  }
  if (%actionCommand == "animateAI") {
    Player::setAnimation(Client::getOwnedObject(AI::getID(%args[0])),%args[1]);
  }
  if (%actionCommand == "repeatedAnimateAI") {
    for (%j = 0; %j < %args[2]; %j++) {
      schedule("Player::setAnimation("@Client::getOwnedObject(AI::getID(%args[0]))@","@%args[1]@");", %j * %args[3]);
    }
  }
  if (%actionCommand == "giveAIWeapon") {
    %ai = %args[0];
    %obj = Client::getOwnedObject(AI::getID(%ai));
    %item = %args[1];

    %obj.weaponMode[%item] = %args[3];
    Player::setItemCount(%obj, %item, 1);
    Player::setItemCount(%obj, $WeaponAmmo[%item], %args[2]);
    Player::useItem(%obj, %item);
  }
  if (%actionCommand == "targetPointAI") {
    %ai = %args[0];
    AI::setVar(%ai, "iq", 10000);
    AI::setVar(%ai, "smartGuyMinAccuracy", 10000);
    AI::setVar(%ai, "attackMode", %args[2]);
    AI::setVar(%ai, "triggerPct", %args[3]);
    AI::DirectiveTargetPoint(%ai, %args[1], $Cinematic::AIOrder[%movieid, %args[0]]);
  }
  if (%actionCommand == "cancelAICommand") {
    for (%j = 0; %j < %args[1]; %j++)
      AI::DirectiveRemove(%args[0], $Cinematic::AIOrder[%movieid, %args[0]]-%j);
    AI::DirectiveList(%args[0]);
  }

  // OBJECT CONTROL COMMANDS
  if (%actionCommand == "newObjectSet") {
    %name = %args[0];
    %path1 = %args[1];
    %path2 = "MissionCleanup\\CinematicSet" @ %movieid @ tern(%path1 != "", "\\"@%path1, "");
    %parentID = nameToID(%path2);

    if (%parentID == -1) { echo("ERROR: CANNOT FIND OBJECT PATH " @ %path1); }
    else {
      %x = newObject(%name, SimSet);
      addToSet(%parentID, %x);
    }
  }
  if (%actionCommand == "newObject") {
    %name = %args[0];
    %path1 = %args[1];
    %path2 = "MissionCleanup\\CinematicSet" @ %movieid @ tern(%path1 != "", "\\"@%path1, "");
    %parentID = nameToID(%path2);

    if (%parentID == -1) { echo("ERROR: CANNOT FIND OBJECT PATH " @ %path1); }
    else {
      %x = newObject(%name, %args[2], %args[3], %args[4]);
      if (%args[5] != "") GameBase::setPosition(%x, %args[5]);
      if (%args[6] != "") GameBase::setRotation(%x, %args[6]);
      addToSet(%parentID, %x);
    }
  }
  if (%actionCommand == "deleteObject") {
    %name = %args[0];
    %path1 = %args[1];
    %path2 = "MissionCleanup\\CinematicSet" @ %movieid @ tern(%path1 != "", "\\"@%path1, "");
    %id = nameToID(%path2 @ "\\" @ %name);
    if (%id == -1) { echo("ERROR: CANNOT FIND OBJECT " @ %path2 @ "\\" @ %name); }
    else { deleteObject(%id); }
  }
  if (%actionCommand == "loadObject") {
    %name = %args[0];
    %path1 = %args[1];
    %path2 = "MissionCleanup\\CinematicSet" @ %movieid @ tern(%path1 != "", "\\"@%path1, "");
    %parentID = nameToID(%path2 @ "\\" @ %name);
    if (%parentID == -1) { echo("ERROR: CANNOT FIND OBJECT PATH " @ %path2); }
    else { %x = loadObject(%name, %args[2]); addToSet(%parentID, %x); }
  }

  // MISC COMMANDS
  if (%actionCommand == "messageAll") {
    for (%j = 0; %j < $Cinematic::numViewers[%movieid]; %j++)
      Client::sendMessage($Cinematic::viewers[%movieid, %j], %args[0], %args[1]);
  }
  if (%actionCommand == "cheers") {
    for (%j = 0; %j < %args[0]; %j++) {
      schedule("messageAll(0,\"~wmale1.wcheer"@radnomItems(3,1,2,3)@".wav\");", %args[1]*%j);
    }
  }
  if (%actionCommand == "eval") {
    eval(%args[0]);
  }
  if (%actionCommand == "exec") {
    exec(%args[0]);
  }
  if (%actionCommand == "resetTime") {
    $Cinematic::movieStartTime[%movieid] = getSimTime();
  }
  if (%actionCommand == "endMovie") {
    Cinematic::stop(%movieid);
    Cinematic::clearViewers(%movieid);
    return;
  }

  $Cinematic::timelineMarker[%movieid]++;
  %elem = Cinematic::getTimelineElement(%movieid, $Cinematic::timelineMarker[%movieid], "action");
  if (%elem == "") return;
  echo("ELEM: " @ %elem);

  %time = getWord(%elem, 0) - Cinematic::getMovieTime(%movieid);
  schedule("Cinematic::timelineNext("@%movieid@");", tern(%time >= 0, %time, 0));
}

//
//
// --- CINEMATIC COMMANDS LIST ---
//
//

//
// *** setCamModeLookAtAI ***
//   Sets the camera mode to stay at a fixed point but angle to keep a certain AI in the center of the screen
// args:
//   0: camera position
//   1: AI's name
//

//
// *** setCamModeLookAtPoint ***
//   Sets the camera mode to stay at a fixed point and stare at a fixed point
// args:
//   0: camera position
//   1: look at position
//

//
// *** setCamModeMoveAlongPath ***
//   Sets the camera mode to move along a path, from a start position and rotation to a final position and rotation
// args:
//   0: start position
//   1: start rotation
//   2: end position
//   3: end rotation
//   4: total move time
//

//
// *** setCamModeOrbitPoint ***
//   Sets the camera mode to look at and orbit a fixed point
// args:
//   0: look at and orbit center position
//   1: start rotation (of the radius)
//   2: end rotation
//   3: radius of orbit
//   4: total orbit time
//

//
// *** spawnAI ***
//   Spawns an AI actor
// args:
//   0: AI's internal name
//   1: AI's display name
//   2: AI spawn position
//   3: AI spawn rotation
//   4: AI voice pack (male1, female2, etc.)
//

//
// *** moveAI ***
//   Commands an AI actor to move to a specified position
// args:
//   0: AI's internal name
//   1: target position
//   2: AI iq for move
//

//
// *** moveAI ***
//   Commands an AI actor to move to a specified position
// args:
//   0: AI's internal name
//   1: target position
//   2: AI iq for move
//

//
// *** animateAI ***
//   Makes an AI execute a certain animation
// args:
//   0: AI's internal name
//   1: animation number
//

//
// *** repeatedAnimateAI ***
//   Repeatedly animate an AI actor
// args:
//   0: AI's internal name
//   1: animation number
//   2: number of animations
//   3: time between animations
//

//
// *** giveAIWeapon ***
//   Give an AI actor a weapon
// args:
//   0: AI's internal name
//   1: weapon
//   2: amount of ammo
//   3: weapon mode
//

//
// *** targetPointAI ***
//   Commands an AI actor to target a specific point
// args:
//   0: AI's internal name
//   1: target point
//   2: attack mode (0, move toward the point; 1, stand still)
//   3: triggerPct (0.0 to 1.0)
//

//
// *** cancelAICommand ***
//   Cancels the last command issued to an AI actor
// args:
//   0: AI's internal name
//

//
// *** newObjectSet ***
//   Adds a new object set
// args:
//   0: object set name
//   1: parent path ("" for root path)
//

//
// *** newObject ***
//   Adds a new object
// args:
//   0: object name
//   1: parent path ("" for root path)
//   2: constructor arg 1
//   3: constructor arg 2
//   4: constructor arg 3
//   5: object position (optional)
//   6: object rotation (optional)
//

//
// *** deleteObject ***
//   Deletes an existing object or object set
// args:
//   0: object\object set name
//   1: parent path ("" for root path)
//

//
// *** loadObject ***
//   Loads an stored object from file
// args:
//   0: object name
//   1: parent path ("" for root path)
//   2: filename of stored object
//

//
// *** messageAll ***
//   Messages all players (not just viewers; this is a bug currently)
// args:
//   0: message type
//   1: message
//

//
// *** cheers ***
//   Plays cheering sounds to all players (not just viewers; same as above)
// args:
//   0: number of cheers
//   1: time between cheers
//

//
// *** eval ***
//   eval()s a command
// args:
//   0: command
//

//
// *** exec ***
//   exec()s a file
// args:
//   0: file
//

//
// *** resetTime ***
//   resets the movie time to 0. Useful to simplify a movie file, but do not reset when a command is currently being executed
// args:
//   none
//

//
// *** endMovie ***
//   ends the movie, cleans up, and resets all viewers
// args:
//   none
//





// FOR TESTING PURPOSES
function doMovie() {
  focusServer();
  exec("misc\\cinematic2.cs");
  exec(testmovie2);

//  %id = Cinematic::start(%movieid);
//  Cinematic::addAllViewers(%id);
//  Cinematic::parseTimeline(%id, "");
}