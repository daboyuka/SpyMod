function forestArea(%group, %p1, %p2, %numTrees, %maxHeight) {
  if (%maxHeight == "") %maxHeight = 500;

  %minX = min(getWord(%p1,0), getWord(%p2,0));
  %maxX = max(getWord(%p1,0), getWord(%p2,0));
  %minY = min(getWord(%p1,1), getWord(%p2,1));
  %maxY = max(getWord(%p1,1), getWord(%p2,1));

  focusServer();

  %dummy = newObject("", StaticShape, TreeShape);

  for (%i = 0; %i < %numTrees; %i++) {
    GameBase::setPosition(%dummy, Vector::randomVec(%minX, %maxX, %minY, %maxY, %maxHeight, %maxHeight));
    if (GameBase::getLOSInfo(%dummy, 10000, "-1.57 0 0")) {
      %pos[%i] = $los::position;
    } else echo("NOTHING");
  }

  for (%i = 0; %i < %numTrees; %i++) {
//    echo("makeTree("@%group@",\""@%pos[%i]@"\");");
    schedule("makeTree("@%group@",\""@%pos[%i]@"\");", %i/20);
  }

  deleteObject(%dummy);

  focusClient();
}

function makeTree(%group, %pos) {
  %obj = newObject("", StaticShape, radnomItems(2, TreeShape, TreeShapeTwo));
  addToSet(%group, %obj);
  GameBase::setPosition(%obj, Vector::add(%pos, "0 0 -0.5"));
  GameBase::setRotation(%obj, Vector::randomVec(0,0,0,0,-$PI,$PI));
}

//forestArea(8232, "-80 -45", "110 60", 100, 500);

function makeGlobe(%group, %pos, %rad, %jump) {
  focusServer();

  %objs = 0; // Count num objects it takes to make the globe

  // Make all the blocks around the sides, but leave a hole in the top and bottom
  for (%z = 0; %z < $PI * 2; %z += %jump) {
    for (%x = $PI / -2 + %jump; %x < $PI / 2 - %jump; %x += %jump) {
      %o = newObject("", InteriorShape, "iblock.dis");
      GameBase::setPosition(%o, Vector::add(%pos, Vector::getFromRot(%x @ " 0 " @ %z, %rad)));
      GameBase::setRotation(%o, (%x-$PI/2) @ " 0 " @ %z);
      addToSet(%group, %o);
      %objs++;
    }
  }

  // Plug the top and bottom with ONE block each
  %o = newObject("", InteriorShape, "iblock.dis");
  GameBase::setPosition(%o, Vector::add(%pos, "0 0 " @ -%rad));
  GameBase::setRotation(%o, 0);
  addToSet(%group, %o);

  %o = newObject("", InteriorShape, "iblock.dis");
  GameBase::setPosition(%o, Vector::add(%pos, "0 0 " @ %rad));
  GameBase::setRotation(%o, 0);
  addToSet(%group, %o);

  %objs += 2; // For the top and bottom

  echo("Used " @ %objs @ " iblock.dis interior shapes");

  focusClient();
}

function repeatObject(%group, %arg1, %arg2, %a, %n, %j, %rot, %arg3) {
  focusServer();
  %x = getWord(%a, 0);
  %objs = 0;
  for (%xi = 0; %xi < getWord(%n, 0); %xi++) {
    %y = getWord(%a, 1);
    for (%yi = 0; %yi < getWord(%n, 1); %yi++) {
      %z = getWord(%a, 2);
      for (%zi = 0; %zi < getWord(%n, 2); %zi++) {
        if (%arg3 == "") %o = newObject("", %arg1, %arg2);
        else             %o = newObject("", %arg1, %arg2, %arg3);
        GameBase::setPosition(%o, %x @ " " @ %y @ " " @ %z);
        GameBase::setRotation(%o, %rot);
        addToSet(%group, %o);
        %objs++;

        %z += getWord(%j, 2);
      }
      %y += getWord(%j, 1);
    }
    %x += getWord(%j, 0);
  }
  echo(%objs, " objects created.");
  focusClient();
}

