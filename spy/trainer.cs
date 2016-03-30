// TRAINING

function Help::quikN00bTeacher(%client) {
  if (!%client.training) {
    %client.training = true;
    %client.trainingType = "quick";
    %client.trainingStep = 0;
  }

  %step = %client.trainingStep;

  %s = -1;
  %msg = "";
  %delay = 10;
  %lock = true;

  if (%step == %s++) {
    %msg =
    "<f1>Welcome to SpyMod quick training!<f0>" @ "\n" @
    "(press ctrl-n to continue and press ctrl-y to repeat a message)";
    %delay = 30;
  }
  if (%step == %s++) {
    %msg =
    "<jc><f1>General stuff:<f0><jl><l3>" @ "\n" @
    "* Your health bar is health, your energy bar is body armor" @ "\n" @
    "* There is no jetting" @ "\n" @
    "* Falling hurts lots" @ "\n" @
    "* Head shots hurt" @ "\n" @
    "* Leg shots not so much";
    %delay = 20;
  }
  if (%step == %s++) {
    %msg =
    "<jc><f1>Controls:<f0><jl><l3>" @ "\n" @
    "1: Uses a weapon" @ "\n" @
    "2: Uses a gadget" @ "\n" @
    "3: Changes weapon modes" @ "\n" @
    "4: Reload (guns auto-reload when out of ammo)" @ "\n" @
    "5: Change grenade type";
    %delay = 20;
  }
  if (%step == %s++) {
    %msg =
    "<jc><f1>Tips and advice:<f0><jl><l3>" @ "\n" @
    "* Use stealth over force" @ "\n" @
    "* Aim for the head" @ "\n" @
    "* Choose your weapon based on range (machine gun for close, rifle for far)" @ "\n" @
    "* Don't fall" @ "\n" @
    "* The Grappler is very useful; read the Grappler Training and learn to use it";
    %delay = 20;
  }
  if (%step == %s++) {
    %msg =
    "That completes basic training" @ "\n" @
    "Press Tab then Personal Options then Training and then select Full Training for" @ "\n" @
    "complete training, or Grappler Training for info on how to use the Grappler" @ "\n\n" @
    "<f1>Have fun playing SpyMod!";
    %delay = 25;
    %lock = false;
    %client.training = false;
  }

  %delay = 10000;
  clearPrintLock(%client);
  centerprint(%client, "<jc><s1>" @ %msg, %delay, %lock);
}

function Help::n00bTeacher(%client) {
  if (!%client.training) {
    %client.training = true;
    %client.trainingType = "full";
    %client.trainingStep = 0;
  }

  %step = %client.trainingStep;

  %s = -1;
  %msg = "";
  %delay = 10;

  if (%step == %s++) {
    %msg =
    "<f1>Welcome to SpyMod training! Let's get started<f0>" @ "\n" @
    "(press ctrl-n to continue and press ctrl-y to repeat a message while you are in training)";
    %delay = 30;
  }
  if (%step == %s++) {
    %msg =
    "The controls and gameplay are similar to standard Tribes with a few exceptions.  There" @ "\n" @
    "is no jetting in this mod, nor is there radar.  Falling hurts more.  Also, head-shots" @ "\n" @
    "do more damage than shots to the torso, and leg shots do less.";
    %delay = 30;
  }
  if (%step == %s++) {
    %msg =
    "Items are divided into 2 categories: weapons and gadgets.  You can access the weapons" @ "\n" @
    "by pressing \"1\" and then cycling through them with \"q\".  You can access the gadgets" @ "\n" @
    "by pressing \"2\" and using \"q\" to cycle through them.";
    
    %delay = 30;
  }
  if (%step == %s++) {
    %msg =
    "Some weapons and gadgets have different modes.  You can press \"3\" to cycle through" @ "\n" @
    "the modes of a weapon or gadget.";
    %delay = 15;
  }
  if (%step == %s++) {
    %msg =
    "Your weapon GUI is also different in SpyMod.  On the left middle side of the screen are" @ "\n" @
    "2 weapon icons.  The top number is how much ammo is loaded into your weapon or" @ "\n" @
    "gadget, and the bottom number is how much ammo you have in reserve.";
    %delay = 30;
  }
  if (%step == %s++) {
    %msg =
    "Your weapon or gadget will automatically reload when it runs out of ammo.  To reload" @ "\n" @
    "before then, press 4.  Reloading takes a few seconds and you cannot shoot while" @ "\n" @
    "reloading, so be careful.";
    %delay = 23;
  }
  if (%step == %s++) {
    %msg =
    "Grappler:" @ "\n" @
    "The Grappler has 5 modes: Grapple, ascend, descend, swing, expert:" @ "\n" @
    "Grapple: fire to attach onto stuff, fire again to detach." @ "\n" @
    "Ascend and descend: fire and hold to ascend/descend." @ "\n" @
    "Swing: fire and hold to swing forward." @ "\n" @
    "Expert: next page";
    %delay = 30;
  }
  if (%step == %s++) {
    %msg =
    "Expert: for advanced players.  Fire to grapple.  While grappled, fire and hold to" @ "\n" @
    "ascend, jet and hold to descend, and fire and jet at the same time to detach.  While" @ "\n" @
    "you are using an item other than the grappler, jet to detach.";
    %delay = 30;
  }
  if (%step == %s++) {
    %msg =
    "That completes SpyMod training.  Have fun playing the mod!";
    %delay = 5;
    %client.training = false;
  }

  %delay = 10000;
  centerprint(%client, "<jc><s1>" @ %msg, %delay);
}

