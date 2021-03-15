cmake -B build_x64 -DBUILD_SHARED_LIBS=ON -DYAISO_BUILD_NI=ON
cmake --build build_x64 --config MinSizeRel

md plugin_win\Plugins\x86_64
copy /y build_x64\MinSizeRel\yasio.dll plugin_win\Plugins\x86_64\

cmake -A Win32 -B build_x86 -DBUILD_SHARED_LIBS=ON -DYAISO_BUILD_NI=ON
cmake --build build_x86 --config MinSizeRel

md plugin_win\Plugins\x86
copy /y build_x86\MinSizeRel\yasio.dll plugin_win\Plugins\x86\
