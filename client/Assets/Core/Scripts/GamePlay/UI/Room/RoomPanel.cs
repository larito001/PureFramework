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
        playerList.SetRenderer(ItemRender);
        leaveBtn.onClick.AddListener(() =>
        {
            YOTOFramework.netMgr.StopHost();
            YOTOFramework.netMgr.LeaveHost();
            CloseSelf();
        });
        readyBtn.onClick.AddListener(() =>
        {
            LoginPlugin.Instance.GameStartRequest();
        });
        
    }

    private void ItemRender(YOTOScrollViewItem arg1,int  index)
    {
        var info =LoginPlugin.Instance.GetPlayerDatas();
        (arg1 as RoomListItem).SetData(info[index]);
    }


    public override void OnShow()
    {
        YOTOFramework.eventMgr.AddEventListener(YOTOEventType.RefreshRoleList,RefreshRoleList);
        RefreshRoleList();
    }

    private void RefreshRoleList()
    {
     var tempList =   LoginPlugin.Instance.GetPlayerDatas();
   
     
     playerList.Initialize(10);
     
     playerList.SetData(tempList.Count);

    }

   

    public override void OnHide()
    {
       
    }
}
