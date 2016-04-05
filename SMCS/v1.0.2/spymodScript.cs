////////////////////////////////////////////////////////////////////////
// SpyMod Client Script v1.0.2
//
// Copyright 2009 "I AM BOB".
////////////////////////////////////////////////////////////////////////
// - W00T -
////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////
// VERSION NUMBER
////////////////////////////////////////////////////////////////////////

// Don't mess with this, or stuff will blow up. Don't come crying to me
// when your harddrive gets nuked because you monkeyed around with this...
$SpyModClientScript::CLIENT_VERSION = "1 0 2";

// Standard functions
function tern(%a, %b, %c) {
  if (%a) return %b;
  else    return %c;
}

function getSuffix(%i) {
  if (floor((%i % 100)/10) == 1) return "th";

  if (%i%10 == 1) return "st";
  if (%i%10 == 2) return "nd";
  if (%i%10 == 3) return "rd";
  return "th";
}

////////////////////////////////////////////////////////////////////////
// BASIC FUNCTIONS
////////////////////////////////////////////////////////////////////////

function SpyMod::exportSettings() {
  export("$SpyMod::*", "config\\spymodSettings.cs", false);
}

////////////////////////////////////////////////////////////////////////
// CONFIG FUNCTIONS
////////////////////////////////////////////////////////////////////////

function useSpyModControls() {
  if (isObject(OptionsGui)) deleteObject(OptionsGui);
  exec("configSpyMod.cs");
}
function useNormalControls() {
  if (isObject(OptionsGui)) deleteObject(OptionsGui);
  exec("configNormal.cs");
}

function turnOnAutoSwitchControls() {
  function remoteSVInfo(%server, %version, %hostname, %mod, %info, %favKey) {
     if(%server == 2048) {
        $ServerVersion = %version;
        $ServerName = %hostname;
        $modList = %mod;
        $ServerMod = $modList;
        $ServerInfo = %info;
        $ServerFavoritesKey = %favKey;
        EvalSearchPath();

        if (String::findSubStr($modList, "spy") != -1) {
          loadSpyModControls();
        } else {
          loadNormalControls();
        }
     }
  }
}

function loadSpyModControls() {
  if (!$SpyMod::SpyModControls) {
    saveActionMap("config\\configNormal.cs", "actionMap.sae", "playMap.sae", "pdaMap.sae");
    useSpyModControls();
    $SpyMod::SpyModControls = true;
    SpyMod::exportSettings();
  }
}

function loadNormalControls() {
  if ($SpyMod::SpyModControls) {
    saveActionMap("config\\configSpyMod.cs", "actionMap.sae", "playMap.sae", "pdaMap.sae");
    useNormalControls();
    $SpyMod::SpyModControls = false;
    SpyMod::exportSettings();
  }
}

function SpyMod::enableAutoSwitchControls() {
  if ($SpyMod::autoSwitchControls) return;

  turnOnAutoSwitchControls();

  $SpyMod::autoSwitchControls = true;
  SpyMod::exportSettings();
}

function SpyMod::disableAutoSwitchControls() {
  if (!$SpyMod::autoSwitchControls) return;

  $SpyMod::autoSwitchControls = false;

  GuiPopDialog(MainWindow, 0);
  GuiPushDialog(MainWindow, "gui\\MessageDialog.gui");
  Control::setValue(MessageDialogTextFormat, "<jc><f1>Auto switch controls will be disabled next time you start Tribes");

  SpyMod::exportSettings();
}

////////////////////////////////////////////////////////////////////////
// SERVER-CLIENT STUFF
////////////////////////////////////////////////////////////////////////

//    remote methods
function remoteIsUsingSpyModClientScript(%server) {
  if (%server != 2048) return;

  remoteEval(2048, "IAmUsingSpyModClientScript", $SpyModClientScript::CLIENT_VERSION);
  SpyMod::uploadPreferences();
}

function remoteOnFirstSpawn(%server) {
  if (%server != 2048) return;
  schedule("SpyMod::uploadPassword();", 3);
}

