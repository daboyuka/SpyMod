exec(editorScripts);

function GetFieldButton::onAction() {
  focusServer();

  %field = Control::getValue(FieldName);
  %value = eval("%x = ($ME::InspectObject)." @ %field @ ";");
  echo(%field, ",", %value);
  echo("%x = ($ME::InspectObject)." @ %field @ ";");
  echo($ME::InspectObject);
  Control::setValue(FieldValue, %value);

  focusClient();
}

function SetFieldButton::onAction() {
  focusServer();

  %field = Control::getValue(FieldName);
  %value = Control::getValue(FieldValue);
  eval("($ME::InspectObject)." @ %field @ " = \"" @ escapeString(%value) @ "\";");

  focusClient();
}

function METoggleFieldEditor() {
  Control::setVisible(FieldEditor, ($ME::FieldEditorVisible = !Control::getVisible(FieldEditor)));
}

function MEGameMode(%doRebuild) {
  if (%doRebuild != false)
    ME::RebuildCommandMap();
  unfocus(TedObject);
  unfocus(MissionEditor);
  unfocus(editCamera);
  GuiLoadContentCtrl(MainWindow, "gui\\play.gui");
  cursorOff(MainWindow);
  focus(playDelegate);
}

function MEHide() {
  if(!isObject(EditorGui))
    GuiLoadContentCtrl("MainWindow", "spy\\editor.gui");

  focus(editCamera);
  postAction(EditorGui, Attach, editCamera);

  Control::setVisible("TedBar", false);
  Control::setVisible("MEObjectList", false);
  Control::setVisible("Inspector", false);
  Control::setVisible("Creator", false);
  Control::setVisible("SaveBar", false);
  Control::setVisible("LightingBox", false);
  Control::setVisible("FieldEditor", false);
  MEHideOptions();
  MEHideHelp();
}

function MEMode() {
   $ME::InspectObject = -1;
   if($ME::loaded != true)
      ME::init();
   
   if(!isObject(editCamera))
      return;

   $Server::timeLimit = 0;
   if(!isObject(EditorGui))
      GuiLoadContentCtrl("MainWindow", "spy\\editor.gui");

   if(!$ME::Loaded)
      return;

   MEHideOptions();
   MEHideHelp();
   
   MissionObjectList::ClearDisplayGroups();
   MissionObjectList::AddDisplayGroup(1, "MissionGroup");
   MissionObjectList::AddDisplayGroup(1, "MissionCleanup");
   MissionObjectList::SetSelMode(1);
   MEShowInspector();
   ME::GetConsoleOptions();

   Control::setVisible("FieldEditor", $ME::FieldEditorVisible);
}

function MissionObjectList::onUnselected(%world, %obj)
{
   if(%obj == $ME::InspectObject && %world == $ME::InspectWorld)
   {
      MissionObjectList::Inspect(1, -1);
      $ME::InspectObject = "";
   }

   focusServer();
   if (!isObject(MESelectionSet)) newObject(MESelectionSet, SimSet);
   removeFromSet(MESelectionSet, %obj);
   focusClient();

   ME::onUnselected( %world, %obj );
}

function MissionObjectList::onSelectionCleared()
{
   MissionObjectList::Inspect(1, -1);
   $ME::InspectObject = "";


   focusServer();
   if (!isObject(MESelectionSet)) newObject(MESelectionSet, SimSet);
   else {
     while (Group::objectCount(MESelectionSet) > 0) removeFromSet(MESelectionSet, Group::getObject(MESelectionSet, 0));
   }
   focusClient();

   ME::onSelectionCleared();
}

function MissionObjectList::onSelected(%world, %obj)
{
   if($ME::InspectObject == "")
   {
      $ME::InspectObject = %obj;
      $ME::InspectWorld = %world;
      MissionObjectList::Inspect($ME::InspectWorld, %obj);
      
      // it's on the server:
      focusServer();
      %locked = %obj.locked;
      focusClient();

      if(%locked)
         Control::setText("LockButton", "Unlock");
      else
         Control::setText("LockButton", "Lock");
   }

   focusServer();
   if (!isObject(MESelectionSet)) newObject(MESelectionSet, SimSet);
   addToSet(MESelectionSet, %obj);
   focusClient();
   
   ME::onSelected( %world, %obj );
}

function MENudge(%dir) {
	if ($ME::InspectObject == "") return;
	
	if (%dir == left)       %delta = -$ME::XGridSnap @ " 0 0";
	else if (%dir == right) %delta = $ME::XGridSnap @ " 0 0";
	else if ($ME::Mod1) { // ctrl
		if (%dir == up)        %delta = "0 0 " @ $ME::ZGridSnap;
		else if (%dir == down) %delta = "0 0 " @ -$ME::ZGridSnap;
	} else {		
		if (%dir == up)        %delta = "0 " @ $ME::YGridSnap @ " 0";
		else if (%dir == down) %delta = "0 " @ -$ME::YGridSnap @ " 0";
	}

	if ($ME::Mod2) %delta = Vector::mul(%delta, 10); // shift
	
	focusServer();
	Group::iterateRecursive(MESelectionSet, GameBase::addPosition, %delta);
	focusClient();
}