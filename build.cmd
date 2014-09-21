@ECHO OFF

:: Note: We've disabled node reuse because it causes file locking issues.
::       The issue is that we extend the build with our own targets which
::       means that that rebuilding cannot successfully delete the task
::       assembly.

"%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe" "%~dp0\build.proj" /nologo /m /v:m /nr:false /flp:verbosity=normal %1 %2 %3 %4 %5 %6 %7 %8 %9