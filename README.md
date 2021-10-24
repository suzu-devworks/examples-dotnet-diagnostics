# examples-dotnet-crashdump
Crash Dump Analysis for dotnet Application.

## What is this.

I found a way to output and analyze a crash dump in a .NET application debugger, so I tried it out.

* [.NET (core) debugging - Part 1 - Simple Managed Crash dump analysis with SOS](https://www.youtube.com/watch?v=ngKBJIV-BOs)
* [.NET (core) debugging - Part 2 - Simple Managed Hang dump analysis with SOS](https://www.youtube.com/watch?v=PBtQMIDNLmU)
* [.NET (core) debugging - Part 3 - Save Module and looking at code from dump](https://www.youtube.com/watch?v=RSWR4ZlpO48)
* [.NET (core) debugging - Part 4 - Debugging Managed memory leak](https://www.youtube.com/watch?v=tsaVPS3uPXI)


## Table of contents

* [Setting For Windows...](docs/SettingForWIndows.md)
* [Basic Crash dump Analyze...](docs/BasicCrashDumpAnalyze.md)
* Analize Probrems
    * [Deadlock...](src/Examples.Deadlock/README.md)
    * [Memoryleak...](src/Examples.Memoryleak/README.md)
    * [Stackoverflow...](src/Examples.Stackoverflow/README.md)
* [Save Modules from dump...](docs/SaveModulesFromDump.md)
* [Tools...](docs/Tools.md)


## The way to the present

```shell
git clone https://github.com/suzu-devworks/examples-dotnet-crashanalysis.git
cd examples-dotnet-crashanalysis

dotnet new sln -o .

dotnet new console -o src/Examples.Deadlock
dotnet sln add src/Examples.Deadlock/Examples.Deadlock.csproj

dotnet new console -o src/Examples.Memoryleak
dotnet sln add src/Examples.Memoryleak/Examples.Memoryleak.csproj

dotnet new console -o src/Examples.Stackoverflow
dotnet sln add src/Examples.Stackoverflow/Examples.Stackoverflow.csproj

dotnet build.


# Update outdated package
dotnet list package --outdated

```
