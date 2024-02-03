mkdir -p build_osx && cd build_osx
cmake -GXcode ../ -DBUILD_SHARED_LIBS=ON -DYASIO_NO_DEPS=ON -DYASIO_ENABLE_NI=ON -DYASIO_ENABLE_KCP=ON -DYASIO_BUILD_TESTS=OFF
cd ..
cmake --build build_osx --config Release
mkdir -p plugin_osx/Plugins/yasio.bundle/Contents/MacOS/
cp build_osx/Release/yasio.bundle/Contents/MacOS/yasio plugin_osx/Plugins/yasio.bundle/Contents/MacOS/yasio
