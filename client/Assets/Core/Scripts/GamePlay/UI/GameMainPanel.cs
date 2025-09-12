using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YOTO;

public class GameMainPanel : UIPageBase
{
    public TextMeshProUGUI txt_satiety;
    public TextMeshProUGUI txt_satisfaction;
    public Button findBtn;
    public override void OnLoad()
    {
    
    }

    public override void OnShow()
    {
        YOTOFramework.eventMgr.AddEventListener(YOTOEventType.RefreshPlayerProperty,RefreshPlayerProperty);
        findBtn.onClick.AddListener(OnClickFindBtn);
    }

    private void OnClickFindBtn()
    {
        PlayerPlugin.Instance.OnFindHostPlayerClick();
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
        findBtn.onClick.RemoveAllListeners();
    }
}
