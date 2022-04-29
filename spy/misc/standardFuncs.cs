$PI = 3.141592653589793238;

function error(%fmt, %a0, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8) {
	%err = sprintf(%fmt, %a0, %a1, %a2, %a3, %a4, %a5, %a6, %a7); // Note: sprintf takes a maximum of 8 extra args, despite help message
	dbecho(0, "ERROR: " @ %err);
	trace();
}

function assert(%cond, %fmt, %a0, %a1, %a2, %a3, %a4, %a5, %a6, %a7) {
	if (!%cond) error(%fmt, %a0, %a1, %a2, %a3, %a4, %a5, %a6, %a7);
	return %cond;
}

function abs(%x) {
  if (%x >= 0) return %x;
  else return -%x;
}

function min(%a, %b) {
  if (%a < %b) return %a;
  else return %b;
}

function max(%a, %b) {
  if (%a > %b) return %a;
  else return %b;
}

function ceil(%x) {
  %f = floor(%x);
  if (%f == %x) return %f;
  else          return %f + 1;
}

function tern(%v, %a, %b) {
  if (%v) return %a;
  else    return %b;
}

function defval(%v, %def) {
	if (%v == "") return %def;
	else          return %v;
}

function gint(%x) {
  %f = floor(%x);
  if (%x >= 0 || %x == %f) return %f;
  else return %f - 1;
}

function randint(%n) {
	return floor(getRandom() * %n);
}

$TaylorSeries = true;
if ($TaylorSeries) {
  // |Error| <= ($PI/2)^11/11! ~= 3.6e-6
  function sin(%x) {
    %x = %x - gint(%x / (2*$PI)) * (2*$PI);
    if (%x > $PI) {
      %x -= $PI;
      if (%x > $PI/2) %x = $PI/2 - %x;

      return -%x + pow(%x, 3)/6 - pow(%x, 5)/120 + pow(%x, 7)/5040 - pow(%x, 9)/362880;
    } else {
      if (%x > $PI/2) %x = $PI/2 - %x;

      return %x - pow(%x, 3)/6 + pow(%x, 5)/120 - pow(%x, 7)/5040 + pow(%x, 9)/362880;
    }
  }

  // |Error| <= ($PI/2)^12/12! ~= 4.7e-7
  function cos(%x) {
    %x = abs(%x - gint(%x / (2*$PI)) * (2*$PI) - $PI);
    if (%x > $PI/2) {
      %x = $PI/2 - %x;

      return -1 + pow(%x, 2)/2 - pow(%x, 4)/24 + pow(%x, 6)/720 - pow(%x, 8)/40320 + pow(%x, 10)/3628800;
    } else {
      return 1 - pow(%x, 2)/2 + pow(%x, 4)/24 - pow(%x, 6)/720 + pow(%x, 8)/40320 - pow(%x, 10)/3628800;
    }
  }

  // Error increases as %x->$PI/2
  function tan(%x) {
    %sin = sin(%x);
    return sqrt(1/%cos/%cos - 1);
  }

} else {
function sin(%angle) { return -getWord(Vector::getFromRot("0 0 " @ %angle), 0); }
function cos(%angle) { return getWord(Vector::getFromRot("0 0 " @ %angle), 1); }
//function atan(%x) { return %x - %x/3 + %x/5 - %x/7 + %x/9 - %x/11; }
}

function atan(%x) { if (%x == 1) return $PI/4; return getWord(Vector::getRotation(-%x @ " 1 0"), 2); }

function Vector::mul(%vec, %c) {
  %newVec = "";
  for (%i = 0; %i < 3; %i++) {
    %x = getWord(%vec, %i);
    %newVec = %newVec @ tern(%i > 0, " ", "") @ (%x * %c);
  }
  return %newVec;
}

function Vector::length(%vec) { return Vector::getDistance(%vec, 0); }
function Vector::lengthSquared(%vec) { return Vector::dot(%vec, %vec); }

function Vector::resize(%vec, %len) {
  return Vector::mul(%vec, %len / Vector::length(%vec, 0));
}

function Vector::lerp(%vec1, %vec2, %p) {
  return Vector::add(Vector::mul(%vec1, 1 - %p), Vector::mul(%vec2, %p));
}

function Vector::randomVec(%ax, %bx, %ay, %by, %az, %bz) {
  return (getRandom() * (%bx - %ax) + %ax) @ " " @
         (getRandom() * (%by - %ay) + %ay) @ " " @
         (getRandom() * (%bz - %az) + %az);
}

