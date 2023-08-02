using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DJFrameWork.Singleton
{
    public interface ISingleton
    {
        /// <summary>
        /// 单例被创建时
        /// </summary>
        /// <param name="createParam"></param>
        void OnCreate(System.Object createParam);
        /// <summary>
        /// 更新单例
        /// </summary>
        void OnUpdate();
        /// <summary>
        /// 销毁单例
        /// </summary>
        void OnDestroy();
        
    }

}
