using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YOTO;

public class SettingItemBtn : YOTOScrollViewItem
{
    public TextMeshProUGUI text;
    SettingPanel panel;
    private int index = -1;
    public void SetBtnData(SettingPanel panel, int index)
    {
        this.panel= panel;
        text.text = panel.settingList[index];
        this.index = index;
    }

    public override void OnRenderItem()
    {
        base.OnRenderItem();
        this.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        YOTOFramework.soundMgr.PlaySFX("Sound/SFX_UI_Click_Designed_Pop_Open_2",0.5f);
        panel?.ShowSetting(index);
    }
    public override void OnHidItem()
    {
        base.OnHidItem();
        this.GetComponent<Button>().onClick.RemoveAllListeners();
    }
}