@echo off

cd "%~dp1"
%~d1

echo *******************************************************************************
echo *
echo *    %~dp1%~nx1
echo *    正在转换文件，请等待。可能需要一些时间....

"%~dp0Pub.Class.ToSwf.exe" "%~dp1%~nx1"
if %ERRORLEVEL% == 0 (
	echo *
	echo *    文件转换成功！
	echo *
) else (
	echo *
	echo *    文件转换失败！
	echo *
	goto End
)
goto End


:End
echo *******************************************************************************
echo.