function Help::grapplerTeacher(%client) {
  if (!%client.training) {
    %client.training = true;
    %client.trainingType = "grappler";
    %client.trainingStep = 0;
  }

  %step = %client.trainingStep;

  %s = -1;
  %msg = "";
  %delay = 10;

  if (%step == %s++) {
    %msg =
    "Welcome to Grappler training!" @ "\n" @
    "This training assumes that you have at least read the Quick Training." @ "\n" @
    "(press ctrl-n to continue and press ctrl-y to repeat a message while you are in training)";
    %delay = 30;
  }
  if (%step == %s++) {
    %msg =
    "<jc><f1>Grappler basics:<f0>" @ "\n" @
    "The Grappler has 5 modes:<jl><l3>" @ "\n" @
    "Grapple: Fire to grapple, fire again to release" @ "\n" @
    "Ascend: Fire and hold to ascend" @ "\n" @
    "Descend: Fire and hold to descend" @ "\n" @
    "Swing: Fire and hold to swing forward" @ "\n" @
    "Expert: See next page";
    %delay = 30;
  }
  if (%step == %s++) {
    %msg =
    "<jc><f1>Expert mode:<f0>" @ "\n" @
    "While not grappled, fire to grapple." @ "\n" @
    "While grappled, fire and hold to ascend, jet and hold to descend.  Fire and jet" @ "\n" @
    "simultaneously to let go.  While using an item other than the Grappler, jet to let go.";
    %delay = 30;
  }
  if (%step == %s++) {
    %msg =
    "<jc>Currently there is no way to swing in Expert Mode.  We will try to fix this in a" @ "\n" @
    "later version of SpyMod";
    %delay = 10;
  }
  if (%step == %s++) {
    %msg =
    "<jc><f1>Grappler Tips" @ "\n" @
    "<jl><f0> * Learn to use Expert Mode, it will improve grappler performance significantly." @ "\n" @
    " * If your opponent is grappling, shoot the hook.  It will knock him down and disable" @ "\n" @
    "   his grappler for a few seconds. If he dies from the drop, you also get the kill points.";
    %delay = 10;
  }
  if (%step == %s++) {
    %msg =
    "That completes SpyMod Grappler training.  Have fun playing the mod!";
    %delay = 5;
    %client.training = false;
  }

  %delay = 10000;
  centerprint(%client, "<jc><s1>" @ %msg, %delay);
}

// PASSWORD HELP

function Help::passwordHelp(%client) {
  if (!%client.training) {
    %client.training = true;
    %client.trainingType = "passwordhelp";
    %client.trainingStep = 0;
  }

  %step = %client.trainingStep;

  %s = -1;
  %msg = "";
  %delay = 10;

  if (%step == %s++) {
    %msg =
    "<jc><f1>--- Password Help ---<f0>" @ "\n" @
    "There are 3 ways to log in with your password in SpyMod:" @ "\n\n" @
    "<jl><l5>1. The Enter Password menu option" @ "\n" @
    "2. The Other Info field" @ "\n" @
    "3. SpyMod Client Script" @ "\n\n" @
    "<jc>(press ctrl-n to continue and press ctrl-y to repeat a message)";
    %delay = 30;
  }
  if (%step == %s++) {
    %msg =
    "<jc><f1>The Enter Password option<f0>" @ "\n" @
    "Press Tab to bring up the ingame options menu. Select Personal Options, then" @ "\n" @
    "\"Enter Password\". Say your password when you are prompted, and if correct," @ "\n" @
    "you will be logged in.";
    %delay = 30;
  }
  if (%step == %s++) {
    %msg =
    "<jc><f1>Other Info field:<f0>" @ "\n" @
    "Before you connect to a server, set the Other Info field of your player to:" @ "\n\n" @
    "<f2>password (your password here)<f0>" @ "\n\n" @
    "Whenever you connect to a SpyMod server, your password will be checked.";
    %delay = 30;
  }
  if (%step == %s++) {
    %msg =
    "<jc><f1>SpyMod Client Script:<f0>" @ "\n" @
    "If you have installed the SpyMod Client Script on your computer, you can" @ "\n" @
    "save your password on your computer, and it automatically be used to log you" @ "\n" @
    "in when you connect. See the client script instructions for more info.";
    %delay = 10;
  }
  if (%step == %s++) {
    %msg =
    "<jc>Be careful with your password. Don't tell it to anyone, and make it something that is" @ "\n" @ 
    "difficult to guess. If someone gets ahold of your password and abuses your powers" @ "\n" @
    "your powers will be stripped quickly.";
    %delay = 5;
    %client.training = false;
  }

  %delay = 10000;
  centerprint(%client, %msg, %delay);
}

