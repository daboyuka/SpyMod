MarkerData NodeMarker {
   description = "Path Node";
	shapeFile = "dirArrows";
};
MarkerData NavMeshTriMarker {
   description = "Tri Centroid";
	shapeFile = "endarrow";
};

StaticShapeData NodeRender {
	shapeFile = "grenade";
	maxDamage = 10000000;
};

StaticShapeData HighlightRender {
	shapeFile = "shotgunex";
	maxDamage = 10000000;
	disableCollision = true;
};

//
// navmesh (SimGroup)
//   .lastNodeID (int or "")
//   \Nodes (SimGroup)
//     \<anon> (Marker)
//       .id (int)
//   \Tris (SimGroup)
//     \<anon> (Marker)
//       .id (int)
//       .edges ("x y z" for nodes x, y, z)
//

function NavMesh::new(%name) {
	%nm = newObject(%name, SimGroup);
	addToSet(%nm, newObject("Nodes", SimGroup));
	addToSet(%nm, newObject("Tris", SimGroup));
	return %nm;
}

function NavMesh::addNode(%nm, %pos) {
	// Create marker
	%m = newObject("", Marker, NodeMarker);
	%m.id = %nm.lastNodeID++;
	GameBase::setPosition(%m, %pos);
	addToSet(%nm @ "\\Nodes", %m);
	
	// Other updates
	NavMesh::computeNodeLookup(%nm, %m);
	NavMesh::render::updateNode(%nm, %m);
	
	return %m;
}

// returns true iff the triangle formed by the given points is wound counterclockwise
function NavMesh::isTriCCW(%pos0, %pos1, %pos2) {
	%e01 = Vector::sub(%pos1, %pos0);
	%e12 = Vector::sub(%pos2, %pos1);
	%ecross = Vector::cross(%e01, %e12);
	%vertDot = Vector::dot(%ecross, "0 0 1");
	return %vertDot >= 0;
}

function NavMesh::addTri(%nm, %m0, %m1, %m2) {
	// Cycle vertex order until least ID is first
	for (%i=1; %i<3; %i++) {
		if (%m0.id > %m[%i].id) {
			%tmp = %m0; %m0 = %m[%i]; %m[%i] = %tmp;
		}
	}

	// Grab positions
	for (%i=0; %i<3; %i++) %pos[%i] = GameBase::getPosition(%m[%i]);

	// Reverse vertex order if tri upside down
	if (!NavMesh::isTriCCW(%pos0, %pos1, %pos2)) {
		%tmp = %m0; %m0 = %m1; %m1 = %tmp;
		%tmp = %pos0; %pos0 = %pos1; %pos1 = %tmp;
	}
	
	// Compute centroid
	for (%i=0; %i<3; %i++) %centroid = Vector::add(%centroid, %pos[%i]);
	%centroid = Vector::mul(%centroid, 1/%i);

	// Create tri; guaranteed first marker is minimum ID, and oriented to positive Z
	%t = newObject("", Marker, NavMeshTriMarker);
	%t.id = %nm.lastTriID++;
	%t.edges = %m0.id @ " " @ %m1.id @ " " @ %m2.id;
	GameBase::setPosition(%t, %centroid);
    addToSet(%nm @ "\\Tris", %t);

	// Other updates
	NavMesh::computeTriLookup(%nm, %t);
	NavMesh::render::updateTri(%nm, %t);

	return %t;
}

function NavMesh::moveNode(%nm, %m, %newPos) {
	GameBase::setPosition(%m, %newPos);
	NavMesh::render::updateNode(%nm, %m);

	%tris = nameToID(%nm @ "\\Tris");
	for (%i = 0; (%t = Group::getObject(%tris, %i)) != -1; %i++) {
		if (NavMesh::triContainsNode(%nm, %t, %m))
			NavMesh::render::updateTri(%nm, %t);
	}
}

