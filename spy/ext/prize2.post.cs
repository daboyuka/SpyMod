$Powers::POWER_PRIZE1 = Powers::addNewPower(256, "FlamingMunkee", false);

function PrizeFlamingMunkee::activate(%this) {
  if ((Player::getClient(%this).powers & $Powers::POWER_PRIZE1) == 0) return;

  if (getSimTime() - %this.lastFlamingMunkee < 60) { Client::sendMessage(Player::getClient(%this), 1, "Sorry, you can only catch on fire every 60 seconds."); return; }
  Effects::burn(%this, "-0.25 -0.25 0", "0.25 0.25 2.5", 30);
  Client::sendMessage(Player::getClient(%this), 1, "W00T!!! You are now a FLAMING MUNKEE!~wmale1.wcheer3.wav");
  %this.lastFlamingMunkee = getSimTime();
}