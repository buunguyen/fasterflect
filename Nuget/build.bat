echo off
set MSBUILD="%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
set MSTEST="C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\MSTest.exe"

rem ************** Build ************** 
set /P BUILD=Do you want to build now [y/n]? 
if "%BUILD%"=="y" goto BUILD
goto END_BUILD

:BUILD
echo *** Building for .NET 4...
%MSBUILD% /t:Rebuild /p:TargetFrameworkVersion=v4.0 /p:Configuration=Release /p:DefineConstants=DOT_NET_4  "..\Fasterflect\Fasterflect\Fasterflect.csproj"
if errorlevel 1 goto BUILD_FAIL
mkdir package\lib\net40
move /Y ..\Fasterflect\Fasterflect\bin\Release\Fasterflect.dll package\lib\net40\
move /Y ..\Fasterflect\Fasterflect\bin\Release\Fasterflect.xml package\lib\net40\

rem echo *** Running tests...
rem %MSBUILD% /t:Rebuild /p:TargetFrameworkVersion=v4.0 /p:Configuration=Release "..\Fasterflect\FasterflectTest\FasterflectTest.csproj"
rem if errorlevel 1 goto BUILD_FAIL
rem %MSTEST% /testcontainer:..\Fasterflect\FasterflectTest\bin\Release\FasterflectTest.dll
rem if errorlevel 1 goto TEST_FAIL

echo *** Building for .NET 3.5...
%MSBUILD% /t:Rebuild /p:TargetFrameworkVersion=v3.5 /p:Configuration=Release /p:DefineConstants=DOT_NET_35  "..\Fasterflect\Fasterflect\Fasterflect.csproj"
if errorlevel 1 goto BUILD_FAIL
mkdir package\lib\net35
move /Y ..\Fasterflect\Fasterflect\bin\Release\Fasterflect.dll package\lib\net35\
move /Y ..\Fasterflect\Fasterflect\bin\Release\Fasterflect.xml package\lib\net35\
:END_BUILD

rem ************** NuGet ************** 
set /P NUGET=Do you want to publish to NuGet now [y/n]? 
if /i "%NUGET%"=="y" goto NUGET
goto END

:NUGET
NOTEPAD Fasterflect.nuspec
echo *** Creating NuGet package
xcopy Fasterflect.nuspec package
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