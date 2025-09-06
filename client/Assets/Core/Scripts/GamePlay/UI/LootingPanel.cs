using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YOTO;

public class LootingPanel : UIPageBase
{
    public Scrollbar self;
    public Scrollbar other;
    public override void OnLoad()
    {
        
    }

    public override void OnShow()
    {
        YOTOFramework.eventMgr.AddEventListener<int,float>(YOTOEventType.RefreshProgress,OnRefreshProgress);
    }

    private void OnRefreshProgress(int id,float value)
    {
         if (id == LoginPlugin.Instance.PlayerId)
        {
            self.value = value;
        }
        else 
        {
            other.value = value;
        }
    }

    public override void OnHide()
    {
        YOTOFramework.eventMgr.RemoveEventListener<int,float>(YOTOEventType.RefreshProgress,OnRefreshProgress);
        
    }
}
