#!/bin/bash

version="0.0.0"
if [ -n "$1" ]; then version="$1"
fi

tag="0.0.0"
if [ -n "$2" ]; then tag="$2"
fi
tag=${tag/tags\//}

dotnet pack .\\src\\MessageDispatch.Snapshotting.Core\\MessageDispatch.Snapshotting.Core.csproj -o .\\dist -p:Version="$version" -p:PackageVersion="$version" -p:Tag="$tag" -c Release
dotnet pack .\\src\\MessageDispatch.Snapshotting.Json\\MessageDispatch.Snapshotting.Json.csproj -o .\\dist -p:Version="$version" -p:PackageVersion="$version" -p:Tag="$tag" -c Release
dotnet pack .\\src\\MessageDispatch.Snapshotting.Protobuf\\MessageDispatch.Snapshotting.Protobuf.csproj -o .\\dist -p:Version="$version" -p:PackageVersion="$version" -p:Tag="$tag" -c Release
