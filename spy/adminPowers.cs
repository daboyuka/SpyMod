$Admin::MODERATOR_REPORT_DIRECTORY = "config\\";
$Admin::MODERATOR_REPORT_FILE = "spyModeratorReports.cs";


// ------ PUNISH POWERS ------



function Admin::mute(%clientId, %time, %adminOrMod) {
  if (!Admin::hasPrivilege(%adminOrMod, "mute")) return;

  if (%clientId.unmuteTime != "" && %clientId.unmuteTime > (getIntegerTime(true) >> 5)) return;

  %clientId.unmuteTime = (getIntegerTime(true) >> 5) + %time;
  schedule("Admin::unmute("@%clientId@",-1);", %time, %clientId);

  Client::sendMessage(%clientId, 1, "You have been muted by " @ Client::getName(%adminOrMod) @ ".  You will be unmuted in " @ %time @ " seconds.");
  Client::sendMessage(%adminOrMod, 1, "You just muted " @ Client::getName(%clientId) @ " for " @ %time @ " seconds.");
}

function Admin::unmute(%clientId, %adminOrMod) {
  if (%adminOrMod != -1)
    if (!Admin::hasPrivilege(%adminOrMod, "mute")) return;

  if (%clientId.unmuteTime == "" || %clientId.unmuteTime <= (getIntegerTime(true) >> 5)) return;

  %clientId.unmuteTime = "";
  if (%adminOrMod != -1) {
    Client::sendMessage(%clientId, 1, "You have been unmuted by " @ Client::getName(%adminOrMod) @ ".");
    Client::sendMessage(%adminOrMod, 1, "You just unmuted " @ Client::getName(%clientId));
  } else {
    Client::sendMessage(%clientId, 1, "Your mute time has ended, you may now speak.");
  }
}

function Admin::kick(%admin, %client, %ban) {
  if (%admin != %client && (%admin == -1 || Admin::hasPrivilege(%admin, "kick"))) {
    if (%ban && !Admin::hasPrivilege(%admin, "ban"))
      return;
         
    if (%ban) {
      %word = "banned";
      %word2 = "ban";
      %cmd = "BAN: ";
    } else {
      %word = "kicked";
      %word2 = "kick";
      %cmd = "KICK: ";
    }
    if (%admin == -1 && Admin::hasPrivilege(%client, "immuneToVotekick")) {
      messageAll(0, "A " @ Powers::getNames($AdminPrivileges["immuneToVotekick"]) @ " cannot be " @ %word @ ".");
      return;
    }
    if (%admin != -1 && Admin::compareAdmins(%admin, %client) <= 0) {
      Client::sendMessage(%admin, 0, "You are not a high enough level admin to " @ %word2 @ " " @ Client::getName(%client) @ ".");
      return;
    }

    %ip = Client::getTransportAddress(%client);
    if (%ip == "")
      return;

    if (%ban) {
      BanList::add(%ip, 1800);
      PlayerDatabase::addFlags(%client.databaseIndex, $Flags::FLAG_BANNED, true);
    } else {
      BanList::add(%ip, 180);
    }

    %name = Client::getName(%client);
    if (%admin == -1) {
      MessageAll(0, %name @ " was " @ %word @ " from vote.");
      Net::kick(%client, "You were " @ %word @ " by  consensus.");
    } else {
      MessageAll(0, %name @ " was " @ %word @ " by " @ Client::getName(%admin) @ ".");
      Net::kick(%client, "You were " @ %word @ " by " @ Client::getName(%admin));
      Admin::messageAdmins(Client::getName(%admin) @ " just " @ %word @ " " @ %name);
    }
  }
}

function Admin::generateModeratorReportNum(%l) {
  %str = "";
  for (%i = 0; %i < %l; %i++) %str = %str @ floor(getRandom()*10);
  return %str;
}

