using DJFrameWork.Log;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 游戏加载界面
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
    /// 设置提示内容
    /// </summary>
    /// <param name="tips"></param>
    public void SetTips(string tips)
    {
        if(string.IsNullOrEmpty(tips))
        {
            DJLog.Error("提示内容不能为空");
        }
        if(tipsText != null)
        {
            tipsText.text = tips;
        }
    }

    /// <summary>
    /// 更新进度
    /// </summary>
    /// <param name="percent">进度小数显示，值为0-1</param>
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
    /// 隐藏进度条
    /// </summary>
    public void HideSlider()
    {
        if(loadingSlider != null)
        {
            loadingSlider.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 显示进度条
    /// </summary>
    /// <param name="progress">默认进度</param>
    public void ShowSlider(float progress = 0)
    {
        if(loadingSlider != null)
        {
            loadingSlider.gameObject.SetActive(true);
            loadingSlider.value = progress;
        }
    }

    /// <summary>
    /// 隐藏进度百分比文本
    /// </summary>
    public void HidePercent()
    {
        if(percentText != null)
        {
            percentText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 显示进度百分比文本
    /// </summary>
    /// <param name="progress">默认进度</param>
    public void ShowPercent(float progress = 0)
    {
        if (percentText != null)
        {
            percentText.gameObject.SetActive(true);
            percentText.text = $"{progress * 100}%";
        }
    }
}
