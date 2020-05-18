@echo off

rem ************** Build ************** 
set /P BUILD=Do you want to build now [y/n]? 
if "%BUILD%"=="y" goto BUILD
goto END_BUILD

:BUILD
echo *** Building for .NET Standard 2.0...

dotnet build ..\Fasterflect\Fasterflect\Fasterflect.csproj -c release

md package
md package\lib
move /y ..\Fasterflect\Fasterflect\bin\Release\netstandard20 package\lib\netstandard20

rem ************** NuGet ************** 
set /P NUGET=Do you want to publish to NuGet now [y/n]? 
if /i "%NUGET%"=="y" goto NUGET
goto END

:NUGET
NOTEPAD Fasterflect.nuspec
echo *** Creating NuGet package
copy /y Fasterflect.nuspec .\package
nuget pack package\Fasterflect.nuspec
if errorlevel 1 goto PACK_FAIL

:VERSION
set /P VERSION=Enter version: 
if /i "%VERSION%"=="" goto VERSION
set PACKAGE=Fasterflect.%VERSION%.nupkg
echo *** Publishing NuGet package...
nuget push %PACKAGE%
goto END

:BUILD_FAIL
echo *** BUILD FAILED ***
goto END

:TEST_FAIL
echo *** TEST FAILED ***
goto END

:PACK_FAIL
echo *** PACKING FAILED ***
goto END

:END