function remoteDownloadPassword(%server, %password) {
  if (%server != 2048) return;

  $SpyMod::gamePrefs::password[$PCFG::CurrentPlayer] = %password;
  $SpyMod::gamePrefs::passwordPlayer[$PCFG::CurrentPlayer] = $PCFG::Name[$PCFG::CurrentPlayer];

  SpyMod::exportSettings();
}

function remoteDownloadPreferences(%server, %prefs) {
  if (%server != 2048) return;

  $SpyMod::gamePrefs::prefs = %prefs;
  SpyMod::exportSettings();
}

function remoteAddBuddy(%server, %name) {
  if (%server != 2048) return;

  for (%i = 0; %i < 1024; %i++) {
    if ($pref::buddyList[%i] == "") {
      $pref::buddyList[%i] = %name;
      break;
    }
  }
}

////////////////////////////////////////////////////////////////////////
// KILL COUNTER STUFF
////////////////////////////////////////////////////////////////////////

function remoteOnKill(%server, %you, %him, %dmgType, %tk) {
  if (%server != 2048) return;

  %yourName = Client::getName(%you);
  if (%him > 0 && Client::getName(%him) != "") {
    if (%him != %you) {
      if (%tk) {
        $SpyMod::killCounter::teamkills++;
        if ($SpyMod::showTKMsgs)
          say(0,"I just TKed " @ Client::getName(%him) @ "! Ach, get outta my way next time! (" @ $SpyMod::killCounter::teamkills @ getSuffix($SpyMod::killCounter::teamkills) @ " teamkill)");
      } else {
        $SpyMod::killCounter::kills++;
        if ($SpyMod::showKillMsgs)
          say(0,Client::getName(%him) @ " got so owned by " @ %yourName @ "! (" @ $SpyMod::killCounter::kills @ getSuffix($SpyMod::killCounter::kills) @ " kill)");
      }
    }
  } else {
    if (%tk) { $SpyMod::killCounter::teamkills++; } else { $SpyMod::killCounter::kills++; }
    if ($SpyMod::showKillMsgs)
      say(0,"Just killed something, not sure what it was though... (" @ $SpyMod::killCounter::kills @ getSuffix($SpyMod::killCounter::kills) @ " kill)");
  }
  SpyMod::exportSettings();
}

function remoteOnKilled(%server, %you, %him, %dmgType, %tk) {
  if (%server != 2048) return;

  if (!(%tk && %you != %him)) $SpyMod::killCounter::deaths++;
  if (%him > 0 && Client::getName(%him) != "") {
    if (%you != %him) {
      if (%tk) {
        if ($SpyMod::showDeathMsgs)
          say(0,"Watch where you're shooting " @ Client::getName(%him) @ ", Mr. Trigger Happy! (" @ $SpyMod::killCounter::deaths @ getSuffix($SpyMod::killCounter::deaths) @ " death)");
      } else {
        if ($SpyMod::showDeathMsgs)
          say(0,Client::getName(%him) @ " got a few lucky shots in (" @ $SpyMod::killCounter::deaths @ getSuffix($SpyMod::killCounter::deaths) @ " death)");
      }
    } else {
      if ($SpyMod::showDeathMsgs)
        say(0,"Well that hurt... (" @ $SpyMod::killCounter::deaths @ getSuffix($SpyMod::killCounter::deaths) @ " death)");
    }
  } else {
    if ($SpyMod::showDeathMsgs)
      say(0,"Just got killed by some random object... (" @ $SpyMod::killCounter::deaths @ getSuffix($SpyMod::killCounter::deaths) @ " death)");
  }
  SpyMod::exportSettings();
}

//   other stuff

function SpyMod::uploadPassword() {
  if ($PCFG::Name[$PCFG::CurrentPlayer] != $SpyMod::gamePrefs::passwordPlayer[$PCFG::CurrentPlayer]) {
    $SpyMod::gamePrefs::password[$PCFG::CurrentPlayer] = "";
    SpyMod::exportSettings();
  }
  if ($SpyMod::gamePrefs::password[$PCFG::CurrentPlayer] != "")
    remoteEval(2048, "uploadPassword", $SpyMod::gamePrefs::password[$PCFG::CurrentPlayer]);
}

