@ECHO OFF

DEL scripts.vol

FOR %%Q IN (*.cs,*.gui) DO vt.exe -q -lzh -rle scripts.vol %%Q

FOR /D %%Q IN (*) DO FOR %%X IN (%%Q\*.cs,%%Q\*.gui) DO IF NOT %%Q==ext (
  IF NOT %%Q==missions (
    vt.exe -q -lzh -rle scripts.vol %%X
  )
)

vtlist *.cs scripts.vol