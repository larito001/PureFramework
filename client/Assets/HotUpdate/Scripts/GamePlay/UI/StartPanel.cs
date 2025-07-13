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
            // YOTOFramework.uIMgr.Show(UIEnum.StartLoadingPanel);
            // YOTOFramework.timeMgr.DelayCall(() =>
            // {
            //     YOTOFramework.sceneMgr.LoadScene(Scenes.Normal); 
            //     YOTOFramework.sceneMgr.cameraCtrl.UsePlayerCamera();
            // },1);
            // YOTOFramework.timeMgr.DelayCall(() =>
            // {
            //     YOTOFramework.uIMgr.Hide(UIEnum.StartLoadingPanel);
            // },8);
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
