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

$Cinematic::numViewers = 0;
$Cinematic::movieRunning = false;
$Cinematic::movieStartTime = -1;

//$Cinematic::viewers[index] = client id
//$Cinematic::viewerObject[index] = control object of viewer before controlling camera
//$Cinematic::numViewers = num viewers

//$Cinematic::movieMode = movie type
//$Cinematic::movieArgs[index] = args to movie mode
//$Cinematic::movieModeCounter = each time the mode is set it gets a number

//$Cinematic::movieStartTime = getSimTime() at movie start
//$Cinematic::movieRunning = is movie running

//$Cinematic::objectSet = SimSet of movie objects

function Cinematic::setMovieMode(%mode, %args0, %args1, %args2, %args3, %args4, %args5, %args6, %args7) {
  %oldMode = $Cinematic::movieMode;
  $Cinematic::movieMode = %mode;
  $Cinematic::movieModeCounter++;
  for (%i = 0; %i < 8 && %args[%i] != ""; %i++) $Cinematic::movieArgs[%i] = %args[%i];
  for (%j = 0; %i < 8; %i++) $Cinematic::movieArgs[%i] = "";

  if ($Cinematic::movieRunning) {
    if (%oldMode != $Cinematic::NO_CAMERA_MODE)
      eval($Cinematic::STOP_FUNCTION[%oldMode] @ "();");
    if ($Cinematic::movieMode != $Cinematic::NO_CAMERA_MODE)
      eval($Cinematic::START_FUNCTION[$Cinematic::movieMode] @ "();");
  }
}

function Cinematic::addViewer(%client) {
  $Cinematic::viewers[$Cinematic::numViewers] = %client;
  $Cinematic::numViewers++;
}

function Cinematic::clearViewers() {
  $Cinematic::numViewers = 0;
}

function Cinematic::start() {
  $Cinematic::movieStartTime = getSimTime();
  $Cinematic::movieRunning = true;
  $Cinematic::movieModeCounter = 0;
  $Cinematic::numAIs = 0;

  $Cinematic::objectSet = newObject("CinematicSet", SimSet);
  $Cinematic::objectSet.isObjectSet = true;
  addToSet(MissionCleanup, $Cinematic::objectSet);

  if ($Cinematic::movieMode == "")
    $Cinematic::movieMode = $Cinematic::NO_CAMERA_MODE;

  for (%i = 0; %i < $Cinematic::numViewers; %i++) {
    $Cinematic::viewerObject[%i] = Client::getControlObject($Cinematic::viewers[%i]);
    Client::setControlObject($Cinematic::viewers[%i], Client::getObserverCamera($Cinematic::viewers[%i]));
  }

  if ($Cinematic::movieMode != $Cinematic::NO_CAMERA_MODE)
    eval($Cinematic::START_FUNCTION[$Cinematic::movieMode] @ "();");

}

function Cinematic::stop() {
  $Cinematic::movieRunning = false;

  eval($Cinematic::STOP_FUNCTION[$Cinematic::movieMode] @ "();");

  for (%i = 0; %i < $Cinematic::numAIs; %i++) {
    schedule("AI::Delete(\""@$Cinematic::ai[%i]@"\");",%i*0.25);
  }

  for (%i = 0; %i < $Cinematic::numViewers; %i++) {
    %v = $Cinematic::viewers[%i];
    %vc = Client::getObserverCamera(%v);
    Observer::setFlyMode(%v, GameBase::getPosition(%vc), GameBase::getRotation(%vc), true, true);
    Client::setControlObject(%v, $Cinematic::viewerObject[%i]);
  }

  $Cinematic::movieMode = $Cinematic::NO_CAMERA_MODE;

  if ($Cinematic::objectSet.isObjectSet) deleteObject($Cinematic::objectSet);
}

function Cinematic::addAllViewers() {
  for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c)) {
    Cinematic::addViewer(%c);    
  }
}

function Cinematic::getMovieTime() {
  return getSimTime() - $Cinematic::movieStartTime;
}

// --- Camera mode functions ---

// Arguments:
// 0: Camera position
// 1: Camera rotation
function Cinematic::startFixedCamera() {
  for (%i = 0; %i < $Cinematic::numViewers; %i++) {
    Observer::setFlyMode($Cinematic::viewers[%i], $Cinematic::movieArgs[0], $Cinematic::movieArgs[1], false, false);
  }
}

function Cinematic::stopFixedCamera() {
  return;
}


// Arguments:
// 0: Camera position
// 1: Look-at position
function Cinematic::startFixedCameraLookAt() {
  %camPos = $Cinematic::movieArgs[0];
  %targPos = $Cinematic::movieArgs[1];
  %rot = Vector::add(Vector::getRotation(Vector::sub(%targPos, %camPos)), "1.57 0 0");
  for (%i = 0; %i < $Cinematic::numViewers; %i++) {
    Observer::setFlyMode($Cinematic::viewers[%i], %camPos, %rot, false, false);
  }
}

