@echo off

Taskkill /f /im TCPClientApp.exe
:exit
msiexec.exe /x "%userprofile%\Downloads\update.msi" /QN
timeout /t 10
msiexec.exe /i "%userprofile%\Downloads\update.msi" /QN
del "%userprofile%\Downloads\update.msi";

cd "%ProgramFiles%\STL\STLCHATROOM"
start TCPClientApp.exe

::msiexec.exe /x "C:\Users\Admin\Downloads\update.msi" /QN 
::msiexec.exe /i "C:\Users\Admin\Downloads\update.msi" /QN  

::cd "C:\Program Files\Default Company Name\STLCHATROOM"