function Vector::randomVec2(%vecA, %vecB) {
  return (getRandom() * (getWord(%vecB,0) - getWord(%vecA,0)) + getWord(%vecA,0)) @ " " @
         (getRandom() * (getWord(%vecB,1) - getWord(%vecA,1)) + getWord(%vecA,1)) @ " " @
         (getRandom() * (getWord(%vecB,2) - getWord(%vecA,2)) + getWord(%vecA,2));
}

function Vector::randomRotVec(%rxa, %rxb, %rya, %ryb, %rza, %rzb, %minR, %maxR) {
  return Vector::getFromRot(Vector::randomVec(%rxa, %rxb, %rya, %ryb, %rza, %rzb), (%maxR - %minR) * getRandom() + %minR);
}

function Vector::randomRotVecQWeight(%rxa, %rxb, %rya, %ryb, %rza, %rzb, %minR, %maxR) {
  return Vector::getFromRot(Vector::randomVec(%rxa, %rxb, %rya, %ryb, %rza, %rzb), (%maxR - %minR) * (1-getRandom()*getRandom()) + %minR);
}
function Vector::rotate(%vec, %rot) {
  %len = Vector::length(%vec);
  return Vector::getFromRot(Vector::add(Vector::getRotation(%vec), Vector::add("1.57 0 0", %rot)), %len);
}

function Vector::getRot(%vec) {
  return Vector::add(Vector::getRotation(%vec), "1.57079 0 0");
}

function Vector::cross(%vec1, %vec2) {
  %a1 = getWord(%vec1, 0);
  %a2 = getWord(%vec1, 1);
  %a3 = getWord(%vec1, 2);
  %b1 = getWord(%vec2, 0);
  %b2 = getWord(%vec2, 1);
  %b3 = getWord(%vec2, 2);
  return (%a2*%b3-%b2*%a3) @ " " @ (%a3*%b1-%b3*%a1) @ " " @ (%a1*%b2-%b1*%a2);
}

// %d = dist to target, %h = height difference to target (%h>0 -> target is above)
// %g = accel due to gravity, %v = launch velocity
function calcAngle(%d, %h, %g, %v, %high, %allowDownward) {
  %d2 = %d * %d;
  %q = %g*%d2/(%v*%v);
  %deter = %d2 - %q*(%q-%h);
  if (%deter < 0) return "NaN";
  %x1 = (-%d + sqrt(%deter)) / %q;
  %x2 = (-%d - sqrt(%deter)) / %q;

  %x = tern(%high, max(%x1, %x2), min(%x1, %x2));

  if (%x < 0) %x = tern(!%high, max(%x1, %x2), min(%x1, %x2));

  if (%x < 0 && !%allowDownward) return "NaN";

  echo(%x1, ",", %x2);
  if (%x > 1) return $PI/2 - atan(1/%x);
  else        return atan(%x);
}

function TransformMatrix::addToPos(%matrix, %pos) {
  return Matrix::subMatrix(%matrix, 3, 4, 3, 3) @ " " @ Vector::add(Matrix::subMatrix(%matrix, 3, 4, 3, 1, 0, 3), %pos);
}

function GameBase::addPosition(%this, %delta) { GameBase::setPosition(%this, Vector::add(GameBase::getPosition(%this), %delta)); }



function invoke(%func, %numArgs, %arg0, %arg1, %arg2, %arg3, %arg4, %arg5, %arg6, %arg7, %arg8, %arg9) {
  %cmd = %func @ tern(%numArgs > 0, "(\"" @ %arg0 @ "\"", "(");
  for (%i = 1; %i < %numArgs; %i++) %cmd = %cmd @ ",\"" @ %arg[%i] @ "\"";
  %cmd = %cmd @ ");";
  return eval(%cmd);
}

function radnomItems(%num, %an0, %an1, %an2, %an3, %an4, %an5, %an6, %an7, %an8) {
  return %an[floor(getRandom() * (%num - 0.01))];
}

function String::length(%str) {
  for (%i = 0; %i < 10240; %i++) {
    if (String::getSubStr(%str, %i, 1) == "") return %i;
  }
  return -1;
}

$String::vowels[0] = "a";
$String::vowels[1] = "e";
$String::vowels[2] = "i";
$String::vowels[3] = "o";
$String::vowels[4] = "u";
function aOrAn(%str, %caps) {
  %x = String::getSubStr(%str, 0, 1);
  for (%i = 0; $String::vowels[%i] != ""; %i++)
    if (String::ICompare(%x, $String::vowels[%i]) == 0)
      return tern(%caps, "An", "an");

  return tern(%caps, "A", "a");
}

