using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YOTO;
using DG.Tweening;
using UnityEngine.EventSystems;

public class CardCtrl : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI content;
    private GameRule _rule;
    public Button btn;
    public RectTransform cardTransform;
    public CanvasGroup cardCanvasGroup;
    
    // 动画参数
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isSelected = false;

    private void Awake()
    {
        // 保存原始状态
        originalScale = cardTransform.localScale;
        originalPosition = cardTransform.localPosition;
        originalRotation = cardTransform.localRotation;
        
        // 如果没有CanvasGroup，自动添加
        if (cardCanvasGroup == null)
            cardCanvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
    }

    public void SetCard(GameRule rule)
    {
        _rule = rule;
        title.text = rule.roleName;
        content.text = rule.roleDetail;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClick);
        
        // 设置鼠标事件
        SetupHoverEffects();
        
        // 初始状态
        ResetCardState();
    }

    private void SetupHoverEffects()
    {
        // 添加事件触发器
        var trigger = btn.gameObject.GetComponent<EventTrigger>() ?? btn.gameObject.AddComponent<EventTrigger>();
        
        // 鼠标进入事件
        var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((data) => { OnCardHoverEnter(); });
        trigger.triggers.Add(entryEnter);
        
        // 鼠标离开事件
        var entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((data) => { OnCardHoverExit(); });
        trigger.triggers.Add(entryExit);
    }

    // 鼠标悬停进入效果
    public void OnCardHoverEnter()
    {
        if (isSelected) return;
        
        // 停止之前的动画
        cardTransform.DOKill();
        
        // 悬停效果：放大并轻微上浮
        cardTransform.DOScale(originalScale * 1.1f, 0.3f)
            .SetEase(Ease.OutBack);
            
        cardTransform.DOLocalMoveY(originalPosition.y + 30f, 0.3f)
            .SetEase(Ease.OutQuad);
            
        // 增加亮度
        cardTransform.DOBlendableLocalMoveBy(new Vector3(0, 0, 10f), 0.3f);
    }

    // 鼠标悬停离开效果
    public void OnCardHoverExit()
    {
        if (isSelected) return;
        
        // 停止之前的动画
        cardTransform.DOKill();
        
        // 恢复原始状态
        cardTransform.DOScale(originalScale, 0.3f)
            .SetEase(Ease.OutQuad);
            
        cardTransform.DOLocalMove(originalPosition, 0.3f)
            .SetEase(Ease.OutQuad);
            
        // 恢复深度
        cardTransform.DOLocalMoveZ(0f, 0.3f);
    }

    // 卡牌选择动效
    public void SelectCard()
    {
        isSelected = true;
        
        // 停止所有动画
        cardTransform.DOKill();
        
        // 选择效果：更大程度放大并上浮
        cardTransform.DOScale(originalScale * 1.2f, 0.4f)
            .SetEase(Ease.OutBack);
            
        cardTransform.DOLocalMoveY(originalPosition.y + 50f, 0.4f)
            .SetEase(Ease.OutQuad);
            
        // 添加发光效果（如果有发光组件）
        cardTransform.DOBlendableLocalMoveBy(new Vector3(0, 0, 20f), 0.4f);
        
        // 轻微旋转增加立体感
        cardTransform.DOLocalRotate(new Vector3(0, 0, 5f), 0.4f)
            .SetEase(Ease.OutBack);
    }

    // 取消选择动效
    public void DeselectCard()
    {
        isSelected = false;
        
        // 停止所有动画
        cardTransform.DOKill();
        
        // 恢复到原始状态
        ResetCardState();
    }

    // 卡牌出场动效（旋转翻牌）
    public void PlayEntranceAnimation(float delay = 0f)
    {
        // 初始状态：设置为背面（缩小并旋转）
        cardTransform.localScale = new Vector3(0.1f, originalScale.y, originalScale.z);
        cardTransform.localRotation = Quaternion.Euler(0, 90f, 0);
        cardCanvasGroup.alpha = 0f;
        
        // 翻牌出场动画序列
        Sequence entranceSequence = DOTween.Sequence();
        
        // 第一步：淡入并开始旋转
        entranceSequence.Append(cardCanvasGroup.DOFade(1f, 0.3f));
        entranceSequence.Join(cardTransform.DORotate(new Vector3(0, 45f, 0), 0.3f));
        
        // 第二步：完成旋转并恢复正常比例
        entranceSequence.Append(cardTransform.DORotate(new Vector3(0, 0, 0), 0.4f));
        entranceSequence.Join(cardTransform.DOScaleX(originalScale.x, 0.4f));
        
        // 第三步：轻微弹跳效果
        entranceSequence.Append(cardTransform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.3f, 2, 0.5f));
        
        entranceSequence.SetDelay(delay);
        entranceSequence.OnStart(() => gameObject.SetActive(true));
    }

    // 卡牌退场动效（旋转消失）
    public void PlayExitAnimation(System.Action onComplete = null)
    {
        // 停止所有动画
        cardTransform.DOKill();
        
        // 退场动画序列
        Sequence exitSequence = DOTween.Sequence();
        
        // 第一步：缩小并开始旋转
        exitSequence.Append(cardTransform.DOScale(new Vector3(0.1f, originalScale.y, originalScale.z), 0.4f));
        exitSequence.Join(cardTransform.DORotate(new Vector3(0, 90f, 0), 0.4f));
        
        // 第二步：淡出
        exitSequence.Join(cardCanvasGroup.DOFade(0f, 0.4f));
        
        exitSequence.OnComplete(() => 
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }

    // 重置卡牌状态
    private void ResetCardState()
    {
        cardTransform.localScale = originalScale;
        cardTransform.localPosition = originalPosition;
        cardTransform.localRotation = originalRotation;
        cardTransform.localEulerAngles = Vector3.zero;
    }

    public void OnClick()
    {
        // 先播放选择动画
        SelectCard();
        // 延迟执行原来的点击逻辑
        DOVirtual.DelayedCall(0.3f, () =>
        {
            YOTOFramework.eventMgr.TriggerEvent(YOTOEventType.CardFinsh);
            // 动画完成后的回调
            StagePlugin.Instance.SetRule(_rule.ruleId);
            YOTOFramework.uIMgr.Hide(UIEnum.RuleSelectPanel);
        });
    }

    // 清理DOTween
    private void OnDestroy()
    {
        cardTransform.DOKill();
    }
}