function Cinematic::stopFixedCameraLookAt() {
  return;
}



// Arguments:
// 0: Camera position
// 1: Target object id
function Cinematic::startFixedCameraFollow() {
  for (%i = 0; %i < $Cinematic::numViewers; %i++) {
    Observer::setFlyMode($Cinematic::viewers[%i], $Cinematic::movieArgs[0], "0 0 0", false, false);
  }
  Cinematic::updateFixedCameraFollow($Cinematic::movieModeCounter);
}

function Cinematic::updateFixedCameraFollow(%counter) {
  if (!$Cinematic::movieRunning || $Cinematic::movieModeCounter != %counter) return;

  %camPos = $Cinematic::movieArgs[0];
  %targPos = GameBase::getPosition($Cinematic::movieArgs[1]);
  %rot = Vector::add(Vector::getRotation(Vector::sub(%targPos, %camPos)), "1.57 0 0");

  for (%i = 0; %i < $Cinematic::numViewers; %i++) {
    Observer::setFlyMode($Cinematic::viewers[%i], %camPos, %rot, false, false);
    if (Client::getControlObject($Cinematic::viewers[%i]) != Client::getObserverCamera($Cinematic::viewers[%i]))
      Client::setControlObject($Cinematic::viewers[%i], Client::getObserverCamera($Cinematic::viewers[%i]));
  }

  schedule("Cinematic::updateFixedCameraFollow("@%counter@");", $Cinematic::CAMERA_REFRESH_RATE);
}

function Cinematic::stopFixedCameraFollow() {
  return;
}



// Arguments:
// 0: Camera start position
// 1: Camera start rotation
// 2: Camera end position
// 3: Camera end rotation
// 4: Move time
function Cinematic::startMovingCameraPath() {
  for (%i = 0; %i < $Cinematic::numViewers; %i++) {
    Observer::setFlyMode($Cinematic::viewers[%i], $Cinematic::movieArgs[0], $Cinematic::movieArgs[1], false, false);
  }
  $Cinematic::movieArgs[5] = Cinematic::getMovieTime();
  Cinematic::updateMovingCameraPath($Cinematic::movieModeCounter);
}

function Cinematic::updateMovingCameraPath(%counter) {
  if (!$Cinematic::movieRunning || $Cinematic::movieModeCounter != %counter) return;

  %percent = min((Cinematic::getMovieTime() - $Cinematic::movieArgs[5]) / $Cinematic::movieArgs[4], 1);
  %camPos = Vector::add(Vector::mul($Cinematic::movieArgs[0], 1 - %percent), Vector::mul($Cinematic::movieArgs[2], %percent));
  %camRot = Vector::add(Vector::mul($Cinematic::movieArgs[1], 1 - %percent), Vector::mul($Cinematic::movieArgs[3], %percent));

  for (%i = 0; %i < $Cinematic::numViewers; %i++) {
    Observer::setFlyMode($Cinematic::viewers[%i], %camPos, %camRot, false, false);
    if (Client::getControlObject($Cinematic::viewers[%i]) != Client::getObserverCamera($Cinematic::viewers[%i]))
      Client::setControlObject($Cinematic::viewers[%i], Client::getObserverCamera($Cinematic::viewers[%i]));
  }

  if (%percent == 1) return;
  schedule("Cinematic::updateMovingCameraPath("@%counter@");", $Cinematic::CAMERA_REFRESH_RATE);
}

function Cinematic::stopMovingCameraPath() {
  return;
}


// Arguments:
// 0: Orbit position
// 1: Start radius rotation
// 2: End radius rotation
// 3: Radius
// 4: Move time
function Cinematic::startMovingCameraOrbit() {
//  for (%i = 0; %i < $Cinematic::numViewers; %i++) {
//    Observer::setFlyMode($Cinematic::viewers[%i], $Cinematic::movieArgs[0], $Cinematic::movieArgs[1], false, false);
//  }
  $Cinematic::movieArgs[5] = Cinematic::getMovieTime();
  Cinematic::updateMovingCameraOrbit($Cinematic::movieModeCounter);
}

