//
// The mission list is the list of mission displayed to the players
// and admins in the server menu, used on the client side to display
// mission avialable on the Host Server screen, and is is also used
// to initialize the nextMission array which controls which mission
// to load after a mission finishes.
//
// function MissionList::clear();
//
// function MissionList::addMission(%missionName);
//
//    Add a mission to the current mission list.
//    Mission information is extracted from the mission .dsc file.
//    The mission file does not need to be in the base/missions dir.
//
// function MissionList::build()
//
//    Add any missions files that match the missions/*.dsc pattern.
//    This will include any missions in base/misssions/* or missions
//    in mods as long as they are in a missions subdirectory.
//
// function MissionList::initNextMission()
//
//    Initialize the nextMission array based on the current mission list.
//    When a mission changes it references the nextMission array to determin
//    which mission to load next.  The function initializes the array based
//    on mission type, so missions don't cycle to a mission of a different
//    type.
//    The array is easy to initialize manually:
//       $nextMission["CrissCross"] = "Raindance";
//       $nextMission["Raindance"] = "CrissCross";
//

function MissionList::clear()
{
   $MLIST::Count = 0;
   $MLIST::TypeCount = 1;
   $MLIST::Type[0] = "All Types";
   $MLIST::MissionList[0] = "";
}  

function MissionList::addMission(%mission) {
   $MDESC::Type = "";
   $MDESC::Name = "";
   $MDESC::Text = "";
   $MDESC::MinMaxPlayers = "";

   if (String::findSubStr(%mission,".dsc") == -1)
      %mission = %mission @ ".dsc";
   exec(%mission);

   if($MDESC::Type == "")
      return false;

   for(%i = 0; %i < $MLIST::TypeCount; %i++) {
      if($MLIST::Type[%i] == $MDESC::Type)
         break;
   }
   if(%i == $MLIST::TypeCount) {
      $MLIST::Type[%i] = $MDESC::Type;
      $MLIST::TypeCount++;
      $MLIST::MissionList[%i] = "";
   }
   %ct = $MLIST::Count;
   $MLIST::Count++;

   $MLIST::EType[%ct] = $MDESC::Type;
   $MLIST::EName[%ct] = File::getBase(%mission);
   $MLIST::EText[%ct] = $MDESC::Text;
   $MLIST::EMinMaxPlayers[%ct] = $MDESC::MinMaxPlayers;
   if($MDESC::Type != "Training")
      $MLIST::MissionList[0] = %ct @ " " @ $MLIST::MissionList[0];
   $MLIST::MissionList[%i] = %ct @ " " @ $MLIST::MissionList[%i];

   $MLIST::missionNumber[$MLIST::EName[%ct]] = %ct;

   return true;
}

function MissionList::build() {
   MissionList::clear();

   for (%f = File::findFirst("MissionPack*.vol"); %f != ""; %f = File::findNext("MissionPack*.vol")) {
     %vn = File::getBase(%f)@"Volume";
     if (!isObject(%vn)) { newObject(%vn, SimVolume, %f); echo("Loading " @ %f); }
   }

   %file = File::findFirst("missions\\*.dsc");
   while(%file != "") {
      MissionList::addMission(%file);
      %file = File::findNext("missions\\*.dsc");
   }
}

function MissionList::alphabetizeMissionTypes() {
  %notInOrder = true;
  for (%z = 0; %z < 1000 && %notInOrder; %z++) {
    %notInOrder = false;
    for (%i = 1; %i < $MLIST::TypeCount - 1; %i++) { // Don't touch the "All Types" type (besides, it's probably going to be at the top alphabetically anyway...)
      if (String::ICompare($MLIST::Type[%i], $MLIST::Type[%i+1]) > 0) {
        %tt = $MLIST::Type[%i+1];
        %tm = $MLIST::MissionList[%i+1];
        $MLIST::Type[%i+1] = $MLIST::Type[%i];
        $MLIST::MissionList[%i+1] = $MLIST::MissionList[%i];
        $MLIST::Type[%i] = %tt;
        $MLIST::MissionList[%i] = %tm;
        %notInOrder = true;
      }
    }
  }

  if (!%notInOrder) echo("MISSION TYPES ALPHABETIZED");
}

function MissionList::initNextMission() {
   for(%type = 1; %type < $MLIST::TypeCount; %type++) {
      %prev = getWord($MLIST::MissionList[%type], 0);
      %ml = $MLIST::MissionList[%type] @ %prev;
      %prevName = $MLIST::EName[%prev];
      for(%i = 1; (%mis = getWord(%ml, %i)) != -1; %i++) {
         %misName = $MLIST::EName[%mis];
         $nextMission[%prevName] = %misName;
         %prevName = %misName;
      }
   }
}

function MissionList::execMissionModFiles() {
  for (%f = File::findFirst("missions\\MissionPack*Mods.cs"); %f != ""; %f = File::findNext("missions\\MissionPack*Mods.cs")) {
    exec(%f);
  }
}

// Go ahead and build the list
%f = File::findFirst("missions\\missions.vol");
for (%i == 0; %f != ""; %i++) {
  newObject("MissionVolume"@%i, SimVolume, %f);
  %f = File::findNext(%f);
}

MissionList::build();
MissionList::alphabetizeMissionTypes(); // YAY!
MissionList::initNextMission();
