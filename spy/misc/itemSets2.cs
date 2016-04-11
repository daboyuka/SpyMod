// $ITEM_GROUP_FUNCS[name] = true
// $ITEM_GROUPS[name] = numItems
// $ITEM_GROUPS[name, i] = item count | group | func args



function ItemGroup::getEntryType(%entryName) {
	if (isItemData(%entryName))
		return "item";
	else if ($ITEM_GROUPS[%entryName] != "")
		return "group";
	else if ($ITEM_GROUP_FUNCS[%entryName] != "")
		return "func";
	else if (String::startsWith(%entryName, "@") && isItemData(String::substr(%entryName, 1)))
		return "itemWithAmmo";
	else
		return "";
}

function ItemGroup::isItem(%itemName)         { return ItemGroup::getEntryType(%itemName) == "item"; }
function ItemGroup::isItemWithAmmo(%itemName) { return ItemGroup::getEntryType(%itemName) == "itemWithAmmo"; }
function ItemGroup::isGroup(%groupName)       { return ItemGroup::getEntryType(%groupName) == "group"; }
function ItemGroup::isFunc(%fnName)           { return ItemGroup::getEntryType(%fnName) == "func"; }

function ItemGroup::checkItem(%name, %other)         { assert(ItemGroup::isItem(%name), "Attempt to use non-item %1 as item" @ tern(%other, " (%2)",""), %name, %other); }
function ItemGroup::checkItemWithAmmo(%name, %other) { assert(ItemGroup::isItemWithAmmo(%name), "Attempt to use non-item-with-ammo %1 as item-with-ammo" @ tern(%other, " (%2)",""), %name, %other); }
function ItemGroup::checkGroup(%name, %other)        { assert(ItemGroup::isGroup(%name), "Attempt to use non-group %1 as group" @ tern(%other, " (%2)",""), %name, %other); }
function ItemGroup::checkFunc(%name, %other)         { assert(ItemGroup::isFunc(%name), "Attempt to use non-func %1 as func" @ tern(%other, " (%2)",""), %name, %other); }

function ItemGroup::firstEntry(%groupName) {
	if (!ItemGroup::isGroup(%groupName)) return "";
	
	$ItemGroup::iterGroupName = %groupName;
	$ItemGroup::iterIdx = 0;
	return ItemGroup::nextEntry(%groupName);
}

function ItemGroup::nextEntry() {
	%groupName = $ItemGroup::iterGroupName;
	if (%groupName == "") return "";
	
	%i = $ItemGroup::iterIdx++ - 1;
	if (%i >= $ITEM_GROUPS[%groupName]) {
		$ItemGroup::iterGroupName = "";
		$ItemGroup::iterIdx = "";
		return "";
	}
	
	return $ITEM_GROUPS[%groupName, %i];
}

function ItemGroup::makeEmptyGroup(%groupName) {
	assert(ItemGroup::getEntryType(%groupName) == "", "attempt to make empty group with already-existing entry name '%1'", %groupName);
	$ITEM_GROUPS[%groupName] = 0;
}

function ItemGroup::addRaw(%groupName, %entry) {
	%n = defval($ITEM_GROUPS[%groupName], 0);
	$ITEM_GROUPS[%groupName, %n] = %entry;
	$ITEM_GROUPS[%groupName] = %n++;
}

function ItemGroup::addItem(%groupName, %itemName, %count) {
	%count = defval(%count, 1);

	ItemGroup::checkItem(%itemName, "in add to group " @ %groupName);
	assert(%count > 0, "attempt to add %1 of item %2 to group %3", %count, %itemName, %groupName);

	ItemGroup::addRaw(%groupName, %itemName @ " " @ %count);
}

function ItemGroup::addItems(%groupName, %items) {
	while (true) {
		%item = getWord(%items, %i++ - 1);
		%count = getWord(%items, %i++ - 1);
		if (%item == -1 || %count == -1) break;
		ItemGroup::addItem(%groupName, %item, %count);
	}
}

function ItemGroup::addItemWithAmmo(%groupName, %itemName, %armor) {
	ItemGroup::checkItem(%itemName, "in add to group " @ %groupName);

	ItemGroup::addRaw(%groupName, "@" @ %itemName @ tern(%armor, " " @ %armor, ""));
}

function ItemGroup::addItemsWithAmmo(%groupName, %items, %armor) {
	while (true) {
		%item = getWord(%items, %i++ - 1);
		if (%item == -1) break;
		ItemGroup::addItemWithAmmo(%groupName, %item, %armor);
	}
}

