mkdir obj
mkdir bin

call "\git\corert\bin\Windows_NT.x64.Release\tools\ilc.exe" @".\ilc.rsp"

call link @".\link.rsp"
