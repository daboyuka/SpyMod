//exec("misc\\cinematic2.cs");

%i = -1;
$timeline2[%i++, "action"]  = "1 setCamModeMoveAlongPath";
$timeline2[%i, "args", 0] = "-10 18 3";
$timeline2[%i, "args", 1] = "0 0 3.14";
$timeline2[%i, "args", 2] = "10 18 3";
$timeline2[%i, "args", 3] = "0 0 3.14";
$timeline2[%i, "args", 4] = "5";

$timeline2[%i++, "action"]  = "7 setCamModeOrbitPoint";
$timeline2[%i, "args", 0] = "0 0 0";
$timeline2[%i, "args", 1] = "0.3 0 1.97";
$timeline2[%i, "args", 2] = "0.3 0 1.17";
$timeline2[%i, "args", 3] = "30";
$timeline2[%i, "args", 4] = "10";

$timeline2[%i++, "action"] = "17 endMovie";

$timeline2[%i++, "action"] = "";

  %id = Cinematic::newMovie();
  Cinematic::addViewer(%id, 2050);
  Cinematic::start(%id);
  Cinematic::parseTimeline(%id, "timeline2");
