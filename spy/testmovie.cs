//exec("misc\\cinematic2.cs");

%i = -1;
$timeline3[%i++, "action"]  = "0 spawnAI";
$timeline3[%i, "args", 0] = "AI1";
$timeline3[%i, "args", 1] = "I AM BOB";
$timeline3[%i, "args", 2] = "LightAIMale";
$timeline3[%i, "args", 3] = "0 0 0";
$timeline3[%i, "args", 4] = "0 0 0";

$timeline3[%i++, "action"]  = "1 setCamModeMoveAlongPath";
$timeline3[%i, "args", 0] = "10 18 3";
$timeline3[%i, "args", 1] = "0 0 3.14";
$timeline3[%i, "args", 2] = "-10 18 3";
$timeline3[%i, "args", 3] = "0 0 3.14";
$timeline3[%i, "args", 4] = "5";

$timeline3[%i++, "action"]  = "7 setCamModeLookAtAI";
$timeline3[%i, "args", 0] = "-10 18 3";
$timeline3[%i, "args", 1] = "AI1";

$timeline3[%i++, "action"]  = "7 moveAI";
$timeline3[%i, "args", 0] = "AI1";
$timeline3[%i, "args", 1] = "-10 0 0";
$timeline3[%i, "args", 2] = "100";

$timeline3[%i++, "action"] = "17 endMovie";

$timeline3[%i++, "action"] = "";

  %id = Cinematic::newMovie();
  Cinematic::addViewer(%id, 2049);
  Cinematic::start(%id);
  Cinematic::parseTimeline(%id, "timeline3");
