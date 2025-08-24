using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomListItem: YOTOScrollViewItem 
{
    public TextMeshProUGUI nameText;
    public override void OnRenderItem()
    {
        base.OnRenderItem();
    }

    public void SetData(PlayerData playerData)
    {
        nameText.text = playerData.playerName;
    }
    public override void OnHidItem()
    {
        base.OnHidItem();
    }
}
