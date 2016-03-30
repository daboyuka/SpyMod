// $WINNERS_NAME
// $WINNERS_GENDER (0 = male, 1 = female)

exec("misc\\cinematic2.cs");

if ($WINNERS_NAME != "" && $WINNERS_GENDER != "") {

%i = -1;
$timeline[%i++, "action"]  = "0 eval";
$timeline[%i, "args", 0] = "$obj1 = newObject(\"\",InteriorShape,\"iobservation.dis\",false);GameBase::setPosition($obj1,\"0 0 -15\");";

$timeline[%i++, "action"]  = "0 spawnAI";
$timeline[%i, "args", 0] = "AI1";
$timeline[%i, "args", 1] = $WINNERS_NAME;
$timeline[%i, "args", 2] = "LightAI"@tern($WINNERS_GENDER, "Female", "Male");
$timeline[%i, "args", 3] = "0 0 10.5";
$timeline[%i, "args", 4] = "0 0 0";

$timeline[%i++, "action"]  = "0 spawnAI";
$timeline[%i, "args", 0] = "AI2";
$timeline[%i, "args", 1] = "Audience Dude";
$timeline[%i, "args", 2] = "LightAIMale";
$timeline[%i, "args", 3] = "0 10 0";
$timeline[%i, "args", 4] = "0 0 3.14";

$timeline[%i++, "action"]  = "0 spawnAI";
$timeline[%i, "args", 0] = "AI3";
$timeline[%i, "args", 1] = "Audience Dude";
$timeline[%i, "args", 2] = "LightAIFemale";
$timeline[%i, "args", 3] = "5 9 0";
$timeline[%i, "args", 4] = "0 0 3.14";

$timeline[%i++, "action"]  = "0 spawnAI";
$timeline[%i, "args", 0] = "AI4";
$timeline[%i, "args", 1] = "Audience Dude";
$timeline[%i, "args", 2] = "LightAIMale";
$timeline[%i, "args", 3] = "-6 12 0";
$timeline[%i, "args", 4] = "0 0 3.14";

$timeline[%i++, "action"]  = "0 spawnAI";
$timeline[%i, "args", 0] = "AI5";
$timeline[%i, "args", 1] = "Audience Dude";
$timeline[%i, "args", 2] = "LightAIFemale";
$timeline[%i, "args", 3] = "2 15 0";
$timeline[%i, "args", 4] = "0 0 3.14";

$timeline[%i++, "action"]  = "0.5 repeatedAnimateAI";
$timeline[%i, "args", 0] = "AI2";
$timeline[%i, "args", 1] = 45;
$timeline[%i, "args", 2] = 20/3;
$timeline[%i, "args", 3] = 3;

$timeline[%i++, "action"]  = "0.5 repeatedAnimateAI";
$timeline[%i, "args", 0] = "AI3";
$timeline[%i, "args", 1] = 45;
$timeline[%i, "args", 2] = 20/3;
$timeline[%i, "args", 3] = 3;

$timeline[%i++, "action"]  = "0.5 repeatedAnimateAI";
$timeline[%i, "args", 0] = "AI4";
$timeline[%i, "args", 1] = 45;
$timeline[%i, "args", 2] = 20/3;
$timeline[%i, "args", 3] = 3;

$timeline[%i++, "action"]  = "0.5 repeatedAnimateAI";
$timeline[%i, "args", 0] = "AI5";
$timeline[%i, "args", 1] = 45;
$timeline[%i, "args", 2] = 20/3;
$timeline[%i, "args", 3] = 3;

///////////////////////////////////////////
$timeline[%i++, "action"]  = "1 resetTime";
///////////////////////////////////////////

$timeline[%i++, "action"]  = "0 cheers";
$timeline[%i, "args", 0] = 10*4;
$timeline[%i, "args", 1] = 0.25;

$timeline[%i++, "action"]  = "0 setCamModeMoveAlongPath";
$timeline[%i, "args", 0] = "10 18 3";
$timeline[%i, "args", 1] = "0 0 3.14";
$timeline[%i, "args", 2] = "-10 18 3";
$timeline[%i, "args", 3] = "0 0 3.14";
$timeline[%i, "args", 4] = "5";

$timeline[%i++, "action"]  = "0 eval";
$timeline[%i, "args", 0] = "Fireworks::fireFirework1(Vector::randomVec(-10,10,-2,-10,1,1));";

$timeline[%i++, "action"]  = "2.5 eval";
$timeline[%i, "args", 0] = "Fireworks::fireFirework2(Vector::randomVec(-10,10,-2,-10,1,1));";

$timeline[%i++, "action"]  = "5 setCamModeMoveAlongPath";
$timeline[%i, "args", 0] = "-10 11 1";
$timeline[%i, "args", 1] = "1.1 0 3.14";
$timeline[%i, "args", 2] = "10 11 1";
$timeline[%i, "args", 3] = "1.1 0 3.14";
$timeline[%i, "args", 4] = "5";

$timeline[%i++, "action"]  = "5 eval";
$timeline[%i, "args", 0] = "Fireworks::fireFirework1(Vector::randomVec(-10,10,-2,-10,1,1));";

///////////////////////////////////////////
$timeline[%i++, "action"]  = "10 resetTime";
///////////////////////////////////////////

$timeline[%i++, "action"]  = "0 cheers";
$timeline[%i, "args", 0] = 10*4;
$timeline[%i, "args", 1] = 0.25;

$timeline[%i++, "action"]  = "0 setCamModeOrbitPoint";
$timeline[%i, "args", 0] = "0 0 11";
$timeline[%i, "args", 1] = "0.3 0 0.3";
$timeline[%i, "args", 2] = "0.3 0 0.5";
$timeline[%i, "args", 3] = "30";
$timeline[%i, "args", 4] = "5";

$timeline[%i++, "action"]  = "5 setCamModeOrbitPoint";
$timeline[%i, "args", 0] = "0 0 11";
$timeline[%i, "args", 1] = "-0.3 0 -0.3";
$timeline[%i, "args", 2] = "-0.3 0 -0.5";
$timeline[%i, "args", 3] = "30";
$timeline[%i, "args", 4] = "5";

///////////////////////////////////////////
$timeline[%i++, "action"]  = "10 resetTime";
///////////////////////////////////////////

$timeline[%i++, "action"]  = "0 setCamModeMoveAlongPath";
$timeline[%i, "args", 0] = "0 30 15";
$timeline[%i, "args", 1] = "-0.1 0 3.14";
$timeline[%i, "args", 2] = "0 1.5 11.8";
$timeline[%i, "args", 3] = "0 0 3.14";
$timeline[%i, "args", 4] = "2.5";

$timeline[%i++, "action"]  = "0 eval";
$timeline[%i, "args", 0] = "Fireworks::fireFirework3(\"-40 -140 -190\");";
$timeline[%i++, "action"]  = "0 eval";
$timeline[%i, "args", 0] = "Fireworks::fireFirework3(\"40 -140 -190\");";

$timeline[%i++, "action"]  = "3 animateAI";
$timeline[%i, "args", 0] = "AI1";
$timeline[%i, "args", 1] = "39";

$timeline[%i++, "action"]  = "3.5 messageAll";
$timeline[%i, "args", 0] = "0";
$timeline[%i, "args", 1] = "~w"@tern($WINNERS_GENDER, "female", "male")@"1.wcheer1.wav";

$timeline[%i++, "action"]  = "6 messageAll";
$timeline[%i, "args", 0] = "1";
$timeline[%i, "args", 1] = "Congratulations, " @ $WINNERS_NAME @ ", you have owned all in Stryke's SpyMod Tourney!";

$timeline[%i++, "action"]  = "11 messageAll";
$timeline[%i, "args", 0] = "1";
$timeline[%i, "args", 1] = "As a prize for first place, you get the plasglow tornado forever!";

///////////////////////////////////////////
$timeline[%i++, "action"]  = "16 resetTime";
///////////////////////////////////////////

$timeline[%i++, "action"]  = "0 messageAll";
$timeline[%i, "args", 0] = "1";
$timeline[%i, "args", 1] = "Check it out!!!!1!1!111one";

$timeline[%i++, "action"]  = "0 setCamModeLookAtAI";
$timeline[%i, "args", 0] = "-3 -2 14";
$timeline[%i, "args", 1] = "AI5";

$timeline[%i++, "action"]  = "0 moveAI";
$timeline[%i, "args", 0] = "AI1";
$timeline[%i, "args", 1] = "0 2 10.5";
$timeline[%i, "args", 2] = "1000";

$timeline[%i++, "action"]  = "0 giveAIWeapon";
$timeline[%i, "args", 0] = "AI1";
$timeline[%i, "args", 1] = "Tornado";
$timeline[%i, "args", 2] = "1000";
$timeline[%i, "args", 3] = "1";

$timeline[%i++, "action"]  = "1 targetPointAI";
$timeline[%i, "args", 0] = "AI1";
$timeline[%i, "args", 1] = "2 15 0";
$timeline[%i, "args", 2] = "1";
$timeline[%i, "args", 3] = "0.7";

$timeline[%i++, "action"]  = "10 cancelAICommand";
$timeline[%i, "args", 0] = "AI1";

$timeline[%i++, "action"]  = "40 eval";
$timeline[%i, "args", 0] = "deleteObject($obj1);";

$timeline[%i++, "action"] = "40 endMovie";

$timeline[%i++, "action"] = "";

  %id = Cinematic::newMovie();
  Cinematic::addAllViewers(%id);
  Cinematic::start(%id);
  Cinematic::parseTimeline(%id, "timeline");

} else {
  echo("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
  echo("ERROR, NEED WINNER'S NAME AND GENDER!!!");
  echo("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
}