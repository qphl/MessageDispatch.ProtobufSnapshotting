#!/bin/bash

target="Default"
if [ -n "$1" ]; then target="$1"
fi

buildmode="Release"
if [ -n "$2" ]; then buildmode="$2"
fi

version="0.0.0"
if [ -n "$3" ]; then version="$3"
fi

dotnet pack src -o ../../dist -p:target="$target" -p:Version="$version" -p:PackageVersion="$version" -c "$buildmode"