function SpyMod::uploadPreferences() {
  remoteEval(2048, "uploadPreferences",
             $SpyMod::gamePrefs::prefs);
}


////////////////////////////////////////////////////////////////////////
// CUSTOM BINDS
////////////////////////////////////////////////////////////////////////

function SpyMod::customBind(%x) {
  eval($SpyModBindCommands[$SpyMod::customBind[%x]]);
}

$SpyModBindCommands[0] = "use(\"Magnum\");";
$SpyModBindCommands[1] = "use(\"Dragon\");";
$SpyModBindCommands[2] = "use(\"Tornado MK III\");";
$SpyModBindCommands[3] = "use(\"MG27\");";
$SpyModBindCommands[4] = "use(\"X-9 Challenger\");";
$SpyModBindCommands[5] = "use(\"V7 Grenade Launcher\");";
$SpyModBindCommands[6] = "use(\"DoomSlayer\");";
$SpyModBindCommands[7] = "use(\"Rocket Launcher\");";
$SpyModBindCommands[8] = "use(\"Magdalon\");";
$SpyModBindCommands[9] = "use(\"Shotgun M2\");";
$SpyModBindCommands[10] = "use(\"Flamer\");";
$SpyModBindCommands[11] = "use(\"AGP84\");";
$SpyModBindCommands[12] = "use(\"Knife\");";
$SpyModBindCommands[13] = "use(\"Grappler\");";
$SpyModBindCommands[14] = "use(\"Plastique Explosive\");";
$SpyModBindCommands[15] = "use(\"Binoculars\");";
$SpyModBindCommands[16] = "use(\"Smoke Grenade\");";
$SpyModBindCommands[17] = "use(\"Distraction Grenade\");";
$SpyModBindCommands[18] = "use(\"Poison Gas Grenade\");";
$SpyModBindCommands[19] = "use(\"Raider\");";

function SpyModCustomBinds::populateBindCommands() {
  for (%i = 1; %i <= 3; %i++) {
    FGCombo::clear("SpyModBindCommand" @ %i);
    FGCombo::addEntry("SpyModBindCommand" @ %i, "Use Magnum", 0);
    FGCombo::addEntry("SpyModBindCommand" @ %i, "Use Dragon", 1);
    FGCombo::addEntry("SpyModBindCommand" @ %i, "Use Tornado", 2);
    FGCombo::addEntry("SpyModBindCommand" @ %i, "Use MG27", 3);
    FGCombo::addEntry("SpyModBindCommand" @ %i, "Use Challenger", 4);
    FGCombo::addEntry("SpyModBindCommand" @ %i, "Use Grenade Launcher", 5);
    FGCombo::addEntry("SpyModBindCommand" @ %i, "Use DoomSlayer", 6);
    FGCombo::addEntry("SpyModBindCommand" @ %i, "Use Rocket Launcher", 7);
    FGCombo::addEntry("SpyModBindCommand" @ %i, "Use Magdalon", 8);
    FGCombo::addEntry("SpyModBindCommand" @ %i, "Use Shotgun M2", 9);
    FGCombo::addEntry("SpyModBindCommand" @ %i, "Use Flamer", 10);
    FGCombo::addEntry("SpyModBindCommand" @ %i, "Use AGP84", 11);
    FGCombo::addEntry("SpyModBindCommand" @ %i, "Use Knife", 12);
    FGCombo::addEntry("SpyModBindCommand" @ %i, "Use Grappler", 13);
    FGCombo::addEntry("SpyModBindCommand" @ %i, "Use Plastique Explosives", 14);
    FGCombo::addEntry("SpyModBindCommand" @ %i, "Use Binoculars", 15);
    FGCombo::addEntry("SpyModBindCommand" @ %i, "Drop Smoke Grenade", 16);
    FGCombo::addEntry("SpyModBindCommand" @ %i, "Drop Distraction Grenade", 17);
    FGCombo::addEntry("SpyModBindCommand" @ %i, "Drop Poison Gas Grenade", 18);
    FGCombo::addEntry("SpyModBindCommand" @ %i, "Use Raider", 19);
  }
}

