
function Graph::new() {
  return newObject("Graph", SimGroup);
}

function Graph::addNode(%g, %l) {
	%id = %g.nextID++;
	%g.node[%id] = %l;
	return %id;
}

function Graph::addEdge(%g, %a, %b, %l) {
	%g.fedge_[%a, %b] = %l;
	%g.redge_[%b, %a] = %l;
	%g.fedges[%a] = %g.fedges[%a] @ " " @ $b;
	%g.redges[%b] = %g.redges[%b] @ " " @ $a;
}

function Graph::edgeLabel(%g, %a, %b) { return %g.fedge_[%a, %b]; }
function Graph::inNeighbors(%g, %a) { return %g.redges[%a]; }
function Graph::outNeighbors(%g, %a) { return %g.fedges[%a]; }
