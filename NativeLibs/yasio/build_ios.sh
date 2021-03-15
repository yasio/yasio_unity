mkdir -p build_ios && cd build_ios
cmake -DCMAKE_SYSTEM_NAME=iOS "-DCMAKE_OSX_ARCHITECTURES=armv7;arm64" -GXcode ../
cd ..
cmake --build build_ios --config Release
mkdir -p plugin_ios/Plugins/iOS/
cp build_ios/Release-iphoneos/libyasio.a plugin_ios/Plugins/iOS/libyasio.a 

