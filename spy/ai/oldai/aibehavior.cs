// - Usage:
//
//  SuperAI::Spawn("Bob", DMMale, "1 2 3", "0 0 0", "SuperAI", -1, 0);
//

function SuperAIBehavior::registerBehavior

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