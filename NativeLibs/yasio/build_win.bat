set build_cfg=%1
if not defined build_cfg set build_cfg=MinSizeRel
set cmake_options=-DBUILD_SHARED_LIBS=ON -DYASIO_NO_DEPS=ON -DYASIO_ENABLE_NI=ON -DYASIO_ENABLE_KCP=ON -DYASIO_BUILD_TESTS=OFF
cmake -B build_x64 -A x64 %cmake_options%
cmake --build build_x64 --config %build_cfg%
md plugin_win\Plugins\x86_64
copy /y build_x64\%build_cfg%\yasio.dll plugin_win\Plugins\x86_64\

cmake -B build_x86 -A Win32 %cmake_options%
cmake --build build_x86 --config %build_cfg%
md plugin_win\Plugins\x86
copy /y build_x86\%build_cfg%\yasio.dll plugin_win\Plugins\x86\

if "%GITHUB_ACTIONS%"=="" copy /y plugin_win\Plugins\x86_64\yasio.dll ..\..\Assets\Plugins\x86_64\
