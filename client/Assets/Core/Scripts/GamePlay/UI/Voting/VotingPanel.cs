using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using YOTO;

public class VotingPanel : UIPageBase
{
    public YOTOScrollView votList;
    private List<PlayerEntity> tempPlayers;

    public override void OnLoad()
    {
        // btn_vote1.onClick.AddListener();
    }

    private List<Vector2Int> result = new List<Vector2Int>();
    public override void OnShow()
    {
        result.Clear();
        YOTOFramework.eventMgr.AddEventListener<VotEndNotify>(YOTOEventType.VotEndNotify,OnVotEndNotify);
        votList.SetRenderer(ItemRender);
        votList.Initialize(10);
        tempPlayers = PlayerPlugin.Instance.players.Values.ToList();
        votList.SetData(tempPlayers.Count);
        
    }

    private void OnVotEndNotify(VotEndNotify arg0)
    {
        YOTOFramework.timeMgr.DelayCall(CloseSelf,3);
        result=arg0.pidAndvots;
        votList.SetData(tempPlayers.Count);
    }

    private void ItemRender(YOTOScrollViewItem item, int index)
    {
        var it = item as VotButtonItem;
        var p = tempPlayers[index].GetPlayerDta();
       var votNum= result.Find(x => x.x == p.playerId);
        it.SetData(p.playerId,p.playerName,votNum.y);
    }

    public override void OnHide()
    {
        YOTOFramework.eventMgr.RemoveEventListener<VotEndNotify>(YOTOEventType.VotEndNotify,OnVotEndNotify);

    }
}