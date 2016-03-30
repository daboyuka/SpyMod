function Client::addMenuItem(%clientId, %option, %code) {
  remoteEval(%clientId, "AddMenuItem", %option, %code);

  %clientId.curMenuItem[%clientId.curMenuItemBuffer, %clientId.curMenuItemIndex[%clientId.curMenuItemBuffer]] = %code;
  %clientId.curMenuItemIndex[%clientId.curMenuItemBuffer]++;
}

function Client::buildMenu(%clientId, %menuTitle, %menuCode, %cancellable) {
   Client::setMenuScoreVis(%clientId, true);
   %clientId.menuLock = !%cancellable;
   %clientId.menuMode = %menuCode;

   if (%clientId.curMenuItemBuffer == "") %clientId.curMenuItemBuffer = 0;

   %clientId.curMenuItemBuffer ^= 1;
   %clientId.curMenuItemIndex[%clientId.curMenuItemBuffer] = 0;
   remoteEval(%clientId, "NewMenu", %menuTitle);
}

function remoteMenuSelect(%clientId, %code) {
  %mm = %clientId.menuMode;
  if(%mm == "")
     return;

  if(String::findSubStr(%code, "\"") != -1 ||
     String::findSubStr(%code, "\\") != -1)
     return;

  %hack = true;
  %curBuf = %clientId.curMenuItemBuffer;
  for (%i = 0; %i < %clientId.curMenuItemIndex[%curBuf]; %i++) {
    if (String::ICompare(%clientId.curMenuItem[%curBuf, %i], %code) == 0) { %hack = false; break; }
  }
  if (%hack) {
    for (%i = 0; %i < %clientId.curMenuItemIndex[%curBuf^1]; %i++) {
      if (String::ICompare(%clientId.curMenuItem[%curBuf^1, %i], %code) == 0) { return; }
    }
  }
  if (%hack) {
    H4XX0R::hackerDetected(%clientId, "Invalid menu item value: " @ String::getSubStr(%code, 0, 60), $H4XX0RPunishment::menuHack);
    return;
  }

  %evalString = "processMenu" @ %mm @ "(" @ %clientId @ ", \"" @ %code @ "\");";
  %clientId.menuMode = "";
  %clientId.menuLock = "";
  eval(%evalString);
  if(%clientId.menuMode == "") {
     Client::setMenuScoreVis(%clientId, false);
     %clientId.selClient = "";
  }
}

function Client::isReadyForNewMenu(%clientId) {
  return !%clientId.usingMenu;
}

function Client::usedMenu(%clientId) {
  %clientId.usingMenu = true;
  schedule(%clientId @ ".usingMenu = false;", 0.125, %clientId);
}