# DJFramework
Unity Framework by use YooAsset and Hybridclr  
客户端框架基于[Hybridclr热更框架](https://github.com/focus-creative-games/hybridclr/tree/main) 以及YooAsset资源管理框架进行设计，同时借鉴了UniFramework设计思路进行流程设计。服务器参考了《网络游戏开发实战》相关内容进行设计开发。通讯协议使用Protobuf协议，数据库使用SqlServer，数据存储框架使用EntityFramework框架。  
# 使用流程:  
## 1.整个框架目录如下：  
  Client：客户端内容  
  Libs:工具目录  
  Protobuf-Net：proto-net工具（无需改变）  
  Server：服务端目录  
## 2.客户端目录如下：  
  BundleResources目录用于存放热更资源，包括热更代码、热更场景以及热更的美术资源等等。  
  Editor存放编辑器扩展相关逻辑代码。  
  HotUpdate存放热更逻辑，需要热更的代码在该目录下进行编写。  
  Plugins存放第三方插件。  
  Resources目录存放第三方插件配置信息。  
  Scenes：存放游戏包体的场景（及不需要热更新的场景）。  
  Scripts：存放游戏框架代码，即不需要做热更新的代码。（更新该内容需要打新包，大版本更新）  
## 3.服务端目录如下：  
  ConfigDefine：用于存放游戏配置表的定义。  
  Configs：存放游戏配置表，游戏配置使用json文件进行存储。  
  GameService:存放服务端处理客户端消息的逻辑以及服务端必要的逻辑。  
  Manager：存放对应模块的管理类。  
  MessageHandler：存放消息分发逻辑。  
  NetWork：存放网络框架底层逻辑。  
  Proto：存放网络消息协议。（由Proto工具生成）  
  Tools：存放相关工具类。  
## 使用流程如下
### 1.客户端逻辑编写：
      非热更逻辑在Scripts目录下进行编写，热更逻辑在HotUpdate逻辑进行编写。  
### 2.客户端代码：
      HotUpdate代码热更，编写好的热更代码用Hybridclr工具点击CompleteDll，选择当前平台，生成的热更DLL在Client/HyBridClrData/HotUpdateDlls目录下，将生成的dll复制到BundleResources/Codes目录下，将其后缀名改为“ *.dll.bytes ”。
      补充元数据dll在Client/HyBridClrData/AssembliesPostIl2CppStrip目录下，也将其生成的dll复制到BundleResources/Codes，将其后缀名改为“ *.dll.bytes ”。
### 3.客户端资源打包
      （1）参考YooAssets资源打包。
      （2）将打包好的资源放在CDN资源服务器。  
### 4.服务端代码编写
      （1）MessagePatch中添加对应消息分发  
      （2）GameService中增加对应模块的逻辑处理  
      （3）Manager中添加对应模块的管理逻辑  
### 5.配置表生成及使用
      （1）\DJFramework\Libs\ConfigTools\Tables目录下添加对应的配置表excel文件  
      （2）使用Excel2Json工具生成对应的Json配置文件  
      （3）将生成的配置文件分别复制到客户端和服务端的Configs目录下  
      （4）在客户端和服务端的ConfigManager中分别添加对应配置的读取逻辑和存储逻辑  
### 6.ProtoBuf协议生成及使用
      （1）DJFramework\Libs\ProtoTool\proto\Message.proto文件中添加相关的proto协议消息  
      （2）使用DJFramework\Libs\ProtoTool\protogen.bat 工具将proto协议生成对应CS脚本文件  
      （3）将DJFramework\Libs\ProtoTool\Message\proto\Message.CS协议文件分别复制到客户端和服务端的Proto文件夹下  
### 7.数据库的创建及使用
      （1）打开GameDB.edmx文件，右键生成对应的数据表及其字段  
      （2）右键根据模型生成数据库  
      （3）将生成的sql语句进行复制，到数据库工具中新建查询，粘贴复制的sql语句，执行语句即可得到最新的数据库表（此时会覆盖掉数据库中已经存在的数据需要谨慎处理）