function SpyModCustomBinds::updateBindCommands() {
  for (%i = 1; %i <= 3; %i++) {
    FGCombo::setSelected("SpyModBindCommand" @ %i, $SpyMod::customBind[%i]);
  }  
}

function SpyModCustomBinds::setBindText() {
  Control::setValue(SpyModBind1Text, "<jc><f0>Custom Bind 1 <f2>(CTRL+1)");
  Control::setValue(SpyModBind2Text, "<jc><f0>Custom Bind 2 <f2>(CTRL+2)");
  Control::setValue(SpyModBind3Text, "<jc><f0>Custom Bind 3 <f2>(CTRL+3)");
}

function SpyMod::setCustomBindCommand(%x) {
  $SpyMod::customBind[%x] = FGCombo::getSelected("SpyModBindCommand" @ %x);
  SpyMod::exportSettings();
}

////////////////////////////////////////////////////////////////////////
// GUI SCRIPTING
////////////////////////////////////////////////////////////////////////

function SpyMod::options() {
  EnterLobbyMode();
  schedule("SpyMod::showOptions();",0);
}

function SpyMod::showOptions() {
  GuiLoadContentCtrl(MainWindow, "config\\spymodOptions.gui");

  if ($ConnectedToServer) $PlayGameGui = "gui\\Lobby.gui";
  else                    $PlayGameGui = "gui\\MainMenu.gui";
}

function SpyModOptionsGui::onOpen() {
  %a = tern($SpyMod::SpyModControls    , true, false);
  %b = tern(!$SpyMod::SpyModControls   , true, false);
  %c = tern($SpyMod::autoSwitchControls, true, false);

  Control::setValue("SpyModControls", %a);
  Control::setValue("NormalControls", %b);
  Control::setValue("AutoSwitchControls", %c);

  SpyModPlayerNames::populate();
  SpyModKillCounter::update();
  SpyModCustomBinds::populateBindCommands();
  SpyModCustomBinds::updateBindCommands();
  SpyModCustomBinds::setBindText();
  SpyModHelp::setHelpText();
}

function SpyModOptionsGui::onClose() {
  SpyMod::exportSettings();
}

function SpyModPlayerNames::populate() {
  FGCombo::clear(SpyModPlayerNames);

  $PCFG::LastPlayer = -1;
  for (%i = 0; $PCFG::Name[%i] != ""; %i++) {
    FGCombo::addEntry(SpyModPlayerNames, $PCFG::Name[%i], %i);
    $PCFG::LastPlayer = %i;
  }

  if ($PCFG::CurrentPlayer != -1 && $PCFG::CurrentPlayer <= $PCFG::LastPlayer)
    FGCombo::setSelected(SpyModPlayerNames, $PCFG::CurrentPlayer);

  SpyMod::onPlayerSelected();
}

function SpyMod::onPlayerSelected() {
  %num = FGCombo::getSelected(SpyModPlayerNames);
  if ($PCFG::Name[%num] != $SpyMod::gamePrefs::passwordPlayer[%num]) {
    $SpyMod::gamePrefs::passwordPlayer[%num] = $PCFG::Name[%num];
    $SpyMod::gamePrefs::password[%num] = "";
    SpyMod::exportSettings();
  }
  if (%num >= 0) {
    Control::setText(SpyModPlayerPassword, $SpyMod::gamePrefs::password[%num]);
  }
}

function SpyMod::onChangePassword() {
  %num = FGCombo::getSelected(SpyModPlayerNames);

  $SpyMod::gamePrefs::password[%num] = Control::getText(SpyModPlayerPassword);
  $SpyMod::gamePrefs::passwordPlayer[%num] = $PCFG::Name[%num];
  SpyMod::exportSettings();
}

function SpyModKillCounter::update() {
  Control::setText(KillCounterKills, $SpyMod::killCounter::kills);
  Control::setText(KillCounterDeaths, $SpyMod::killCounter::deaths);
  Control::setText(KillCounterTKs, $SpyMod::killCounter::teamkills);
}

