using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinishPanel : UIPageBase
{
    public TextMeshProUGUI winName;
    public override void OnLoad()
    {
        
    }

    public override void OnShow()
    {
        winName.text = StagePlugin.Instance.winPlayerData[0].playerName;
    }

    public override void OnHide()
    {
        
    }

    public override void OnResize()
    {
        
    }
}
