@echo off 
echo *******************************************************************************
echo *
echo *              视频转flv，文档转swf
echo *
echo *    在同一目录生成一个同名的.swf或.flv文件！
echo *
echo *******************************************************************************
echo.

call :Fd %1 
goto :End 

:Fd 
for /F "tokens=*" %%i in ('dir %1 /ad /B') do call :Fd "%~f1\%%~nxi" 

cd "%~dp1"
%~d1
for /F "tokens=*" %%i in ('dir %1\*.* /B 2^>nul') do call %~dp0converter_close.cmd %~1\%%i
goto :EOF

:End
echo.
ENDLOCAL

@echo off
ping -n 2 127.0.0.1 > nul
