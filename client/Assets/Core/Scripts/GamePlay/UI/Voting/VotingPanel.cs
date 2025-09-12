using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class VotingPanel : UIPageBase
{
    public YOTOScrollView votList;
    private List<PlayerEntity> tempPlayers;

    public override void OnLoad()
    {
        // btn_vote1.onClick.AddListener();
    }

    public override void OnShow()
    {
        votList.SetRenderer(ItemRender);
        votList.Initialize(10);
        tempPlayers = PlayerPlugin.Instance.players.Values.ToList();
        votList.SetData(tempPlayers.Count);
    }

    private void ItemRender(YOTOScrollViewItem item, int index)
    {
        var it = item as VotButtonItem;
        var p = tempPlayers[index].GetPlayerDta();
        it.SetData(p.playerId,p.playerName);
    }

    public override void OnHide()
    {
    }
}