function NavMesh::triContainsNode(%nm, %t, %m) {
	for (%i = 0; %i < 3; %i++) {
		if (getWord(%t.edges, %i) == %m.id) return true;
	}
	return false;
}

function NavMesh::triEdge(%nm, %t, %i) {
	return getWord(%t.edges, %i) @ " " @ getWord(%t.edges, (%i+1)%3);
}

function NavMesh::findContainingTri(%nm, %pos, %h) {
	%tris = nameToID(%nm @ "\\Tris");
	for (%i = 0; (%t = Group::getObject(%tris, %i)) != -1; %i++) {
		if (NavMesh::triContainsPos(%nm, %t, %pos, %h))
			return %t;
	}
	return "";
}

// returns %pos projected to the plane of triangle %t, or -1 if it's outside [%minH, %maxH]
// distance from the plane (%minH/%maxH == "" means unbounded in that direction)
function NavMesh::projectToTriPlane(%nm, %t, %pos, %minH, %maxH) {
	for (%i=0; %i<3; %i++) %pos[%i] = GameBase::getPosition(%nm.nodes[getWord(%t.edges, %i)]);
	
	%posRel = Vector::sub(%pos, %pos0);
	
	%v01 = Vector::sub(%pos1, %pos0);
	%v02 = Vector::sub(%pos2, %pos0);

	%vnorm = Vector::resize(Vector::cross(%v01, %v02), 1);
	%posRelVertDist = Vector::dot(%vnorm, %posRel);
	if (%minH != "" && %posRelVertDist < %minH) return -1;
	if (%maxH != "" && %posRelVertDist > %maxH) return -1;
	
	%posRelVert = Vector::mul(%vnorm, %posRelVertDist);
	%posInPlane = Vector::sub(%pos, %posRelVert);
	return %posInPlane;
}

function NavMesh::triContainsPos(%nm, %t, %pos, %h) {
	if ((%posInPlane = NavMesh::projectToTriPlane(%nm, %t, %pos, -%h, %h)) == -1) return false;
		
	for (%i=0; %i<3; %i++) %pos[%i] = GameBase::getPosition(%nm.nodes[getWord(%t.edges, %i)]);
	%posRelInPlane = Vector::sub(%posInPlane, %pos0);

	// Taken from https://blackpawn.com/texts/pointinpoly/ with great thievery

	%v01 = Vector::sub(%pos1, %pos0);
	%v02 = Vector::sub(%pos2, %pos0);

	%v01L2 = Vector::lengthSquared(%v01);
	%v02L2 = Vector::lengthSquared(%v02);
	%angleDot = Vector::dot(%v01, %v02);
		
	%v01PosDot = Vector::dot(%posRelInPlane, %v01);
	%v02PosDot = Vector::dot(%posRelInPlane, %v02);
	
	%v01Interp = (%v01L2 * %v02PosDot - %angleDot * %v01PosDot);
	if (%v01Interp < 0) return false;
	
	%v02Interp = (%v02L2 * %v01PosDot - %angleDot * %v02PosDot);
	if (%v02Interp < 0) return false;
	
	%interpScale = %v01L2 * %v02L2 - %angleDot * %angleDot;
	return (%v01Interp + %v02Interp) <= %interpScale;
}

