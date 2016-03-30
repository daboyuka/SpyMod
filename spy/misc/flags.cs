$Flags::FLAG_BANNED = 1;

$Flags::FLAGS[0] = $Flags::FLAG_BANNED;

$Flags::FLAG_NAMES[$Flags::FLAG_BANNED]  = "Banned";

$Flags::ALL_FLAGS = $Flags::FLAG_BANNED;

function Flags::getNames(%flags) {
  %str = "";
  for (%i = 0; $Flags::FLAGS[%i] != ""; %i++) {
    %p = $Flags::FLAGS[%i];
    if (%flags & %p) %str = %str @ tern(%str == "", $Flags::FLAG_NAMES[%p], ", " @ $Flags::FLAG_NAMES[%p]);
  }
  if (%str == "") %str = "None";
  return %str;
}

function Flags::areValidFlags(%flags) {
  if (((%flags & $Flags::ALL_FLAGS) != %flags) || %flags < 0 || floor(%flags) != %flags) return false;
  return true;
}