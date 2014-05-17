@echo off
cls
call build.cmd
msiexec /qb /i bin\raw\NuProj.msi