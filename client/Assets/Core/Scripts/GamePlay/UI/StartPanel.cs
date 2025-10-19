using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YOTO;

public class StartPanel : UIPageBase
{
    public Button joinBtn;
    public Button createBtn;
    public Button settingBtn;
    
    // textMeshpro的input
    public TMP_InputField IPInput;
    public TMP_InputField NameInput;
    public TMP_InputField PortInput;

    public CanvasGroup bg;
    public CanvasGroup leftBg;

    public Button startBtn;

    // 原始布局位置（目标点）
    private Vector2 bgOriginalPos;
    private Vector2 leftOriginalPos;

    private RectTransform bgRect;
    private RectTransform leftRect;

    public override void OnLoad()
    {
        bgRect = bg.GetComponent<RectTransform>();
        leftRect = leftBg.GetComponent<RectTransform>();

        // 记录初始布局点
        bgOriginalPos = bgRect.anchoredPosition;
        leftOriginalPos = leftRect.anchoredPosition;

        PortInput.text = "9999";
        IPInput.text = "127.0.0.1";
        NameInput.text = "testName";
    }

    public override void OnShow()
    {
        // 初始隐藏
        leftBg.alpha = 0;
        bg.alpha = 0;
        startBtn.gameObject.SetActive(true);
        settingBtn.onClick.AddListener(() =>
        {
            YOTOFramework.uIMgr.Show(UIEnum.SettingPanel);
            YOTOFramework.soundMgr.PlaySFX("Sound/SFX_UI_Click_Designed_Pop_Open_2");
        });
        // 绑定按钮事件
        joinBtn.onClick.AddListener(() =>
        {
            LoginPlugin.Instance.Name = NameInput.text;
            YOTOFramework.netMgr.JoinHost(IPInput.text, ushort.Parse(PortInput.text));
            YOTOFramework.soundMgr.PlaySFX("Sound/SFX_UI_Click_Designed_Pop_Open_2");
        });

        startBtn.onClick.AddListener(() =>
        {
            // 设置起始位置（屏幕外）
            bgRect.anchoredPosition = bgOriginalPos + new Vector2(0, -Screen.height);
            leftRect.anchoredPosition = leftOriginalPos + new Vector2(-Screen.width, Screen.height);

            // 淡入 + 移动到目标位置
            Sequence sequence = DOTween.Sequence();
            bg.alpha = 1;
            leftBg.alpha = 1;
            sequence.Append(bgRect.DOAnchorPos(bgOriginalPos, 1f).SetEase(Ease.OutQuint))
                    .Join(leftRect.DOAnchorPos(leftOriginalPos, 1f).SetEase(Ease.OutQuint));
            startBtn.gameObject.SetActive(false);
            YOTOFramework.soundMgr.PlaySFX("Sound/SFX_UI_Click_Designed_Pop_Open_2");
        });

        createBtn.onClick.AddListener(() =>
        {
            LoginPlugin.Instance.Name = NameInput.text;
            YOTOFramework.netMgr.CreateHost(ushort.Parse(PortInput.text));
            YOTOFramework.soundMgr.PlaySFX("Sound/SFX_UI_Click_Designed_Pop_Open_2");
        });
    }

    public override void OnHide()
    {
        // 执行出场动画
        Sequence sequence = DOTween.Sequence();
        bg.alpha = 1;
        leftBg.alpha = 1;

        bgRect.anchoredPosition = bgOriginalPos;
        leftRect.anchoredPosition = leftOriginalPos;

        sequence.Append(bgRect.DOAnchorPos(new Vector2(bgOriginalPos.x , bgOriginalPos.y - Screen.height), 1f)
                         .SetEase(Ease.OutQuint))
                .Join(leftRect.DOAnchorPos(new Vector2(leftOriginalPos.x - Screen.width, leftOriginalPos.y + Screen.height), 1f)
                         .SetEase(Ease.OutQuint));

        // 注销按钮事件
        joinBtn.onClick.RemoveAllListeners();
        createBtn.onClick.RemoveAllListeners();
        startBtn.onClick.RemoveAllListeners();

        startBtn.gameObject.SetActive(false);
        
    }

    public override void OnResize()
    {
        // 重新计算原始位置（适配分辨率变化）
        bgOriginalPos = bgRect.anchoredPosition;
        leftOriginalPos = leftRect.anchoredPosition;

        // 强制完成动画，避免停在中间状态
        DOTween.Complete(bgRect);
        DOTween.Complete(leftRect);

        // 恢复到布局目标点
        bgRect.anchoredPosition = bgOriginalPos;
        leftRect.anchoredPosition = leftOriginalPos;

        // 保证透明度正确
        bg.alpha = Mathf.Clamp01(bg.alpha);
        leftBg.alpha = Mathf.Clamp01(leftBg.alpha);
    }
}
