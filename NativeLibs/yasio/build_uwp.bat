set cmake_options=-DBUILD_SHARED_LIBS=ON -DYASIO_NO_DEPS=ON -DYASIO_ENABLE_NI=ON -DYASIO_ENABLE_KCP=ON -DYASIO_BUILD_TESTS=OFF
cmake -B build_uwp -A Win32 -DCMAKE_SYSTEM_NAME=WindowsStore "-DCMAKE_SYSTEM_VERSION=10.0" %cmake_options%
cmake --build build_uwp --config MinSizeRel
md plugin_win\Plugins\WSA\x86
copy /Y build_uwp\MinSizeRel\yasio.dll plugin_win\Plugins\WSA\x86\

cmake -B build_uwp64 -A x64 -DCMAKE_SYSTEM_NAME=WindowsStore "-DCMAKE_SYSTEM_VERSION=10.0" %cmake_options%
cmake --build build_uwp64 --config MinSizeRel
md plugin_win\Plugins\WSA\x64
copy /Y build_uwp64\MinSizeRel\yasio.dll plugin_win\Plugins\WSA\x64\

cmake -B build_uwp_arm -A ARM -DCMAKE_SYSTEM_NAME=WindowsStore "-DCMAKE_SYSTEM_VERSION=10.0" %cmake_options%
cmake --build build_uwp_arm --config MinSizeRel
md plugin_win\Plugins\WSA\ARM
copy /Y build_uwp_arm\MinSizeRel\yasio.dll plugin_win\Plugins\WSA\ARM\

cmake -B build_uwp_arm64 -A ARM64 -DCMAKE_SYSTEM_NAME=WindowsStore "-DCMAKE_SYSTEM_VERSION=10.0" %cmake_options%
cmake --build build_uwp_arm64 --config MinSizeRel
md plugin_win\Plugins\WSA\ARM64
copy /Y build_uwp_arm64\MinSizeRel\yasio.dll plugin_win\Plugins\WSA\ARM64\