function ItemGroup::addGroup(%groupName, %subgroupName) {
	ItemGroup::checkGroup(%subgroupName, "in add to group " @ %groupName);
	
	ItemGroup::addRaw(%groupName, %subgroupName);
}

function ItemGroup::addGroups(%groupName, %subgroupNames) {
	while (true) {
		%subgroupName = getWord(%subgroupNames, %i++ - 1);
		if (%subgroupName == -1) break;
		ItemGroup::addGroup(%groupName, %subgroupName);
	}
}

function ItemGroup::addGroupFlattened(%groupName, %subgroupName, %depth) {
	ItemGroup::checkGroup(%subgroupName, "in add to group " @ %groupName);
	
	ItemGroup::flattenGroup(%groupName, %subgroupName, %depth);
}

function ItemGroup::addGroupsFlattened(%groupName, %subgroupNames, %depth) {
	while (true) {
		%subgroupName = getWord(%subgroupNames, %i++ - 1);
		if (%subgroupName == -1) break;
		ItemGroup::addGroupFlattened(%groupName, %subgroupName, %depth);
	}
}

function ItemGroup::addFunc(%groupName, %fnName, %args) {
	ItemGroup::checkFunc(%fnName, "in add to group " @ %groupName);

	ItemGroup::addRaw(%groupName, %fnName @ " " @ %args);
}

function ItemGroup::registerFunc(%fnName) {
	$ITEM_GROUP_FUNCS[%fnName] = true;
}

function ItemGroup::clear(%groupName) {
	%n = $ITEM_GROUPS[%groupName];

	$ITEM_GROUPS[%groupName] = "";
	for (%i = 0; %i < %n; %i++) 
		$ITEM_GROUPS[%groupName, %i] = "";
}

// Returns true iff %groupName consists of only items (no subgroups or funcs)
function ItemGroup::isGroupRealized(%groupName) {
	for (%i = 0; %i < $ITEM_GROUPS[%groupName]; %i++) {
		%entry = $ITEM_GROUPS[%groupName, %i];
		%entryFirst = getWord(%entry, 0);
		
		if (!ItemGroup::isItem(%entryFirst))
			return false;
	}
	return true;
}

// Creates a new group %groupName from existing group %groupToRealize by evaluating all
// funcs and flattening/realizing all subgroups recursively.
function ItemGroup::realizeEntry(%groupName, %entry) {
	%entryFirst = getWord(%entry, 0);
	%entryType = ItemGroup::getEntryType(%entryFirst);
	if (%entryType == "item")
		ItemGroup::realizeItem(%groupName, %entry);
	else if (%entryType == "group")
		ItemGroup::realizeGroup(%groupName, %entry);
	else if (%entryType == "func")
		ItemGroup::realizeFunc(%groupName, %entry);
	else if (%entryType == "itemWithAmmo")
		ItemGroup::realizeItemWithAmmo(%groupName, %entry);
	else
		error("Unrecognized entry type '%1' when realizing into group %2", %entryType, %groupName);
}

function ItemGroup::realizeItem(%groupName, %entry) {
	%item = getWord(%entry, 0);
	%count = getWord(%entry, 1);
	ItemGroup::addItem(%groupName, %item, %count);
}

function ItemGroup::realizeGroup(%groupName, %entry) {
	%groupToRealize = %entry;
	for (%i = 0; %i < $ITEM_GROUPS[%groupToRealize]; %i++) {
		%entry = $ITEM_GROUPS[%groupToRealize, %i];
		ItemGroup::realizeEntry(%groupName, %entry);
	}
}

function ItemGroup::realizeFunc(%groupName, %entry) {
	%fnName = getWord(%entry, 0);
	%args = String::splitAfter(%entry, " ");
	
	%tmpGroup = ItemGroup::makeTmp();
	
	eval("ItemGroupFunc::" @ %fnName @ '("' @ %tmpGroup @ '","' @ %args @ '");');
	ItemGroup::realizeGroup(%groupName, %tmpGroup); // Realize the output, in case it contains groups, etc.

	ItemGroup::clear(%tmpGroup);
}

