using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YOTO;

public class GameMainPanel : UIPageBase
{
    public TextMeshProUGUI txt_satiety;
    public TextMeshProUGUI txt_satisfaction;
    public override void OnLoad()
    {
        
    }

    public override void OnShow()
    {
        YOTOFramework.eventMgr.AddEventListener(YOTOEventType.RefreshPlayerProperty,RefreshPlayerProperty);
    }

    private void RefreshPlayerProperty()
    {
        var self = PlayerPlugin.Instance.GetSelf();
        txt_satiety.text ="satiety:" +self.SatietyValue;
        txt_satisfaction.text ="satisfaction:" +self.SatisfactionValue;
        
    }

    public override void OnHide()
    {
        YOTOFramework.eventMgr.RemoveEventListener(YOTOEventType.RefreshPlayerProperty,RefreshPlayerProperty);
        
    }
}
