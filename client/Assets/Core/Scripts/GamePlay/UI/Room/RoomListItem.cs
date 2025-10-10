using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListItem : YOTOScrollViewItem
{
    public TextMeshProUGUI nameText;
    private RectTransform rect;
    private Vector2 originPos; // 记录原始位置
    private Image bg;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();

    }

    public void SetData(PlayerData playerData,bool needAnim,int index)
    {
        
        // 获取背景组件
        if (bg == null) bg = GetComponent<Image>();

        // 计算彩虹颜色（循环渐变）
        float hue = (index * 0.15f) % 1f;   // 控制色相步进，越小过渡越柔和
        float saturation = 0.7f;            // 保持颜色鲜艳但不刺眼
        float value = 0.9f;                 // 提高亮度，看起来清晰
        Color bgColor = Color.HSVToRGB(hue, saturation, value);
        bg.color = bgColor;
        originPos = rect.anchoredPosition;
        float brightness = 0.299f * bgColor.r + 0.587f * bgColor.g + 0.114f * bgColor.b;
        if (brightness > 0.6f)
        {
            nameText.color = new Color(0.1f, 0.1f, 0.1f); // 深色文字
        }
        else
        {
            nameText.color = Color.white; // 浅色文字
        }
        nameText.text = playerData.playerName;

        rect.DOKill();
        if (needAnim)
        {
            // 起点：屏幕左边（可以根据需要调整偏移量）
            rect.anchoredPosition = new Vector2(-500f, originPos.y);

            // 移动到原始位置
            rect.DOAnchorPos(originPos, 0.5f*(index+1)).SetEase(Ease.OutCubic);   
        }
    
    }

    public override void OnHidItem()
    {
        base.OnHidItem();

        rect.DOKill();
    }
}