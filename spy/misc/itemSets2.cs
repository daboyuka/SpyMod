// $ITEM_GROUP_FUNCS[name] = true
// $ITEM_GROUPS[name] = numItems
// $ITEM_GROUPS[name, i] = item count | group | func args

function ItemGroup::isGroup(%groupName) {
	return $ITEM_GROUPS[%groupName] != "";
}

function ItemGroup::isFunc(%fnName) {
	return $ITEM_GROUP_FUNCS[%fnName] != "";
}

function ItemGroup::firstEntry(%groupName) {
	if (!ItemGroup::isGroup(%groupName)) {
		return "";
	}
	
	$ItemGroup::iterGroupName = %groupName;
	$ItemGroup::iterIdx = 0;
	return ItemGroup::nextEntry(%groupName);
}

function ItemGroup::nextEntry() {
	%groupName = $ItemGroup::iterGroupName;
	if (%groupName == "") {
		return "";
	}
	
	%i = $ItemGroup::iterIdx++ - 1;
	if (%i >= $ITEM_GROUPS[%groupName]) {
		$ItemGroup::iterGroupName = "";
		$ItemGroup::iterIdx = "";
		return "";
	}
	
	return $ITEM_GROUPS[%groupName, %i];
}

function ItemGroup::add(%groupName, %entry) {
	%n = defval($ITEM_GROUPS[%groupName], 0);
	$ITEM_GROUPS[%groupName, %n] = %entry;
	$ITEM_GROUPS[%groupName] = %n++;
}

function ItemGroup::addItem(%groupName, %itemName, %count) {
	%count = defval(%count, 1);
	ItemGroup::add(%groupName, %itemName @ " " @ %count);
}

function ItemGroup::addItems(%groupName, %items) {
	while (true) {
		%item = getWord(%items, %i++ - 1);
		%count = getWord(%items, %i++ - 1);
		if (%item == -1 || %count == -1) break;
		ItemGroup::addItem(%groupName, %item, %count);
	}
}

function ItemGroup::addGroup(%groupName, %subgroupName) {
	if (!ItemGroup::isGroup(%subgroupName)) { echo("ERROR: attempt to add subgroup '" @ %subgroupName @ "' that does not exist to item group '" @ %groupName @ "'"); trace(); return; }
	ItemGroup::add(%groupName, %subgroupName);
}

function ItemGroup::addGroups(%groupName, %subgroupNames) {
	while (true) {
		%subgroupName = getWord(%subgroupNames, %i++ - 1);
		if (%subgroupName == -1) break;
		ItemGroup::addGroup(%groupName, %subgroupName);
	}
}

function ItemGroup::addFunc(%groupName, %fnName, %args) {
	ItemGroup::add(%groupName, %fnName @ " " @ %args);
}

function ItemGroup::registerFunc(%fnName) {
	$ITEM_GROUP_FUNCS[%fnName] = true;
}

function ItemGroup::addWeaponAndAmmo(%groupName, %weaponItem, %armor) {
	%armor = defval(%armor, DMMale);
	
	%ammoItem = $WeaponAmmo[%weaponItem];
	%ammoMax = $ItemMax[%armor, %ammoItem];
	
	if (%ammoItem != "" && %ammoMax != "")
		ItemGroup::addItem(%groupName, %ammoItem, %ammoMax); // Add ammo before weapon
	if (%weaponItem != %ammoItem)
		ItemGroup::addItem(%groupName, %weaponItem);
}

function ItemGroup::addWeaponsAndAmmo(%groupName, %weaponItems, %armor) {
	for (%i = 0; (%weaponItem = getWord(%weaponItems, %i)) != -1; %i++)
		ItemGroup::addWeaponAndAmmo(%groupName, %weaponItem, %armor);
}

function ItemGroup::clear(%groupName) {
	$ITEM_GROUPS[%groupName] = "";
}

// Returns true iff %groupName consists of only items (no subgroups or funcs)
function ItemGroup::isRealized(%groupName) {
	for (%i = 0; %i < $ITEM_GROUPS[%groupName]; %i++) {
		%entry = $ITEM_GROUPS[%groupName, %i];
		%entryFirst = getWord(%entry, 0);
		
		if (ItemGroup::isGroup(%entryFirst) || ItemGroup::isFunc(%entryFirst))
			return false;
	}
	return true;
}

// Creates a new group %groupName from existing group %groupToRealize by evaluating all
// funcs and flattening/realizing all subgroups recursively.
function ItemGroup::addRealized(%groupName, %groupToRealize) {
	for (%i = 0; %i < $ITEM_GROUPS[%groupToRealize]; %i++) {
		%entry = $ITEM_GROUPS[%groupToRealize, %i];
		%entryFirst = getWord(%entry, 0);
		
		if (ItemGroup::isGroup(%entryFirst)) {
			%subgroupName = %entryFirst;
			ItemGroup::addRealized(%groupName, %subgroupName);
		} else if (ItemGroup::isFunc(%entryFirst)) {
			%fnName = %entryFirst;
			%args = String::splitAfter(%entry, " ");
			eval("ItemGroupFunc::" @ %fnName @ '("' @ %groupName @ '","' @ %args @ '");');
		} else { // item
			%item = %entryFirst;
			%count = getWord(%entry, 1);
			ItemGroup::addItem(%groupName, %item, %count);
		}
	}
}

function ItemGroup::print(%groupName) {
	for (%entry = ItemGroup::firstEntry(%groupName); %entry != ""; %entry = ItemGroup::nextEntry())
		echo(%entry);
}

