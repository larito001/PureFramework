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
    private void Start()
    {
        btn.onClick.AddListener(() =>
        {
            PlayerPlugin.Instance.OnVotClick(playerId);
            YOTOFramework.uIMgr.Hide(UIEnum.VotingPanel);
        });
    }

    private void OnDestroy()
    {
        btn.onClick.RemoveAllListeners();
    }

    public void SetData(int pid)
    {
        playerId = pid;
        nameText.text = pid.ToString();
    }
}