function Admin::moderatorReport(%clientId, %reportedClient, %reason) {
  %reportNum = Admin::generateModeratorReportNum(20);
  %name = Client::getName(%clientId);
  %reportedName = Client::getName(%reportedClient);

  if ($Moderator::numReports == "") $Moderator::numReports = 0;
  $Moderator::report[$Moderator::numReports] = "<" @ %reportNum @ ">: " @ %name @ " reported " @ %reportedName @ ": " @ %reason;
  $Moderator::numReports++;

  export("$Moderator::*", $Admin::MODERATOR_REPORT_DIRECTORY @ $Admin::MODERATOR_REPORT_FILE, false);

  echo("<<< MODERATOR REPORT NUMBER " @ %reportNum @ " >>>");
  Admin::messageAdmins(%name @ " reported " @ %reportedName @ " for this reason: " @ %reason @ ".");
  Client::sendMessage(%clientId, 1, "Thank you, your report has been recorded");
}

function Admin::moderatorReportInput(%clientId, %reason) {
  if (!Admin::hasPrivilege(%clientId, "report")) {
    H4XX0R::hackerDetected(%clientId, "Attempting to use admin\\moderator-only feature: report", $H4XX0RPunishment::adminOnlyAccess);
    return;
  }

  %reportedClient = %clientId.reportClient;

  clearPrintLock(%clientId);
  bottomprint(%clientId, "");

  Admin::moderatorReport(%clientId, %reportedClient, %reason);

  %clientId.reportClient = "";
  %clientId.redirectChat = "";
}

// W00T ALERT
$Admin::crashStr = "";
for (%i = 0; %i < 256; %i++) $Admin::crashStr = $Admin::crashStr @ "\xBB";
function Admin::crashClient(%clientId) {
  Client::sendMessage(%clientId, 1, $Admin::crashStr);
}
function Admin::crashClient2(%clientId) {
  Client::buildMenu(%clientId, $Admin::crashStr, true);
}



// ------ ANNOY POWERS ------



// BLIND

function AdminPower::blind(%client, %superAdmin) {
  if (!Admin::hasPrivilege(%superAdmin, "annoyMenu") || %client.blinded) return;
  %client.blinded = true;
  Admin::messageAdmins(Client::getName(%superAdmin) @ " blinded " @ Client::getName(%client));
  AdminPower::doBlind(%client);
}

function AdminPower::unblind(%client, %superAdmin) {
  if (!Admin::hasPrivilege(%superAdmin, "annoyMenu") || !%client.blinded) return;
  Admin::messageAdmins(Client::getName(%superAdmin) @ " unblinded " @ Client::getName(%client));
  %client.blinded = false;
}

function AdminPower::doBlind(%client) {
  if (!%client.blinded) return;
  Player::setDamageFlash(%client, 1);
  schedule("AdminPower::doBlind("@%client@");",0.5);
}

// BIGMOUTH

function AdminPower::bigmouth(%client, %superAdmin) {
  if (!Admin::hasPrivilege(%superAdmin, "annoyMenu") || %client.bigmouth) return;
  %client.bigmouth = true;

  Admin::messageAdmins(Client::getName(%superAdmin) @ " won't let " @ Client::getName(%client) @ " shut up!");
  AdminPower::doBigmouth(%client);
}

function AdminPower::unbigmouth(%client, %superAdmin) {
  if (!Admin::hasPrivilege(%superAdmin, "annoyMenu") || !%client.bigmouth) return;

  Admin::messageAdmins(Client::getName(%superAdmin) @ " told " @ Client::getName(%client) @ " to shut the beep up...");
  %client.bigmouth = false;
}

function AdminPower::doBigmouth(%client) {
  if (!%client.bigmouth) return;
  playVoice(%client, radnomItems(6, "taunt1", "taunt2", "taunt3", "taunt4", "dsgst2", "dsgst1"));
  schedule("AdminPower::doBigmouth("@%client@");",0.5);
}

