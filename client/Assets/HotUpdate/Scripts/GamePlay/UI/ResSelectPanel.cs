using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YOTO;

public class EnemeyHeaderData : YOTOScrollViewDataBase
{
}

public class ResSelectPanel : UIPageBase
{
    public YOTOScrollViewItem item;
    public YOTOScrollView list;
    public Button Goto;
    public Button GoBack;

    public override void OnLoad()
    {
        list.Initialize(item.gameObject, 10);
        Goto.onClick.AddListener(() =>
        {   
            YOTOFramework.uIMgr.Show(UIEnum.StartLoadingPanel);
            YOTOFramework.timeMgr.DelayCall(() =>
            {
                YOTOFramework.sceneMgr.LoadScene<NormalScene>(new NormalScene.NormalSceneParam()
                {
                    level = GameMapPlugin.Instance.level
                });
            },1);
     
            YOTOFramework.timeMgr.DelayCall(() =>
            {
                YOTOFramework.uIMgr.Hide(UIEnum.StartLoadingPanel);
            },2);
            CloseSelf();
        });
        GoBack.onClick.AddListener(() =>
        {
            YOTOFramework.uIMgr.Show(UIEnum.GameMap);
            CloseSelf();
        });
    }

    public override void OnShow()
    {
        List<EnemeyHeaderData> data = new List<EnemeyHeaderData>()
        {
            new EnemeyHeaderData(),
            new EnemeyHeaderData(),
            new EnemeyHeaderData(),
            new EnemeyHeaderData(),
            new EnemeyHeaderData(),
            new EnemeyHeaderData(),
            new EnemeyHeaderData(),
        };
        list.SetData(data);
    }

    public override void OnHide()
    {
    }
}