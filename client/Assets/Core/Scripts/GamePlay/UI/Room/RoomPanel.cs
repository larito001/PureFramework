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
    private float duration =1f;
    private float startXOffset = 1500f;
    private float btnstartXOffset = 500f;
    // 保存原始位置
    private Vector2 bgOriginalPos;
    private Vector2 leftBtnGroupOriginalPos;

    public override void OnLoad()
    {
        // 保存原始位置
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
        // 设置起始位置
        // 背景从右侧开始（向右偏移）
        bg.anchoredPosition = new Vector2(bgOriginalPos.x + startXOffset, bgOriginalPos.y);
        // 按钮组从左侧开始（向左偏移）
        leftBtnGroup.anchoredPosition = new Vector2(leftBtnGroupOriginalPos.x - btnstartXOffset, leftBtnGroupOriginalPos.y);
        
        // 创建动画序列
        Sequence enterSequence = DOTween.Sequence();
        
        // 背景从右往左移动到原始位置
        enterSequence.Join(bg.DOAnchorPos(bgOriginalPos, duration)
            .SetEase(Ease.OutQuint));
        
        // 按钮组从左往右移动到原始位置
        enterSequence.Join(leftBtnGroup.DOAnchorPos(leftBtnGroupOriginalPos, duration)
            .SetEase(Ease.OutQuint));
        
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
            LoginPlugin.Instance.OnNetError();
        });
        readyBtn.onClick.AddListener(() => { LoginPlugin.Instance.GameStartRequest(); });
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
        // 创建出场动画序列
        Sequence exitSequence = DOTween.Sequence();
        
        // 背景往左移出屏幕（向左偏移）
        exitSequence.Join(bg.DOAnchorPos(new Vector2(bgOriginalPos.x + startXOffset, bgOriginalPos.y), duration)
            .SetEase(Ease.OutQuint));
        
        // 按钮组往右移出屏幕（向右偏移）
        exitSequence.Join(leftBtnGroup.DOAnchorPos(new Vector2(leftBtnGroupOriginalPos.x - btnstartXOffset, leftBtnGroupOriginalPos.y), duration)
            .SetEase(Ease.OutQuint));
        
        exitSequence.OnStart(() => 
        {
            Debug.Log("出场动画开始");
            SetUIElementsInteractable(false);
        })
        .OnComplete(() => 
        {
            Debug.Log("出场动画完成");
            // 重置位置以便下次正确显示
            bg.anchoredPosition = bgOriginalPos;
            leftBtnGroup.anchoredPosition = leftBtnGroupOriginalPos;
        });

        leaveBtn.onClick.RemoveAllListeners();
        readyBtn.onClick.RemoveAllListeners();
        YOTOFramework.eventMgr.RemoveEventListener(YOTOEventType.RefreshRoleList, RefreshRoleList);
    }

    // 可选：添加一个强制完成动画的方法
    public void CompleteAnimationsImmediately()
    {
        DOTween.Complete(bg);
        DOTween.Complete(leftBtnGroup);
        
        // 立即设置到最终状态
        bg.anchoredPosition = bgOriginalPos;
        leftBtnGroup.anchoredPosition = leftBtnGroupOriginalPos;
        SetUIElementsInteractable(true);
    }
}