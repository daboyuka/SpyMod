//exec("misc\\cinematic2.cs");

%i = -1;
$SpySabotage::movieTimeline0[%i++, "action"]  = "0 eval";
$SpySabotage::movieTimeline0[%i, "args", 0] = "$simgame::timescale = 1;";

$SpySabotage::movieTimeline0[%i++, "action"]  = "0 messageAll";
$SpySabotage::movieTimeline0[%i, "args", 0] = "1";
$SpySabotage::movieTimeline0[%i, "args", 1] = "------------------------------";

$SpySabotage::movieTimeline0[%i++, "action"]  = "0 messageAll";
$SpySabotage::movieTimeline0[%i, "args", 0] = "0";
$SpySabotage::movieTimeline0[%i, "args", 1] = "Welcome to SpySabotage team " @ getTeamName(0) @ "!";

$SpySabotage::movieTimeline0[%i++, "action"]  = "0 messageAll";
$SpySabotage::movieTimeline0[%i, "args", 0] = "1";
$SpySabotage::movieTimeline0[%i, "args", 1] = "------------------------------";

$SpySabotage::movieTimeline0[%i++, "action"]  = "0 setCamModeLookAtPoint";
$SpySabotage::movieTimeline0[%i, "args", 0] = "-490.75 -2094.5 49.5";
$SpySabotage::movieTimeline0[%i, "args", 1] = "-489.75 -2096 49.0";

///////////////////////////////////////////////////////////////////
$SpySabotage::movieTimeline0[%i++, "action"]  = "2 resetTime";
///////////////////////////////////////////////////////////////////

$SpySabotage::movieTimeline0[%i++, "action"]  = "0 messageAll";
$SpySabotage::movieTimeline0[%i, "args", 0] = "0";
$SpySabotage::movieTimeline0[%i, "args", 1] = "Your objective is to destroy all three enemy generators...";

$SpySabotage::movieTimeline0[%i++, "action"]  = "2 messageAll";
$SpySabotage::movieTimeline0[%i, "args", 0] = "0";
$SpySabotage::movieTimeline0[%i, "args", 1] = "...or kill all opposing team members.";

///////////////////////////////////////////////////////////////////
$SpySabotage::movieTimeline0[%i++, "action"]  = "4 resetTime";
///////////////////////////////////////////////////////////////////

$SpySabotage::movieTimeline0[%i++, "action"]  = "0 setCamModeMoveAlongPath";
$SpySabotage::movieTimeline0[%i, "args", 0] = "-490.75 -2094.5 49.5";
$SpySabotage::movieTimeline0[%i, "args", 1] = "-0.27 0 -2.55";
$SpySabotage::movieTimeline0[%i, "args", 2] = "-470 -2096.5 41";
$SpySabotage::movieTimeline0[%i, "args", 3] = "-0.4 0 -3.92";
$SpySabotage::movieTimeline0[%i, "args", 4] = "2";

$SpySabotage::movieTimeline0[%i++, "action"]  = "2 setCamModeMoveAlongPath";
$SpySabotage::movieTimeline0[%i, "args", 0] = "-470 -2096.5 41";
$SpySabotage::movieTimeline0[%i, "args", 1] = "-0.4 0 2.35";
$SpySabotage::movieTimeline0[%i, "args", 2] = "-476 -2108.5 39.5";
$SpySabotage::movieTimeline0[%i, "args", 3] = "-0.2 0 1.67";
$SpySabotage::movieTimeline0[%i, "args", 4] = "1.2";

$SpySabotage::movieTimeline0[%i++, "action"]  = "3.5 messageAll";
$SpySabotage::movieTimeline0[%i, "args", 0] = "0";
$SpySabotage::movieTimeline0[%i, "args", 1] = "The inventory stations are down here.";

///////////////////////////////////////////////////////////////////
$SpySabotage::movieTimeline0[%i++, "action"]  = "5 resetTime";
///////////////////////////////////////////////////////////////////

$SpySabotage::movieTimeline0[%i++, "action"]  = "0 setCamModeMoveAlongPath";
$SpySabotage::movieTimeline0[%i, "args", 0] = "-476 -2108.5 39.5";
$SpySabotage::movieTimeline0[%i, "args", 1] = "-0.2 0 1.67";
$SpySabotage::movieTimeline0[%i, "args", 2] = "-470 -2096.5 41";
$SpySabotage::movieTimeline0[%i, "args", 3] = "-0.4 0 2.35";
$SpySabotage::movieTimeline0[%i, "args", 4] = "0.75";