function mirror(%group, %point) {
  %n = Group::objectCount(%group);
  for (%i = 0; %i < %n; %i++) {
    %o = Group::getObject(%group, %i);
    echo(getObjectType(%o));
    if (getObjectType(%o) == "SimGroup" || getObjectType(%o) == "SimPath") mirror(%o,%point);
    else {
      %newPoint = tern(getWord(%point, 0) != 12321, getWord(%point, 0), getWord(GameBase::getPosition(%o), 0))*2 @ " " @
                  tern(getWord(%point, 1) != 12321, getWord(%point, 1), getWord(GameBase::getPosition(%o), 1))*2 @ " " @
                  tern(getWord(%point, 2) != 12321, getWord(%point, 2), getWord(GameBase::getPosition(%o), 2))*2;
      echo(%newPoint, GameBase::getPosition(%o));//Vector::sub(%newPoint, GameBase::getPosition(%o)));
      GameBase::setPosition(%o, Vector::sub(%newPoint, GameBase::getPosition(%o)));
    }
  }
}

function moveGroup(%group, %move) {
  %n = Group::objectCount(%group);
  for (%i = 0; %i < %n; %i++) {
    %o = Group::getObject(%group, %i);
    if (getObjectType(%o) == "SimGroup" || getObjectType(%o) == "SimPath") moveGroup(%o,%move);
    else {
      GameBase::setPosition(%o, Vector::add(%move, GameBase::getPosition(%o)));
    }
  }
}

function round(%x, %d) { %s = pow(10,%d); return floor(%x*%s+0.5)/%s; }
function rotate(%group, %point, %rot, %round) {
  %n = Group::objectCount(%group);
  for (%i = 0; %i < %n; %i++) {
    %o = Group::getObject(%group, %i);
    if (getObjectType(%o) == "SimGroup" || getObjectType(%o) == "SimPath" || getObjectType(%o) == "TeamGroup") rotate(%o,%point,%rot);
    else {
      %pos = GameBase::getPosition(%o);
      %posOff = Vector::sub(%pos, %point);
      %posRot = Vector::getRot(%posOff);
      %newPosRot = Vector::add(%posRot, %rot);
      %newPos = Vector::add(Vector::getFromRot(%newPosRot, Vector::length(%posOff)),%point);
      echo(%posOff, ",", Vector::length(%posOff));
      GameBase::setPosition(%o, %newPos);
      GameBase::setRotation(%o, Vector::add(GameBase::getRotation(%o), %rot));
    }
  }
}

function rotateZ90(%group, %point) {
  %n = Group::objectCount(%group);
  for (%i = 0; %i < %n; %i++) {
    %o = Group::getObject(%group, %i);
    if (getObjectType(%o) == "SimGroup" || getObjectType(%o) == "SimSet" || getObjectType(%o) == "SimPath" || getObjectType(%o) == "TeamGroup") rotateZ90(%o,%point);
    else {
      %pos = GameBase::getPosition(%o);
      %posOff = Vector::sub(%pos, %point);

      %newPosOff = -getWord(%posOff, 1) @ " " @ getWord(%posOff, 0) @ " " @ getWord(%posOff, 2);
      %newPos = Vector::add(%newPosOff, %point);

      GameBase::setPosition(%o, %newPos);
      GameBase::setRotation(%o, Vector::add(GameBase::getRotation(%o), "0 0 " @ ($PI/2)));
    }
  }
}

function connectPlats(%a, %b, %rotA, %rotB) {
  %g = getGroup(%a);
  %pos1 = Vector::add(GameBase::getPosition(%a), Vector::getFromRot(Vector::add(GameBase::getRotation(%a), %rotA), 8, 17));
  %pos2 = Vector::add(GameBase::getPosition(%b), Vector::getFromRot(Vector::add(GameBase::getRotation(%b), %rotB), 8, 17));

  %g2 = newObject("Elev", SimGroup);
  %p = newObject("Path", SimPath);
  %m1 = newObject("", Marker, PathMarker);
  %m2 = newObject("", Marker, PathMarker);
  %e = newObject("Elev", Moveable, Elevator4x4);

  GameBase::setPosition(%m1, %pos1);
  GameBase::setPosition(%m2, %pos2);
  %e.loop = true;

  addToSet(%g, %g2);
  addToSet(%g2, %p);
  addToSet(%g2, %e);
  addToSet(%p, %m1);
  addToSet(%p, %m2);

  Moveable::setWaypoint(%e, 0);
}

