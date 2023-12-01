#!/bin/bash

echo 'build cs...'
haxe cs.hxml
if [ $? -ne 0 ]; then
  echo "Error: Haxe build failed for cs.hxml"
  exit 1
fi

echo 'build js...'
haxe js.hxml

echo 'inject dll into Unity project...'
mv bin/cs/bin/SL.dll ../Unity/Assets/
if [ $? -eq 0 ]; then
  echo "SUCCESS"
else
  echo "done with errors"
fi