function Cinematic::updateMovingCameraOrbit(%counter) {
  if (!$Cinematic::movieRunning || $Cinematic::movieModeCounter != %counter) return;

  %percent = min((Cinematic::getMovieTime() - $Cinematic::movieArgs[5]) / $Cinematic::movieArgs[4], 1);
  %radiusRot = Vector::add(Vector::mul($Cinematic::movieArgs[1], 1 - %percent), Vector::mul($Cinematic::movieArgs[2], %percent));
  %camPos = Vector::add(Vector::getFromRot(%radiusRot, $Cinematic::movieArgs[3]), $Cinematic::movieArgs[0]);
  %camRot = -getWord(%radiusRot, 0) @ " " @ -getWord(%radiusRot, 1) @ " " @ (getWord(%radiusRot, 2) + $PI);

  for (%i = 0; %i < $Cinematic::numViewers; %i++) {
    Observer::setFlyMode($Cinematic::viewers[%i], %camPos, %camRot, false, false);
    if (Client::getControlObject($Cinematic::viewers[%i]) != Client::getObserverCamera($Cinematic::viewers[%i]))
      Client::setControlObject($Cinematic::viewers[%i], Client::getObserverCamera($Cinematic::viewers[%i]));
  }

  if (%percent == 1) return;
  schedule("Cinematic::updateMovingCameraOrbit("@%counter@");", $Cinematic::CAMERA_REFRESH_RATE);
}

function Cinematic::stopMovingCameraOrbit() {
  return;
}

// --- Timeline parser thingy ---

function Cinematic::parseTimeline() {
  Cinematic::start();
  $Cinematic::timelineMarker = 0;
  %time = getWord($timeline[0, "action"], 0) - Cinematic::getMovieTime();
  schedule("Cinematic::timelineNext();", tern(%time >= 0, %time, 0));
}

