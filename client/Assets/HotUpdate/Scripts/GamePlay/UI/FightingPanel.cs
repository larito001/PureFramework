using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YOTO;
using EventType = YOTO.EventType;

public class TestData : YOTOScrollViewDataBase
{
    public string name;
    public int price;
}
public class FightingPanel : UIPageBase
{
    public Button settingUI;
    public TextMeshProUGUI wood;
    public TextMeshProUGUI iron;
    public YOTOScrollView itemList;
    public YOTOScrollViewItem item;
    public override void OnLoad()
    {
        settingUI.onClick.AddListener(() =>
        {
            YOTOFramework.uIMgr.Show(UIEnum.FightingEndPanel);
        });
    }

    public override void OnShow()
    {
        itemList.Initialize(item.gameObject,10,true);
        itemList.SetData(new List<TestData>(10)
        {
            new TestData(),
            new TestData(),
            new TestData(),
            new TestData(),
            new TestData(),
        });
      YOTOFramework.eventMgr.AddEventListener(EventType.RefreshResInfo,RefreshInfo);
      RefreshInfo();
    }

    private void RefreshInfo()
    {
        wood.text = "Wood:" + PlayerResManager.Instance.GetWoodNum();
        iron.text = "Iron:" + PlayerResManager.Instance.GetIronNum();
    }
    public override void OnHide()
    {
        YOTOFramework.eventMgr.RemoveEventListener(EventType.RefreshResInfo,RefreshInfo);
    }
}
