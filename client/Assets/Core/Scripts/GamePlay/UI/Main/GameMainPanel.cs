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
    public TextMeshProUGUI txt_satisfaction;
    public TextMeshProUGUI txt_timer;
    public Button findBtn;
    public RectTransform topInfoBg;
    private Vector2 topOriginalPosition;
    private Vector2 btnOriginalPosition;
    private RectTransform findBtnRect;
    private Vector3 btnOriginalScale;
    public List<MainPlayerInfoCtrl> playerInfoCtrls = new List<MainPlayerInfoCtrl>();
    // private 
    public override void OnLoad()
    {
        topOriginalPosition = topInfoBg.anchoredPosition;
        findBtnRect = findBtn.GetComponent<RectTransform>();
        btnOriginalPosition = findBtnRect.anchoredPosition;
        btnOriginalScale = findBtnRect.localScale;
    }

    public override void OnShow()
    {
        YOTOFramework.eventMgr.AddEventListener<int>(YOTOEventType.GameTimerNotify, OnTimerNotify);
        YOTOFramework.eventMgr.AddEventListener(YOTOEventType.RefreshPlayerProperty, RefreshPlayerProperty);
        findBtn.onClick.AddListener(OnClickFindBtn);
        
        // 添加鼠标悬停效果
        AddButtonHoverEffect();
        
        // 确保UI元素在动画开始前位于屏幕外
        topInfoBg.anchoredPosition = topOriginalPosition - new Vector2(topInfoBg.rect.width + 100, 0);
        findBtnRect.anchoredPosition = btnOriginalPosition + new Vector2(findBtnRect.rect.width + 100, 0);
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(topInfoBg.DOAnchorPos(topOriginalPosition, 1f).SetEase(Ease.OutQuint))
                .Join(findBtnRect.DOAnchorPos(btnOriginalPosition, 1f).SetEase(Ease.OutQuint));
        
        sequence.OnStart(() => { Debug.Log("入场动画开始"); })
                .OnComplete(() => { Debug.Log("入场动画完成"); });
        for (var i = 0; i < playerInfoCtrls.Count; i++)
        {
            playerInfoCtrls[i].Reset();
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

    private void OnTimerNotify(int index )
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

        // 鼠标进入事件
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => { OnButtonPointerEnter(); });
        trigger.triggers.Add(entryEnter);

        // 鼠标离开事件
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { OnButtonPointerExit(); });
        trigger.triggers.Add(entryExit);
    }

    private void OnButtonPointerEnter()
    {
        // 直接放大按钮
        findBtnRect.DOScale(btnOriginalScale * 1.2f, 0.3f)
                  .SetEase(Ease.OutBack);
    }

    private void OnButtonPointerExit()
    {
        // 恢复原始大小
        findBtnRect.DOScale(btnOriginalScale, 0.3f)
                  .SetEase(Ease.OutBack);
    }

    private void OnClickFindBtn()
    {
        PlayerPlugin.Instance.OnFindHostPlayerClick();
    }

    private void RefreshPlayerProperty()
    {
        var self = PlayerPlugin.Instance.GetSelf();
        slider_satiety.value =(float)self.SatietyValue /(float)PlayerEntity.maxStatiety;
        txt_satisfaction.text = "满意度:" + self.SatisfactionValue;
    }

    public override void OnHide()
    {
        YOTOFramework.eventMgr.RemoveEventListener<int>(YOTOEventType.GameTimerNotify, OnTimerNotify);
        YOTOFramework.eventMgr.RemoveEventListener(YOTOEventType.RefreshPlayerProperty, RefreshPlayerProperty);
        findBtn.onClick.RemoveAllListeners();
        for (var i = 0; i < playerInfoCtrls.Count; i++)
        {
            playerInfoCtrls[i].Reset();
        }
        // 移除事件触发器
        EventTrigger trigger = findBtn.gameObject.GetComponent<EventTrigger>();
        if (trigger != null)
        {
            trigger.triggers.Clear();
        }
        
        // 重置位置和缩放
        topInfoBg.anchoredPosition = topOriginalPosition;
        findBtnRect.anchoredPosition = btnOriginalPosition;
        findBtnRect.localScale = btnOriginalScale;
    }
}