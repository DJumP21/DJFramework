# DJFramework
Unity Framework by use YooAsset and Hybridclr 
客户端框架基于Hybridclr热更框架以及YooAsset资源管理框架进行设计，同时借鉴了UniFramework设计思路进行流程设计。服务器参考了《网络游戏开发实战》相关内容进行设计开发。通讯协议使用Protobuf协议，数据库使用SqlServer，数据存储框架使用EntityFramework框架。
使用流程:
1.整个框架目录如下：
  Client：客户端内容
  Libs:工具目录
  Protobuf-Net：proto-net工具（无需改变）
  Server：服务端目录
2.客户端目录如下：
  BundleResources目录用于存放热更资源，包括热更代码、热更场景以及热更的美术资源等等。
  Editor存放编辑器扩展相关逻辑代码。
  HotUpdate存放热更逻辑，需要热更的代码在该目录下进行编写。
  Plugins存放第三方插件。
  Resources目录存放第三方插件配置信息。
  Scenes：存放游戏包体的场景（及不需要热更新的场景）。
  Scripts：存放游戏框架代码，即不需要做热更新的代码。（更新该内容需要打新包，大版本更新）
3.服务端目录如下：
    ConfigDefine：用于存放游戏配置表的定义
  

