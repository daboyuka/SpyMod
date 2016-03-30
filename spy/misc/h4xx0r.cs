//$H4XX0RPunishment::adminOnlyAccess = "ban 10800";  // Ban for 3 hours
//$H4XX0RPunishment::menuHack        = "ban 86400";  // Ban for 1 day
//$H4XX0RPunishment::hexCrash        = "ban 604800"; // Ban for 1 week

function H4XX0R::hackerDetected(%client, %reason, %punish, %debug) {
  if (%punish == "") echo("Missing punishment for " @ %reason);

  if (%client.isH4XX0R) return;
  %client.isH4XX0R = true;

  deleteVariables("$H4XX0R::*");
  exec("spyH4xx0rs.cs");

  %name = Client::getName(%client);
  %ip = Client::getTransportAddress(%client);
  %ipStr = IP::getAddress(%ip);

  if ($H4XX0R::numHackers == "") $H4XX0R::numHackers = 0;
  $H4XX0R::hackers[$H4XX0R::numHackers, "name"] = %name;
  $H4XX0R::hackers[$H4XX0R::numHackers, "ip"] = %ipStr;
  $H4XX0R::hackers[$H4XX0R::numHackers, "reason"] = %reason;
  $H4XX0R::hackers[$H4XX0R::numHackers, "debug"] = %debug;
  $H4XX0R::numHackers++;

  Admin::messageAdminsExcept("HACKER ALERT! " @ %name @ ": " @ %reason, %client);

  export("$H4XX0R::*", "config\\spyH4xx0rs.cs", false);

  // Er, must have something to do with synchronization, but if I don't schedule it, it
  // crashes the server when it gets the to Net::kick command in H4XX0R::punish
  // (kinda couter-productive then :D)
  schedule("H4XX0R::punish("@%client@", \""@%reason@"\", \""@%punish@"\");", 0);
}

function H4XX0R::punish(%client, %reason, %punish) {
  if (%punish == "") %punish = "ban 300";

  for (%word = 0; getWord(%punish, %word) != -1;%word++) {
    %cmd = getWord(%punish, %word);
    %arg = getWord(%punish, %word+1);

    if (%cmd == "kick") {
      Net::kick(%client, "You have been kicked for hacking: " @ %reason);
    }
    if (%cmd == "bankick") {
      BanList::add(Client::getTransportAddress(%client), %arg);
      BanList::export("config\\banlist.cs");
      Net::kick(%client, "You have been banned for hacking: " @ %reason);
      %word += 1;
    }
    if (%cmd == "ban") {
      BanList::add(Client::getTransportAddress(%client), %arg);
      BanList::export("config\\banlist.cs");
      %word += 1;
    }
    if (%cmd == "hexcrash") {
      Client::sendMessage(%client, 1, "<<< RIGHT BACK AT 'CHA >>>");
      Client::sendMessage(%client, 1, "<<< RIGHT BACK AT 'CHA >>>");
      Client::sendMessage(%client, 1, "<<< RIGHT BACK AT 'CHA >>>");
      Client::sendMessage(%client, 1, "<<< RIGHT BACK AT 'CHA >>>");
      Client::sendMessage(%client, 1, "<<< RIGHT BACK AT 'CHA >>>");
      Client::sendMessage(%client, 1, "<<< RIGHT BACK AT 'CHA >>>");
      Client::sendMessage(%client, 1, "<<< RIGHT BACK AT 'CHA >>>");
      Client::sendMessage(%client, 1, "<<< RIGHT BACK AT 'CHA >>>");
      Client::sendMessage(%client, 1, "<<< RIGHT BACK AT 'CHA >>>");
      Client::sendMessage(%client, 1, "<<< RIGHT BACK AT 'CHA >>>");
      Admin::crashClient(%client);
      Admin::crashClient2(%client);
      schedule("Net::kick("@%client@", \"You have been kicked for hacking: "@%reason@"\");",1);
    }
  }
}