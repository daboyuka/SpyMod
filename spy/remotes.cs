function remoteActiveMode() {}
function remoteFetchData() {}
function remoteBWAdmin::isCompatible() {}
function remoteCheckDune() {}
function remoteTrue(%c) {if (!%c.remoteTrue) {schedule("Net::kick("@%c@", \"You have been kicked for hacking: remote function call spamming\");", 0);%c.remoteTrue=true;}}

function remoteFakeDeath(%client) { // :D
  remoteKill(%client);
  Client::sendMessage(%client, 1, "Sorry, this mod doesn't support fakedeath. Hope this is close enough.");
}