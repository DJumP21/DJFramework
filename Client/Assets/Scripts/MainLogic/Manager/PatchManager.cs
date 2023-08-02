using DJFrameWork.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
using DJFrameWork.StateMachine;
using DJFrameWork.Singleton;
using DJFrameWork.Log;
using System.Reflection;

namespace DJFrameWork.Manager
{
    /// <summary>
    /// 热更管理
    /// </summary>
    public class PatchManager : SingletonInstance<PatchManager>,ISingleton
    {
        //运行模式
        public EPlayMode playMode;
        //资源包版本信息
        public string packageVersion;
        //下载器
        public ResourceDownloaderOperation downLoader;

        //更新加载界面  放在单例里面方便其他流程调用
        public LoadingWindow loadingWindow;

        //热更dll  用于启动热更流程
        public Assembly hotUpdateAssembly;

        private bool isRun = false;
        //事件组
        private EventGroup eventGroup = new EventGroup();
        //状态机
        private DJStateMachine  stateMachine;


        public void OnCreate(object createParam)
        {
       
        }

        public void OnDestroy()
        {
            eventGroup.RemoveAllListeners();
        }

        public void OnUpdate()
        {
            if(stateMachine != null)
            {
                stateMachine.Update();
            }
        }

        public void Run(EPlayMode playMode)
        {
            DJLog.Log("开始热更");
            this.playMode = playMode;
            if (!isRun)
            {
                isRun = true;
                this.playMode = playMode;
                //注册监听事件
                //用户初始化游戏
                eventGroup.AddListener<UserEventDefine.UserTryInitializeAgain>(OnEventMessageHandler);
                //下载资源包
                eventGroup.AddListener<UserEventDefine.UserDownLoadWebFilesBegin>(OnEventMessageHandler);
                //更新资源版本
                //更新资源
                //下载资源
                DJLog.Log("启动状态机");
                stateMachine = new DJStateMachine(this);
                //添加状态节点
                stateMachine.AddNode<PatchPrepareNode>();
                stateMachine.AddNode<InitializeNode>();
                stateMachine.AddNode<UpdateVersionNode>();
                stateMachine.AddNode<UpdateManifestNode>();
                stateMachine.AddNode<DownLoadWebFilesNode>();
                stateMachine.AddNode<LoadHotUpdateDllNode>();
                stateMachine.AddNode<PatchFinishNode>();
                //默认启动更新准备节点
                stateMachine.Run<PatchPrepareNode>();
            }
            else
            {
                Debug.Log("更新正在进行中...");
            }
        }


        private void OnEventMessageHandler(IEventMessage message)
        {
            //用户更新 切换到资源初始化节点
            if(message is UserEventDefine.UserTryInitializeAgain)
            {
                stateMachine.ChangeStateNode<InitializeNode>();
            }
            if(message is UserEventDefine.UserDownLoadWebFilesBegin)
            {
                //stateMachine.ChangeStateNode<DownLoadWebFilesNode>();
            }

        }

    }
}

