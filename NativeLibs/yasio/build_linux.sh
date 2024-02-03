cmake -B build_linux64 -DBUILD_SHARED_LIBS=ON -DYASIO_NO_DEPS=ON -DYASIO_ENABLE_NI=ON -DYASIO_ENABLE_KCP=ON -DYASIO_BUILD_TESTS=OFF
cmake --build build_linux64 --config Release

mkdir -p plugin_linux/Plugins/x86_64
cp build_linux64/libyasio.so plugin_linux/Plugins/x86_64/