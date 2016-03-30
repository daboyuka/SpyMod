function Admin::hasPrivilege(%client, %priv) {
  if ($AdminPrivileges[%priv] == -1 || $AdminPrivileges[%priv] == "") return false;
  return Admin::compareAdminToPowers(%client, $AdminPrivileges[%priv]) >= 0;
}