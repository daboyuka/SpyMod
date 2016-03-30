function Matrix::mul(%m1, %r1, %c1, %m2, %r2, %c2) {
  // %words[0-2,0-2] = row,col
  if (%c1 != %r2) return -1;

  for (%row = 0; %row < %r1; %row++) {
    for (%col = 0; %col < %c2; %col++) {
      %sum = 0;
      for (%i = 0; %i < %r1; %i++)
        %sum += Matrix::getElem(%m1, %row, %i, %c1) * Matrix::getElem(%m2, %i, %col, %c2);
      %words[%row, %col] = %sum;
      if (%row == 0 && %col == 0) %returnM = %words[%row, %col];
      else                        %returnM = %returnM @ " " @ %words[%row, %col];
    }
  }
  return %returnM;

}

function Matrix::getElem(%m, %r, %c, %mc) {
  return getWord(%m, %r * %mc + %c);
}

function Matrix::subMatrix(%m, %mr, %mc, %r, %c, %sr, %sc) {

  if (%sr + %r > %mr || %sc + %c > %mc) return -1;

  for (%row = %sr; %row < %sr + %r; %row++) {
    for (%col = %sc; %col < %sc + %c; %col++) {
      if (%row == %sr && %col == %sc) %returnM = Matrix::getElem(%m, %row, %col, %mc);
      else                            %returnM = %returnM @ " " @ Matrix::getElem(%m, %row, %col, %mc);
    }
  }
  return %returnM;
}

function sin(%angle) { return -getWord(Vector::getFromRot("0 0 " @ %angle), 0); }
function cos(%angle) { return getWord(Vector::getFromRot("0 0 " @ %angle), 1); }

// Affine transform matricies
function Matrix::rotationMatrix(%a, %b) {
  //%a = %a + 1.57;
  return -sin(%a)        @ " " @ cos(%a)         @ " " @ 0        @ " " @ //cos(%a)*cos(%b) @ " " @ sin(%a)*cos(%b) @ " " @ -sin(%b) @ " " @ 
         cos(%a)*cos(%b) @ " " @ sin(%a)*cos(%b) @ " " @ -sin(%b) @ " " @ //-sin(%a)        @ " " @ cos(%a)         @ " " @ 0        @ " " @ 
         cos(%a)*sin(%b) @ " " @ sin(%a)*sin(%b) @ " " @ cos(%b);
}

function Matrix::rotX(%x) {
  return 1 @ " " @ 0       @ " " @ 0        @ " " @
         0 @ " " @ cos(%x) @ " " @ -sin(%x) @ " " @
         0 @ " " @ sin(%x) @ " " @ cos(%x);
}
function Matrix::rotY(%x) {
  return cos(%x) @ " " @ 0 @ " " @ -sin(%x) @ " " @
         0       @ " " @ 1 @ " " @ 0        @ " " @
         sin(%x) @ " " @ 0 @ " " @ cos(%x);
}
function Matrix::rotZ(%x) {
  return cos(%x) @ " " @ -sin(%x) @ " " @ 0 @ " " @
         sin(%x) @ " " @ cos(%x)  @ " " @ 0 @ " " @
         0       @ " " @ 0        @ " " @ 1;
}

function Matrix::rotXYZ(%x, %y, %z) {
  return cos(%y)*cos(%z) @ " " @ (-sin(%x)*sin(%y)*cos(%z)-cos(%x)*sin(%z)) @ " " @ (-cos(%x)*sin(%y)*cos(%z)+sin(%x)*sin(%z)) @ " " @
         cos(%y)*sin(%z) @ " " @ (-sin(%x)*sin(%y)*sin(%z)+cos(%x)*cos(%z)) @ " " @ (-cos(%x)*sin(%y)*sin(%z)-sin(%x)*cos(%z)) @ " " @
         sin(%y)         @ " " @ sin(%x)*cos(%y)                            @ " " @ cos(%x)*cos(%y);
}

function Matrix::transpose(%m, %mr, %mc) {
  %newM = "";
  for (%r = 0; %r < %mr; %r++) {
    for (%c = 0; %c < %mc; %c++) {
      if (%r == 0 && %c == 0) %newM = getWord(%m, 0);
      else                    %newM = %newM @ " " @ Matrix::getElem(%m, %c, %r, %mr);
    }
  }
  return %newM;
}

function Matrix::mirrorOverDiagonal(%m, %mr, %mc) {
  return Matrix::transpose(%m, %mr, %mc);
}

function Matrix::matrixToMuzzleTransform(%m) {
  return Matrix::mirrorOverDiagonal(%m, 3, 3);
}
function Matrix::muzzleTransformToMatrix(%m) {
  return Matrix::mirrorOverDiagonal(%m, 3, 3);
}

function Matrix::transformMuzzle(%muzzle, %trans) {
  %muzzleTrans = Matrix::muzzleTransformToMatrix(Matrix::subMatrix(%muzzle, 4, 3, 3, 3));
  %newTrans = Matrix::mul(%trans, %muzzleTrans);
  return Matrix::matrixToMuzzleTransform(%newTrans) @ " " @
         getWord(%muzzle, 9) @ " " @ getWord(%muzzle, 10) @ " " @ getWord(%muzzle, 11);
}