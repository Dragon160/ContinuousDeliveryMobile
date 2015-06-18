@echo off

"../../../Tools/NuGet/nuget.exe" install FAKE -Version 3.35.2

"FAKE.3.35.2/tools/FAKE.exe" Targets.fsx "%*"