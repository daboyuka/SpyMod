//---------------------------------*
// SuperAI custom scripting engine |
//---------------------------------*
//
// A SuperAI custom script must include the following functions (scriptname is the name of your script):
//   * scriptname::init(%aiName, %marker): This function is called after the the AI is spawn.  You should setup you AI here.
//                                         %marker is a marker object supplied if the ai was spawn from map data.  It should
//                                         contain arguments for the creation of the ai.  If the ai has not spawned from map
//                                         data, this will be -1.
//   * scriptname::onTargetDied(%aiName, %target): This function is called when an AI's target dies for some reason.
//   * scriptname::onTargetLOSAcquired(%aiName, %target): This function is called when AI::onTargetLOSAcquired is called.
//   * scriptname::onTargetLOSLost(%aiName, %target): This function is called when AI::onTargetLOSLost is called.
//   * scriptname::onKilled(%aiName, %this): This function is called when the AI dies.  %this is the player object id.
//   * scriptname::onDamage(%aiName, %shooter, %type, %value): This function is called when the AI is damaged.
//
// Also, if your script chooses to use the standard SuperAI::checkForEnemies() function must define
// scriptname::checkTarget(%aiName, %target), which should directiveTarget the %target if it should be attacked.

//
// Stub:
//
//function scriptname::init(%aiName, %marker) { SuperAI::init(%aiName, %marker); }
//function scriptname::onTargetDied(%aiName, %target) { SuperAI::onTargetDied(%aiName, %target); }
//function scriptname::onTargetLOSAcquired(%aiName, %target) { SuperAI::onTargetLOSAcquired(%aiName, %target); }
//function scriptname::onTargetLOSLost(%aiName, %target) { SuperAI::onTargetLOSLost(%aiName, %target); }
//function scriptname::onKilled(%aiName, %this) { SuperAI::onKilled(%aiName, %this); }
//function scriptname::onDamage(%aiName, %shooter, %type, %value) { SuperAI::onDamage(%aiName, %shooter, %type, %value); }
//function scriptname::targetFound(%aiName, %target) { SuperAI::targetFound(%aiName, %target); }



deleteVariables("$SuperAI::AIData*");
$SuperAI::curCounter = 0;

//////////////////
// Spawn functions
//////////////////

function SuperAI::Spawn(%name, %armor, %pos, %rot, %customScript, %marker, %team) {
  if (%name == "") %name = SuperAI::nextName();
  else %name = %name @ ($SuperAI::curCounter++);

  AI::spawn(%name, %armor, %pos, %rot);
  AI::setScriptedTargets(%name);

  %obj = Client::getOwnedObject(AI::getID(%name));
  %obj.aiName = %name;

  $SuperAI::AIData[%name, "customScript"] = %customScript;
  $SuperAI::AIData[%name, "objectId"] = %obj;

  SuperAI::callCustom(%name, "init", 2, tern(%marker, %marker, -1), %team);

  return %name;
}

function SuperAI::nextName() { return "SuperAI" @ ($SuperAI::curCounter++); }

function SuperAI::SpawnFromMapData() {
  for (%i = 0; %i < getNumTeams(); %i++) {
    %groupId = nameToID("MissionGroup\\Teams\\team" @ %i @ "\\SuperAI");
    if (%groupId == -1) continue;
    SuperAI::SpawnFromGroup(%groupId, %i);
  }
}

function SuperAI::SpawnFromGroup(%groupId, %team) {
  %numAIS = Group::objectCount(%groupId);
  for (%j = 0; %j < %numAIS; %j++) {
    %marker = Group::getObject(%groupId, %j);
    if (getObjectType(%marker) == "SimGroup") SuperAI::SpawnFromGroup(%marker);
    else {
      %name = SuperAI::Spawn(Object::getName(%marker), %marker.armor,
                             GameBase::getPosition(%marker), GameBase::getRotation(%marker),
                             %marker.customScript, %marker, %team);
    }
  }
}

///////////////////////////////////////
// Low level custom scripting functions
///////////////////////////////////////

function SuperAI::callCustom(%aiName, %funcName, %numArgs, %arg0, %arg1, %arg2, %arg3, %arg4, %arg5, %arg6, %arg7) {
  %customScript = $SuperAI::AIData[%aiName, "customScript"];
  if (%customScript == "") %customScript = "SuperAI";
  if (%numArgs == "") %numArgs = 0;

  invoke(%customScript @ "::" @ %funcName, %numArgs+1, %aiName, %arg0, %arg1, %arg2, %arg3, %arg4, %arg5, %arg6, %arg7);
//  %command = %customScript @ "::" @ %funcName @ "(" @ %aiName;
//  for (%i = 0; %i < %numArgs; %i++) %command = %command @ ",\"" @ %arg[%i] @ "\"";
//  %command = %command @ ");";
//  eval(%command);
}

////////////////////////
// Standard AI callbacks
////////////////////////
function AI::onTargetDied(%aiName, %target) { SuperAI::callCustom(%aiName, "onTargetDied", 1, %target); }
function AI::onTargetLOSAcquired(%aiName, %target) { SuperAI::callCustom(%aiName, "onTargetLOSAcquired", 1, %target); }
function AI::onTargetLOSRegained(%aiName, %target) { SuperAI::callCustom(%aiName, "onTargetLOSAcquired", 1, %target); }
function AI::onTargetLOSLost(%aiName, %target) { SuperAI::callCustom(%aiName, "onTargetLOSLost", 1, %target); }
function AI::onKilled(%aiName, %this) { SuperAI::callCustom(%aiName, "onKilled", 1, %this); }

////////////////////
// Utility functions
////////////////////
function SuperAI::cancelTarget(%aiName) { AI::directiveRemove(%aiName, 100); }

function SuperAI::isDead(%aiName) {
  if (!isObject($SuperAI::AIData[%aiName, "objectId"])) return true;
  else return Player::isDead($SuperAI::AIData[%aiName, "objectId"]);
}

function AI::getVar(%aiName, %varName) { return $AI::[%aiName @ "::" @ %varName]; }

function SuperAI::getVar(%aiName, %varName) { return $SuperAI::AIData[%aiName, %varName]; }
function SuperAI::setVar(%aiName, %varName, %value) { $SuperAI::AIData[%aiName, %varName] = %value; }

function SuperAI::getPosition(%aiName) { return GameBase::getPosition(SuperAI::getVar(%aiName, "objectId")); }
function SuperAI::setPosition(%aiName, %pos) { return GameBase::setPosition(SuperAI::getVar(%aiName, "objectId"), %pos); }

function SuperAI::getRotation(%aiName) { return GameBase::getRotation(SuperAI::getVar(%aiName, "objectId")); }
function SuperAI::setRotation(%aiName, %pos) { return GameBase::setRotation(SuperAI::getVar(%aiName, "objectId"), %pos); }

function SuperAI::getOwnedObject(%aiName) { return SuperAI::getVar(%aiName, "objectId"); }

function SuperAI::getName(%id) { return %id.aiName; }

function AI::soundHelper( %sourceId, %destId, %waveFileName ) {
   %wName = strcat( "~w", Client::getVoiceBase( %sourceId ) );
   %wName = strcat( %wName, ".w" );
   %wName = strcat( %wName, %waveFileName );
   %wName = strcat( %wName, ".wav" );
   
   dbecho( 2, "Trying to play " @ %wName );
   
   Client::sendMessage( %destId, 0, %wName );
}



exec("ai\\oldai\\superaiscript.cs");