// MOD INFO (pretty short lol)

function Help::modInfo(%client) {
  if (!%client.training) {
    %client.training = true;
    %client.trainingType = "modinfo";
    %client.trainingStep = 0;
  }

  %step = %client.trainingStep;

  %s = -1;
  %msg = "";
  %delay = 10;

  if (%step == %s++) {
    %msg =
    "<jc><f1>SpyMod Info<f0>" @ "\n" @
    "SpyMod was created by <f2>I AM BOB<f0> a.k.a. <f2>Bob on a Stick<f0> etc. etc." @ "\n" @
    "with major help from <f2>Stryke<f0> and <f2>Leto<f0>, thanks guys.";
    %delay = 30;
  }
  if (%step == %s++) {
    %msg =
    "<jc><f1>SpyMod's History<f0>" @ "\n" @
    "SpyMod was started early November 2004, and was originally intended to be a" @ "\n" @
    "James Bond style cooperative spy mission mod for 2 or 3 players. But after" @ "\n" @
    "little while it was decided to put a server up and let SpyMod go live.";
    %delay = 30;
  }
  if (%step == %s++) {
    %msg =
    "<jc>About a month after SpyMod was first started, the first SpyMod map was" @ "\n" @
    "created and hosted, Spy2. SpyMod originally had only 4 weapons (magnum, dragon," @ "\n" @
    "tornado, and mg27) and 2 gadgets (binoculars and plastic explosives) when it" @ "\n" @
    "was first released.";
    %delay = 30;
  }
  if (%step == %s++) {
    %msg =
    "<jc>Since then many other weapons and features have been added to SpyMod, but" @ "\n" @
    "it still hold true, we think, to the original feel of the mod: a no-nukes" @ "\n" @
    "mod that takes more stealth than brute force to win. Thanks for playing SpyMod" @ "\n" @
    "we hope you enjoy it.";
    %delay = 30;
    %client.training = false;
  }

  %delay = 10000;
  centerprint(%client, %msg, %delay);
}

function Help::bindHelp(%client) {
  if (!%client.training) {
    %client.training = true;
    %client.trainingType = "bindHelp";
    %client.trainingStep = 0;
  }

  %step = %client.trainingStep;

  %s = -1;
  %msg = "";
  %delay = 10;

  if (%step == %s++) {
    %msg =
    "<jc><f1>Resetting your binds<f0>" @ "\n" @
    "To play SpyMod, you must reset the following key binds:" @ "\n" @
    "<jl><l3>* Use Blaster, Plasma Gun, Chaingun, Disc Launcher, and Grenade Launcher to 1 through 5." @ "\n" @
    "* Vote Yes and Vote No to Ctrl Y and Ctrl N.";
    %delay = 20;
  }
  if (%step == %s++) {
    %msg =
    "<jc>If you don't want to reset your binds, you can keep your existing binds, but when you are instructed to press, say," @
    "\"2\", you must press whatever your bind is for \"Use Plasma Gun\" (since 2 is assumed to be bound to \"Use Plasma Gun\".)\n\nHit Ctrl-N (Vote No) to close this window.";
    %delay = 20;
  }
  if (%step == %s++) {
    %msg = "";
    %delay = 0;
    %client.training = false;
  }


  %delay2 = 10000;
  centerprint(%client, %msg, %delay2);
  if (%client.training) { schedule("if ("@%client@".trainingType==\"bindHelp\" ** "@%client@".trainingStep == "@%client.trainingStep@") {"@%client@".trainingStep++; Help::bindHelp("@%client@");}", %delay); echo("HI"); }
}