//$Cinematic::AIOrder[%aiName] = current order level
function Cinematic::timelineNext() {
  %i = $Cinematic::timelineMarker;
  %action = $timeline[%i, "action"];
  if (%action == "") return;
  %actionTime = getWord(%action, 0);
  %actionCommand = getWord(%action, 1);

  // --- CAMERA MODE COMMANDS ---
  if (%actionCommand == "setCamModeLookAtAI") {
    Cinematic::setMovieMode($Cinematic::FIXED_CAMERA_FOLLOW, $timeline[%i, "args", 0],
                            Client::getOwnedObject(AI::getID($timeline[%i, "args", 1])));
  }
  if (%actionCommand == "setCamModeLookAtPoint") {
    Cinematic::setMovieMode($Cinematic::FIXED_CAMERA_LOOK_AT, $timeline[%i, "args", 0], $timeline[%i, "args", 1]);
  }
  if (%actionCommand == "setCamModeMoveAlongPath") {
    Cinematic::setMovieMode($Cinematic::MOVING_CAMERA_PATH, $timeline[%i, "args", 0], $timeline[%i, "args", 1], $timeline[%i, "args", 2], $timeline[%i, "args", 3], $timeline[%i, "args", 4]);
  }
  if (%actionCommand == "setCamModeOrbitPoint") {
    Cinematic::setMovieMode($Cinematic::MOVING_CAMERA_ORBIT, $timeline[%i, "args", 0], $timeline[%i, "args", 1], $timeline[%i, "args", 2], $timeline[%i, "args", 3], $timeline[%i, "args", 4]);
  }

  // --- AI CONTROL COMMANDS ---
  if (%actionCommand == "spawnAI") {
    AI::Spawn($timeline[%i, "args", 0], $timeline[%i, "args", 2], $timeline[%i, "args", 3], $timeline[%i, "args", 4], $timeline[%i, "args", 1]);
    AI::SetVar($timeline[%i, "args", 0], "pathType", 1);

    $Cinematic::AIOrder[$timeline[%i, "args", 0]] = 10;
    $Cinematic::ai[$Cinematic::numAIs] = $timeline[%i, "args", 0];
    $Cinematic::numAIs++;
  }
  if (%actionCommand == "moveAI") {
    AI::setVar($timeline[%i, "args", 0], "iq", $timeline[%i, "args", 2]);
    AI::directiveWaypoint($timeline[%i, "args", 0], $timeline[%i, "args", 1], $Cinematic::AIOrder[$timeline[%i, "args", 0]]++);
  }
  if (%actionCommand == "animateAI") {
    Player::setAnimation(Client::getOwnedObject(AI::getID($timeline[%i, "args", 0])),$timeline[%i, "args", 1]);
  }
  if (%actionCommand == "repeatedAnimateAI") {
    for (%j = 0; %j < $timeline[%i, "args", 2]; %j++) {
      schedule("Player::setAnimation("@Client::getOwnedObject(AI::getID($timeline[%i, "args", 0]))@","@$timeline[%i, "args", 1]@");", %j * $timeline[%i, "args", 3]);
    }
  }
  if (%actionCommand == "giveAIWeapon") {
    %ai = $timeline[%i, "args", 0];
    %obj = Client::getOwnedObject(AI::getID(%ai));
    %item = $timeline[%i, "args", 1];

    %obj.weaponMode[%item] = $timeline[%i, "args", 3];
    Player::setItemCount(%obj, %item, 1);
    Player::setItemCount(%obj, $WeaponAmmo[%item], $timeline[%i, "args", 2]);
    Player::useItem(%obj, %item);
  }
  if (%actionCommand == "targetPointAI") {
    %ai = $timeline[%i, "args", 0];
    AI::setVar(%ai, "iq", 10000);
    AI::setVar(%ai, "smartGuyMinAccuracy", 10000);
    AI::setVar(%ai, "attackMode", $timeline[%i, "args", 2]);
    AI::setVar(%ai, "triggerPct", $timeline[%i, "args", 3]);
    AI::DirectiveTargetPoint(%ai, $timeline[%i, "args", 1], $Cinematic::AIOrder[$timeline[%i, "args", 0]]);
  }
  if (%actionCommand == "cancelAICommand") {
    for (%j = 0; %j < $timeline[%i, "args", 1]; %j++)
      AI::DirectiveRemove($timeline[%i, "args", 0], $Cinematic::AIOrder[$timeline[%i, "args", 0]]-%j);
    AI::DirectiveList($timeline[%i, "args", 0]);
  }

  // OBJECT CONTROL COMMANDS
  if (%actionCommand == "newObjectSet") {
    %name = $timeline[%i, "args", 0];
    %path1 = $timeline[%i, "args", 1];
    %path2 = "MissionCleanup\\CinematicSet" @ tern(%path1 != "", "\\"@%path1, "");
    %parentID = nameToID(%path2);

    if (%parentID == -1) { echo("ERROR: CANNOT FIND OBJECT PATH " @ %path1); }
    else {
      %x = newObject(%name, SimSet);
      addToSet(%parentID, %x);
    }
  }
  if (%actionCommand == "newObject") {
    %name = $timeline[%i, "args", 0];
    %path1 = $timeline[%i, "args", 1];
    %path2 = "MissionCleanup\\CinematicSet" @ tern(%path1 != "", "\\"@%path1, "");
    %parentID = nameToID(%path2);

    if (%parentID == -1) { echo("ERROR: CANNOT FIND OBJECT PATH " @ %path1); }
    else {
      %x = newObject(%name, $timeline[%i, "args", 2], $timeline[%i, "args", 3], $timeline[%i, "args", 4]);
      if ($timeline[%i, "args", 5] != "") GameBase::setPosition(%x, $timeline[%i, "args", 5]);
      if ($timeline[%i, "args", 6] != "") GameBase::setRotation(%x, $timeline[%i, "args", 6]);
      addToSet(%parentID, %x);
    }
  }
  if (%actionCommand == "deleteObject") {
    %name = $timeline[%i, "args", 0];
    %path1 = $timeline[%i, "args", 1];
    %path2 = "MissionCleanup\\CinematicSet" @ tern(%path1 != "", "\\"@%path1, "");
    %id = nameToID(%path2 @ "\\" @ %name);
    if (%id == -1) { echo("ERROR: CANNOT FIND OBJECT " @ %path2 @ "\\" @ %name); }
    else { deleteObject(%id); }
  }
  if (%actionCommand == "loadObject") {
    %name = $timeline[%i, "args", 0];
    %path1 = $timeline[%i, "args", 1];
    %path2 = "MissionCleanup\\CinematicSet" @ tern(%path1 != "", "\\"@%path1, "");
    %parentID = nameToID(%path2 @ "\\" @ %name);
    if (%parentID == -1) { echo("ERROR: CANNOT FIND OBJECT PATH " @ %path2); }
    else { %x = loadObject(%name, $timeline[%i, "args", 2]); addToSet(%parentID, %x); }
  }

  // MISC COMMANDS
  if (%actionCommand == "messageAll") {
    messageAll($timeline[%i, "args", 0], $timeline[%i, "args", 1]);
  }
  if (%actionCommand == "cheers") {
    for (%j = 0; %j < $timeline[%i, "args", 0]; %j++) {
      schedule("messageAll(0,\"~wmale1.wcheer"@radnomItems(3,1,2,3)@".wav\");", $timeline[%i, "args", 1]*%j);
    }
  }
  if (%actionCommand == "eval") {
    eval($timeline[%i, "args", 0]);
  }
  if (%actionCommand == "exec") {
    exec($timeline[%i, "args", 0]);
  }
  if (%actionCommand == "resetTime") {
    $Cinematic::movieStartTime = getSimTime();
  }
  if (%actionCommand == "endMovie") {
    Cinematic::stop();
    Cinematic::clearViewers();
    return;
  }

  $Cinematic::timelineMarker++;
  if ($timeline[$Cinematic::timelineMarker, "action"] == "") return;

  %time = getWord($timeline[$Cinematic::timelineMarker, "action"], 0) - Cinematic::getMovieTime();
  schedule("Cinematic::timelineNext();", tern(%time >= 0, %time, 0));
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
  exec("misc\\cinematic.cs");
  exec(testmovie2);
  Cinematic::addAllViewers();
  Cinematic::parseTimeline();
}