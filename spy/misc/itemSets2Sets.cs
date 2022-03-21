ItemGroup::clearAll(); // Clear any previous item groups, to be safe

//
// Primitive item groups
//

ItemGroup::makeEmptyGroup(empty); // Might be useful for convenience?

ItemGroup::addItemsWithAmmo(weps_handgun,   "Magnum MG27 Magdalon");
ItemGroup::addItemsWithAmmo(weps_rifle,     "Challenger Raider Stormbow");
ItemGroup::addItemsWithAmmo(weps_heavy,     "Dragon Tornado Shotgun2");
ItemGroup::addItemsWithAmmo(weps_explosive, "GrenadeLauncher2 RocketLauncher DoomSlayer");
ItemGroup::addItemsWithAmmo(weps_misc,      "Flamer AGP84 Knife");

ItemGroup::addGroupsFlattened(weps_all, 	"weps_handgun weps_rifle weps_heavy weps_explosive weps_misc");

ItemGroup::addGroupsFlattened	(weps_noexplo, "weps_handgun weps_rifle weps_heavy weps_misc");
ItemGroup::addItem				(weps_noexplo, "GrenadeLauncher2"); // no (explosive) ammo

ItemGroup::addItemsWithAmmo(grenades, "SmokeGrenadeItem DistractionGrenadeItem GasGrenadeItem SpikeGrenadeItem");

ItemGroup::addItemsWithAmmo(gadgets, "Binoculars PlasticMineAmmo Grappler Toolbox");

ItemGroup::addItem(packs, ParachutePack);

// subsets of gadgets/grenades commonly used in actual spawn sets
ItemGroup::addItemsWithAmmo(gadgets_std, "PlasticMineAmmo Grappler");
ItemGroup::addItemsWithAmmo(grenades_std, "SmokeGrenadeItem GasGrenadeItem");

//
// Basic spawn item sets
//

ItemGroup::makeAdminable(spawn_all, "Kitchen Sink");
ItemGroup::addGroups	(spawn_all, "weps_all grenades gadgets");

// spawn_std_nograp: standard weapon set (Spy3 classic), but without the Grappler
ItemGroup::makeAdminable	(spawn_std_nograp, "All But Explo/Grappler");
ItemGroup::addGroups        (spawn_std_nograp, "weps_noexplo grenades_std");
ItemGroup::addItemWithAmmo  (spawn_std_nograp, PlasticMineAmmo);

// spawn_std: standard weapon set (Spy3 classic)
ItemGroup::makeAdminable	(spawn_std, "All But Explo");
ItemGroup::addGroup			(spawn_std, spawn_std_nograp);
ItemGroup::addItem 			(spawn_std, Grappler);

// spawn_mg_only: MG27, Grappler, smoke nades (no Grappler)
ItemGroup::makeAdminable	(spawn_mg_only, "Pistol and Grappler");
ItemGroup::addItemsWithAmmo	(spawn_mg_only, "MG27 Grappler SmokeGrenadeItem");

// spawn_mg_only: Knife and smoke nades (no Grappler)
ItemGroup::makeAdminable	(spawn_knife_only, "Knives Out");
ItemGroup::addItemsWithAmmo	(spawn_knife_only, "Knife SmokeGrenadeItem");

// spawn_mg_knife: MG27, knife, smoke nades (no Grappler)
ItemGroup::makeAdminable	(spawn_mg_knife, "Pistol and Knife");
ItemGroup::addItemsWithAmmo	(spawn_mg_knife, "MG27 Knife SmokeGrenadeItem");

// spawn_itembuy: item set used on maps where items can be purchased; MG27, Knife, Grappler, smoke nades
ItemGroup::addItemsWithAmmo(spawn_itembuy, "MG27 Knife Grappler SmokeGrenadeItem");



//
// Complex/randomized item sets
//

// spawn_balanced_rand: one rifle, one heavy weapon, one handgun, one misc weapon, smoke nades, plastique explosives, and a Grappler
ItemGroup::makeAdminable	(spawn_balanced_rand, "Balanced");
ItemGroup::addFunc			(spawn_balanced_rand, randselectInGroup, "1 weps_rifle");
ItemGroup::addFunc			(spawn_balanced_rand, randselectInGroup, "1 weps_heavy");
ItemGroup::addFunc			(spawn_balanced_rand, randselectInGroup, "1 weps_handgun");
ItemGroup::addFunc			(spawn_balanced_rand, randselectInGroup, "1 weps_misc");
ItemGroup::addItemWithAmmo	(spawn_balanced_rand, SmokeGrenadeItem);
ItemGroup::addItemsWithAmmo	(spawn_balanced_rand, "PlasticMineAmmo Grappler");

// spawn_explosive: all explosive weapons, all nades, plastiques, and the Grappler
ItemGroup::makeAdminable	(spawn_explosive, "Explosives!");
ItemGroup::addGroups		(spawn_explosive, "weps_explosive grenades");
ItemGroup::addItemsWithAmmo	(spawn_explosive, "AGP84 PlasticMineAmmo Grappler");

// spawn_crazyshot: all high-randomness low-fire-rate weapons, spike nades, Grappler
ItemGroup::makeAdminable	(spawn_crazyshot, "Crazyshot");
ItemGroup::addItemsWithAmmo	(spawn_crazyshot, "Magnum Magdalon SpikeGrenadeItem Grappler");

// spawn_snipe: all highly-accurate weapons (at least in one mode), smoke nades, Grappler
ItemGroup::makeAdminable	(spawn_snipe, "Snipe");
ItemGroup::addItemsWithAmmo	(spawn_snipe, "MG27 Challenger Raider Stormbow SmokeGrenadeItem Grappler");

// spawn_steamroller: all short-range heavy weapons, knife, plastiques, spike nade, no Grappler
ItemGroup::makeAdminable	(spawn_steamroller, "Steamroller");
ItemGroup::addItemsWithAmmo	(spawn_steamroller, "Magnum Tornado Shotgun2 Flamer Knife PlasticMineAmmo SpikeGrenadeItem");

// spawn_dm_rand: random selection among spawn sets suitable for fun DMs
ItemGroup::makeAdminable	(spawn_dm_rand, "Surprise Me (DM)");
ItemGroup::addFunc			(spawn_dm_rand, randselect, "1 spawn_balanced_rand spawn_explosive spawn_steamroller spawn_crazyshot spawn_snipe spawn_knife_grap spawn_mg_only");