$ItemGroup::TMP_GROUP = "tmp";
function ItemGroup::setAsSpawnList(%groupName) {
	if (!ItemGroup::isGroup(%groupName)) { echo("ERROR: attempt to set non-group '" @ %groupName @ "' as spawn list"); trace(); return; }
	
	clearSpawnBuyList();
	for (%t = 0; %t < getNumTeams(); %t++) clearTeamSpawnBuyList(%t);

	%tmp = $ItemGroup::TMP_GROUP;
	ItemGroup::clear(%tmp);
	ItemGroup::addRealized(%tmp, %groupName);
	
	for (%entry = ItemGroup::firstEntry(%tmp); %entry != ""; %entry = ItemGroup::nextEntry()) {
		%item = getWord(%entry, 0);
		%count = getWord(%entry, 1);
		$spawnItems[%i++ - 1] = %count @ " " @ %item;
	}
}

function ItemGroup::setAsTeamSpawnList(%groupName, %team) {
	if (!ItemGroup::isGroup(%groupName)) { echo("ERROR: attempt to set group '" @ %groupName @ "' as team spawn list"); trace(); return; }
	
	clearTeamSpawnBuyList(%team);

	%tmp = $ItemGroup::TMP_GROUP;
	ItemGroup::clear(%tmp);
	ItemGroup::addRealized(%tmp, %groupName);
	
	for (%entry = ItemGroup::firstEntry(%tmp); %entry != ""; %entry = ItemGroup::nextEntry()) {
		%item = getWord(%entry, 0);
		%count = getWord(%entry, 1);
		$spawnItems[%team, %i++ - 1] = %count @ " " @ %item;
	}
}


ItemGroup::registerFunc(randselectWeaponsAndAmmo);
function ItemGroupFunc::randselectWeaponsAndAmmo(%groupName, %args) {
	%selN = getWord(%args, 0);
	for (%i = 0; (%entry = getWord(%args, %i + 1)) != -1; %i++) {
		if (%i < %selN) {
			// First %selN items/groups get added immediately
			%sel[%i] = %entry;
		} else {
			// Additional items/groups have decreasing chance to overwrite existing ones
			%j = randint(%i + 1);
			if (%j < %selN)
				%sel[%j] = %entry;
		}
	}
	
	%selN = min(%selN, %i); // trim to the number of entries actually seen if less
	for (%i = 0; %i < %selN; %i++) {
		//%entry = %sel[%i];
		//if (ItemGroup::isGroup(%entry)) {
		//	ItemGroup::addGroup(%groupName, %entry);
		//} else {
		//	ItemGroup::addItem(%groupName, %entry);
		//}
		ItemGroup::addWeaponAndAmmo(%groupName, %sel[%i]);
	}
}

//
// Basic item groups
//
deleteVariables("$ITEM_GROUPS*"); // Clear any previous item groups, to be safe

$ITEM_GROUPS[empty] = 0; // Empty group (different from non-existent; does this matter?)

ItemGroup::addWeaponsAndAmmo(weps_handgun,   "Magnum MG27 Magdalon");
ItemGroup::addWeaponsAndAmmo(weps_rifle,     "Challenger Raider");
ItemGroup::addWeaponsAndAmmo(weps_heavy,     "Dragon Tornado Shotgun2");
ItemGroup::addWeaponsAndAmmo(weps_explosive, "GrenadeLauncher2 RocketLauncher DoomSlayer");
ItemGroup::addWeaponsAndAmmo(weps_misc,      "Flamer AGP84 Knife");

ItemGroup::addWeaponsAndAmmo(grenades, "SmokeGrenadeItem DistractionGrenadeItem GasGrenadeItem SpikeGrenadeItem");

ItemGroup::addWeaponsAndAmmo(gadgets, "Binoculars Grappler PlasticMineAmmo Toolbox");

ItemGroup::addWeaponAndAmmo(packs, Parachute);

//
// Spawn sets
//
ItemGroup::addGroups        (spawn_all, "weps_handgun weps_rifle weps_heavy weps_explosive weps_misc grenades");
ItemGroup::addWeaponsAndAmmo(spawn_all, "Binoculars Grappler PlasticMineAmmo");

ItemGroup::addGroups        (spawn_dm_nograp, "weps_handgun weps_rifle weps_heavy weps_explosive weps_misc");
ItemGroup::addWeaponsAndAmmo(spawn_dm_nograp, "PlasticMineAmmo SmokeGrenadeItem GasGrenadeItem");

ItemGroup::addGroup        (spawn_dm, spawn_dm_nograp);
ItemGroup::addWeaponAndAmmo(spawn_dm, Grappler);

ItemGroup::addWeaponsAndAmmo(spawn_mg_only, "MG27 Grappler SmokeGrenadeItem");

ItemGroup::addWeaponsAndAmmo(spawn_knife_only, "Knife SmokeGrenadeItem");

ItemGroup::addWeaponsAndAmmo(spawn_itembuy, "MG27 Knife Grappler SmokeGrenadeItem");

ItemGroup::addWeaponsAndAmmo(spawn_mg_knife, "MG27 Knife SmokeGrenadeItem");

ItemGroup::addFunc(spawn_rand, randselectWeaponsAndAmmo, "5 Magnum MG27 Magdalon Challenger Raider Dragon Tornado Shotgun2 GrenadeLauncher2 RocketLauncher DoomSlayer Flamer AGP84 Knife");

