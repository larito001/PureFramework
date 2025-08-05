using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YOTO;
using EventType = YOTO.EventType;

public class StartPanel : UIPageBase
{
    public Button startBtn;
    public Button treeBtn;
    public override void OnLoad()
    {
       
        startBtn.onClick.AddListener(() =>
        {
            YOTOFramework.uIMgr.Show(UIEnum.StartLoadingPanel);
            YOTOFramework.sceneMgr.LoadScene<CityScene>();
            CloseSelf();
    
            YOTOFramework.timeMgr.DelayCall(() =>
            {
                YOTOFramework.eventMgr.TriggerEvent(EventType.GameStart);
                YOTOFramework.uIMgr.Hide(UIEnum.StartLoadingPanel);
            },2);
       
            // YOTOFramework.uIMgr.Show(UIEnum.GameMap);
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
