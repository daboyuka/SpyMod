// - Usage:
//
//  SuperAI::Spawn("Bob", DMMale, "1 2 3", "0 0 0", "SuperAI", -1, 0);
//

// var names:
//   ["bstack"] = size
//   ["bstack", %i, "type"] = %i'th behavior type
//   ["bstack", %i, "arg", %j] = %i'th behavior's %j'th argument

// --- Behavior stack initialization ---

function SuperAIBehavior::initBStack(%aiName) {
  $SuperAI::AIData[%aiName, "bstack"] = 0;
}

// --- Push/pop operations ---

function SuperAIBehavior::pushBehavior(%aiName, %type, %arg0, %arg1, %arg2, %arg3, %arg4, %arg5, %arg6, %arg7) {
  SuperAIBehavior::deactivateBehavior(%aiName);

  %size = SuperAIBehavior::getBStackSize(%aiName);

  $SuperAI::AIData[%aiName, "bstack", %size, "type"] = %type;
  for (%i = 0; %i < 8 && %arg[%i] != ""; %i++)
    $SuperAI::AIData[%aiName, "bstack", %size, "args", %i] = %arg[%i];

  $SuperAI::AIData[%aiName, "bstack"]++;

  SuperAIBehavior::activateBehavior(%aiName);
}

function SuperAIBehavior::popBehavior(%aiName) {
  if (SuperAIBehavior::getBStackSize(%aiName) == 0) return;

  SuperAIBehavior::deactivateBehavior(%aiName);

  $SuperAI::AIData[%aiName, "bstack"]--;

  SuperAIBehavior::activateBehavior(%aiName);
}

// --- Accessor functions ---

function SuperAIBehavior::getBStackSize(%aiName) {
  return $SuperAI::AIData[%aiName, "bstack"];
}

function SuperAIBehavior::peekBehaviorType(%aiName) {
  if (SuperAIBehavior::getBStackSize(%aiName) == 0) return "";

  return $SuperAI::AIData[%aiName, "bstack", SuperAIBehavior::getBStackSize(%aiName) - 1, "type"];
}

function SuperAIBehavior::peekBehaviorArgument(%aiName, %index2) {
  if (SuperAIBehavior::getBStackSize(%aiName) == 0) return "";

  return $SuperAI::AIData[%aiName, "bstack", SuperAIBehavior::getBStackSize(%aiName) - 1, "args", %index2];
}

// --- Behavior (de)activate functions ---
// These work on the top behavior on the bstack

function SuperAIBehavior::activateBehavior(%aiName) {
  if (SuperAIBehavior::getBStackSize(%aiName) == 0) return;

  %type = SuperAIBehavior::peekBehaviorType(%aiName);

  for (%i = 0; %i < 7; %i++) {
    %args[%i] = SuperAIBehavior::peekBehaviorArgument(%aiName, %i);
    if (%args[%i] == "") break;
  }

  invoke("SuperAIBehavior::" @ %type @ "::start", %arg0, %arg1, %arg2, %arg3, %arg4, %arg5, %arg6, %arg7);
}

function SuperAIBehavior::deactivateBehavior(%aiName) {
  if (SuperAIBehavior::getBStackSize(%aiName) == 0) return;

  %type = SuperAIBehavior::peekBehaviorType(%aiName);

  invoke("SuperAIBehavior::" @ %type @ "::stop");
}



function SuperAIBehavior::followPath::start(%aiName) {

}

function SuperAIBehavior::followPath::stop(%aiName) {

}