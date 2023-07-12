# yasio_unity - The unity c# wrapper of yasio

## Repo Introduction
- NSM(NetworkServiceManager)
- Plugins buld with vs2022, if can't load, please install the redist: https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170
- If you wan't `non-stripped` android so, please download via https://github.com/yasio/yasio_unity/actions
  - `Plugins/Android/normal` are `non-stripped` so
  - `Plugins/Android/libs` are `strippped` so

## Class name
- YASIO_NI: yasio dllimport interfaces and enums
- NetworkServiceManager: manage network connection, message send, recv and others.
- INetworkPacketHandler: protocol abstract, could implemented with protobuf, flatbuffers and etc.
- SampleNetworkPacketHandler: The simplest binary protocol implementation.

## Run
Open the scene `SampleScene`, then click button `Login`
