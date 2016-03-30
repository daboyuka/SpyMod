function IP::getAddress(%ip) {
  if (String::getSubStr(%ip, 0, 8) == "LOOPBACK") {
    %ipStr = "LOOPBACK";
  } else {
    %ind = String::findSubStr(%ip, ":") + 1;
    %len = String::findSubStr(String::getSubStr(%ip, %ind, 1024), ":");
    %ipStr = String::getSubStr(%ip, %ind, %len);
  }
  return %ipStr;
}

function IP::getPrefix(%ip) {
  %firstColon = String::findSubStr(%ip, ":");
  %prefix = String::getSubStr(%ip, 0, %firstColon);
  return %prefix;
}

// Gotta put something in the beginning, otherwise tribes thinks the IPs are numbers :?
function IP::areEqual(%ip1, %ip2) {
  return "X" @ %ip1 == "X" @ %ip2;
}

function IP::isLocalAddress(%ip) {
  if (String::getSubStr(%ip, 0, 8) == "LOOPBACK") return true;

  %prefix = IP::getPrefix(%ip);
  %address = IP::getAddress(%ip);

  if (%prefix == "IPX" && IP::areEqual(%address, "1234CDEF")) return true;
  if (%prefix == "IP" && IP::areEqual(%address, "127.0.0.1")) return true;
  return false;
}

function IP::isNetworkAddress(%ip) {
  %prefix = IP::getPrefix(%ip);
  %address = IP::getAddress(%ip);

  if (%prefix == "IPX") return true;
  if (%prefix == "IP" && IP::areEqual(String::getSubStr(%address, 0, 7), "192.168")) return true;
  return false;
}

function IP::isBob(%ip) {
  %address = IP::getAddress(%ip);
  if (IP::areEqual(%address, "69.132.116.83")) return true;
  return false;
}