#!/bin/bash

mono --runtime=v4.0 ../Tools/NuGet/nuget.exe install FAKE -Version 3.35.2
mono --runtime=v4.0 FAKE.3.35.2/tools/FAKE.exe Targets.fsx $@