function NavMesh::shortestTriPath(%nm, %t1, %t2) {
	%h = Heap::new(); // entries: "dist tri prevTri"
	// %shortpath[%t] = prev tri
	// %shortpathDist[%t] = dist
	
	Heap::push(%h, "0 " @ %t1);
	while (Heap::size(%h) > 0 && %shortpath[%t2] == "") {
		%entry = Heap::pop(%h);

		%t = getWord(%entry, 1);
		if (%shortpath[%t] != "") continue; // already locked in shortest path

		%dist = getWord(%entry, 0);
		%tprev = getWord(%entry, 2);
		
		%shortpath[%t] = %tprev;
		%shortpathDist[%t] = %dist;
		
		%pos = GameBase::getPosition(%t);
		
		%tnbrs = NavMesh::triNeighbors(%nm, %t);
		for (%i = 0; (%tnbr = getWord(%tnbrs, %i)) != -1; %i++) {
			if (%shortpath[%tnbr] != "") continue;

			%pos2 = GameBase::getPosition(%tnbr);
			%nbrdist = %dist + Vector::getDistance(%pos, %pos2);
			Heap::push(%h, %nbrdist @ " " @ %tnbr @ " " @ %t);
		}
	}
	
	for (%cur = %t2; %cur != %t1; %cur = %shortpath[%cur]) {
		%path = %cur @ " " @ %path;
	}
	%path = %t1 @ " " @ %path;
	
	echo("NavMesh::shortestTriPath ", %shortestDist[%t2], " ", %path);
	return %path;
}

// returns neighbor tris as words
function NavMesh::triNeighbors(%nm, %t) {
	%prevMId = getWord(%t.edges, 0);
	for (%j = 0; %j < 3; %j++) {
		%nextMId = getWord(%t.edges, (%j+1)%3);
		%revEdge = %nextMId @ " " @ %prevMId;
		if ((%ntri = %nm.edgeToTri[%revEdge]) != "") {
			%ntris = %ntris @ " " @ %ntri;
		}
		%prevMId = %nextMId;
	}
	return %ntris;
}

function NavMesh::stringpullDraw(%nm, %sp, %clean) {
	for (%i=0; (%step = %sp.step[%i]) != ""; %i++) {
		%side = nextWord(%step);
		%step = popWord(%step);
		%fromPos = Vector::add(Matrix::subMatrix(%step, 3, 2, 3, 1, 0, 0), "0 0 0.25");
		%toPos = Vector::add(Matrix::subMatrix(%step, 3, 2, 3, 1, 0, 1), "0 0 0.25");
		
		if (Vector::getDistance(%fromPos, %toPos) < 0.01) %toPos = Vector::add(%fromPos, "0 0 1");
		
		%color = GeomDraw::BlueLaser;
		if (%side == 1) %color = GeomDraw::GreenLaser;
		
		%coneVar = %sp @ ".cone[" @ %side @ "]";
		%eval = "if (" @ %coneVar @ " != \"\") deleteObject(" @ %coneVar @ "); " @ %coneVar @ " = GeomDraw::line(\"" @ %fromPos @ "\", \"" @ %toPos @ "\", " @ %color @ ", " @ %clean @ ");";
		schedule(%eval, %i);
	}
	
	schedule("for (%i=0;%i<2;%i++) if (" @ %sp @ ".cone[%i] != \"\") deleteObject(" @ %sp @ ".cone[%i]);", %i);
}

