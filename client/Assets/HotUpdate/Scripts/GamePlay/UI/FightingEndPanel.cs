using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YOTO;

public class FightingEndPanel : UIPageBase
{
    public Button gobackBtn;
    public override void OnLoad()
    {
        gobackBtn.onClick.AddListener(() =>
        {
            YOTOFramework.sceneMgr.LoadScene<StartScene>();
        });
    }

    public override void OnShow()
    {

    }

    public override void OnHide()
    {

    }
}
