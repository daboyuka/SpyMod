
function Group::indexOf(%group, %obj) {
  %obj = nameToID(%obj);

  %n = Group::objectCount(%group);
  for (%i = 0; %i < %n; %i++)
    if (Group::getObject(%group, %i) == %obj)
      return %i;
  return -1;
}

function Group::getNextSibling(%group, %obj) {
  %n = Group::objectCount(%group);
  %i = Group::indexOf(%group, %obj);
  if (%i == -1 || %i == %n - 1) return -1;
  return Group::getObject(%group, %i + 1);
}

// Returns argument if argument is not a set, or is a set with no children
function Group::getFirstDescendant(%obj) {
  while (Group::objectCount(%obj) != 0) {
    %obj = Group::getObject(%obj, 0);
  }
  return %obj;
}

// Returns the next leaf after %obj in the in-order traversal of the tree rooted at %topgroup.
// If %obj is not a leaf, returns the first leaf under %obj
// If %obj is determined to not reside in the subtree under %topgroup, returns -1 (NOTE: This is not guaranteed to be detected!)
function Group::getNextLeaf(%topgroup, %obj) {
  if (Group::objectCount(%obj) != 0)
    return Group::getFirstDescendant(%obj);

  %topgroup = nameToID(%topgroup);

  %grp = getGroup(%obj);
  %next = Group::getNextSibling(%grp, %obj);
  while (%next == -1 && %grp != %topgroup) {
    %next = %grp;          // %next up
    %grp = getGroup(%grp); // %grp up
    %next = Group::getNextSibling(%grp, %next); // %next over within %grp
  }

  if (%next == -1) return -1;
  return Group::getFirstDescendant(%next);
}

sethelp("Group::instantFile", "filename ?group", "Loads the instant objects in filename into the given group (or group 0 if omitted)");
function Group::instantFile(%filename, %group) {
	if (%group == "") %group = 0;

	setInstantGroup(%group);

	%oldcount = Group::objectCount(%group);
	exec(%filename);
	%newcount = Group::objectCount(%group);

	if (%newcount != %oldcount + 1) return -1;

	return Group::getObject(%group, %oldcount);
}