function makePlat(%g, %pos, %s) {
  focusServer();

  %p = newObject("", InteriorShape, "BE"@%s@"FloatingPad.dis");
  GameBase::setPosition(%p, %pos);
  addToSet(%g, %p);

  %g2 = nameToID("MissionGroup\\Teams\\team0\\DropPoints\\Start");
  %g3 = nameToID("MissionGroup\\Teams\\team0\\DropPoints\\Random");

  %m = newObject("", Marker, DropPointMarker, true);
  GameBase::setPosition(%m, Vector::add(%pos, "0 0 17.5"));
  addToSet(%g2, %m);

  %m = newObject("", Marker, DropPointMarker, true);
  GameBase::setPosition(%m, Vector::add(%pos, "0 0 17.5"));
  addToSet(%g3, %m);

  return %p;
}


function makeAllPlats(%g) {
  if (%g == "") { echo("N00B"); return; }

  $p1 = makePlat(%g, "0 0 1000", "M");
  $p2 = makePlat(%g, "50 50 1030", "S");
  $p3 = makePlat(%g, "-50 -50 1030", "S");
  $p4 = makePlat(%g, "-50 50 980", "S");
  $p5 = makePlat(%g, "50 -50 980", "S");
  $p6 = makePlat(%g, "-100 0 1000", "S");
  $p7 = makePlat(%g, "100 0 1000", "S");
  $p8 = makePlat(%g, "0 -30 960", "S");
  $p9 = makePlat(%g, "0 30 960", "S");

  connectAllPlats();
}

function connectAllPlats() {
  connectPlats($p1, $p2, "0 0 -1.57", "0 0 3.14");
  connectPlats($p1, $p3, "0 0 1.57", "0 0 0");
  connectPlats($p1, $p4, "0 0 0", "0 0 3.14");
  connectPlats($p1, $p5, "0 0 3.14", "0 0 0");

  connectPlats($p2, $p3, "0 0 1.57", "0 0 -1.57");

  connectPlats($p4, $p9, "0 0 -1.57", "0 0 0");
  connectPlats($p5, $p8, "0 0 1.57", "0 0 3.14");

  connectPlats($p8, $p9, "0 0 0", "0 0 3.14");

  connectPlats($p4, $p6, "0 0 1.57", "0 0 0");
  connectPlats($p5, $p7, "0 0 -1.57", "0 0 3.14");

  connectPlats($p2, $p7, "0 0 -1.57", "0 0 0");
  connectPlats($p3, $p6, "0 0 1.57", "0 0 3.14");

  connectPlats($p2, $p4, "0 0 0", "0 0 0");
  connectPlats($p3, $p5, "0 0 3.14", "0 0 3.14");

  connectPlats($p6, $p7, "0 0 -1.57", "0 0 1.57");
}

$newPosNum = 0;
function makeTrack(%g, %p, %r, %jump, %dontInc) {
  if (%jump == "") %jump = 16;
  %a = newObject("Track", InteriorShape, "expbridgecap.dis");
  GameBase::setRotation(%a, %r);
  GameBase::setPosition(%a, Vector::add(%p, Vector::getFromRot(%r, -%jump)));
  if (!%dontInc) $newPosNum++;
  $newPos[$newPosNum] = Vector::add(%p, Vector::getFromRot(%r, -2*%jump));
  addToSet(%g, %a);
}

function undoTrack() {$newPosNum--; }

function saveTrackVars() { $savedNewPosNum = $newPosNum; $newPos[$newPosNum+1] = $newPos[$newPosNum]; $newPosNum++; }
function restoreTrackVars() { $newPosNum = $savedNewPosNum; }

function countObjects(%group) {
  if (%group == "") %group = MissionGroup;

  %count = 0;

  %n = Group::objectCount(%group);
  for (%i = 0; %i < %n; %i++) {
    %o = Group::getObject(%group, %i);
    if (getObjectType(%o) == "SimGroup" || getObjectType(%o) == "TeamGroup" || getObjectType(%o) == "SimPath") { %count += countObjects(%o); continue; }
    if (getObjectType(%o) != "Marker") %count++;
  }

  if (%group == "MissionGroup") echo(%count @ " objects total.");

  return %count;
}