function ItemGroup::realizeItemWithAmmo(%groupName, %entry) {
	%item = getWord(%entry, 0);
	%armor = getWord(%entry, 1);
	if (%armor == "" || %armor == -1) %armor = DMMale;
	
	ItemGroup::checkItemWithAmmo(%item, "in add to group " @ %groupName);
	%item = String::substr(%item, 1);

	%ammoItem = $WeaponAmmo[%item];
	%ammoMax = $ItemMax[%armor, %ammoItem];
	
	if (%ammoItem != "" && %ammoMax != "")
		ItemGroup::addItem(%groupName, %ammoItem, %ammoMax);  // Add ammo before weapon
	if (%item != %ammoItem)
		ItemGroup::addItem(%groupName, %item); // Only add weapon if it is not the same as the ammo type (some weapons are their own ammo, like PlasticMineAmmo)
}

// %depth = integer -> flatten only %depth levels (%depth == 0 -> just copies %gorupToFlatten to %groupName)
// %depth == "" -> flatten to infinite depth
function ItemGroup::flattenGroup(%groupName, %groupToFlatten, %depth) {
	if (%depth == "0") { // means %depth == 0 && %depth != ""
		ItemGroup::addGroup(%groupName, %groupToFlatten);
	}

	for (%i = 0; %i < $ITEM_GROUPS[%groupToFlatten]; %i++) {
		%entry = $ITEM_GROUPS[%groupToFlatten, %i];
		%entryName = getWord(%entry, 0);
		if (ItemGroup::isGroup(%entry))
			ItemGroup::flattenToGroup(%groupName, %entryName, tern(%depth != "", %depth - 1, ""));
		else
			ItemGroup::addRaw(%groupName, %entry);
	}
}

function ItemGroup::print(%groupName) {
	for (%entry = ItemGroup::firstEntry(%groupName); %entry != ""; %entry = ItemGroup::nextEntry())
		echo(%entry);
}

$ItemGroup::TMP_GROUP_PREFIX = "tmp";
$ItemGroup::TMP_GROUP_NEXT = 0;
function ItemGroup::makeTmp() {
	%tmp = $ItemGroup::TMP_GROUP_PREFIX @ ($ItemGroup::TMP_GROUP_PREFIX_NEXT++ - 1);
	ItemGroup::makeEmptyGroup(%tmp);
	return %tmp;
}

function ItemGroup::setAsSpawnList(%groupName) {
	if (!ItemGroup::isGroup(%groupName)) { echo("ERROR: attempt to set non-group '" @ %groupName @ "' as spawn list"); trace(); return; }
	
	clearSpawnBuyList();
	for (%t = 0; %t < getNumTeams(); %t++) clearTeamSpawnBuyList(%t);

	%tmp = ItemGroup::makeTmp();
	ItemGroup::realizeGroup(%tmp, %groupName);
	
	for (%entry = ItemGroup::firstEntry(%tmp); %entry != ""; %entry = ItemGroup::nextEntry()) {
		%item = getWord(%entry, 0);
		%count = getWord(%entry, 1);
		$spawnItems[%i++ - 1] = %count @ " " @ %item;
	}
	
	ItemGroup::clear(%tmp);
}

function ItemGroup::setAsTeamSpawnList(%groupName, %team) {
	if (!ItemGroup::isGroup(%groupName)) { echo("ERROR: attempt to set group '" @ %groupName @ "' as team spawn list"); trace(); return; }
	
	clearTeamSpawnBuyList(%team);

	%tmp = ItemGroup::makeTmp();
	ItemGroup::realizeGroup(%tmp, %groupName);
	
	for (%entry = ItemGroup::firstEntry(%tmp); %entry != ""; %entry = ItemGroup::nextEntry()) {
		%item = getWord(%entry, 0);
		%count = getWord(%entry, 1);
		$spawnItems[%team, %i++ - 1] = %count @ " " @ %item;
	}
	
	ItemGroup::clear(%tmp);
}


//ItemGroup::registerFunc(randselect);
//function ItemGroupFunc::randselect(%groupName, %args) {
//	%selN = getWord(%args, 0);
//	for (%i = 0; (%entry = getWord(%args, %i + 1)) != -1; %i++) {
//		if (%i < %selN) {
//			// First %selN items/groups get added immediately
//			%sel[%i] = %entry;
//		} else {
//			// Additional items/groups have decreasing chance to overwrite existing ones
//			%j = randint(%i + 1);
//			if (%j < %selN)
//				%sel[%j] = %entry;
//		}
//	}
//	
//	%selN = min(%selN, %i); // trim to the number of entries actually seen if less
//	for (%i = 0; %i < %selN; %i++) {
//		//%entry = %sel[%i];
//		//if (ItemGroup::isGroup(%entry)) {
//		//	ItemGroup::addGroup(%groupName, %entry);
//		//} else {
//		//	ItemGroup::addItem(%groupName, %entry);
//		//}
//		ItemGroup::addWeaponAndAmmo(%groupName, %sel[%i]);
//	}
//}

