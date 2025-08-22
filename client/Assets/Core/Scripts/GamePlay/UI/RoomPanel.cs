using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YOTO;

public class RoomPanel : UIPageBase
{
    public YOTOScrollView playerList;
    public Button leaveBtn;
    public Button readyBtn;
    public override void OnLoad()
    {
        leaveBtn.onClick.AddListener(() =>
        {
            YOTOFramework.netMgr.LeaveHost();
        });
    }

    public override void OnShow()
    {
        YOTOFramework.eventMgr.AddEventListener(YOTOEventType.RefreshRoleList,RefreshRoleList);
        playerList.Initialize();
    }

    private void RefreshRoleList()
    {
     var tempList =   LoginPlugin.Instance.GetPlayerDatas();
        
        playerList.SetData(tempList);
    }

    public override void OnHide()
    {
       
    }
}