// refineTriPath returns a series of waypoints as a refinement of %path using string-pulling
function NavMesh::refineTriPath(%nm, %fromPos, %toPos, %path) {
	%debug = newObject("", SimSet);
	%debug.lastStep = -1;
	addToSet(MissionCleanup, %debug);
	
	%anchor = -1; // -1 for fromPos, otherwise a node
	%anchorPos = %fromPos;
	for (%i=0; %i<2; %i++) {
		// %coneTip[%i] = %anchorPos; // %coneTip winds CCW relative to %anchorPos; %coneTip0 is right of %coneTip1
		%coneTipTri[%i] = 0;
	}

	%outpath = %fromPos;

	%i = 0;
	for (%stop = 0; %stop < 100; %stop++) {
		%prevTri = getWord(%path, %i);
		%nextTri = getWord(%path, %i+1);
		if (%prevTri == -1) break;
		
		if (%nextTri == -1) {
			for (%j=0; %j<2; %j++) {
				%nextConeTip[%j] = -1;
				%nextConeTipPos[%j] = %toPos;
			}
		} else {
			%nextEdge = NavMesh::sharedEdge(%nm, %prevTri, %nextTri);
			if (!assert(%portalEdge != -1, "somehow got missing portal edge from %1 to %2", %prevTri, %nextTri)) return;

			for (%j=0; %j<2; %j++) {
				%nextConeTip[%j] = getWord(%nextEdge, %j);
				%nextConeTipPos[%j] = GameBase::getPosition(%nm.nodes[%nextConeTip[%j]]);
			}
		}

		echo("step ", %i, " ", %nextConeTip[0], " ", %nextConeTip[1]);

		if (%anchor != -1 && (%anchor == %nextConeTip[0] || %anchor == %nextConeTip[1])) {
			echo("pivoting around ", %anchor);
			%i++;
			continue;
		}		
		
		%resetToTri = "";
		for (%j=0; %j<2; %j++) {
			%inwardCCW = (%j == 0);
			
			%advance = "";
			if (%coneTip[%j] == "") {
				echo("tip", %j, " first set");
				%advance = true;
			} else if (%coneTip[%j] == %nextConeTip[%j]) {
				echo("tip", %j, " unchanged");
				%coneTipTri[%j] = %i+1;
				continue; // do nothing (no move)
			} else if (%inwardCCW != NavMesh::isTriCCW(%anchorPos, %coneTipPos[%j], %nextConeTipPos[%j])) { // outside, same side
				echo("tip", %j, " outside same side");
				continue; // do nothing (can't expand funnel)
			} else if (%inwardCCW != NavMesh::isTriCCW(%anchorPos, %coneTipPos[1-%j], %nextConeTipPos[%j])) { // inside
				echo("tip", %j, " inside");
				%advance = true;
			} else { // outside, other side
				echo("tip", %j, " outside other side, reset to ", %coneTipTri[1-%j], " anchor ", %a);
				%anchor = %coneTip[1-%j];
				%anchorPos = %coneTipPos[1-%j];

				%coneTip[0] = %coneTip[1] = "";
				%coneTipPos[0] = %coneTipPos[1] = "";
				%resetToTri = %coneTipTri[0] = %coneTipTri[1] = %coneTipTri[1-%j];
				%debug.step[%debug.lastStep++] = "0 " @ %anchorPos @ " " @ %anchorPos;
				%debug.step[%debug.lastStep++] = "1 " @ %anchorPos @ " " @ %anchorPos;
				
				%outpath = %outpath @ " " @ %anchorPos;
				
				break;
			}

			if (%advance) {
				echo("tip", %j, " advance to ", %nextConeTip[%j]);
				%coneTip[%j] = %nextConeTip[%j];
				%coneTipPos[%j] = %nextConeTipPos[%j];
				%coneTipTri[%j] = %i+1;
				%debug.step[%debug.lastStep++] = %j @ " " @ %anchorPos @ " " @ %coneTipPos[%j];
			}
		}
		if (%resetToTri) {
			%i = %resetToTri;
			continue;
		}

		%i++;
	}
	
	%outpath = %outpath @ " " @ %toPos;
	
	return %debug @ " " @ %outpath;
}

// returns the common edge ("x y" with nodes x and y) joining t1 and t2 (in order as in t1), or -1 if not adjacent
function NavMesh::sharedEdge(%nm, %t1, %t2) {
	for (%i=0; %i<3; %i++) {
		%e = NavMesh::triEdge(%nm, %t1, %i);
		%e2 = getWord(%e, 1) @ " " @ getWord(%e, 0); // reverse edge
		if (%nm.edgeToTri[%e2] == %t2) {
			return %e;
		}
	}
	return -1;
}

function NavMesh::computeNodeLookup(%nm, %m) { %nm.nodes[%m.id] = %m; }
function NavMesh::uncomputeNodeLookup(%nm, %m) { %nm.nodes[%m.id] = ""; }

