
//////
// RespawnAI scripting
//////

function RespawnAI::init(%aiName, %marker, %team) {
  SuperAI::init(%aiName, %marker, %team);
  SuperAI::setVar(%aiName, "marker", %marker);
  SuperAI::setVar(%aiName, "team", %team);
}

function RespawnAI::onKilled(%aiName, %player) {
  SuperAI::onKilled(%aiName, %player);
  schedule("RespawnAI::respawn(\""@%aiName@"\");", 5);
}

function RespawnAI::respawn(%name) {
  %marker = SuperAI::getVar(%name, "marker");
  AI::spawn(%name, %marker.armor, GameBase::getPosition(%marker), GameBase::getRotation(%marker));
  AI::setScriptedTargets(%name);
  %obj = Client::getOwnedObject(AI::getID(%name));
  %obj.aiName = %name;

  $SuperAI::AIData[%name, "customScript"] = %marker.customScript;
  $SuperAI::AIData[%name, "objectId"] = %obj;

  SuperAI::callCustom(%name, "init", 2, %marker, SuperAI::getVar(%name, "team"));

  return %name;
}

function RespawnAI::onTargetDied(%aiName, %target) { SuperAI::onTargetDied(%aiName, %target); }
function RespawnAI::onTargetLOSAcquired(%aiName, %target) { SuperAI::onTargetLOSAcquired(%aiName, %target); }
function RespawnAI::onTargetLOSLost(%aiName, %target) { SuperAI::onTargetLOSLost(%aiName, %target); }
function RespawnAI::onDamage(%aiName, %shooter, %type, %value) { SuperAI::onDamage(%aiName, %shooter, %type, %value); }
function RespawnAI::targetFound(%aiName, %target) { echo("YAY"); SuperAI::targetFound(%aiName, %target); }
function RespawnAI::checkTarget(%aiName, %target, %curTarget) { return SuperAI::checkTarget(%aiName, %target, %curTarget); }