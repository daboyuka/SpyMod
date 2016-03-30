$AIStateSystem::eventRedirect["UberAI", "engage", "targetLOSLost"] = true;
$AIStateSystem::eventRedirect["UberAI", "patrol", "checktarget"] = true;
$AIStateSystem::eventRedirect["UberAI", "patrol", "enemiesFound"] = true;

function UberAI::init(%aiName, %marker) {
  SuperAI::changeState(%aiName, "patrol");
}

function UberAI::onTargetDied() {}
function UberAI::onTargetLOSAcquired() {}
function UberAI::onTargetLOSLost() {}
function UberAI::onKilled() {}
function UberAI::onDamage() {}





function UberAI::patrolState::enter(%aiName) {
  echo("Entering patrol state");

  SuperAI::enemySearchMacro(%aiName);

  AI::setVar(%aiName, "iq", 10000);
}

function UberAI::patrolState::exit(%aiName) {
  echo("Exiting patrol state");
}

function UberAI::patrolState::onCheckTarget(%aiName, %target) {
  echo("Checking target: " @ %target);
  return true;
}

function UberAI::patrolState::onEnemiesFound(%aiName, %enemies) {
  SuperAI::changeState(%aiName, engage, 1, Group::getObject(%enemies, 0));
}






function UberAI::engageState::enter(%aiName, %target) {
  echo("Entering engage state");

  echo(%target);
  AI::DirectiveTarget(%aiName, GameBase::getOwnerClient(%target), 1000);
  SuperAI::statePeriodic(%aiName, "SuperAI::jump", 3);
  SuperAI::statePeriodic(%aiName, "UberAI::quickTurn", 0.125);
}

function UberAI::engageState::exit(%aiName) {
  echo("Exiting engage state");
}

function UberAI::engageState::onTargetLOSLost(%aiName, %target) {
  echo("onTargetLOSLost @ Patrol state");
  SuperAI::changeState(%aiName, "patrol");
}



function UberAI::quickTurn(%aiName) {
  %pos1 = SuperAI::getPosition(%aiName);
  %pos2 = GameBase::getPosition(AI::getTarget(%aiName));


  %dirRot = getWord(Vector::getRot(Vector::sub(%pos2, %pos1)), 2);
  %aiRot = getWord(SuperAI::getRotation(%aiName), 2);

  if (abs(%dirRot - %aiRot) > $PI/24) SuperAI::setRotation(%aiName, "0 0 " @ %dirRot);
}