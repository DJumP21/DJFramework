using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DJFrameWork.Singleton
{
    public class SingletonDriver : MonoBehaviour
    {
        void Update()
        {
            DJSingleton.Update();
        }
    }
}

