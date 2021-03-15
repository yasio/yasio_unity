mkdir -p build_osx && cd build_osx
cmake -GXcode ../ -DYAISO_BUILD_NI=ON
cd ..
cmake --build build_osx --config Release
mkdir -p plugin_osx/Plugins/yasio.bundle/Contents/MacOS/
cp build_osx/Release/yasio.bundle/Contents/MacOS/yasio plugin_osx/Plugins/yasio.bundle/Contents/MacOS/yasio
