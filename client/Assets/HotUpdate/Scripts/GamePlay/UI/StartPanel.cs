using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YOTO;

public class StartPanel : UIPageBase
{
    public Button startBtn;
    public Button treeBtn;
    public override void OnLoad()
    {
        YOTOFramework.sceneMgr.cameraCtrl.UseStartCamera();
        startBtn.onClick.AddListener(() =>
        {
           
            YOTOFramework.uIMgr.Show(UIEnum.GameMap);
            CloseSelf();
        });
        treeBtn.onClick.AddListener(() =>
        {
            YOTOFramework.uIMgr.Show(UIEnum.SkillTreePanel);
        });
    }

    public override void OnShow()
    {

    }

    public override void OnHide()
    {

    }
}