$SpySabotage::movieTimeline0[%i++, "action"]  = "0.75 setCamModeMoveAlongPath";
$SpySabotage::movieTimeline0[%i, "args", 0] = "-470 -2096.5 41";
$SpySabotage::movieTimeline0[%i, "args", 1] = "-0.4 0 -3.92";
$SpySabotage::movieTimeline0[%i, "args", 2] = "-490.75 -2094.5 49.5";
$SpySabotage::movieTimeline0[%i, "args", 3] = "-0.27 0 -2.55";
$SpySabotage::movieTimeline0[%i, "args", 4] = "0.75";

$SpySabotage::movieTimeline0[%i++, "action"]  = "1.5 setCamModeMoveAlongPath";
$SpySabotage::movieTimeline0[%i, "args", 0] = "-490.75 -2094.5 49.5";
$SpySabotage::movieTimeline0[%i, "args", 1] = "-0.27 0 3.73";
$SpySabotage::movieTimeline0[%i, "args", 2] = "-477.5 -2125 45.5";
$SpySabotage::movieTimeline0[%i, "args", 3] = "0 0 1.7";
$SpySabotage::movieTimeline0[%i, "args", 4] = "1";

$SpySabotage::movieTimeline0[%i++, "action"]  = "2.5 setCamModeMoveAlongPath";
$SpySabotage::movieTimeline0[%i, "args", 0] = "-477.5 -2125 45.5";
$SpySabotage::movieTimeline0[%i, "args", 1] = "0 0 1.7";
$SpySabotage::movieTimeline0[%i, "args", 2] = "-494.5 -2127.5 45.5";
$SpySabotage::movieTimeline0[%i, "args", 3] = "0.8 0 -1";
$SpySabotage::movieTimeline0[%i, "args", 4] = "1.5";

$SpySabotage::movieTimeline0[%i++, "action"]  = "4 setCamModeMoveAlongPath";
$SpySabotage::movieTimeline0[%i, "args", 0] = "-494.5 -2127.5 45.5";
$SpySabotage::movieTimeline0[%i, "args", 1] = "0.8 0 -1";
$SpySabotage::movieTimeline0[%i, "args", 2] = "-480 -2122 55";
$SpySabotage::movieTimeline0[%i, "args", 3] = "0 0 0";
$SpySabotage::movieTimeline0[%i, "args", 4] = "1";

$SpySabotage::movieTimeline0[%i++, "action"]  = "5 setCamModeMoveAlongPath";
$SpySabotage::movieTimeline0[%i, "args", 0] = "-480 -2122 55";
$SpySabotage::movieTimeline0[%i, "args", 1] = "0 0 0";
$SpySabotage::movieTimeline0[%i, "args", 2] = "-480 -2105 55";
$SpySabotage::movieTimeline0[%i, "args", 3] = "1.57 0 0";
$SpySabotage::movieTimeline0[%i, "args", 4] = "1";

$SpySabotage::movieTimeline0[%i++, "action"]  = "6 setCamModeMoveAlongPath";
$SpySabotage::movieTimeline0[%i, "args", 0] = "-480 -2105 55";
$SpySabotage::movieTimeline0[%i, "args", 1] = "1.57 0 0";
$SpySabotage::movieTimeline0[%i, "args", 2] = "-480 -2105 80";
$SpySabotage::movieTimeline0[%i, "args", 3] = "0 0 0";
$SpySabotage::movieTimeline0[%i, "args", 4] = "1";

$SpySabotage::movieTimeline0[%i++, "action"]  = "7.5 messageAll";
$SpySabotage::movieTimeline0[%i, "args", 0] = "0";
$SpySabotage::movieTimeline0[%i, "args", 1] = "Get everyone in the transport and fly north!";

$SpySabotage::movieTimeline0[%i++, "action"]  = "7.5 eval";
$SpySabotage::movieTimeline0[%i, "args", 0] = "$simgame::timescale = 1;";

///////////////////////////////////////////////////////////////////
$SpySabotage::movieTimeline0[%i++, "action"]  = "9 resetTime";
///////////////////////////////////////////////////////////////////

$SpySabotage::movieTimeline0[%i++, "action"]  = "0 setCamModeMoveAlongPath";
$SpySabotage::movieTimeline0[%i, "args", 0] = "-480 -2105 80";
$SpySabotage::movieTimeline0[%i, "args", 1] = "0 0 0";
$SpySabotage::movieTimeline0[%i, "args", 2] = "-445 -1192 180";
$SpySabotage::movieTimeline0[%i, "args", 3] = "0 0 0";
$SpySabotage::movieTimeline0[%i, "args", 4] = "3";

