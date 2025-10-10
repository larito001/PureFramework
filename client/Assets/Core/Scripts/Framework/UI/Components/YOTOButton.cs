using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class YOTOButton : Button, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("缩放设置")]
    public float hoverScale = 1.1f;   // 悬停时缩放倍数
    public float clickScale = 0.9f;   // 点击时缩放倍数
    public float duration = 0.2f;     // 动画时长
    public Ease easeType = Ease.OutBack; // 动画缓动类型

    [Header("文字颜色设置")]
    public Color normalColor = new Color(0.36f, 0.24f, 0.17f);  // 深棕色
    public Color hoverColor = new Color(0.90f, 0.49f, 0.13f);   // 橙色
    public Color clickColor = new Color(0.55f, 0.18f, 0.10f);   // 红棕色

    private List<TextMeshProUGUI> textMeshProUGUIs = new List<TextMeshProUGUI>();
    private Vector3 originalScale;
    private Tween currentTween;

    protected override void Start()
    {
        base.Start();
        originalScale = transform.localScale;
        foreach (var text in GetComponentsInChildren<TextMeshProUGUI>())
        {
            textMeshProUGUIs.Add(text);
            text.color = normalColor; // 初始设为 normalColor
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        SetTextColor(hoverColor);
        PlayTween(originalScale * hoverScale);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        SetTextColor(normalColor);
        PlayTween(originalScale);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        SetTextColor(clickColor);
        PlayTween(originalScale * clickScale, () =>
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(
                    transform as RectTransform, Input.mousePosition, eventData.pressEventCamera))
            {
                SetTextColor(hoverColor);
                PlayTween(originalScale * hoverScale);
            }
            else
            {
                SetTextColor(normalColor);
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

    private void SetTextColor(Color color)
    {
        foreach (var text in textMeshProUGUIs)
        {
            text.color = color;
        }
    }
}
