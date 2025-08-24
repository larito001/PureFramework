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
        RefreshRoleList();
    }

    private void RefreshRoleList()
    {
     var tempList =   LoginPlugin.Instance.GetPlayerDatas();
        
     playerList.Initialize(10);
  

     playerList.SetData(tempList, (item, str) =>
     {
         item.OnRenderItem(str); // 类型安全
     });
    }

    public override void OnHide()
    {
       
    }
}
