# OpenNSM2

## 仓库说明
- NSM(NetworkServiceManager), 没错NSM2才开放
- 纯Unity C#对yasio的封装，不依赖xlua
- 对应yasio版本: 3.37.1
- yasio核心相对于3.37.0没变，只是Interop接口变更

## 类说明（名字空间: NSM2）
- YASIO_NI: yasio dllimport接口和常量定义
- NetworkServiceManager: 负责网络服务管理，消息收发，连接管理
- INetworkPacketHandler: 协议抽象，实际实现可基于protobuf, flatbuffers等
- SampleNetworkPacketHandler: 简易协议实现

## 运行
打开 `SampleScene` 场景预览，点击按钮 `登录` 即可
