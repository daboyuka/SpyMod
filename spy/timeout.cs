$Admin::allowTeamchange = false;
for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c)) {
  Observer::enterObserverMode(%c);
}