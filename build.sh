#!/bin/bash

major="0"
if [ -n "$1" ]; then major="$1"
fi

minor="0"
if [ -n "$2" ]; then minor="$2"
fi

patch="0"
if [ -n "$3" ]; then patch="$3"
fi

version="$major"".""$minor"".""$patch"

versionSuffix=""
if [ -n "$4" ]; then versionSuffix="$4"
fi

fullVersion="$version"
if [ -n "$versionSuffix" ]; then fullVersion="$fullVersion-$versionSuffix"
fi

dotnet pack src -o ../../dist -p:Version="$version" -p:PackageVersion="$fullVersion" -c Release