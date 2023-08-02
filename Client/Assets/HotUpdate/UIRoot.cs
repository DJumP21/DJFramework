using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI根节点
/// </summary>
public class UIRoot
{
    private static Transform transform;
    //回收的窗口
    private static Transform recyclePool;
    //正在显示的窗口
    private static Transform workStation;
    //提示信息的窗体
    private static Transform noticeStation;

    private static bool isInit = false;
    
    public static void Init()
    {
        if (transform == null)
        {
            GameObject go;
            ResManager.Instance.LoadAsset(ResManager.Instance.GetUIPath("UIRoot"), (obj) =>
            {
                go = obj as GameObject;
                transform = GameObject.Instantiate(go).GetComponent<Transform>();
                if (recyclePool == null)
                {
                    recyclePool = transform.Find("RecyclePool");    
                }
                if (workStation == null)
                {
                    workStation = transform.Find("WorkStation");
                }
                if (noticeStation == null)
                {
                    noticeStation = transform.Find("NoticeStation");
                }
            });
            isInit = true;
        }
    }

    /// <summary>
    /// 设置窗体节点
    /// </summary>
    /// <param name="window"></param>
    /// <param name="isOpen"></param>
    /// <param name="isTipWindow"></param>
    public static void SetParent(Transform window,bool isOpen, bool isTipWindow=false)
    {
        if (!isInit)
        {
            Init();
        }
        if (isOpen)
        {
            if (isTipWindow)
            {
                window.SetParent(noticeStation,false);
            }
            else
            {
                window.SetParent(workStation,false);
            }
            
        }
        else
        {
            window.SetParent(recyclePool,false);
        }
    }
}
