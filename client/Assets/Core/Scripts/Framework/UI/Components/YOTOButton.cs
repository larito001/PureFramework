using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class YOTOButton : Button, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("缩放设置")]
    public float hoverScale = 1.1f;   // 悬停时缩放倍数
    public float clickScale = 0.9f;   // 点击时缩放倍数
    public float duration = 0.2f;     // 动画时长
    public Ease easeType = Ease.OutBack; // 动画缓动类型

    private Vector3 originalScale;
    private Tween currentTween;

    protected override void Start()
    {
        base.Start();
        originalScale = transform.localScale;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        PlayTween(originalScale * hoverScale);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        PlayTween(originalScale);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        // 点击缩放一下再恢复
        PlayTween(originalScale * clickScale, () =>
        {
            // 点击完后，如果还在按钮上 → 回到 hoverScale
            if (RectTransformUtility.RectangleContainsScreenPoint(
                    transform as RectTransform, Input.mousePosition, eventData.pressEventCamera))
            {
                PlayTween(originalScale * hoverScale);
            }
            else
            {
                PlayTween(originalScale);
            }
        });
    }

    private void PlayTween(Vector3 targetScale, TweenCallback onComplete = null)
    {
        if (currentTween != null && currentTween.IsActive())
            currentTween.Kill();

        currentTween = transform.DOScale(targetScale, duration)
            .SetEase(easeType)
            .OnComplete(onComplete);
    }
}