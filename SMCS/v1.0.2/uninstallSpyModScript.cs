echo("");
echo("-------------------------------------------------------------");
echo("");
echo("Uninstalling...");
echo("");

echo("Restoring old control configuration");
echo("");
File::copy("config\\configNormal.cs", "config\\config.cs");

echo("Deleting SpyMod client script files");
echo("");

File::delete("config\\configNormal.cs");
File::delete("config\\configSpyMod.cs");
//File::delete("config\\spymodSettings.cs");
File::delete("config\\spymodOptions.gui");
File::delete("config\\spymodScript.cs");
File::delete("base\\missions\\spymodScriptDSC.dsc");

//------------------------------------------------------------------------------------------
echo("Uninstallation complete. :'(");
echo("");
echo("All files except a control configuration backup have been removed.");
echo("Tribes will close in 5 seconds to complete the uninstallation.");
echo("");
echo("-------------------------------------------------------------");
echo("");

function SpyMod::uninstallationComplete() {
  if (isObject(MainWindow)) {
    GuiPopDialog(MainWindow, 0);
    GuiPushDialog(MainWindow, "gui\\MessageDialog.gui");
    Control::setValue(MessageDialogTextFormat, "<jc><f2>------- Uninstallation complete -------\n\n" @
                                               "<f0>All files except a control configuration backup have been removed.\n\n" @
                                               "Tribes will close in 5 seconds to complete the uninstallation");
    deleteObject(ConsoleScheduler2);
    $pref::skipIntro = $skipIntro;
    schedule("quit();", 5);
  } else {
    schedule("SpyMod::installationComplete();", 1);
  }
}
newObject(ConsoleScheduler2, SimConsoleScheduler);
$skipIntro = $pref::skipIntro;
$pref::skipIntro = true;
schedule("SpyMod::uninstallationComplete();", 2);
//------------------------------------------------------------------------------------------