function NavMesh::computeTriLookup(%nm, %t, %clear) {
	%nm.tris[%t.id] = tern(%clear,"",%t);

	%prevMId = getWord(%t.edges, 0);
	for (%i = 0; %i < 3; %i++) {
		%nextMId = getWord(%t.edges, (%i+1)%3);
		%edge = %prevMId @ " " @ %nextMId;
		%nm.edgeToTri[%edge] = tern(%clear,"",%t);
		echo("update ", %edge, " clear? ", %clear, " -> ", tern(%clear,"",%t));
		%prevMId = %nextMId;
	}
}
function NavMesh::uncomputeTriLookup(%nm, %t) { NavMesh::computeTriLookup(%nm, %t, true); }

function NavMesh::computeLookups(%nm) {
	%nodes = nameToID(%nm @ "\\Nodes");
	%tris = nameToID(%nm @ "\\Tris");
	for (%i = 0; (%m = Group::getObject(%nodes, %i)) != -1; %i++) NavMesh::computeNodeLookup(%nm, %m);
	for (%i = 0; (%t = Group::getObject(%tris, %i)) != -1; %i++) NavMesh::computeTriLookup(%nm, %t);
}
function NavMesh::uncomputeLookups(%nm) {
	%nodes = nameToID(%nm @ "\\Nodes");
	%tris = nameToID(%nm @ "\\Tris");
	for (%i = 0; (%m = Group::getObject(%nodes, %i)) != -1; %i++) NavMesh::uncomputeNodeLookup(%nm, %m);
	for (%i = 0; (%t = Group::getObject(%tris, %i)) != -1; %i++) NavMesh::uncomputeTriLookup(%nm, %t);
}

function NavMesh::import(%filename) {
	%nm = Group::instantFile(%filename);
	NavMesh::computeLookups(%nm);
	return %nm;
}

function NavMesh::export(%nm, %filename) {
	%renderClean = getGroup(%nm.render);
	NavMesh::render::stop(%nm);
	NavMesh::uncomputeLookups(%nm);

	exportObjectToScript(%nm, %filename);
	
	NavMesh::computeLookups(%nm);
	if (%renderClean) NavMesh::render::start(%nm, %renderClean);
}

function NavMesh::render::deleteNode(%nm, %m) {
	if (%nm.render == "" || %m.render == "") return;
	deleteObject(%m.render);
	%m.render = "";
}
function NavMesh::render::updateNode(%nm, %m) {
	if (%nm.render == "") return;
	if (%m.render == "") {
		%r = newObject("", StaticShape, NodeRender, true);
		GameBase::playSequence(%r, 0, ambient);
		addToSet(%nm.render, %r);
		%r.node = %m;
		%m.render = %r;
	} else {
		%r = %m.render;
	}
	
	GameBase::setPosition(%r, GameBase::getPosition(%m));
	if (%r.active)	GameBase::playSequence(%m.render, 0, ambient);
	else			GameBase::stopSequence(%m.render, 0);
}

function NavMesh::render::activateNode(%nm, %m) { %m.render.active = true; NavMesh::render::updateNode(%nm, %m); }
function NavMesh::render::deactivateNode(%nm, %m) { %m.render.active = ""; NavMesh::render::updateNode(%nm, %m); }

function NavMesh::render::deleteTri(%nm, %t) {
	if (%nm.render == "" || %t.render == "") return;
	deleteObject(%t.render);
	%t.render = "";
}
function NavMesh::render::updateTri(%nm, %t) {
	if (%nm.render == "") return;

	if (%t.render == "") {
		%r = newObject("", SimGroup);
		addToSet(%nm.render, %r);
		%r.tri = %t;
		%t.render = %r;
	} else {
		%r = %t.render;
	}
	
	%nodes = nameToID(%nm @ "\\Nodes");
	for (%i = 0; %i < 3; %i++) {
		%mId = getWord(%t.edges, %i);
		%m = %nm.nodes[%mId];
		%p = %p @ " " @ GameBase::getPosition(%m);
	}

	while ((%obj = Group::getObject(%r, 0)) != -1) deleteObject(%obj); // clear set
	
	GeomDraw::path(%p, true, tern(%r.active, GeomDraw::BlueLaser, GeomDraw::RedLaser), %r);
}