$SpySabotage::movieTimeline0[%i++, "action"]  = "3 setCamModeMoveAlongPath";
$SpySabotage::movieTimeline0[%i, "args", 0] = "-445 -1192 180";
$SpySabotage::movieTimeline0[%i, "args", 1] = "0 0 0";
$SpySabotage::movieTimeline0[%i, "args", 2] = "-435 -192 100";
$SpySabotage::movieTimeline0[%i, "args", 3] = "0 0 0";
$SpySabotage::movieTimeline0[%i, "args", 4] = "3";

$SpySabotage::movieTimeline0[%i++, "action"]  = "6.5 messageAll";
$SpySabotage::movieTimeline0[%i, "args", 0] = "0";
$SpySabotage::movieTimeline0[%i, "args", 1] = "Infiltrate the enemy base! Try not to be seen!";

///////////////////////////////////////////////////////////////////
$SpySabotage::movieTimeline0[%i++, "action"]  = "8 resetTime";
///////////////////////////////////////////////////////////////////

$SpySabotage::movieTimeline0[%i++, "action"]  = "0 setCamModeMoveAlongPath";
$SpySabotage::movieTimeline0[%i, "args", 0] = "-435 -192 100";
$SpySabotage::movieTimeline0[%i, "args", 1] = "0 0 0";
$SpySabotage::movieTimeline0[%i, "args", 2] = "-371.5 -36.5 46";
$SpySabotage::movieTimeline0[%i, "args", 3] = "-0.4 0 1.57";
$SpySabotage::movieTimeline0[%i, "args", 4] = "2";

$SpySabotage::movieTimeline0[%i++, "action"]  = "2.5 messageAll";
$SpySabotage::movieTimeline0[%i, "args", 0] = "0";
$SpySabotage::movieTimeline0[%i, "args", 1] = "You can go in here...";

///////////////////////////////////////////////////////////////////
$SpySabotage::movieTimeline0[%i++, "action"]  = "4 resetTime";
///////////////////////////////////////////////////////////////////

$SpySabotage::movieTimeline0[%i++, "action"]  = "0 setCamModeMoveAlongPath";
$SpySabotage::movieTimeline0[%i, "args", 0] = "-371.5 -36.5 46";
$SpySabotage::movieTimeline0[%i, "args", 1] = "-0.4 0 1.57";
$SpySabotage::movieTimeline0[%i, "args", 2] = "-383 -36.5 115";
$SpySabotage::movieTimeline0[%i, "args", 3] = "-0.6 0 1.57";
$SpySabotage::movieTimeline0[%i, "args", 4] = "1";

$SpySabotage::movieTimeline0[%i++, "action"]  = "1 setCamModeMoveAlongPath";
$SpySabotage::movieTimeline0[%i, "args", 0] = "-383 -36.5 115";
$SpySabotage::movieTimeline0[%i, "args", 1] = "-0.6 0 1.57";
$SpySabotage::movieTimeline0[%i, "args", 2] = "-515 -36.5 115";
$SpySabotage::movieTimeline0[%i, "args", 3] = "-0.6 0 -1.57";
$SpySabotage::movieTimeline0[%i, "args", 4] = "2";

$SpySabotage::movieTimeline0[%i++, "action"]  = "3 setCamModeMoveAlongPath";
$SpySabotage::movieTimeline0[%i, "args", 0] = "-515 -36.5 115";
$SpySabotage::movieTimeline0[%i, "args", 1] = "-0.6 0 -1.57";
$SpySabotage::movieTimeline0[%i, "args", 2] = "-515 -36.5 45";
$SpySabotage::movieTimeline0[%i, "args", 3] = "-0.4 0 -1.57";
$SpySabotage::movieTimeline0[%i, "args", 4] = "1";

$SpySabotage::movieTimeline0[%i++, "action"]  = "4.5 messageAll";
$SpySabotage::movieTimeline0[%i, "args", 0] = "0";
$SpySabotage::movieTimeline0[%i, "args", 1] = "...or here. Get inside and destroy the generators to win!";

///////////////////////////////////////////////////////////////////
$SpySabotage::movieTimeline0[%i++, "action"]  = "6 resetTime";
///////////////////////////////////////////////////////////////////

$SpySabotage::movieTimeline0[%i++, "action"] = "1 endMovie";

$SpySabotage::movieTimeline0[%i++, "action"] = "";

function testmovie() {
  %id = Cinematic::newMovie();
  Cinematic::addAllViewers(%id);
  Cinematic::start(%id);
  Cinematic::parseTimeline(%id, "SpySabotage::movieTimeline0");
}