function SpyModHelp::setHelpText() {
  Control::setValue(SpyModHelp, "<JC><F1>--- SpyMod Client Script Help ---" @ "\n\n" @

                                "<L1><JL><F0>Welcome to the SpyMod Client Script options screen. You can access this screen any " @
                                "time by pressing <F2>Alt+F12<F0>. Let's start with an overview of the features of " @
                                "SpyMod Client Script (SMCS), and then move on to the various options." @ "\n\n" @
                                "<L3>- 2 seperate control schemes, so you can keep your custom binds to key 1-5 and still " @
                                "play SpyMod" @ "\n\n" @

                                "- The ability to save ingame preferences (weapon modes, selected grenade type)" @ "\n\n" @
                                "- Password management, which includes the automatic downloading and saving of your spymod " @
                                "password(s), and automatic login" @ "\n\n" @
                                "- SpyMod kill counter. Works only in SpyMod. Doesn't interfere with other kill counters. " @
                                "100% accurate." @ "\n\n" @
                                "- Custom binds to SpyMod commands, such as \"Use weapon\\gadget X\" or \"Drop grenade X\"" @ "\n\n" @
                                "- \"Add buddy\" function ingame, so you can add a buddy to your buddy list without having to remember " @
                                "all of the tags and weird characters in his\\her name." @ "\n\n" @

                                "<L1><JC><F1>Options screen overview" @ "\n\n" @
                                "<JL><F0>The options screen is divided into 3 sections: General Options, Kill Counter " @
                                "Options, and Custom Bind Options." @ "\n\n" @

                                "<L2><F1>General Options" @ "\n\n" @

                                "<L3><F1>SpyMod Controls\\Normal Controls:<F0> Select your control scheme here. SpyMod Controls " @
                                "defaults keys 1-5 to their normal functions so that they function properly in SpyMod. Normal " @
                                "controls are left unmodified from before you installed SMCS, so if you bound keys 1-5 to " @
                                "something other than the default, they will retain their function. In order to modify either control " @
                                "scheme, first activate the scheme you want to modify. Then, go to the normal Tribes options screen, and " @
                                "make whatever changes you like. These changes will be saved in the currently active control scheme." @ "\n\n" @

                                "<L3><F1>Player Passwords:<F0> This is a list of all of your player names and their associated SpyMod passwords, " @
                                "if they have one. To change a password associated with a certain player, select the player " @
                                "name from the drop-down box, type the new password into the text box to the right, and press " @
                                "enter. To erase a password, select the player name, erase anything in the text box, and press " @
                                "enter." @ "\n\n" @

                                "<L2><F1>Kill Counter Options" @ "\n\n" @

                                "<L3><F1>Display kill\\death\\teamkill messages:<F0> Changes whether or not you want SMCS " @
                                "to announce each of your kills\\deaths\\teamkills. SMCS will still count your kills\\deaths\\teamkills " @
                                "even if these options are turned off, but it won't display messages about them." @ "\n" @
                                "<F1>Kill\\death\\teamkill count box:<F0> This box, the one right below the display message " @
                                "checkboxes, shows your total kills\\deaths\\teamkills from the time you installed SMCS until " @
                                "now." @ "\n\n" @

                                "<L2><F1>Custom Bind Options" @ "\n\n" @

                                "<L3><F1>Custom Bind 1\\2\\3:<F0> Here you can select what each of the 3 custom binds do. Note " @
                                "that the custom binds are only recorded in the SpyMod Controls scheme, and therefore won't conflict " @
                                "with anything bound to these keys in your Normal Control scheme." @ "\n\n"
                    );

  Control::setText(SpyModVersion, getWord($SpyModClientScript::CLIENT_VERSION, 0) @ "." @
                                  getWord($SpyModClientScript::CLIENT_VERSION, 1) @ "." @
                                  getWord($SpyModClientScript::CLIENT_VERSION, 2));
}

////////////////////////////////////////////////////////////////////////
// INITIALIZATION CODE
////////////////////////////////////////////////////////////////////////

if (!$smcsInstalled) $smcsfirstRun = true;

