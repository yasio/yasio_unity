# OpenNSM2

## 仓库说明
- NSM(NetworkServiceManager), 没错NSM2才开放
- 纯Unity C#对yasio的封装
- 对应yasio版本: 3.37.1
- Plugins下win平台库使用vs2019编译，如果无法运行，请安装运行库: https://dl.x-studio.net/#vs2019-redists
- 如需未 `strip` 的android so，请前往 https://github.com/yasio/OpenNSM2/actions 下载
  - `Plugins/Android/normal` 目录下是未 `strip` 的so
  - `Plugins/Android/libs` 目录下是已经 `strip` 后的so

## 类说明（名字空间: NSM2）
- YASIO_NI: yasio dllimport接口和常量定义
- NetworkServiceManager: 负责网络服务管理，消息收发，连接管理
- INetworkPacketHandler: 协议抽象，实际实现可基于protobuf, flatbuffers等
- SampleNetworkPacketHandler: 简易协议实现

## 运行
打开 `SampleScene` 场景预览，点击按钮 `登录` 即可
