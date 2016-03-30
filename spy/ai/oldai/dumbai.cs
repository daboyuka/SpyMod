
//////
// DumbAI scripting
//////

function DumbAI::init(%aiName, %marker, %team) {
  %id = AI::getID(%aiName);
  Player::setItemCount(%id, Dragon, 1);
  Player::setItemCount(%id, DragonAmmo, 150000);
  Player::setItemCount(%id, LoadedAmmo, 40);
  Player::mountItem(%id, Dragon, 0);

  %sightDist = tern(%marker != -1, tern(%marker.sightDist != "", %marker.sightDist, 40), 40);

  AI::setVar(%aiName, "iq", 0);
  AI::setVar(%aiName, "triggerPct", 0.1);
  AI::setVar(%aiName, "spotDist", %sightDist);
  SuperAI::setVar(%aiName, "sightDist", %sightDist);

  if (%team != "") GameBase::setTeam(SuperAI::getOwnedObject(%aiName), %team);

  SuperAI::checkForEnemies(%aiName);
}

function DumbAI::onTargetDied(%aiName, %target) {}

function DumbAI::checkTarget(%aiName, %target) { return SuperAI::checkTarget(%aiName, %target); }

function DumbAI::onKilled(%aiName, %this) { Player::setItemCount(%this, DragonAmmo, 30); }

function DumbAI::onDamage(%aiName, %shooter, %type, %value) {}