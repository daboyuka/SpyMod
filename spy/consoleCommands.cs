function help() {
  echo("------------------------------------------------------------------");
  echo(" ");
  echo("ban(%clientId, %time, %reason);");
  echo("  Bans the specified client");
  echo("  %clientId: client ID; %time: ban time in seconds; %reason: a string that is displayed to the person banned");
  echo(" ");
  echo(" ");
  echo("kick(%clientId, %reason);");
  echo("  Kicks the specified client");
  echo("  %clientId: client ID; %reason: a string that is displayed to the person kicked");
  echo(" ");
  echo(" ");
  echo("uberadmin(%clientId, %password);");
  echo("  Sets up an UberAdmin account");
  echo("  %clientId: client ID; %password: the password that will be given to the client IF they don't already have one");
  echo(" ");
  echo(" ");
  echo("ssay(%message);");
  echo("  Broadcasts a red message to all players on the server");
  echo("  %message: the message to say");
  echo(" ");
  echo("------------------------------------------------------------------");
}

function ban(%clientId, %time, %reason) {
  if (%time == "") %time = 24*60*60; // Ban for a day if time isn't specified.
  BanList::add(Client::getTransportAddress(%clientId), %time);
  kick(%clientId, %reason);
}

function kick(%clientId, %reason) {
  Net::kick(%clientId, %reason);
}

function uberadmin(%clientId, %password) {
  %index = %clientId.databaseIndex;
  if (String::empty(%index)) {
    echo("Error: cannont find database index for client " @ %clientId @ ". Make sure you have the correct client ID.");
    return;
  }

  if (%password == "" && PlayerDatabase::getPassword(%index,true) == "") {
    echo("Error: no password specified, and player doesn't have an existing password.");
    return;
  }

  if (%password == "") %password = PlayerDatabase::getPassword(%index,true);

  %uberadmin = Powers::includeAllLowerAdminPowers($Powers::POWER_UBERADMIN);
  PlayerDatabase::setPowers(%index, %uberadmin, true);
  PlayerDatabase::setPassword(%index, %password, true);
  PlayerDatabase::export();

  Powers::grantPowers(%clientId, %uberadmin);

  echo("UberAdmin account set up complete. The password is \"" @ %password @ "\".");
  Client::sendMessage(%clientId, 3, "You have been given UberAdmin! Your password is " @ %password @ ".~wmale1.wcheer3.wav");
}

function ssay(%msg) { messageAll(1,%msg); }

function setSADPassword(%pass) { $AdminPassword = %pass; }
function setUADPassword(%pass) { $UberAdminPassword = %pass; }