function NavMesh::render::activateTri(%nm, %t) { %t.render.active = true; NavMesh::render::updateTri(%nm, %t); }
function NavMesh::render::deactivateTri(%nm, %t) { %t.render.active = ""; NavMesh::render::updateTri(%nm, %t); }

function NavMesh::render::start(%nm, %clean) {
	if (%clean == "") %clean = MissionCleanup;
	if (%nm.render == "") {
		%nm.render = newObject("NavMeshRender", SimGroup);
		addToSet(%clean, %nm.render);
	}

	%nodes = nameToID(%nm @ "\\Nodes");
	%tris = nameToID(%nm @ "\\Tris");
	for (%i = 0; (%m = Group::getObject(%nodes, %i)) != -1; %i++) NavMesh::render::updateNode(%nm, %m);
	for (%i = 0; (%t = Group::getObject(%tris, %i)) != -1; %i++) NavMesh::render::updateTri(%nm, %t);
}

function NavMesh::render::stop(%nm) {
	%nodes = nameToID(%nm @ "\\Nodes");
	%tris = nameToID(%nm @ "\\Tris");
	for (%i = 0; (%m = Group::getObject(%nodes, %i)) != -1; %i++) NavMesh::render::deleteNode(%nm, %m);
	for (%i = 0; (%t = Group::getObject(%tris, %i)) != -1; %i++) NavMesh::render::deleteTri(%nm, %t);
	if (%nm.render) {
		deleteObject(%nm.render);
		%nm.render = "";
	}
}

$NavMesh::standoff = 0.1;

