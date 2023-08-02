using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DJFrameWork.Singleton
{
    public interface ISingleton
    {
        /// <summary>
        /// ����������ʱ
        /// </summary>
        /// <param name="createParam"></param>
        void OnCreate(System.Object createParam);
        /// <summary>
        /// ���µ���
        /// </summary>
        void OnUpdate();
        /// <summary>
        /// ���ٵ���
        /// </summary>
        void OnDestroy();
        
    }

}
