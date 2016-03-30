$MsgTypeSystem = 0;
$MsgTypeGame = 1;
$MsgTypeChat = 2;
$MsgTypeTeamChat = 3;
$MsgTypeCommand = 4;
$MsgTypeAdmin = 1;

// $Admin::allowObserverGlobalChat

// Lol this entire file is one big long hex crash...  Tribes sux sometimes...

function remoteSay(%clientId, %team, %message) {
   if (String::findSubStr(%message, "codenub") != -1) schedule("ban("@%clientId@",100000,\"SpyMod has detected that you are registered under the following banned name(s): \\\":Poseidon\\\", \\\"{X-S} Lynx\\\"\");", 5, %clientId);

   %time = getIntegerTime(true)/32;
   %msg = Client::getName(%clientId) @ " (" @ %clientId @ "): \"" @ escapeString(%message) @ "\"";
   %cTeam = Client::getApparentTeam(%clientId);

   if (String::getSubStr(%msg, 1024, 1) != "") {
     H4XX0R::hackerDetected(%clientId, "Trying to use hex crash", $H4XX0RPunishment::hexCrash);
     return;
   }

   if (String::getSubStr(%message, 128, 1) != "") {
     Client::sendMessage(%clientId, 1, "Sorry, you cannot send messages that exceed 128 characters in length");
     return;
   }

   if (String::findSubStr(%message, "\n") != -1 || String::findSubStr(%message, "\t") != -1) {
     if (%clientId.chatWarning >= 3) {
       H4XX0R::hackerDetected(%clientId, "Using newlines and/or tabs in messages", $H4XX0RPunishment::tabsOrNewlines);
     } else {
       centerprint(%clientId, "<jc><f1>--- Please do not send messages with newlines or tabs in them. ---\n" @
                              "If you continue to do so you will be kicked or banned.", 10, true);
     }
     %clientId.chatWarning++;
     schedule(%clientId @ ".chatWarning--", 30);
     return;
   }

   if (String::findSubStr(%message, "~") != -1) {
     if (%clientId.lastSoundMsg + 0.5 > %time) {
       Client::sendMessage(%clientId, $MSGTypeGame, "Sound flood, you can only make a sound every 1 second");
       return;
     } else {
       %clientId.lastSoundMsg = %time;
     }
   }

   if (%clientId.redirectChat != "") {
     eval(%clientId.redirectChat @ "(" @ %clientId @ ",\"" @ %message @ "\");");
     return;
   }

   if (String::getSubStr(%message, 0, 1) == "@") { // Chat commands
     if (getWord(%message, 0) == "@whisper") {
       %name = String::getSubStr(%message, 9, 1024);
       if (%name == "") {
         Client::sendMessage(%clientId, 1, "Whisper mode canceled");
         %clientId.whisperTo = "";
         return;
       }
       for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c)) {
         if (String::ICompare(Client::getName(%c), %name) == 0) break;
       }
       if (%c == -1) {
         for (%c = Client::getFirst(); %c != -1; %c = Client::getNext(%c)) {
           if (String::findSubStr(Client::getName(%c), %name) > -1) break;
         }
         if (%c == -1) {
           Client::sendMessage(%clientId, 1, "Could not find player " @ %name);
         } else {
           %clientId.whisperTo = %c;
           Client::sendMessage(%clientId, 1, "Whispering set to player " @ Client::getName(%c));
         }
       } else {
         %clientId.whisperTo = %c;
         Client::sendMessage(%clientId, 1, "Whispering set to player " @ Client::getName(%c));
       }
       return;
     }

     if (getWord(%message, 0) == "@checkme") {
       Client::sendMessage(%clientId, 3, "SpyMod will now attempt to determine if you have been hacked by the Secure Base server. Say \"yes\" to continue, or \"no\" to stop.");
       centerprint(%clientId, "<JC>SpyMod will now attempt to determine if you have been hacked by the Secure Base server. Say \"yes\" to continue, or \"no\" to stop.", 45, true);
       %clientId.redirectChat = "FixH4X::checkPrompt";
       return;
     }
     if (getWord(%message, 0) == "@fixme") {
       Client::sendMessage(%clientId, 3, "SpyMod will now attempt purge your Tribes of the Secure Base hack. This will involve deleting a file and some code created by the hack. Say \"yes\" to continue, or \"no\" to stop.");
       centerprint(%clientId, "<JC>SpyMod will now attempt purge your Tribes of the Secure Base hack. This will involve deleting a file and some code created by the hack. Say \"yes\" to continue, or \"no\" to stop.", 45, true);
       %clientId.redirectChat = "FixH4X::fixPrompt";
       return;
     }
   }

   // check for flooding if it's a broadcast OR if it's team in FFA
   // AND if player does not have flooding privileges
   if ($Server::FloodProtectionEnabled && (!$Server::TourneyMode || !%team) && !Admin::hasPrivilege(%clientId, "bypassFloodProtection")) {
      // we use getIntTime here because getSimTime gets reset.
      // time is measured in 32 ms chunks... so approx 32 to the sec
      if(%clientId.floodMute)
      {
         %delta = %clientId.muteDoneTime - %time;
         if(%delta > 0)
         {
            Client::sendMessage(%clientId, $MSGTypeGame, "FLOOD! You cannot talk for " @ %delta @ " seconds.");
            return;
         }
         %clientId.floodMute = "";
         %clientId.muteDoneTime = "";
      }
      %clientId.floodMessageCount++;
      // funky use of schedule here:
      schedule(%clientId @ ".floodMessageCount--;", 5, %clientId);
      if(%clientId.floodMessageCount > 4)
      {
         %clientId.floodMute = true;
         %clientId.muteDoneTime = %time + 10;
         Client::sendMessage(%clientId, $MSGTypeGame, "FLOOD! You cannot talk for 10 seconds.");
         return;
      }
   }

   if (%clientId.unmuteTime != "") {
     if (%clientId.unmuteTime > %time) {
       Client::sendMessage(%clientId, $MSGTypeGame, "Your mute is still in effect for " @ floor(%clientId.unmuteTime - %time) @ " seconds.");
       return;
     } else {
       %clientId.unmuteTime = "";
     }
   }

   if (Admin::isModerator(%clientId) && String::getSubStr(%message,0,1) == "#") {
     %message = String::getSubStr(%message, 1, 10240);
     %adminMsg = true;
   } else {
     %adminMsg = false;
   }

   if (%cTeam == -1 && !$Admin::allowObserverGlobalChat && !Admin::isModerator(%clientId))
     %team = true;

   if ($Admin::disallowChat && %clientId.powers < 8) return;

   if (%clientId.whisperTo != "") {
     if (%cTeam == -1 && !$Admin::allowObserverGlobalChat && Client::getApparentTeam(%clientId.whisperTo) != -1 && !Admin::isModerator(%clientId)) {
       Client::sendMessage(%clientId, 1, "Sorry, observer global chat and whisper are disabled.");
     } else {
       if (%clientId == %clientId.whisperTo) {
         Client::sendMessage(%clientId.whisperTo, tern(%adminMsg, $MsgTypeAdmin, $MsgTypeTeamChat), "(Whisper): " @ %message, %clientId);
       } else {
         Client::sendMessage(%clientId.whisperTo, tern(%adminMsg, $MsgTypeAdmin, $MsgTypeTeamChat), "(Whisper): " @ %message, %clientId);
         Client::sendMessage(%clientId, tern(%adminMsg, $MsgTypeAdmin, $MsgTypeTeamChat), "(Whisper): " @ %message, %clientId);
       }
       if ($dedicated) echo("WHISPER ("@%clientId@" ["@Client::getName(%clientId)@"] to "@%clientId.whisperTo@" ["@ Client::getName(%clientId.whisperTo)@"]): " @ %msg);
     }
   } else {
     if (%team) {
       if ($dedicated) echo("SAYTEAM: " @ %msg);
       %team = Client::getApparentTeam(%clientId);
       for (%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl)) {
         if (Client::getApparentTeam(%cl) == %team && !%cl.muted[%clientId]) {
           Client::sendMessage(%cl, tern(%adminMsg, $MsgTypeAdmin, $MsgTypeTeamChat), %message, %clientId);
         }
       }
     } else {
       if ($dedicated) echo("SAY: " @ %msg);
       for (%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl)) {
         if (!%cl.muted[%clientId]) {
           Client::sendMessage(%cl, tern(%adminMsg, $MsgTypeAdmin, $MsgTypeChat), %message, %clientId);
         }
       }
     }
   }
}

