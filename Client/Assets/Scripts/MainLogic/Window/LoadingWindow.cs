using DJFrameWork.Log;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ��Ϸ���ؽ���
/// </summary>
public class LoadingWindow : MonoBehaviour
{
    public Text tipsText;
    public Text percentText;
    public Slider loadingSlider;

    void Awake()
    {
        tipsText = GameObject.Find("Tips").GetComponent<Text>();
        percentText = GameObject.Find("Percent").GetComponent<Text>();
        loadingSlider = GameObject.Find("LoadingSlider").GetComponent <Slider>();
    }

    /// <summary>
    /// ������ʾ����
    /// </summary>
    /// <param name="tips"></param>
    public void SetTips(string tips)
    {
        if(string.IsNullOrEmpty(tips))
        {
            DJLog.Error("��ʾ���ݲ���Ϊ��");
        }
        if(tipsText != null)
        {
            tipsText.text = tips;
        }
    }

    /// <summary>
    /// ���½���
    /// </summary>
    /// <param name="percent">����С����ʾ��ֵΪ0-1</param>
    public void UpdateProgress(float progress)
    {
        if(percentText != null)
        {
            percentText.text = $"{progress*100}%";
            if(loadingSlider != null && loadingSlider.gameObject.activeSelf)
            {
                loadingSlider.value = progress;
            }
        }
    }

    /// <summary>
    /// ���ؽ�����
    /// </summary>
    public void HideSlider()
    {
        if(loadingSlider != null)
        {
            loadingSlider.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ��ʾ������
    /// </summary>
    /// <param name="progress">Ĭ�Ͻ���</param>
    public void ShowSlider(float progress = 0)
    {
        if(loadingSlider != null)
        {
            loadingSlider.gameObject.SetActive(true);
            loadingSlider.value = progress;
        }
    }

    /// <summary>
    /// ���ؽ��Ȱٷֱ��ı�
    /// </summary>
    public void HidePercent()
    {
        if(percentText != null)
        {
            percentText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ��ʾ���Ȱٷֱ��ı�
    /// </summary>
    /// <param name="progress">Ĭ�Ͻ���</param>
    public void ShowPercent(float progress = 0)
    {
        if (percentText != null)
        {
            percentText.gameObject.SetActive(true);
            percentText.text = $"{progress * 100}%";
        }
    }
}
