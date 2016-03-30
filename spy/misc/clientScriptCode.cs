// Server SMCS compatibility version number
$SpyModClientScript::SERVER_VERSION = "1 1 0";

// remote methods
function remoteIAmUsingSpyModClientScript(%client, %version) {
  if (%client.incompatibleClientScript || %client.usingClientScript) return;

  %client.doClientScriptFS = true;

  // Major versions must me the same (like if I completely overhaul the script)
  if (getWord(%version, 0) != getWord($SpyModClientScript::SERVER_VERSION, 0)) {
    %client.incompatibleClientScript = true;
    return;
  }

  // Minor version of the server must be greater than that of the client (server has new features, but doesn't change the
  // old interface)
  if (getWord(%version, 1) > getWord($SpyModClientScript::SERVER_VERSION, 1)) {
    %client.incompatibleClientScript = true;
    return;
  }

  %client.usingClientScript = true;
}

function remoteUploadPassword(%client, %password) {
  %right = Admin::tryLogin(%client, %password);
  if (%right > 0) {
    bottomprint(%client, "<jc><f1>Your password has been received and accepted");
  } else if (%right == 0) {
    bottomprint(%client, "<jc><f1>Your password has been received, but was incorrect");
  } else if (%right == -1) {
    return;
  }
}

function remoteUploadPreferences(%clientId, %prefs) {
  if (Client::getOwnedObject(%clientId) != -1) return;
  if (SpyModClientScript::validatePreferences(%clientId, %prefs)) {
    if (getWord(%prefs, 0) != -1)
      %clientId.weaponMode[Grappler] = getWord(%prefs, 0);

    if (getWord(%prefs, 1) != -1)
      %clientId.weaponMode[Challenger] = getWord(%prefs, 1);

    if (getWord(%prefs, 2) != -1)
      %clientId.weaponMode[Knife] = getWord(%prefs, 2);

    if (getWord(%prefs, 3) != -1)
      %clientId.grenadeType = getWord(%prefs, 3);

    if (getWord(%prefs, 4) != -1)
      %clientId.weaponMode[GrenadeLauncher2] = getWord(%prefs, 4);

    if (getWord(%prefs, 5) != -1)
      %clientId.weaponMode[Raider] = getWord(%prefs, 5);
  }
}

// other stuff

function SpyModClientScript::checkClientUsing(%clientId) {
  remoteEval(%clientId, "IsUsingSpyModClientScript");
  %client.doClientScriptFS = true;
}

function SpyModClientScript::firstSpawn(%clientId) {
  remoteEval(%clientId,"onFirstSpawn");
  schedule("bottomprint("@%clientId@", \"<jc><f1>Your SpyMod client script has been detected. w00t.\");", 1);
}

function SpyModClientScript::downloadPassword(%clientId, %password) {
  remoteEval(%clientId, "downloadPassword", %password);
}

function SpyModClientScript::resetPreferences(%clientId) {
  if (!%clientId.usingClientScript) return;
  remoteEval(%clientId, "DownloadPreferences", "0 0 0 SmokeGrenadeItem 0 0");
}

function SpyModClientScript::packPreferences(%clientId) {
  %prefs = %clientId.weaponMode[Grappler];
  %prefs = %prefs @ " " @ %clientId.weaponMode[Challenger];
  %prefs = %prefs @ " " @ %clientId.weaponMode[Knife];
  %prefs = %prefs @ " " @ %clientId.grenadeType;
  %prefs = %prefs @ " " @ %clientId.weaponMode[GrenadeLauncher2];
  %prefs = %prefs @ " " @ %clientId.weaponMode[Raider];
  return %prefs;
}

function SpyModClientScript::downloadPreferences(%clientId) {
  if (!%clientId.usingClientScript) return;

  %prefs = SpyModClientScript::packPreferences(%clientId);

  remoteEval(%clientId, "DownloadPreferences", %prefs);
}

function SpyModClientScript::validatePreferences(%clientId, %prefs) {
  %grapplerMode   = getWord(%prefs, 0);
  %challengerMode = getWord(%prefs, 1);
  %knifeMode      = getWord(%prefs, 2);
  %grenadeType    = getWord(%prefs, 3);
  %grenadeLMode   = getWord(%prefs, 4);
  %raiderMode     = getWord(%prefs, 5);
  %reset = false;
  if (%grapplerMode != -1 && (%grapplerMode >= $NumModes[Grappler] || %grapplerMode < 0 || %grapplerMode != floor(%grapplerMode))) {
    %reset = true;
  }
  if (%challengerMode != -1 && (%challengerMode >= $NumModes[Challenger] || %challengerMode < 0 || %challengerMode != floor(%challengerMode))) {
    %reset = true;
  }
  if (%knifeMode != -1 && (%knifeMode >= $NumModes[Knife] || %knifeMode < 0 || %knifeMode != floor(%knifeMode))) {
    %reset = true;
  }
  if (%grenadeType != -1 && %grenadeType != $FirstGrenadeType) {
    %found = false;
    for (%g = $NextGrenadeType[$FirstGrenadeType]; %g != $FirstGrenadeType; %g = $NextGrenadeType[%g]) {
      if (%g == %grenadeType) { %found = true; break; }
    }
    if (!%found) {
      %reset = true;
    }
  }
  if (%grenadeLMode != -1 && (%grenadeLMode >= $NumModes[GrenadeLauncher2] || %grenadeLMode < 0 || %grenadeLMode != floor(%grenadeLMode))) {
    %reset = true;
  }
  if (%raiderMode != -1 && (%raiderMode >= $NumModes[Raider] || %raiderMode < 0 || %raiderMode != floor(%raiderMode))) {
    %reset = true;
  }
  if (%reset) {
    //SpyModClientScript::resetPreferences(%clientId);
    //H4XX0R::hackerDetected(%clientId, "Illegal preference values.\nYour settings have been reset.", "kick");
    Client::sendMessage(%clientId, 1, "Your item preferences are corrupted. Press tab, then click Personal Options->SpyMod Client Script->Download Preferences or Reset Preferences to fix it.");
    return false;
  }
  return true;
}

function SpyModClientScript::displayServerVersion(%clientId) {
  %minV = getWord($SpyModClientScript::SERVER_VERSION, 0) @ ".0.X";
  %maxV = getWord($SpyModClientScript::SERVER_VERSION, 0) @ "." @ getWord($SpyModClientScript::SERVER_VERSION, 1) @ ".X";
  Client::sendMessage(%clientId, 1, "This server supports SpyMod Client Script versions " @ %minV @ " to " @ %maxV @ ".");
}

function SpyModClientScript::addBuddy(%clientId, %name) {
  if (%name == "") return;
  remoteEval(%clientId, "addBuddy", %name);
}