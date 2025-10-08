using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPanel : UIPageBase
{
    public YOTOScrollView scrollView;
    private List<string> settingList = new List<string>()
    {
        "视频",
        "音频",
        "游戏设置",
    };
    public override void OnLoad()
    {
        scrollView.Initialize();
    }

    public override void OnShow()
    {
        scrollView.SetRenderer( ItemRender);
        scrollView.SetData(settingList.Count);
    }

    private void ItemRender(YOTOScrollViewItem item, int index)
    {
        var btn = (item as SettingItemBtn);
        btn?.SetBtnData(settingList[index],index);
    }

    public override void OnHide()
    {
    }

    public override void OnResize()
    {
    }
}