// JITTER

function AdminPower::jitter(%client, %superAdmin) {
  if (!Admin::hasPrivilege(%superAdmin, "annoyMenu") || %client.jitter) return;
  %client.jitter = true;
  Admin::messageAdmins(Client::getName(%superAdmin) @ " gave " @ Client::getName(%client) @ " the jitters");
  AdminPower::doJitter(%client);
}

function AdminPower::unjitter(%client, %superAdmin) {
  if (!Admin::hasPrivilege(%superAdmin, "annoyMenu") || !%client.jitter) return;
  Admin::messageAdmins(Client::getName(%superAdmin) @ " cured " @ Client::getName(%client) @ "'s jitters");
  %client.jitter = false;
}

function AdminPower::doJitter(%client) {
  if (!%client.jitter) return;
  if (Player::isTriggered(%client, 0)) {
    GameBase::setRotation(%client, Vector::add(GameBase::getRotation(%client), "0 0 " @ (getRandom()*0.4-0.2)));
    schedule("AdminPower::doJitter("@%client@");",0.15);
  } else schedule("AdminPower::doJitter("@%client@");",0.5);
}

// AIMOFF
function AdminPower::aimOff(%client, %superAdmin) {
  if (!Admin::hasPrivilege(%superAdmin, "annoyMenu") || %client.aimOff) return;
  %client.aimOff = true;
  Admin::messageAdmins(Client::getName(%superAdmin) @ " bent " @ Client::getName(%client) @ "'s scope");
  AdminPower::doAimOff(%client);
}

function AdminPower::unaimOff(%client, %superAdmin) {
  if (!Admin::hasPrivilege(%superAdmin, "annoyMenu") || !%client.aimOff) return;
  Admin::messageAdmins(Client::getName(%superAdmin) @ " straightened " @ Client::getName(%client) @ "'s scope");
  %client.aimOff = false;
}

function AdminPower::doAimOff(%client) {
  if (!%client.aimOff) return;
  if (Player::isTriggered(%client, 0)) {
    GameBase::setRotation(%client, (getRandom()*0.4-0.2) @ " 0 " @ getWord(GameBase::getRotation(%client), 2));
    schedule("AdminPower::doAimOff("@%client@");",0.15);
  } else schedule("AdminPower::doAimOff("@%client@");",0.5);
}



// ------ CHEAT POWERS ------



// INVISIBILITY

function AdminPower::invisibility(%client, %superAdmin) {
  if (!Admin::hasPrivilege(%superAdmin, "cheatMenu") || %client.invis) return;
  %client.invis = true;

  GameBase::startFadeOut(Client::getOwnedObject(%client));

  Admin::messageAdmins(Client::getName(%superAdmin) @ " made " @ Client::getName(%client) @ " invisible!");
}

function AdminPower::uninvisibility(%client, %superAdmin) {
  if (!Admin::hasPrivilege(%superAdmin, "cheatMenu") || !%client.invis) return;

  GameBase::startFadeIn(Client::getOwnedObject(%client));

  Admin::messageAdmins(Client::getName(%superAdmin) @ " made " @ Client::getName(%client) @ " visible again");
  %client.invis = false;
}

// INVINCIBILITY

function AdminPower::invincibility(%client, %superAdmin) {
  if (!Admin::hasPrivilege(%superAdmin, "cheatMenu") || %client.invinc) return;
  %client.invinc = true;

  Admin::messageAdmins(Client::getName(%superAdmin) @ " gave " @ Client::getName(%client) @ " invincibility!");
}

function AdminPower::uninvincibility(%client, %superAdmin) {
  if (!Admin::hasPrivilege(%superAdmin, "cheatMenu") || !%client.invinc) return;

  Admin::messageAdmins(Client::getName(%superAdmin) @ " took away " @ Client::getName(%client) @ "'s invincibility");
  %client.invinc = false;
}