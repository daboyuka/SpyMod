// Put experimental code here; executed near end of mission load
echo("Experiments!");

function testnm(%filename) {
	%focusclient = !isServerFocused();
	focusServer();

	if ($nm != "") deleteObject($nm);
	
	if (%filename != "") $nm = NavMesh::import(%filename);
	else                 $nm = NavMesh::new(mynavmesh);
	
	addToSet(MissionCleanup, $nm);
	NavMesh::render::start($nm, MissionCleanup);
	NavMesh::scriptgun(2049, $nm);
	echo("selected navmesh: ", Client::getOwnedObject(2049).navmesh);
	
	if (%focusclient) focusClient();
}
