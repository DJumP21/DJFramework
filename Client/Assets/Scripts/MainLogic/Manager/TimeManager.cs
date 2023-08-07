using DJFrameWork.Log;
using DJFrameWork.NetWork;
using DJFrameWork.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TimeManager : SingletonInstance<TimeManager>, ISingleton
{
    public float nowTime { get; private set; }
    private bool openTimer = false;

    public void OnCreate(object createParam)
    {
        nowTime = 0;   
    }

    public void OnDestroy()
    {
        
    }

    public void OnUpdate()
    {
        if(openTimer)
        {
            nowTime += Time.deltaTime;
        }
        if(NetManager.connectSuccess)
        {
            NetManager.Update();
        }
    }

    /// <summary>
    /// 开始计时
    /// </summary>
    public void OpenTime()
    {
        if(!openTimer)
        {
            openTimer = true;
            nowTime = 0;
        }
        else
        {
            DJLog.Error("计时器已经开始，无法重复计时");
        }
    }
}
