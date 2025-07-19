using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YOTO;
using EventType = YOTO.EventType;

public class EnemeyHeaderData : YOTOScrollViewDataBase
{
}

public class ResSelectPanel : UIPageBase
{
    public YOTOScrollViewItem item;
    public YOTOScrollView list;
    public Button Goto;
    public Button GoBack;
    
    public Button AddWoodBtn;
    public Button RemoveWoodBtn;
    public Button AddIronBtn;
    public Button RemoveIronBtn;
    public TextMeshProUGUI WoodNum;
    public TextMeshProUGUI IronNum;
    public override void OnLoad()
    {
        list.Initialize(item.gameObject, 10);
        Goto.onClick.AddListener(OnGotoClick);
        GoBack.onClick.AddListener(OnGoBackClick);
        AddWoodBtn.onClick.AddListener(AddWoodInBag);
        RemoveWoodBtn.onClick.AddListener(RemoveWoodInBag);
        AddIronBtn.onClick.AddListener(AddIronInBag);
        RemoveIronBtn.onClick.AddListener(RemoveIronInBag);
        
    }

    private void OnGotoClick()
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
        PlayerResManager.Instance.Save();
    }
    private void OnGoBackClick()
    {
        YOTOFramework.uIMgr.Show(UIEnum.GameMap);
        CloseSelf();
    }
    private void RemoveIronInBag()
    {
        PlayerResManager.Instance.SetUseIron(-10);
    }

    private void AddIronInBag()
    {
        
        PlayerResManager.Instance.SetUseIron(10);
    }

    private void RemoveWoodInBag()
    {
        PlayerResManager.Instance.SetUseWood(-10);
    }

    private void AddWoodInBag()
    {
        PlayerResManager.Instance.SetUseWood(10);
    }


    public override void OnShow()
    {
        YOTOFramework.eventMgr.AddEventListener(EventType.RefreshResInfo,RefreshInfo);
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

    private void RefreshInfo()
    {
        IronNum.text= "Iron:" + PlayerResManager.Instance.GetIronNum();
        WoodNum.text = "Wood:" + PlayerResManager.Instance.GetWoodNum();
    }

    public override void OnHide()
    {
        YOTOFramework.eventMgr.RemoveEventListener(EventType.RefreshResInfo,RefreshInfo);
    }
}