#!/bin/bash

LOCAL_NUGET=~/source/install/local-nuget
VERSION=1.0.11

nuget delete onesearch.datamapper $VERSION -s ~/source/install/local-nuget -NonInteractive
rm -rf ~/.nuget/packages/onesearch.datamapper

mkdir nupkgs
dotnet publish OneSearch.DataMapper.csproj -c Release
dotnet pack OneSearch.DataMapper.csproj  -c Release --output nupkgs
nuget add nupkgs/OneSearch.DataMapper.*.nupkg -source $LOCAL_NUGET
rm -r nupkgs