using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace View
{
    /// <summary>
    /// 视图基类
    /// </summary>
    public class BaseWindow
    {
        #region 所用到的变量

        //窗体物里
        protected Transform transform;

        //资源名称
        protected string resName;

        //是否常驻
        protected bool resident;

        //是否可见
        protected bool visible;

        //窗体类型
        protected WindowType windowType;

        //场景类型
        protected ScenesType scenesType;
        //UI控件 按钮

        //按钮列表
        protected Button[] buttonList;

        #endregion

        #region 接口

        //初始化
        protected virtual void Awake()
        {
        }

        //UI事件的注册
        protected virtual void RegisterUIEvent()
        {
        }

        //添加监听游戏事件
        protected virtual void OnAddListener()
        {
        }

        //移除游戏事件
        protected virtual void OnRemoveListener()
        {
        }

        //每次打开
        protected virtual void OnEnable()
        {
            //表示隐藏的物体也会被找到
            buttonList = transform.GetComponentsInChildren<Button>(true);
            visible = true;
        }

        //每次关闭
        protected virtual void OnDisable()
        {
        }

        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="deltaTime"></param>
        public virtual void Update(float deltaTime)
        {
        }

        //--------------WindowManager
        public void Open()
        {
            if (string.IsNullOrEmpty(resName))
            {
                return;
            }
            if (transform == null)
            {
                string path = ResManager.Instance.GetUIPath(resName);
                Debug.Log(path);
                ResManager.Instance.LoadAsset(path, (obj) =>
                {
                    GameObject go = obj as GameObject;
                    if (go != null)
                    {
                        transform = GameObject.Instantiate(go).GetComponent<Transform>();
                        transform.gameObject.SetActive(false);
                        UIRoot.SetParent(transform, false, this.windowType == WindowType.TipsWindow);
                        Show();
                    }
                });
            }
            else
            {
                Show();
            }
        }

        private void Show()
        {
            if (transform.gameObject.activeSelf == false)
            {
                UIRoot.SetParent(transform, true, windowType == WindowType.TipsWindow);
                transform.gameObject.SetActive(true);
                //调用激活的时候的事件
                OnEnable();
                //添加事件
                OnAddListener();
                RegisterUIEvent();
            }
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="isForceClose">是否强制关闭</param>
        public void Close(bool isForceClose = false)
        {
            if (transform.gameObject.activeSelf)
            {
                //移除事件监听
                OnRemoveListener();
                //隐藏的时候触发
                OnDisable();
                //是否强制销毁
                if (!isForceClose)
                {
                    if (resident)
                    {
                        transform.gameObject.SetActive(false);
                        UIRoot.SetParent(transform, false);
                    }
                    else
                    {
                        GameObject.Destroy(transform.gameObject);
                        transform = null;
                    }
                }
                else
                {
                    GameObject.Destroy(transform.gameObject);
                    transform = null;
                }
            }

            //不可见
            visible = false;
        }

        public async void PreLoad()
        {
            if (transform == null)
            {
                if ( await Create())
                {
                    //初始化
                    Awake();
                }
            }
        }

        public ScenesType GetSceneType()
        {
            return scenesType;
        }

        public WindowType GetWindowType()
        {
            return windowType;
        }

        public Transform GetRoot()
        {
            return transform;
        }

        /// <summary>
        /// 是否可见
        /// </summary>
        /// <returns></returns>
        public bool IsVisible()
        {
            return visible;
        }

        /// <summary>
        /// 是否常驻
        /// </summary>
        /// <returns></returns>
        public bool IsResident()
        {
            return resident;
        }

        private async UniTask<bool>  Create()
        {
            bool result=false;
            

            return result;
        }

        #endregion
    }
}

/// <summary>
/// 窗体类型
/// </summary>
public enum WindowType
{
    /// <summary>
    /// 登录窗口
    /// </summary>
    LoginWindow,
    /// <summary>
    /// 商店窗口
    /// </summary>
    StoreWindow,
    /// <summary>
    /// 提示窗口
    /// </summary>
    TipsWindow,
}

/// <summary>
/// 场景类型,根据场景提供预加载的功能
/// </summary>
public enum ScenesType
{
    None,
    Login,
    Battle,
}