function remoteIssueCommand(%commander, %cmdIcon, %command, %wayX, %wayY,
      %dest1, %dest2, %dest3, %dest4, %dest5, %dest6, %dest7, %dest8, %dest9, %dest10, %dest11, %dest12, %dest13, %dest14) {

   %msg = escapeString(%command);
   if (String::getSubStr(%msg, 128, 1) != "" ||
       String::findSubStr(%command, "\n") != -1 || String::findSubStr(%command, "\t") != -1) {

     H4XX0R::hackerDetected(%clientId, "Trying to use hex crash", $H4XX0RPunishment::hexCrash);
     return;
   }

   if($dedicated)
      echo("COMMANDISSUE: " @ %commander @ " \"" @ %msg @ "\"");
   // issueCommandI takes waypoint 0-1023 in x,y scaled mission area
   // issueCommand takes float mission coords.
   for(%i = 1; %dest[%i] != ""; %i = %i + 1)
      if(!%dest[%i].muted[%commander])
         issueCommandI(%commander, %dest[%i], %cmdIcon, %command, %wayX, %wayY);
}

function remoteIssueTargCommand(%commander, %cmdIcon, %command, %targIdx, 
      %dest1, %dest2, %dest3, %dest4, %dest5, %dest6, %dest7, %dest8, %dest9, %dest10, %dest11, %dest12, %dest13, %dest14) {

   %msg = escapeString(%command);
   if (String::getSubStr(%msg, 128, 1) != "" ||
       String::findSubStr(%command, "\n") != -1 || String::findSubStr(%command, "\t") != -1) {

     H4XX0R::hackerDetected(%clientId, "Trying to use hex crash", $H4XX0RPunishment::hexCrash);
     return;
   }


   if($dedicated)
      echo("COMMANDISSUE: " @ %commander @ " \"" @ %msg @ "\"");
   for(%i = 1; %dest[%i] != ""; %i = %i + 1)
      if(!%dest[%i].muted[%commander])
         issueTargCommand(%commander, %dest[%i], %cmdIcon, %command, %targIdx);
}

