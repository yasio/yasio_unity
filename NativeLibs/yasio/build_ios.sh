cmake -B build_ios -GXcode -DCMAKE_SYSTEM_NAME=iOS "-DCMAKE_OSX_ARCHITECTURES=arm64" -DYASIO_NO_DEPS=ON -DYASIO_ENABLE_NI=ON -DYASIO_ENABLE_KCP=ON -DYASIO_BUILD_TESTS=OFF
cmake --build build_ios --config Release
mkdir -p plugin_ios/Plugins/iOS/
cp build_ios/Release/libyasio.a plugin_ios/Plugins/iOS/libyasio.a 
