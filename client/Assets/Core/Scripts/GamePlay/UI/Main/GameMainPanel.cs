using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using YOTO;

public class GameMainPanel : UIPageBase
{
    public Slider slider_satiety;
    public List<Sprite> satisfactionSprites = new List<Sprite>();
    public TextMeshProUGUI txt_timer;
    public Button findBtn;
    private Vector2 topOriginalPosition;
    private Vector2 btnOriginalPosition;
    private RectTransform findBtnRect;
    public Image satisfactionImg;

    public List<MainPlayerInfoCtrl> playerInfoCtrls = new List<MainPlayerInfoCtrl>();

    // private 
    public override void OnLoad()
    {
        findBtnRect = findBtn.GetComponent<RectTransform>();
        btnOriginalPosition = findBtnRect.anchoredPosition;
    }

    public override void OnShow()
    {
        YOTOFramework.eventMgr.AddEventListener<int>(YOTOEventType.GameTimerNotify, OnTimerNotify);
        YOTOFramework.eventMgr.AddEventListener(YOTOEventType.RefreshPlayerProperty, RefreshPlayerProperty);
        findBtn.onClick.AddListener(OnClickFindBtn);

        // 添加鼠标悬停效果
        AddButtonHoverEffect();

        // 确保UI元素在动画开始前位于屏幕外
        findBtnRect.anchoredPosition = btnOriginalPosition + new Vector2(findBtnRect.rect.width + 100, 0);

        Sequence sequence = DOTween.Sequence();
        sequence.Join(findBtnRect.DOAnchorPos(btnOriginalPosition, 1f).SetEase(Ease.OutQuint));

        sequence.OnStart(() => { Debug.Log("入场动画开始"); })
            .OnComplete(() => { Debug.Log("入场动画完成"); });
        for (var i = 0; i < playerInfoCtrls.Count; i++)
        {
            playerInfoCtrls[i].Reset(satisfactionSprites);
        }

        int index = 0;
        foreach (var player in PlayerPlugin.Instance.players.Values)
        {
            if (!player.isSelf)
            {
                playerInfoCtrls[index++].SetPlayer(player);
            }
        }

        RefreshPlayerProperty();
    }

    private void OnTimerNotify(int index)
    {
        txt_timer.text = index.ToString();
    }


    private void AddButtonHoverEffect()
    {
        // 添加事件触发器
        EventTrigger trigger = findBtn.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = findBtn.gameObject.AddComponent<EventTrigger>();
        }
        else
        {
            trigger.triggers.Clear();
        }
        
    }


    private void OnClickFindBtn()
    {
        PlayerPlugin.Instance.OnFindHostPlayerClick();
    }

    private void RefreshPlayerProperty()
    {
        var self = PlayerPlugin.Instance.GetSelf();
        slider_satiety.value = (float)self.SatietyValue / (float)PlayerEntity.maxStatiety;
        if (self.SatisfactionValue > 20f)
        {
            satisfactionImg.sprite = satisfactionSprites[0];
            
        }
        else if (self.SatisfactionValue > 10f)
        {
            satisfactionImg.sprite = satisfactionSprites[1];
        }
        else if (self.SatisfactionValue > 5f)
        {
            satisfactionImg.sprite = satisfactionSprites[2];
        }
        else
        {
            satisfactionImg.sprite = satisfactionSprites[3];
        }
    }

    public override void OnHide()
    {
        YOTOFramework.eventMgr.RemoveEventListener<int>(YOTOEventType.GameTimerNotify, OnTimerNotify);
        YOTOFramework.eventMgr.RemoveEventListener(YOTOEventType.RefreshPlayerProperty, RefreshPlayerProperty);
        findBtn.onClick.RemoveAllListeners();
        for (var i = 0; i < playerInfoCtrls.Count; i++)
        {
            playerInfoCtrls[i].Reset(null);
        }

        // 移除事件触发器
        EventTrigger trigger = findBtn.gameObject.GetComponent<EventTrigger>();
        if (trigger != null)
        {
            trigger.triggers.Clear();
        }

        findBtnRect.anchoredPosition = btnOriginalPosition;
    }

    public override void OnResize()
    {
        // 重新记录原始位置和缩放
        btnOriginalPosition = findBtnRect.anchoredPosition;
        // 强制完成动画，避免屏幕变化时位置错乱
        DOTween.Complete(findBtnRect);
        // 确保 UI 处于正确的最终状态
        findBtnRect.anchoredPosition = btnOriginalPosition;
    }
}