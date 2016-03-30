function q() {

newClient();
focusClient();
newObject("", IRCClient);

ircEcho(all);
ircConnect("jasper.heavenlyplace.net", 6667);

focusServer();
}