function makeCylinder(%group, %pos, %rad, %jump) {
  focusServer();

  %objs = 0; // Count num objects it takes to make the globe

  // Make all the blocks around the sides, but leave a hole in the top and bottom
  for (%z = 0; %z < $PI * 2; %z += %jump) {
    %x = $PI;
      %o = newObject("", InteriorShape, "iblock.dis");
      GameBase::setPosition(%o, Vector::add(%pos, Vector::getFromRot(%x @ " 0 " @ %z, %rad)));
      GameBase::setRotation(%o, (%x-$PI/2) @ " 0 " @ %z);
      addToSet(%group, %o);
      %objs++;
//    }
  }

  // Plug the top and bottom with ONE block each
//  %o = newObject("", InteriorShape, "iblock.dis");
//  GameBase::setPosition(%o, Vector::add(%pos, "0 0 " @ -%rad));
//  GameBase::setRotation(%o, 0);
//  addToSet(%group, %o);

//  %o = newObject("", InteriorShape, "iblock.dis");
//  GameBase::setPosition(%o, Vector::add(%pos, "0 0 " @ %rad));
//  GameBase::setRotation(%o, 0);
//  addToSet(%group, %o);

//  %objs += 2; // For the top and bottom

  echo("Used " @ %objs @ " iblock.dis interior shapes");

  focusClient();
}

function makeWalkway(%group, %pos, %rad, %jump) {
  focusServer();

  %objs = 0; // Count num objects it takes to make the globe

  // Make all the blocks around the sides, but leave a hole in the top and bottom
  for (%z = 0; %z < $PI * 2; %z += %jump) {
    %x = $PI;
      %o = newObject("", InteriorShape, "catwalkB.dis");
      %offset = Vector::getFromRot(%x @ " 0 " @ (%z+$PI/2), 16);
      GameBase::setPosition(%o, Vector::add(%pos, Vector::add(Vector::getFromRot(%x @ " 0 " @ %z, %rad), %offset)));
      GameBase::setRotation(%o, (%x-$PI) @ " 0 " @ %z);
      addToSet(%group, %o);

      %o = newObject("", InteriorShape, "catwalkB.dis");
      GameBase::setPosition(%o, Vector::add(Vector::add(%pos, Vector::sub(Vector::getFromRot(%x @ " 0 " @ %z, %rad), %offset)), "0 0 -0.01"));
      GameBase::setRotation(%o, (%x-$PI) @ " 0 " @ %z);
      addToSet(%group, %o);
      %objs += 2;
//    }
  }

  // Plug the top and bottom with ONE block each
//  %o = newObject("", InteriorShape, "iblock.dis");
//  GameBase::setPosition(%o, Vector::add(%pos, "0 0 " @ -%rad));
//  GameBase::setRotation(%o, 0);
//  addToSet(%group, %o);

//  %o = newObject("", InteriorShape, "iblock.dis");
//  GameBase::setPosition(%o, Vector::add(%pos, "0 0 " @ %rad));
//  GameBase::setRotation(%o, 0);
//  addToSet(%group, %o);

//  %objs += 2; // For the top and bottom

  echo("Used " @ %objs @ " iblock.dis interior shapes");

  focusClient();
}


function repeatObjectLinear(%group, %arg1, %arg2, %a, %n, %j, %rot, %arg3) {
  focusServer();
  %x = getWord(%a, 0);
  %objs = 0;
  for (%i = 0; %i < %n; %i++) {
    if (%arg3 == "") %o = newObject("", %arg1, %arg2);
    else             %o = newObject("", %arg1, %arg2, %arg3);
    GameBase::setPosition(%o, Vector::add(%a, Vector::mul(%j, %i)));
    GameBase::setRotation(%o, %rot);
    addToSet(%group, %o);
    %objs++;
  }
  echo(%objs, " objects created.");
  focusClient();
}