function SpyMod::showStartupMessage(%msg, %showOptions, %popup) {
  if (!$dedicated) {
    if (%popup) {
      if (isObject(MainWindow)) {
        if (%showOptions) SpyMod::showOptions();
        GuiPopDialog(MainWindow, 0);
        GuiPushDialog(MainWindow, "gui\\MessageDialog.gui");
        Control::setValue(MessageDialogTextFormat, %msg);

        if (isObject(ConsoleScheduler)) deleteObject(ConsoleScheduler2);
        $pref::skipIntro = $skipIntro;

        echo("POPUP: " @ %msg);
      } else {
        $popupTries++;
        if ($popupTries < 4) schedule("SpyMod::showStartupMessage(\"" @ escapeString(%msg) @ "\", \"" @ %showOptions @ "\", true);", 2);
      }
    } else {
      if (!isObject(ConsoleScheduler)) newObject(ConsoleScheduler2, SimConsoleScheduler);
      $skipIntro = $pref::skipIntro;
      $pref::skipIntro = true;
      schedule("SpyMod::showStartupMessage(\"" @ escapeString(%msg) @ "\", \"" @ %showOptions @ "\", true);", 1);
    }
  }
}

// Initializes any missing config files
function SpyMod::initFiles() {
  %missingFiles = "";

  // Replace spymodSetting.cs if needed
  if (getPathOf("spymodSettings.cs") != "config") {
    $SpyMod::SpyModControls = false;
    $SpyMod::AutoSwitchControls = false;
    $SpyMod::killCounter::kills = 0;
    $SpyMod::killCounter::deaths = 0;
    $SpyMod::killCounter::teamkills = 0;

    $SpyMod::customBind[1] = 0;
    $SpyMod::customBind[2] = 1;
    $SpyMod::customBind[3] = 2;
    SpyMod::exportSettings();
    $ConsoleWorld::DefaultSearchPath = $ConsoleWorld::DefaultSearchPath;

    if (%missingFiles != "") %missingFiles = %missingFiles @ "\n";
    %missingFiles = %missingFiles @ "config\\spymodSettings.cs";
  }

  // Replace configNormal.cs if needed
  if (getPathOf("configNormal.cs") != "config") {
    File::copy("config\\config.cs", "config\\configNormal.cs");
    $ConsoleWorld::DefaultSearchPath = $ConsoleWorld::DefaultSearchPath;

    if (%missingFiles != "") %missingFiles = %missingFiles @ "\n";
    %missingFiles = %missingFiles @ "config\\configNormal.cs";
  }

  // Replace configSpymod.cs if needed
  if (getPathOf("configSpymod.cs") != "config") {
    exec("configNormal.cs");
    editActionMap("playMap.sae");
    bindCommand(keyboard0, make, "1", TO, "use(\"Blaster\");");
    bindCommand(keyboard0, make, "2", TO, "use(\"Plasma Gun\");");
    bindCommand(keyboard0, make, "3", TO, "use(\"Chaingun\");");
    bindCommand(keyboard0, make, "4", TO, "use(\"Disc Launcher\");");
    bindCommand(keyboard0, make, "5", TO, "use(\"Grenade Launcher\");");
    bindCommand(keyboard0, make, "6", TO, "use(\"Laser Rifle\");");
    bindCommand(keyboard0, make, "7", TO, "use(\"ELF gun\");");
    bindCommand(keyboard0, make, "8", TO, "use(\"Mortar\");");
    bindCommand(keyboard0, make, "9", TO, "use(\"Targeting Laser\");");
    bindCommand(keyboard0, make, control, "1", TO, "SpyMod::customBind(1);");
    bindCommand(keyboard0, make, control, "2", TO, "SpyMod::customBind(2);");
    bindCommand(keyboard0, make, control, "3", TO, "SpyMod::customBind(3);");
    saveActionMap("config\\configSpyMod.cs", "actionMap.sae", "playMap.sae", "pdaMap.sae");
    $ConsoleWorld::DefaultSearchPath = $ConsoleWorld::DefaultSearchPath;

    if (%missingFiles != "") %missingFiles = %missingFiles @ "\n";
    %missingFiles = %missingFiles @ "config\\configSpymod.cs";
  }

  return %missingFiles;
}

