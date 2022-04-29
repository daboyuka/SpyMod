// WARNING:
// displayDIS (due to Measure::checkShapePlane underneath) can CRASH the server for weird memory reasons.
// The only reliable fix I have found is to fire a gun several times (>10); I think the projectiles
// (de)spawning preallocate some kind of buffer. I haven't been able to replicate the fix, even with
// Projectile::spawnProjectile...

$brokenDIS["challs2.dis"] = true;
$brokenDIS["Drock.dis"] = true;
$brokenDIS["Dcolumn.dis"] = true;
$brokenDIS["Dbridge.dis"] = true;
$brokenDIS["siam.0.dis"] = true;
$brokenDIS["siam.1.dis"] = true;
$brokenDIS["cc_wall.1.dis"] = true;

$disRowLen = 20;

function displayDIS(%pat, %pos, %padding, %clean) {
	focusServer();
	$Console::logMode = 1; // this is liable to crash, so keep logs for debugging
	
	if (%pat == "") %pat = "*.dis";
	if (%pos == "") %pos = "0 0 0";
	if (%padding == "") %padding = 1;
	if (%clean == "") %clean = MissionCleanup;

    %outset = newObject("displayDIS", SimGroup);
	addToSet(%clean, %outset);

	%idx = 0;
	for (%fn = File::findFirst(%pat); %fn != ""; %fn = File::findNext(%pat)) {
		if ($brokenDIS[%fn]) continue;
		if (%seen[%fn]) continue;
		if ((%middle = middleDotted(%fn)) != "" && %middle != "0") continue;
		%seen[%fn] = true;
		$disToDisplay[%idx++ - 1] = %fn;
	}
	$disToDisplay[%idx++ - 1] = "";

	// containerBoxFillSet(%outset, 4, "0 0 0", 1000, 1000, 1000);
	// for (%i=0;%i<100;%i++)containerBoxFillSet(%outset, 4, "0 0 0", 1000, 1000, 1000);
	// schedule("containerBoxFillSet(" @ %outset @ ", 4, \"0 0 0\", 1000, 1000, 1000);", 0);

	displayNextDISSched(%pat, %pos, %padding, %outset, 0, %pos, 0);

	return %outset;
}

function middleDotted(%s) {
	%off1 = String::findSubStr(%s, ".");
	%off2 = String::findSubStr(String::getSubStr(%s, %off1+1, 99999), ".");
	if (%off1 != -1 && %off2 != -1) return String::getSubStr(%s, %off1+1, %off2);
	return "";
}

function displayNextDISSched(%pat, %pos, %padding, %clean, %idx, %rowStartPos, %maxExtY) {
	%delay = 0;
	if (%idx % 1 == 0) %delay = 2/32;
	schedule("displayNextDIS(\"" @ %pat @ "\", \"" @ %pos @ "\", \"" @ %padding @ "\", \"" @ %clean @ "\", " @ %idx @ ", \"" @ %rowStartPos @ "\", " @ %maxExtY @ ");", %delay);
}

function displayNextDIS(%pat, %pos, %padding, %clean, %idx, %rowStartPos, %maxExtY) {
	focusServer();
	
	%fn = $disToDisplay[%idx];
	if (%fn == "") return;
	
	if (%idx % $disRowLen == 0 && %idx > 0) {
		%pos = %rowStartPos = Vector::add(%rowStartPos, "0 " @ (%maxExtY + %padding) @ " 0");
		%maxExtY = 0;
	}
	
	echo("spawning ", %fn);
	%obj = newObject("", InteriorShape, %fn);
	echo("spawned ", %fn);

	%box = Measure::measure(%obj);
	echo("measured ", %fn);

	for (%i=0;%i<3;%i++) %off = %off @ " " @ getWord(%box, %i);
	%extX = getWord(%box, 3);
	%extY = getWord(%box, 4);
	if (%maxExtY < %extY) %maxExtY = %extY;

	GameBase::setPosition(%obj, Vector::add(%pos, %off));
	addToSet(%clean, %obj);
	echo("added to cleanup ", %fn);

	%pos = Vector::add(%pos, (%extX + %padding) @ " 0 0");
	
	displayNextDISSched(%pat, %pos, %padding, %clean, %idx+1, %rowStartPos, %maxExtY);
}

