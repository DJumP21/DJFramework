using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DJFrameWork.Singleton;
using System;
using DJFrameWork.Log;

public class MySceneManager : SingletonInstance<MySceneManager>, ISingleton
{
    Action onSceneLoaded;
    public void OnCreate(object createParam)
    {

    }

    public void OnDestroy()
    {
        DJSingleton.StopAllCoroutine();
    }

    public void OnUpdate()
    {

    }


    public void LoadScene(string sceneName, Action callBack)
    {
        DJSingleton.StartCoroutine(LoadSceneAsync(sceneName, callBack));
    }



    IEnumerator LoadSceneAsync(string sceneName, Action callBack)
    {
        onSceneLoaded = callBack;
        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        operation.completed += OnSceneLoadComplete;
        if (operation.progress > 0.98)
        {
            operation.allowSceneActivation = true;
        }
        yield return null;
        operation.allowSceneActivation = true;
    }

    void OnSceneLoadComplete(AsyncOperation operation)
    {
        if (operation.isDone)
        {
            onSceneLoaded?.Invoke();
            Debug.Log("场景加载成功");
        }
    }
}
