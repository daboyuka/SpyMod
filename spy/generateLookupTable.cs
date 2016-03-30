exec("misc\\standardFuncs.cs");
exec("misc\\playerDatabase.cs");

$PlayerDatabaseOptions::acceleratedLookups = true;
PlayerDatabase::import();

%num = PlayerDatabase::getNumPlayers(true);
for (%i = %num - 1; %i >= 0; %i--) {
  echo(%i);
  for (%j = 0; %j < $PlayerDatabaseConstants::MAX_NAMES; %j++) {
    %name = PlayerDatabase::getName(%i, %j, true);
    if (%name == "") break;
    PlayerDatabaseLookup::registerName(%name, %i, true);
    echo("Registering " @ %name @ "->" @ %i);
  }
  for (%j = 0; %j < $PlayerDatabaseConstants::MAX_IPS; %j++) {
    %ip = PlayerDatabase::getIP(%i, %j, true);
    if (%ip == "") break;
    PlayerDatabaseLookup::registerIP(%ip, %i, true);
    echo("Registering " @ %ip @ "->" @ %i);
  }
}

PlayerDatabase::export();