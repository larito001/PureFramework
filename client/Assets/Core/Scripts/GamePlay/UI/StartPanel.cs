using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YOTO;
using EventType = YOTO.EventType;

public class StartPanel : UIPageBase
{
    public Button startBtn;
    public override void OnLoad()
    {
       
        startBtn.onClick.AddListener(() =>
        {
            CloseSelf();
        });
  
    }

    public override void OnShow()
    {

    }

    public override void OnHide()
    {

    }
}
