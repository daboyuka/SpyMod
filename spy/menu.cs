function Client::cancelMenu(%clientId)
{
   if(!%clientId.menuLock)
   {
      %clientId.selClient = "";
      %clientId.menuMode = "";
      %clientId.menuLock = "";
      remoteEval(%clientId, "CancelMenu");
      Client::setMenuScoreVis(%clientId, false);
   }
}

// Client::buildMenu and Client::addMenuItem are in menuMods.cs

function remoteCancelMenu(%server)
{
   if(%server != 2048)
      return;
   if(isObject(CurServerMenu))
      deleteObject(CurServerMenu);
}

function remoteNewMenu(%server, %title)
{
   if(%server != 2048)
      return;

   if(isObject(CurServerMenu))
      deleteObject(CurServerMenu);

   newObject(CurServerMenu, ChatMenu, %title);
   setCMMode(PlayChatMenu, 0);
   setCMMode(CurServerMenu, 1);
}

function remoteAddMenuItem(%server, %title, %code)
{
   if(%server != 2048)
      return;
   addCMCommand(CurServerMenu, %title, clientMenuSelect, %code);
}

function clientMenuSelect(%code)
{
   deleteObject(CurServerMenu);
   remoteEval(2048, menuSelect, %code);
}

// remoteMenuSelect in menuMods.cs

exec(menuMods);