function String::replace(%str, %a, %b) {
  %len = String::length(%a);
  %strLeft = %str;
  while (true) {
    %ind = String::findSubStr(%strLeft, %a);
    if (%ind == -1) break;
    %newStr = %newStr @ String::getSubStr(%strLeft, 0, %ind) @ %b;
    %strLeft = String::getSubStr(%strLeft, %ind + %len, 10240);
  }
  %newStr = %newStr @ %strLeft;
  return %newStr;
}

$escapeStuff["0"] = "0";
$escapeStuff["1"] = "1";
$escapeStuff["2"] = "2";
$escapeStuff["3"] = "3";
$escapeStuff["4"] = "4";
$escapeStuff["5"] = "5";
$escapeStuff["6"] = "6";
$escapeStuff["7"] = "7";
$escapeStuff["8"] = "8";
$escapeStuff["9"] = "9";
$escapeStuff["K"] = "A";
$escapeStuff["L"] = "B";
$escapeStuff["M"] = "C";
$escapeStuff["N"] = "D";
$escapeStuff["O"] = "E";
$escapeStuff["P"] = "F";

function String::escapeGood(%str) {
  %str = escapeString(%str);
  while (true) {
    %ind = String::findSubStr(%str, "\\x");
    if (%ind == -1) break;
    %str = String::getSubStr(%str, 0, %ind) @ "~~~~~~" @ $escapeStuff[String::getSubStr(%str, %ind + 2, 1)] @
                                                         $escapeStuff[String::getSubStr(%str, %ind + 3, 1)] @
                                                         String::getSubStr(%str, %ind + 4, 1024);
  }
  %str = String::replace(%str, "~~~~~~", "\\x");
  return %str;
}

function String::splitBefore(%str, %c) {
	%idx = String::findSubStr(%str, %c);
	if (%idx == -1) return "";
	return String::getSubStr(%str, 0, %idx);
}

function String::splitAfter(%str, %c) {
	%idx = String::findSubStr(%str, %c);
	if (%idx == -1) return "";
	return String::getSubStr(%str, %idx+1, 10240);
}

function String::startsWith(%s, %prefix) {
	%len = String::length(%prefix);
	return (String::NCompare(%s, %prefix, %len) == 0);
}

function String::substr(%s, %off, %len) {
	%off = defval(%off, 0);
	%len = defval(%len, 10240);
	return String::getSubStr(%s, %off, %len);
}

function String::stripPrefix(%s, %c) {
	%l = String::length(%c);
	while (String::startsWith(%s, %c)) %s = String::substr(%s, %l);
	return %s;
}

// usage: for (%w = nextWord(%s); (%s = popWord(%s)) || %w; %w = nextWord(%s)) ...;
function nextWord(%s) { return String::splitBefore(String::stripPrefix(%s, " "), " "); }
function popWord(%s) { return String::splitAfter(String::stripPrefix(%s, " "), " "); }

function char(%str) { return String::ICompare(String::getSubStr(%str,0,1), "\x00"); }

function getSuffix(%i) {
  if (floor((%i % 100)/10) == 1) return "th";

  if (%i%10 == 1) return "st";
  if (%i%10 == 2) return "nd";
  if (%i%10 == 3) return "rd";
  return "th";
}

// Add *print lock functionality
function bottomprint(%clientId, %msg, %timeout, %lock) {
   %time = getSimTime();
   if (%clientId.printlockTill != "" && %clientId.printlockTill > %time) return;
   if (%timeout == "")
      %timeout = 5;
   if (%lock) %clientId.printlockTill = %time + %timeout;
   remoteEval(%clientId, "BP", %msg, %timeout);
}

function centerprint(%clientId, %msg, %timeout, %lock) {
   %time = getSimTime();
   if (%clientId.printlockTill != "" && %clientId.printlockTill > %time) return;
   if (%timeout == "")
      %timeout = 5;
   if (%lock) %clientId.printlockTill = %time + %timeout;
   remoteEval(%clientId, "CP", %msg, %timeout);
}

function clearPrintLock(%clientId) {
  %clientId.printlockTill = -1;
}