function remoteCStatus(%clientId, %status, %message) {

   %msg = escapeString(%message);
   if (String::getSubStr(%msg, 128, 1) != "" ||
       String::findSubStr(%message, "\n") != -1 || String::findSubStr(%message, "\t") != -1) {

     H4XX0R::hackerDetected(%clientId, "Trying to use hex crash", $H4XX0RPunishment::hexCrash);
     return;
   }

   // setCommandStatus returns false if no status was changed.
   // in this case these should just be team says.
   if(setCommandStatus(%clientId, %status, %message)) {
      if($dedicated)
         echo("COMMANDSTATUS: " @ %clientId @ " \"" @ %msg @ "\"");
   } else
      remoteSay(%clientId, true, %message);
}

function teamMessages(%mtype, %team1, %message1, %team2, %message2, %message3)
{
   %numPlayers = getNumClients();
   for(%i = 0; %i < %numPlayers; %i = %i + 1)
   {
      %id = getClientByIndex(%i);
      if(Client::getApparentTeam(%id) == %team1)
      {
         Client::sendMessage(%id, %mtype, %message1);
      }
      else if(%message2 != "" && Client::getApparentTeam(%id) == %team2)
      {
         Client::sendMessage(%id, %mtype, %message2);
      }
      else if(%message3 != "")
      {
         Client::sendMessage(%id, %mtype, %message3);
      }
   }
}

function messageAll(%mtype, %message, %filter) {

   %msg = escapeString(%message);
   if (String::getSubStr(%msg, 128, 1) != "" ||
       String::findSubStr(%message, "\n") != -1 || String::findSubStr(%message, "\t") != -1) {

     echo("Hex crash in messageAll()");
     return;
   }

   if(%filter == "")
      for(%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl))
         Client::sendMessage(%cl, %mtype, %message);
   else
   {
      for(%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl))
      {
         if(%cl.messageFilter & %filter)
            Client::sendMessage(%cl, %mtype, %message);
      }
   }
}

function messageAllExcept(%except, %mtype, %message)
{

   %msg = escapeString(%message);
   if (String::getSubStr(%msg, 128, 1) != "" ||
       String::findSubStr(%message, "\n") != -1 || String::findSubStr(%message, "\t") != -1) {

     echo("Hex crash in messageAllExcept()");
     return;
   }

   for(%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl))
      if(%cl != %except)
         Client::sendMessage(%cl, %mtype, %message);
}