using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using YOTO;

public class RoomPanel : UIPageBase
{
    public YOTOScrollView playerList;
    public Button leaveBtn;
    public Button readyBtn;
    public RectTransform bg;
    public RectTransform leftBtnGroup;

    private float duration = 1f;

    // 保存原始位置（UI 正常布局位置）
    private Vector2 bgOriginalPos;
    private Vector2 leftBtnGroupOriginalPos;

    public override void OnLoad()
    {
        // 记录初始布局点
        bgOriginalPos = bg.anchoredPosition;
        leftBtnGroupOriginalPos = leftBtnGroup.anchoredPosition;
    }

    private void ItemRender(YOTOScrollViewItem arg1, int index)
    {
        var info = LoginPlugin.Instance.GetPlayerDatas();
        (arg1 as RoomListItem).SetData(info[index]);
    }

    public override void OnShow()
    {
        YOTOFramework.uIMgr.Hide(UIEnum.StartPanel);

        // 设置起始位置：从屏幕外进入
        bg.anchoredPosition = bgOriginalPos + new Vector2(Screen.width, 0);
        leftBtnGroup.anchoredPosition = leftBtnGroupOriginalPos + new Vector2(-Screen.width, 0);

        // 入场动画
        Sequence enterSequence = DOTween.Sequence();
        enterSequence.Join(bg.DOAnchorPos(bgOriginalPos, duration).SetEase(Ease.OutQuint));
        enterSequence.Join(leftBtnGroup.DOAnchorPos(leftBtnGroupOriginalPos, duration).SetEase(Ease.OutQuint));

        enterSequence.OnStart(() =>
        {
            Debug.Log("入场动画开始");
            SetUIElementsInteractable(false);
        })
        .OnComplete(() =>
        {
            Debug.Log("入场动画完成");
            SetUIElementsInteractable(true);
        });

        playerList.SetRenderer(ItemRender);
        YOTOFramework.eventMgr.AddEventListener(YOTOEventType.RefreshRoleList, RefreshRoleList);
        RefreshRoleList();

        leaveBtn.onClick.AddListener(() =>
        {
            YOTOFramework.netMgr.StopHost();
            YOTOFramework.netMgr.LeaveHost();
        });

        readyBtn.onClick.AddListener(() =>
        {
            LoginPlugin.Instance.GameStartRequest();
        });
    }

    private void SetUIElementsInteractable(bool interactable)
    {
        leaveBtn.interactable = interactable;
        readyBtn.interactable = interactable;
    }

    private void RefreshRoleList()
    {
        var tempList = LoginPlugin.Instance.GetPlayerDatas();
        playerList.Initialize(10);
        playerList.SetData(tempList.Count);
    }

    public override void OnHide()
    {
        // 出场动画
        Sequence exitSequence = DOTween.Sequence();
        exitSequence.Join(bg.DOAnchorPos(bgOriginalPos + new Vector2(Screen.width, 0), duration)
            .SetEase(Ease.OutQuint));
        exitSequence.Join(leftBtnGroup.DOAnchorPos(leftBtnGroupOriginalPos + new Vector2(-Screen.width, 0), duration)
            .SetEase(Ease.OutQuint));

        exitSequence.OnStart(() =>
        {
            Debug.Log("出场动画开始");
            SetUIElementsInteractable(false);
        })
        .OnComplete(() =>
        {
            Debug.Log("出场动画完成");
            // 重置位置
            bg.anchoredPosition = bgOriginalPos;
            leftBtnGroup.anchoredPosition = leftBtnGroupOriginalPos;
        });

        leaveBtn.onClick.RemoveAllListeners();
        readyBtn.onClick.RemoveAllListeners();
        YOTOFramework.eventMgr.RemoveEventListener(YOTOEventType.RefreshRoleList, RefreshRoleList);
    }

    public override void OnResize()
    {
        // 重新获取目标位置
        bgOriginalPos = bg.anchoredPosition;
        leftBtnGroupOriginalPos = leftBtnGroup.anchoredPosition;

        // 完成动画，避免停在中间
        DOTween.Complete(bg);
        DOTween.Complete(leftBtnGroup);

        // 恢复到布局位置
        bg.anchoredPosition = bgOriginalPos;
        leftBtnGroup.anchoredPosition = leftBtnGroupOriginalPos;
    }

    // 可选：立即完成动画的方法
    public void CompleteAnimationsImmediately()
    {
        DOTween.Complete(bg);
        DOTween.Complete(leftBtnGroup);

        bg.anchoredPosition = bgOriginalPos;
        leftBtnGroup.anchoredPosition = leftBtnGroupOriginalPos;
        SetUIElementsInteractable(true);
    }
}