$realModList = $modList;
function EvalSearchPath() {
  if ($realModList == "") {
    $realModList = $modList;
    $modList = "\x01SpyMod"; // Oooooo, so l33t, gets top mod by sorting!!!
  }

  // search path always contains the config directory
  %searchPath = "config";
  if ($realModList == "") {
    $realModList = "base";
  } else {
    for (%i = 0; (%word = getWord($realModList, %i)) != -1; %i++)
      if (%word == "base")
        break;

    if (%word == -1)
      $realModList = $realModList @ " base";
  }
  for (%i = 0; (%word = getWord($realModList, %i)) != -1; %i++) {
    %addPath = %word @ ";" @ %word @ "\\missions;" @ %word @ 
                       "\\fonts;" @ %word @ "\\skins;" @ %word @ "\\voices;" @ %word @ "\\scripts";
    %searchPath = %searchPath @ ";" @ %addPath;
  }
  %searchPath = %searchPath @ ";recordings;temp";
  echo(%searchPath);

  $ConsoleWorld::DefaultSearchPath = %searchPath;

  // clear out the volumes:
  for (%i = 0; isObject(%vol = "VoiceVolume" @ %i); %i++)
    deleteObject(%vol);
  for (%i = 0; isObject(%vol = "SkinVolume" @ %i); %i++)
    deleteObject(%vol);

  // load all the volumes:
  %file = File::findFirst("voices\\*.vol");
  for (%i = 0; %file != ""; %file = File::findNext("voices\\*.vol"))
    if (newObject("VoiceVolume" @ %i, SimVolume, %file))
      %i++;

  %file = File::findFirst("skins\\*.vol");
  for (%i = 0; %file != ""; %file = File::findNext("skins\\*.vol"))
    if (newObject("SkinVolume" @ %i, SimVolume, %file))
      %i++;

  //$realModList = $Server::modName;
}

function isClientObject(%x) {
  return getObjectType(%x) == "Net::PacketStream";
}

function isServerFocused() {
  return getManagerID() == 2048;
}

function isItemData(%itemData) {
	return %itemData.shapeFile != "False"; // equals "" if not set, but only equals "False" if not an item (or if explicitly set to "False", I guess...)
}

//
// To deprecate a function:
//
// deprecated("deprecatedFunction");
// function deprecatedFunction() {
//   deprecated();
//   return; // Put here if you want to make the function a no-op
// }
//
function deprecated(%funcName) {
  if (%funcName == "") {
    echo("DEPRECATED FUNCTION CALLED");
    trace();
  } else {
    $deprecated[$deprecated] = %funcName;
    $deprecated++;
  }
}

function listDeprecated() {
  echo("Deprecated:");
  for (%i = 0; %i < $deprecated; %i++) echo(": " @ $deprecated[%i]);
  echo();
}

//
// Function help: allows usage notes to be associated with functions
//
// To set usage notes: sethelp(function, "arg0 arg1 arg2 arg3 ...", "description");
//   * if argX == "?argX", it will be displayed as optional
//

//$help[func] = help str
function sethelp(%func, %args, %desc) {
  %str = "function " @ %func @ "(";

  for (%i = 0; (%arg = getWord(%args, %i)) != -1; %i++) {
    if (%i > 0) %str = %str @ ", ";

    %optional = "";
    if (String::getSubStr(%arg, 0, 1) == "?") {
      %arg = String::getSubStr(%arg, 1, 10000);
      %optional = true;
    }

    if (%optional) {
      %str = %str @ "[" @ %arg @ "]";
    } else {
      %str = %str @ %arg;
    }
  }

  %str = %str @ ")";

  $help[funcline, %func] = %str;
  $help[funcdesc, %func] = %desc;
}

function help(%func) {
  if (String::ICompare($help[funcline, %func], "") == 0) {
    echo("No help for function " @ %func);
  } else {
    echo($help[funcline, %func]);
    echo($help[funcdesc, %func]);
  }
}

//---------------------------------------------------------------------------------------------------
// W00T! This will (hopefully) annihilate any hex crashing that gets past normal detection.  w00t.
//---------------------------------------------------------------------------------------------------

function fixecho() {
function echo(%a0,%a1,%a2,%a3,%a4,%a5,%a6,%a7,%a8,%a9) {
  %str = %a0 @ %a1 @ %a2 @ %a3 @ %a4 @ %a5 @ %a6 @ %a7 @ %a8 @ %a9;
  if (String::getSubStr(%str, 1023, 0)) { echo("HEX CRASH AVOIDED!"); trace(); }
  else dbecho(1,%str);
}
}

exec("misc\\matrix2.cs");
exec("misc\\ip.cs");
exec("misc\\heap2.cs");
exec("misc\\groups.cs");
