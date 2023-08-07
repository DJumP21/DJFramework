using DJFrameWork.Event;
using DJFrameWork.Manager;
using DJFrameWork.Singleton;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Hosting;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
public class GameStart : MonoBehaviour
{
    /// <summary>
    /// ����ģʽ
    /// </summary>
    public EPlayMode PlayMode=EPlayMode.EditorSimulateMode;
    /// <summary>
    /// ����ƽ̨
    /// </summary>
    public RuntimePlatform platform;

    private GameObject gameManager;

    private void Awake()
    {
        platform = Application.platform;
        Debug.Log($"��Դ����ģʽ:{PlayMode}");
        Debug.Log($"��ǰƽ̨��{platform}");
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        
    }

    private void Start()
    {
        gameManager = GameObject.Find("GameManager");
        DontDestroyOnLoad( gameManager );

        //��ʼ���¼�ϵͳ
        DJEvent.Init(gameManager);
        //��ʼ������ϵͳ
        DJSingleton.Init(gameManager);

        //��ʼ����Դ����ϵͳ
        YooAssets.Initialize();
        YooAssets.SetOperationSystemMaxTimeSlice(30);

        //���س���������
        DJSingleton.CreateSingleton<MySceneManager>();
        //������Դ������
        DJSingleton.CreateSingleton<ResManager>();
        //�������ù�����
        DJSingleton.CreateSingleton<ConfigManager>();

        DJSingleton.CreateSingleton<TimeManager>();

        //�л����ȸ�����
        MySceneManager.Instance.LoadScene("Loading", () =>
        {
            //�������¹�����
            DJSingleton.CreateSingleton<PatchManager>();
            //������������
            PatchManager.Instance.Run(this.PlayMode);
        });

    }

}
