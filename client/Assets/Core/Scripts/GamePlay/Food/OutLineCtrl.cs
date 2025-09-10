using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutLineCtrl : MonoBehaviour
{
    
    public Outline outline;
    private void Awake()
    {
        if (outline == null)
        {
            outline = GetComponent<Outline>();
        }
    }

    public void SetQuality(Quality quality)
    {
        switch (quality)
        {
            case Quality.Normal:
                outline.OutlineColor = new Color(0.78f, 0.78f, 0.78f); // 灰白，普通
                break;
            case Quality.Green:
                outline.OutlineColor = new Color(0.13f, 0.85f, 0.40f); // 亮绿色
                break;
            case Quality.Blue:
                outline.OutlineColor = new Color(0.20f, 0.60f, 1f);    // 天蓝/稀有
                break;
            case Quality.Purple:
                outline.OutlineColor = new Color(0.70f, 0.32f, 0.95f); // 紫晶色
                break;
            case Quality.Glod:
                outline.OutlineColor = new Color(1f, 0.77f, 0.17f);    // 金黄，史诗
                break;
            case Quality.Red:
                outline.OutlineColor = new Color(0.95f, 0.20f, 0.20f); // 鲜红/传说
                break;
            default:
                outline.OutlineColor = new Color(1f, 1f, 1f);          // 兜底白色
                break;
        }
    }


}
