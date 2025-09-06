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

        
    }

    private void ItemRender(YOTOScrollViewItem arg1,int  index)
    {
        var info =LoginPlugin.Instance.GetPlayerDatas();
        (arg1 as RoomListItem).SetData(info[index]);
    }


    public override void OnShow()
    {
        playerList.SetRenderer(ItemRender);
        YOTOFramework.eventMgr.AddEventListener(YOTOEventType.RefreshRoleList,RefreshRoleList);
        RefreshRoleList();

        leaveBtn.onClick.AddListener(() =>
        {
            YOTOFramework.netMgr.StopHost();
            YOTOFramework.netMgr.LeaveHost();
            LoginPlugin.Instance.OnNetError();
        });
        readyBtn.onClick.AddListener(() =>
        {
            LoginPlugin.Instance.GameStartRequest();
        });
    }

    private void RefreshRoleList()
    {
     var tempList =   LoginPlugin.Instance.GetPlayerDatas();
   
     
     playerList.Initialize(10);
     
     playerList.SetData(tempList.Count);

    }

   

    public override void OnHide()
    {
        leaveBtn.onClick.RemoveAllListeners();
        readyBtn.onClick.RemoveAllListeners();
        YOTOFramework.eventMgr.RemoveEventListener(YOTOEventType.RefreshRoleList,RefreshRoleList);
    }
}
