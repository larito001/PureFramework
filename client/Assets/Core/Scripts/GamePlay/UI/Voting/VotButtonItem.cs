using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YOTO;

public class VotButtonItem : YOTOScrollViewItem
{
    public Button btn;
    private int playerId;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI votNumText;
    public Image votImg;
    private void OnClick()
    {
        PlayerPlugin.Instance.OnVotClick(playerId);
        btn.onClick.RemoveAllListeners();
        votImg.color=Color.yellow;
        // YOTOFramework.uIMgr.Hide(UIEnum.VotingPanel);
    }

    private void OnDestroy()
    {
        btn.onClick.RemoveAllListeners();
    }

    public void SetData(int pid,string playerName,int votNum )
    {
        votImg.color=Color.white;
        playerId = pid;
        nameText.text = playerName;
        if (votNum != 0)
        {
            votNumText.text = votNum.ToString();
        }
        else
        {
            votNumText.text = "";
        }
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClick);
    }
}