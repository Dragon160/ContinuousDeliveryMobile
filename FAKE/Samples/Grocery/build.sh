#!/bin/bash

mono --runtime=v4.0 ../Tools/NuGet/nuget.exe install FAKE -Version 3.5.4
mono --runtime=v4.0 FAKE.3.5.4/tools/FAKE.exe Targets.fsx $@