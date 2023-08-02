using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
/// <summary>
/// Mono单例基类
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoSingleton<T>:MonoBehaviour where T : MonoBehaviour
{
    public bool isGlobal;
    
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType<T>();
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (isGlobal)
        {
            if (instance != null && instance != this.gameObject.GetComponent<T>())
            {
                Destroy(this.gameObject);
                return;
            }
            DontDestroyOnLoad(this);
            instance=this.gameObject.GetComponent<T>();
        }
        this.OnStart();
    }

    protected void OnStart()
    {
        
    }
}
