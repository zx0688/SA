#!/bin/bash

echo 'build js...'
haxe js.hxml
echo 'build cs...'
haxe cs.hxml
echo 'inject dll into Unity project...'
mv bin/cs/bin/SL.dll ../Unity/Assets/
if [ $? -eq 0 ]; then
  echo "SUCCESS"
else
  echo "done with errors"
fi