function NavMesh::scriptgun(%clientId, %nm) {
	%player = Client::getOwnedObject(%clientId);
	%player.navmesh = %nm;
	Player::mountItem(%player, Scriptgun, 0);
	
	Scriptgun::setModes(
		"Add Node",
		"Move Node",
		"Add Tri",
		"Find Tri Path"
	);
	function sgpress(%player, %mode, %lookvec, %hitobj, %hitpos, %hitnorm) {
		%nm = %player.navmesh;
		if (%nm == "" || %hitobj == "") return;
		
		if (NavMesh::scriptgun::isNode(%nm, %hitobj)) echo("NavMesh node: ", %hitobj.id, " ", %hitobj);
		
		if (%mode == 0) {
			%addpos = Vector::add(%hitpos, Vector::mul(%hitnorm, $NavMesh::standoff));
			%m = NavMesh::addNode(%player.navmesh, %addpos);
			echo("NavMesh::AddNode ", %m);
		} else if (%mode == 1) {
			if (!NavMesh::scriptgun::isNode(%nm, %hitobj)) return;
			%m = %hitobj.node;
			%player.navmeshSelNode = %m;
			%player.navmeshSelPos = GameBase::getPosition(%m);
			NavMesh::render::activateNode(%m);
			echo("NavMesh::MoveStart ", %m, " ", %player.navmeshSelNode);
		} else if (%mode == 2) {
			if (NavMesh::scriptgun::isNode(%nm, %hitobj)) {
				%m = %hitobj.node;
			}

			for (%nidx = 0; %nidx < 3; %nidx++) {
				%m[%nidx] = %player.navmeshSelNode[%nidx];
				if (%m[%nidx] == "" || %m[%nidx] == %m) {
					break;
				}
			}

			if (%m != "" && %nidx < 3) {
				%player.navmeshSelNode[%nidx] = %m[%nidx] = %m;
				echo("NavMesh::AddTriSelect ", %nidx, " ", %m, " ", %player.navmeshSelNode[%nidx], " ", %m[%nidx]);
				%nidx++;
			}

			if (%nidx == 3) {
				NavMesh::addTri(%nm, %m[0], %m[1], %m[2]);
				echo("NavMesh::AddTri ", %m[0], " ", %m[1], " ", %m[2]);
				%m = "";
			}
			
			if (%m == "") for (%i = 0; %i < 3; %i++) %player.navmeshSelNode[%i] = "";
		} else if (%mode == 3) {
			%t = NavMesh::findContainingTri(%nm, %hitpos, 5);

			if (%t == "") {
				if (%player.navmeshPathRender != "") {
					deleteObject(%player.navmeshPathRender);
					%player.navmeshPathRender = "";
				}
			}

			for (%tidx = 0; %tidx < 2; %tidx++) {
				%t[%tidx] = %player.navmeshSelTri[%tidx];
				if (%t[%tidx] == "" || %t[%tidx] == %t) {
					break;
				}
				%hitpos[%tidx] = %player.navmeshHitPos[%tidx];
			}

			if (%t != "" && %tidx < 2) {
				%player.navmeshSelTri[%tidx] = %t[%tidx] = %t;
				
				%hitposProj = NavMesh::projectToTriPlane(%nm, %t, %hitpos);

				%player.navmeshHitPos[%tidx] = %hitpos[%tidx] = %hitposProj;
				NavMesh::render::activateTri(%nm, %t);
				echo("NavMesh::FindTriPathSelect ", %tidx, " ", %t, " ", %player.navmeshSelTri[%tidx], " ", %t[%tidx]);
				%tidx++;
			}

			if (%tidx == 2) {
				%pathtris = NavMesh::shortestTriPath(%nm, %t0, %t1);
				echo("NavMesh::FindTriPath ", %t0, " ", %t1, " ", %pathtris);
				
				if (%player.navmeshPathRender != "") {
					deleteObject(%player.navmeshPathRender);
					%player.navmeshPathRender = "";
				}
				
				%player.navmeshPathRender = newObject("", SimGroup);
				addToSet(MissionCleanup, %player.navmeshPathRender);
				for (%i = 0; (%pathT = getWord(%pathtris, %i)) != -1; %i++) {
					%path = %path @ " " @ GameBase::getPosition(%pathT);
					echo("Path Tri ", %pathT, " ", %pathT.edges);
				}
				//GeomDraw::path(%path, false, GeomDraw::BlueLaser, %player.navmeshPathRender);
				
				%pathref = NavMesh::refineTriPath(%nm, %hitpos[0], %hitpos[1], %pathtris);
				%debug = nextWord(%pathref);
				%pathref = popWord(%pathref);
				GeomDraw::path(%pathref, false, GeomDraw::GreenLaser, %player.navmeshPathRender);
				
				// Render
				Navmesh::stringpullDraw(%nm, %debug, MissionCleanup);
				
				%t = "";
			}
			
			if (%t == "") {
				for (%i = 0; %i < 2; %i++) {
					NavMesh::render::deactivateTri(%nm, %player.navmeshSelTri[%i]);
					%player.navmeshSelTri[%i] = "";
					%player.navmeshHitPos[%i] = "";
				}
			}
		}
	}
	function sgrelease(%player, %mode, %lookvec, %hitobj, %hitpos, %hitnorm) {
		%nm = %player.navmesh;
		if (%nm == "") return;
		if (%mode == 1) {
			if ((%m = %player.navmeshSelNode) == "") return;
			if (%hitobj != "") {
				%newPos = Vector::add(%hitpos, Vector::mul(%hitnorm, $NavMesh::standoff));
				NavMesh::moveNode(%nm, %m, %newPos);
			}
			NavMesh::render::deactivateNode(%m);
			%player.navmeshSelNode = "";
			%player.navmeshSelPos = "";
			echo("NavMesh::MoveEnd ", %m, "->", %newPos);
		}
	}
}

function NavMesh::scriptgun::isNode(%nm, %mr) { return getGroup(%mr.node) == nameToID(%nm @ "\\Nodes"); }