function makeHidingThingies(%group, %pos, %rad, %jump) {
  focusServer();

  %objs = 0; // Count num objects it takes to make the globe

  // Make all the blocks around the sides, but leave a hole in the top and bottom
  for (%z = 0; %z < $PI * 2; %z += %jump) {
    %x = $PI;
      %o = newObject("", InteriorShape, "BESCargo3.dis");
      %offset = Vector::getFromRot(%x @ " 0 " @ (%z+$PI/2), 12);
      GameBase::setPosition(%o, Vector::add(%pos, Vector::add(Vector::getFromRot(%x @ " 0 " @ %z, %rad), %offset)));
      GameBase::setRotation(%o, (%x-$PI/2) @ " 0 " @ %z);
      addToSet(%group, %o);

      %o = newObject("", InteriorShape, "BESCargo3.dis");
      GameBase::setPosition(%o, Vector::add(Vector::add(%pos, Vector::sub(Vector::getFromRot(%x @ " 0 " @ %z, %rad), %offset)), "0 0 -0.01"));
      GameBase::setRotation(%o, (%x-$PI/2) @ " 0 " @ %z);
      addToSet(%group, %o);
      %objs += 2;
  }

  echo("Used " @ %objs @ " iblock.dis interior shapes");

  focusClient();
}


function makeCylinderFloor(%group, %pos, %rad, %jump) {
  focusServer();

  %objs = 0; // Count num objects it takes to make the globe

  // Make all the blocks around the sides, but leave a hole in the top and bottom
  %zfight = 0;
  for (%z = 0; %z < $PI * 2; %z += %jump) {
    %x = $PI;
      %o = newObject("", InteriorShape, "iblock.dis");
      GameBase::setPosition(%o, Vector::add(%pos, Vector::getFromRot(%x @ " 0 " @ %z, %rad, (%zfight++)*0.05)));
      GameBase::setRotation(%o, (%x-$PI) @ " 0 " @ %z);
      addToSet(%group, %o);
      %objs++;
      %zfight%=5;
//    }
  }

  // Plug the top and bottom with ONE block each
//  %o = newObject("", InteriorShape, "iblock.dis");
//  GameBase::setPosition(%o, Vector::add(%pos, "0 0 " @ -%rad));
//  GameBase::setRotation(%o, 0);
//  addToSet(%group, %o);

//  %o = newObject("", InteriorShape, "iblock.dis");
//  GameBase::setPosition(%o, Vector::add(%pos, "0 0 " @ %rad));
//  GameBase::setRotation(%o, 0);
//  addToSet(%group, %o);

//  %objs += 2; // For the top and bottom

  echo("Used " @ %objs @ " iblock.dis interior shapes");

  focusClient();
}


function repeatObjectCylinder(%group, %arg1, %arg2, %pos, %rad, %jump, %rotOffset) {
  focusServer();

  %objs = 0; // Count num objects it takes to make the globe

  // Make all the blocks around the sides, but leave a hole in the top and bottom
  for (%z = 0; %z < $PI * 2; %z += %jump) {
    %x = $PI;
      %o = newObject("", %arg1, %arg2);
      GameBase::setPosition(%o, Vector::add(%pos, Vector::getFromRot(%x @ " 0 " @ %z, %rad)));
      GameBase::setRotation(%o, Vector::add("0 0 " @ %z, %rotOffset));
      addToSet(%group, %o);
      %objs++;
  }

  echo("Used " @ %objs @ " objects used.");

  focusClient();
}

function moveObjectsTogether(%set, %p) {
  %avgpos = "0 0 0";
  for (%i = 0; %i < Group::objectCount(%set); %i++) {
    %obj = Group::getObject(%set, %i);
    %pos = GameBase::getPosition(%obj);
    %avgpos = Vector::add(%pos, %avgpos);
  }
  %avgpos = Vector::mul(%avgpos, 1 / Group::objectCount(%set));

  for (%i = 0; %i < Group::objectCount(%set); %i++) {
    %obj = Group::getObject(%set, %i);
    %pos = GameBase::getPosition(%obj);
    %newpos = Vector::lerp(%pos, %avgpos, %p);
    GameBase::setPosition(%obj, %newpos);
  }
}

function addVehicleMarker(%s, %v) {
  if ($stopVM) return;
  %x = newObject("Marker"@$mc++, Marker, PathMarker);
  GameBase::setPosition(%x, GameBase::getPosition(%v));
  GameBase::setRotation(%x, GameBase::getRotation(%v));
  addToSet(%s, %x);
}

function doVehicleMarker(%s, %v) {
  addVehicleMarker(%s, %v);
  schedule("doVehicleMarker("@%s@","@%v@");", $incv);
}

$incv = 1.0;
function startVehicleMarker(%s, %v) {
  $stopVM = "";
  $mc = "";
  doVehicleMarker(%s, %v);
}
