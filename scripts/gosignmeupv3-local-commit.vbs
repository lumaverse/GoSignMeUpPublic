' http://tortoisesvn.net/docs/release/TortoiseSVN_en/tsvn-automation.html

const svn="c:\Program Files\TortoiseSVN\bin"
set shell = CreateObject( "WScript.Shell" )
set fso = createobject("scripting.filesystemobject")
set folder = fso.getfolder(shell.CurrentDirectory& "\..\")
project = folder.path

shell.run """" & svn & "\TortoiseProc.exe" & """" & " /command:commit /path:" & project
