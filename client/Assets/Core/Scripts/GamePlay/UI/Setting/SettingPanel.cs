using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YOTO;

public class SettingPanel : UIPageBase
{
    public Button backBtn;
    public YOTOScrollView scrollView;
    public List<BaseSettingCtrl> settingCtrlList =new List<BaseSettingCtrl>();
    public List<string> settingList = new List<string>()
    {
        "视频",
        "音频",
        "游戏设置",
    };
    public override void OnLoad()
    {
   
        scrollView.Initialize();
    }

    private void GoBack()
    {
        CloseSelf();
        YOTOFramework.soundMgr.PlaySFX("Sound/SFX_UI_Click_Designed_Pop_Negative_Close_1",0.5f);
    }

    public override void OnShow()
    {
        scrollView.SetRenderer( ItemRender);
        backBtn.onClick.AddListener(GoBack);
        scrollView.SetData(settingList.Count);
        ShowSetting(0);
    }

    private void ItemRender(YOTOScrollViewItem item, int index)
    {
        var btn = (item as SettingItemBtn);
        btn?.SetBtnData(this,index);
    }

    public void ShowSetting(int index)
    {
        for (int i = 0; i < settingCtrlList.Count; i++)
        {
            if (index == i)
            {
                settingCtrlList[i].gameObject.SetActive(true);
            }
            else
            {
                settingCtrlList[i].gameObject.SetActive(false);
            }
            
        }
    }
    public override void OnHide()
    {
        backBtn.onClick.RemoveAllListeners();
    }

    public override void OnResize()
    {
    }
}