ItemGroup::registerFunc(randselectInGroup);
function ItemGroupFunc::randselectInGroup(%groupName, %args) {
	%selN = getWord(%args, 0);
	%randGroupName = getWord(%args, 1);
	
	ItemGroup::checkGroup(%groupName);
	ItemGroup::checkGroup(%randGroupName, "add into group " @ %groupName);
	
	%n = $ITEM_GROUPS[%randGroupName];
	%selN = min(%selN, %n); // trim to the number of entries actually in the group
	
	// Select the entries to use
	for (%i = 0; %i < %n; %i++) {
		%entry = $ITEM_GROUPS[%randGroupName, %i];
		if (%i < %selN) {
			// First %selN entries get added immediately
			%sel[%i] = %entry;
		} else {
			// Additional entries have decreasing chance to overwrite existing ones
			%j = randint(%i + 1);
			if (%j < %selN)
				%sel[%j] = %entry;
		}
	}

	// Add those entries into the output (use addRaw to simply copy the entry line into the group)
	for (%i = 0; %i < %selN; %i++) {
		ItemGroup::addRaw(%groupName, %sel[%i]);
	}
}

deleteVariables("$ITEM_GROUPS*"); // Clear any previous item groups, to be safe

//
// Basic item groups
//

ItemGroup::makeEmptyGroup(empty); // Might be useful for convenience?

ItemGroup::addItemsWithAmmo(weps_handgun,   "Magnum MG27 Magdalon");
ItemGroup::addItemsWithAmmo(weps_rifle,     "Challenger Raider Stormbow");
ItemGroup::addItemsWithAmmo(weps_heavy,     "Dragon Tornado Shotgun2");
ItemGroup::addItemsWithAmmo(weps_explosive, "GrenadeLauncher2 RocketLauncher DoomSlayer");
ItemGroup::addItemsWithAmmo(weps_misc,      "Flamer AGP84 Knife");

ItemGroup::addGroupsFlattened(weps_all, "weps_handgun weps_rifle weps_heavy weps_explosive weps_misc");

ItemGroup::addItemsWithAmmo(grenades, "SmokeGrenadeItem DistractionGrenadeItem GasGrenadeItem SpikeGrenadeItem");

ItemGroup::addItemsWithAmmo(gadgets, "Binoculars PlasticMineAmmo Grappler Toolbox");

ItemGroup::addItem(packs, ParachutePack);



ItemGroup::addItemsWithAmmo(gadgets_dm, "PlasticMineAmmo Grappler");

ItemGroup::addItemsWithAmmo(grenades_dm, "SmokeGrenadeItem GasGrenadeItem");

//
// Spawn sets
//
ItemGroup::addGroups        (spawn_all, "weps_handgun weps_rifle weps_heavy weps_explosive weps_misc grenades gadgets");

ItemGroup::addGroups        (spawn_dm_nograp, "weps_handgun weps_rifle weps_heavy weps_explosive weps_misc grenades_dm");
ItemGroup::addItemWithAmmo  (spawn_dm_nograp, PlasticMineAmmo);

ItemGroup::addGroup(spawn_dm, spawn_dm_nograp);
ItemGroup::addItem (spawn_dm, Grappler);

ItemGroup::addItemsWithAmmo(spawn_mg_only, "MG27 Grappler SmokeGrenadeItem");

ItemGroup::addItemsWithAmmo(spawn_knife_only, "Knife SmokeGrenadeItem");

ItemGroup::addItemsWithAmmo(spawn_itembuy, "MG27 Knife Grappler SmokeGrenadeItem");

ItemGroup::addItemsWithAmmo(spawn_mg_knife, "MG27 Knife SmokeGrenadeItem");

ItemGroup::addFunc         (spawn_rand_dm, randselectInGroup, "1 weps_rifle");
ItemGroup::addFunc         (spawn_rand_dm, randselectInGroup, "1 weps_heavy");
ItemGroup::addFunc         (spawn_rand_dm, randselectInGroup, "1 weps_handgun");
ItemGroup::addFunc         (spawn_rand_dm, randselectInGroup, "1 weps_misc");
ItemGroup::addItemWithAmmo (spawn_rand_dm, SmokeGrenadeItem);
ItemGroup::addItemsWithAmmo(spawn_rand_dm, "PlasticMineAmmo Grappler");