function SpyMod::installationCheck() {
  // We will assume that this file exists :)

  // Ensure that spymodOptions.gui exists
  if (getPathOf("spymodOptions.gui") != "config") {
    if (%missingFiles != "") %missingFiles = %missingFiles @ "\n";
    %missingFiles = %missingFiles @ "config\\spymodOptions.gui";
  }

  // Ensure that spymodScriptDSC.dsc exists
  if (getPathOf("spymodScriptDSC.dsc") != "base\\missions") {
    if (%missingFiles != "") %missingFiles = %missingFiles @ "\n";
    %missingFiles = %missingFiles @ "base\\missions\\spymodScriptDSC.dsc";
  }

  return %missingFiles;
}

function SpyMod::startup() {
  if ($smcsFirstRun) {
    if ((%missingFiles = SpyMod::installationCheck()) != "") {
      echo();
      echo("ERROR: Your SpyMod Client Script installation is missing the following files:");
      echo(%missingFiles);
      echo("Please close Tribes, ensure that you have properly copied these files into their required locations, and start Tribes again");
      echo("If this problem persists, contact I AM BOB (AIM, xfire: iambob314, SpyMod forums: spymod.proboards57.com)");
      echo();
      SpyMod::showStartupMessage("<jc><f2>------- SpyMod Client Script -------\n\n" @
                                 "<f0>SpyMod Client Script is missing the following files:\n\n" @
                                 "<f1>" @ %missingFiles @ "\n\n" @
                                 "<f0>Please close Tribes, ensure that you have properly copied these files into their " @
                                 "required locations, and start Tribes again\n\n" @
                                 "<f1>If this problem persists, contact I AM BOB\n(AIM, xfire: iambob314, SpyMod forums: spymod.proboards57.com)");
      return;
    }

    // Back up the config.cs file just in case
    File::copy("config\\config.cs", "config\\configBackup.cs");
    $ConsoleWorld::DefaultSearchPath = $ConsoleWorld::DefaultSearchPath;

    SpyMod::initFiles();
  } else {
    if ((%missingFiles = SpyMod::initFiles()) != "") {
      echo();
      echo("The following config files were missing and had to be regenerated:");
      echo(%missingFiles);
      echo("If you just installed SpyMod Client Script, ignore this message");
      echo("Otherwise, you may have lost some settings");
      echo();
      SpyMod::showStartupMessage("<jc><f2>------- SpyMod Client Script -------\n\n" @
                                 "<f0>The following config files were missing and\n" @
                                 "had to be regenerated:\n\n" @
                                 "<f1>" @ %missingFiles @ "\n\n" @
                                 "<f0>If you just installed SpyMod Client Script,\n" @
                                 "ignore this message. Otherwise, you may\n" @
                                 "have lost some settings.");
    }
  }

  exec("spymodSettings.cs");

  if ($SpyMod::autoSwitchControls) turnOnAutoSwitchControls();

  // Whatever the last control scheme was, it was saved into config.cs on quit,
  // so lets update the appropriate config file
  if ($SpyMod::spyModControls) {
    File::copy("config\\config.cs", "config\\configSpymod.cs");
    useSpyModControls();
  } else {
    File::copy("config\\config.cs", "config\\configNormal.cs");
    useNormalControls();
  }

  // Bind Alt-F12 to show the SpyMod Client Script options page
  bind(keyboard, make, alt, "f12", to, "SpyMod::options();");

  if ($smcsFirstRun) {
    echo();
    echo("SpyMod Client Script has been successfully installed");
    echo("You may now configure SpyMod Client Script.");
    echo("Press Alt-F12 to bring up SpyMod Client Script options again later");
    echo("Thank you for using the SpyMod Client Script!");
    echo();
    SpyMod::showStartupMessage("<jc><f2>------- SpyMod Client Script -------\n\n" @
                               "<f0>SpyMod Client Script has been installed!\n" @
                               "You may configure any options on this screen\n" @
                               "Press Alt-F12 to bring up this options screen later\n\n" @
                               "<f1>Thank you for using SpyMod Client Script!", true);

    // Update the .dsc file to reflect that it is no longer the first run
    $smcsInstalled = true;
    export("$smcsInstalled", "base\\missions\\spymodScriptDSC.dsc", true);
  }
}

SpyMod::startup();