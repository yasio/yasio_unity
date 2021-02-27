cmake -B build_x64 -DYASIO_BUILD_AS_SHARED=ON -DYAISO_BUILD_NI=ON
cmake --build build_x64 --config MinSizeRel
copy build_x64\MinSizeRel\yasio.dll ..\..\Assets\Plugins\x86_64
pause
