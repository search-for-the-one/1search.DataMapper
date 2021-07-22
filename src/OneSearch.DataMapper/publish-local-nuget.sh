#!/bin/bash

LOCAL_NUGET=~/source/install/local-nuget

#nuget delete onesearch.datamapper 1.0.10 -s ~/source/install/local-nuget --non-interactive
#delete cache ~/.nuget/packages/onesearch.datamapper
mkdir nupkgs
dotnet publish OneSearch.DataMapper.csproj -c Release
dotnet pack OneSearch.DataMapper.csproj  -c Release --output nupkgs
nuget add nupkgs/OneSearch.DataMapper.*.nupkg -source $LOCAL_NUGET
rm -r nupkgs