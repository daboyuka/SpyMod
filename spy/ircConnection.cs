function IRCConnection::connect(%server, %port) {
  if (!$IRCConnection::clientCreated) {
    newClient();
    focusClient();
    newObject("", IRCClient);

    $IRCConnection::clientCreated = true;
  } else focusClient();

  //ircEcho(all);
  ircConnect(%server, %port);

  focusServer();
}

function IRCConnection::sendMsg(%msg) {
  focusClient();
  ircSend(%msg);
  focusServer();
}

function IRCConnection::joinRoom(%room) {
  IRCConnection::sendMsg("/join " @ %room);
}

function IRCConnection::setRoomTopic(%room, %topic) {
  IRCConnection::sendMsg("/topic " @ %room @ " :" @ %topic);
}

function IRCConnection::keepAlive() {
  focusClient();
  if (isObject(ClientScheduler)) deleteObject(ClientScheduler);
  newObject(ClientScheduler, SimConsoleScheduler);

  IRCConnection::pingSelfLoop();
  focusServer();
}

function IRCConnection::pingSelfLoop() {
  focusClient();
  ircSend("/ping " @ $IRC::nickname);
  schedule("IRCConnection::pingSelfLoop();", 5);
  focusServer();
}

function IRCConnection::disconnect() {
  focusClient();
  ircDisconnect();
  focusServer();
}



function IRCReporting::onServerStart() {
  $IRC::nickname = $Server::IRCReporting::nickname;
  $IRC::realname = $Server::IRCReporting::nickname;
  $IRC::room = $Server::IRCReporting::room;
  IRCConnection::connect($Server::IRCReporting::server, $Server::IRCReporting::serverPort);
  IRCConnection::joinRoom($Server::IRCReporting::room);

  schedule("IRCConnection::keepAlive();", 15);
  schedule("IRCReporting::updateRoomTopic();", 15);
}

function IRCReporting::updateRoomTopic(%numClients) {
  IRCConnection::setRoomTopic($Server::IRCReporting::room, $Server::HostName @ " - " @ %numClients @ " players");
}

function IRCReporting::onClientConnected(%clientId) {
  IRCReporting::updateRoomTopic(getNumClients());
  IRCConnection::sendMsg(Client::getName(%clientId) @ " just connected.");
}

function IRCReporting::onClientDisconnected(%clientId) {
  IRCReporting::updateRoomTopic(getNumClients() - 1);
  IRCConnection::sendMsg(Client::getName(%clientId) @ " dropped.");
}