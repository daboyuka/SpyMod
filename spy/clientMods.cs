function remotePlayAnimWav(%cl, %anim, %wav) {
  %time = getIntegerTime(true)/32;
  if (%cl.lastSoundMsg + 0.5 > %time) {
    Client::sendMessage(%cl, $MSGTypeGame, "Sound flood, you can only make a sound every 1 second");
    return;
  } else {
    %cl.lastSoundMsg = %time;
  }

  remotePlayAnim(%cl, %anim);
  playVoice(%cl, %wav);
}

function remoteLMSG(%cl, %wav) {
  %time = getIntegerTime(true)/32;
  if (%cl.lastSoundMsg + 0.5 > %time) {
    Client::sendMessage(%cl, $MSGTypeGame, "Sound flood, you can only make a sound every 1 second");
    return;
  } else {
    %cl.lastSoundMsg = %time;
  }

  playVoice(%cl, %wav);
}