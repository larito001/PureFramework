using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YOTO;

public class LootingPanel : UIPageBase
{
    public TextMeshProUGUI timer;
    public Scrollbar line;
    public override void OnLoad()
    {
        
    }

    public override void OnShow()
    {
        line.value = 0.5f;
        YOTOFramework.eventMgr.AddEventListener<List<IntKeyFloatValue>>(YOTOEventType.RefreshProgress,OnRefreshProgress);
        YOTOFramework.eventMgr.AddEventListener<int>(YOTOEventType.LootTimerNotify, OnLootTimerNotify);
    }

    private void OnLootTimerNotify(int index)
    {
        timer.text=index.ToString();
    }

    private void OnRefreshProgress(List<IntKeyFloatValue> list)
    {
        float selfValue = 0;
        float otherValue = 0;
        foreach (var info in list)
        {
            if (info.key == LoginPlugin.Instance.PlayerId)
            {
                selfValue = info.value;
            }
            else
            {
                otherValue += info.value;
            }
        }
    
        // 处理特殊情况：当两者都为0时，显示平衡状态（0.5）
        if (selfValue == 0 && otherValue == 0)
        {
            line.value = 0.5f;
            return;
        }
    
        // 计算总值
        float total = selfValue + otherValue;
    
        // 计算selfValue的比例，并将其映射到拔河效果
        // 当selfValue占比为0.5时，line.value为0.5
        // selfValue越大，line.value越接近1；otherValue越大，line.value越接近0
        line.value = selfValue / total;
    }
    public override void OnHide()
    {
        YOTOFramework.eventMgr.RemoveEventListener<List<IntKeyFloatValue>>(YOTOEventType.RefreshProgress,OnRefreshProgress);
        YOTOFramework.eventMgr.RemoveEventListener<int>(YOTOEventType.LootTimerNotify, OnLootTimerNotify);
        
    }

    public override void OnResize()
    {
        
    }
}
