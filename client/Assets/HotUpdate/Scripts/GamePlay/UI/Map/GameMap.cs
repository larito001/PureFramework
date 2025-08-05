using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using YOTO;
using EventType = YOTO.EventType;


public class GameMap : UIPageBase
{
    public YOTOScrollViewItem item;
    public YOTOScrollView list;
    public Button Goto;

    public Button AddWoodBtn;
    public Button RemoveWoodBtn;
    public Button AddIronBtn;
    public Button RemoveIronBtn;
    public TextMeshProUGUI WoodNum;
    public TextMeshProUGUI IronNum;
    public List<MapButton> btnList = new List<MapButton>();
    public Button closeBtn;

    public override void OnLoad()
    {
        list.Initialize(item.gameObject, 10);
        Goto.onClick.AddListener(OnGotoClick);
        AddWoodBtn.onClick.AddListener(AddWoodInBag);
        RemoveWoodBtn.onClick.AddListener(RemoveWoodInBag);
        AddIronBtn.onClick.AddListener(AddIronInBag);
        RemoveIronBtn.onClick.AddListener(RemoveIronInBag);
        for (var i = 0; i < btnList.Count; i++)
        {
            btnList[i].SetCallBack(EnterLevel);
        }

        closeBtn.onClick.AddListener(Gobackbtn);
    }

    private void Gobackbtn()
    {
        YOTOFramework.uIMgr.Show(UIEnum.StartPanel);
        YOTOFramework.sceneMgr.LoadScene<StartScene>();
        CloseSelf();
    }

    private void OnGotoClick()
    {
        // YOTOFramework.uIMgr.Show(UIEnum.StartLoadingPanel);

        YOTOFramework.eventMgr.TriggerEvent(EventType.GameStart);

        YOTOFramework.timeMgr.DelayCall(() => { YOTOFramework.uIMgr.Hide(UIEnum.StartLoadingPanel); }, 2);
        CloseSelf();
        PlayerResManager.Instance.Save();
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

    private void EnterLevel(int level)
    {
        YOTOFramework.sceneMgr.LoadScene<CityScene>();

    }

    public override void OnShow()
    {
        YOTOFramework.uIMgr.Hide(UIEnum.StartPanel);
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
        RefreshInfo();
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