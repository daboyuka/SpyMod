function Extensions::execAlphaExtensions() {
  Extensions::execExtensions("alpha");
}

function Extensions::execPreloadExtensions() {
  Extensions::execExtensions("pre");
}

function Extensions::execPostloadExtensions() {
  Extensions::execExtensions("post");
}

function Extensions::execPremissionExtensions() {
  Extensions::execExtensions("premission");
}

function Extensions::execPostmissionExtensions() {
  Extensions::execExtensions("postmission");
}

function Extensions::execExtensions(%x) {
  %s = "ext\\*."@%x@".cs";
  %f = File::findFirst(%s);
  while (%f != "") {
    exec(%f);
    %f = File::findNext(%s);
  }
}