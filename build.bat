@echo off
rem Build without any SDK: the C# compiler ships with .NET Framework 4.x (every Windows 10/11).
set CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe
if not exist "%CSC%" set CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe
if not exist bin mkdir bin
if not exist model mkdir model

echo === 1. Compile University.exe ===
"%CSC%" /nologo /codepage:65001 /target:exe /out:bin\University.exe /recurse:src\*.cs
if errorlevel 1 exit /b 1

echo === 2. Compile ReverseUml.exe (reverse engineering tool) ===
"%CSC%" /nologo /codepage:65001 /target:exe /out:bin\ReverseUml.exe /r:bin\University.exe tools\ReverseUml.cs
if errorlevel 1 exit /b 1

echo === 3. Reverse engineering: code to class diagram ===
bin\ReverseUml.exe bin\University.exe UniversityIS model\class-diagram.puml
if errorlevel 1 exit /b 1

echo Build OK.
