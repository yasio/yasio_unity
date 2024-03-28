# yasio_unity - The unity c# wrapper of yasio

## Unity version

2021.3.36f1+

## Repo Introduction
- NSM(NetworkServiceManager)
- Plugins buld with vs2022, if can't load, please install the redist: https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170

## Class name
- YASIO_NI: yasio dllimport interfaces and enums
- NetworkServiceManager: manage network connection, message send, recv and others.
- INetworkPacketHandler: protocol abstract, could implemented with protobuf, flatbuffers and etc.
- SampleNetworkPacketHandler: The simplest binary protocol implementation.

## Run
Open the scene `SampleScene`, then click button `Login`