// Measure::measure measures the bounding box of %obj, to a tolerance of %eps (default 0.0625), returning:
//   "OX OY OZ EX EY EZ"
// where "OX OY OZ" is the offset of the centerpoint within the object's bounding box, and
// "EX EY EZ" is the extent of the bounding box.
//
// This function uses containerBoxFillSet, which is somewhat inaccurate (overestimating).
function Measure::measure(%obj, %eps) {
	%tmpset = newObject("tmp", SimSet);
	
	for (%i=0; %i<10; %i++) { addToSet(%tmpset, (%shim[%i] = newObject("shim"@%i, SimSet))); }
	
	for (%i = 0; %i < 3; %i++) %off = %off @ " " @ Measure::measureDim(%obj, Vector::basis(%i, -1), %tmpset, %eps);
	for (%i = 0; %i < 3; %i++) %ext = %ext @ " " @ Measure::measureDim(%obj, Vector::basis(%i, 1), %tmpset, %eps);

	for (%i=0; %i<10; %i++) { deleteObject(%shim[%i]); %shim[%i] = ""; }
	deleteObject(%tmpset);
	
	return %off @ " " @ Vector::add(%off, %ext);
}

function Measure::draw(%obj, %box, %clean) {
	if (%clean == "") %clean = MissionCleanup;

	%off = Matrix::subMatrix(%box, 3, 2, 3, 1, 0, 0);
	%ext = Matrix::subMatrix(%box, 3, 2, 3, 1, 0, 1);
	%pos = GameBase::getPosition(%obj);
	for (%i = 0; %i < 3; %i++) {
		%basis = Vector::basis(%i, getWord(%ext, %i));
		
		%bot = Vector::sub(%pos, %off);
		%top = Vector::add(%bot, %ext);
		echo(%bot, "->", Vector::add(%bot, %basis));
		echo(%top, "->", Vector::sub(%bot, %basis));
		GeomDraw::line(%bot, Vector::add(%bot, %basis), GeomDraw::BlueLaser, %clean);
		GeomDraw::line(%top, Vector::sub(%top, %basis), GeomDraw::BlueLaser, %clean);
	}
}

function Vector::basis(%dim, %len) {
	for (%i = 0; %i < 3; %i++) { if (%i == %dim) %v = %v @ " " @ %len; else %v = %v @ " 0"; }
	return %v;
}

function Vector::resize(%vec, %len) {
	%scale = %len / Vector::getDistance(%vec, "0 0 0");
	for (%i = 0; %i < 3; %i++) %outvec = %outvec @ " " @ getWord(%vec, %i) * %scale;
	return %outvec;
}

function Measure::measureDim(%obj, %dimVec, %tmpset, %eps) {
	if (%tmpset == "") { echo("missing %tmpset"); return; }
	if (%eps == "") %eps = 0.625;
	
	// First gallop to find upper bound
	%lb = 0;
	%ub = 1;
	for (%stop = 0; %stop < 1000 && Measure::checkShapePlane(%obj, Vector::resize(%dimVec, %ub), %tmpset); %stop++) {
		%lb = %ub;
		%ub <<= 1;
	}
	if (%stop == 1000) echo("warning: hit max galloping iterations");
	
	for (%stop = 0; %stop < 1000 && (%ub - %lb) > %eps; %stop++) {
		%mid = (%lb + %ub) / 2;
		if (Measure::checkShapePlane(%obj, Vector::resize(%dimVec, %mid), %tmpset)) %lb = %mid;
		else %ub = %mid;
	}
	if (%stop == 1000) echo("warning: hit max binary search iterations");
	
	return %ub;
}

function Measure::checkShapePlane(%obj, %planeVec, %tmpset) {
	%pos = GameBase::getPosition(%obj);
	%planeCenter = Vector::add(%pos, %planeVec);
	
	%boxSize = 1000;
	%boxCenter = Vector::add(%planeCenter, Vector::resize(%planeVec, %boxSize/2));
	
	echo("checkShapePlane ", Group::getObject(%tmpset, 0));
	removeFromSet(%tmpset, %obj);
	containerBoxFillSet(%tmpset, $SimInteriorObjectType, %boxCenter, %boxSize, %boxSize, %boxSize);
	
	for (%i = 0; (%check = Group::getObject(%tmpset, %i)) != -1; %i++) { if (%check == %obj) { %found = true; break; } }
	removeFromSet(%tmpset, %obj);
	return %found;
}

//for(%i=0;%i<100;%i++)containerBoxFillSet(MissionCleanup, 4, "0 0 0", 1000, 1000, 1000);