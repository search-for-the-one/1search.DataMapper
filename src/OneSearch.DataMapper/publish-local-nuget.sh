#!/bin/bash

LOCAL_NUGET=~/source/install/local-nuget

mkdir nupkgs
dotnet publish OneSearch.DataMapper.csproj -c Release
dotnet pack OneSearch.DataMapper.csproj  -c Release --output nupkgs
nuget add nupkgs/OneSearch.DataMapper.*.nupkg -source $LOCAL_NUGET
rm -r nupkgs