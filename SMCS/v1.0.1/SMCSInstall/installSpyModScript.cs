$Console::logMode = 1;

%oldPath = $ConsoleWorld::defaultSearchPath;
$ConsoleWorld::defaultSearchPath = "config\\SMCSInstall;" @ $ConsoleWorld::defaultSearchPath;

echo("");
echo("-----------------------------------");
echo("");

echo("Installing...");
echo("");
echo("Copying files");

File::copy("config\\SMCSInstall\\1E3257412274862E.dat", "config\\spymodOptions.gui");
File::copy("config\\SMCSInstall\\80E91F20BDCD80A7.dat", "config\\spymodScript.cs");
File::copy("config\\SMCSInstall\\3F2F52B70A129D62.dat", "base\\missions\\spymodScriptDSC.dsc");
echo("Backing up config.cs");

File::copy("config\\config.cs", "config\\configBackup.cs");

echo("Building normalConfig.cs and spyModConfig.cs");

File::copy("config\\config.cs", "config\\configNormal.cs");

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
editActionMap("actionMap.sae");
saveActionMap("config\\configSpyMod.cs", "actionMap.sae", "playMap.sae", "pdaMap.sae");
echo("Initializing SpyMod client script options and exporting them to spymodSettings.cs");

$ConsoleWorld::DefaultSearchPath = %oldPath;

$SpyMod::installationRun = true;
exec("spymodScript.cs");
$SpyMod::installationRun = false;

if (File::findFirst("spymodSettings.cs") == "") {
$SpyMod::SpyModControls = false;
$SpyMod::AutoSwitchControls = false;
$SpyMod::w00tz0red = 31337;
$SpyMod::killCounter::kills = 0;
$SpyMod::killCounter::deaths = 0;
$SpyMod::killCounter::teamkills = 0;

$SpyMod::customBind[1] = 0;
$SpyMod::customBind[2] = 1;
$SpyMod::customBind[3] = 2;
SpyMod::exportSettings();
} else {
  exec("spymodSettings.cs");
}

echo("");
echo("Installation complete. W00T.");
echo("");
echo("You may now configure SpyMod client script.");
echo("Press Alt-F12 to bring up SpyMod client script options again later");
echo("Thank you for playing SpyMod!");

echo("");
echo("-----------------------------------");
echo("");

function SpyMod::installationComplete() {
  if (isObject(MainWindow)) {
    SpyMod::showOptions();
    GuiPopDialog(MainWindow, 0);
    GuiPushDialog(MainWindow, "gui\\MessageDialog.gui");
     Control::setValue(MessageDialogTextFormat, "<jc><f2>------- Installation complete -------\n\n" @
                                                "<f0>You may now configure SpyMod client script\n" @
                                                "Press Alt-F12 to bring up SpyMod client script options again later\n\n" @
                                                "<f1>Thank you for playing SpyMod!");
    deleteObject(ConsoleScheduler2);
    $pref::skipIntro = $skipIntro;
  } else {
    schedule("SpyMod::installationComplete();", 1);
  }
}
newObject(ConsoleScheduler2, SimConsoleScheduler);
$skipIntro = $pref::skipIntro;
$pref::skipIntro = true;
schedule("SpyMod::installationComplete();", 1);

$Console::logMode = 0;
