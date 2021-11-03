# yasio_unity - The unity c# wrapper of yasio

## Repo Introduction
- NSM(NetworkServiceManager)
- Depends on next release of yasio
- Plugins buld with vs2019, if can't load, please install the redist: https://dl.x-studio.net/#vs2019-redists
- If you wan't `non-stripped` android so, please download via https://github.com/yasio/OpenNSM2/actions 
  - `Plugins/Android/normal` are `non-stripped` so
  - `Plugins/Android/libs` are `strippped` so

## Class name
- YASIO_NI: yasio dllimport interfaces and enums
- NetworkServiceManager: manage network connection, message send, recv and others.
- INetworkPacketHandler: protocol abstract, could implemented with protobuf, flatbuffers and etc.
- SampleNetworkPacketHandler: The simplest binary protocol implementation.

## Run
Open the scene `SampleScene`, then click button `Login`
