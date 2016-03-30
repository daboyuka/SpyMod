$Admin::ADMIN_LOG_FILE = "config\\spyModAdminLog.log";

function Admin::logAdmin(%clientId, %wadHeDo) {
  $AdminMsg = Client::getName(%clientId) @ ": " @ %wadHeDo;
  export("$AdminMsg", $Admin::ADMIN_LOG_FILE, true);
}

function Admin::log(%msg) {
  $AdminMsg = %msg;
  export("$AdminMsg", $Admin::ADMIN_LOG_FILE, true);
}