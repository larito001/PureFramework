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

    // textMeshpro的input
    public TMP_InputField IPInput;
    public TMP_InputField NameInput;
    public TMP_InputField PortInput;
    public CanvasGroup bg;
    public CanvasGroup leftBg;

    public Button startBtn;

    // 设置起始状态：屏幕下方且完全透明
    Vector3 originalPosition ;
    Vector3 leftoriginalPosition ;

    public override void OnLoad()
    {
        originalPosition = bg.transform.position;
        leftoriginalPosition = leftBg.transform.position;
        PortInput.text = "9999";
        IPInput.text = "127.0.0.1";
        NameInput.text = "testName";

        joinBtn.onClick.AddListener(() =>
        {
            LoginPlugin.Instance.Name = NameInput.text;
            YOTOFramework.netMgr.JoinHost(IPInput.text, ushort.Parse(PortInput.text));
            // CloseSelf();
        });
        startBtn.onClick.AddListener(() =>
        {
            // 设置起始状态：屏幕下方且完全透明
            bg.transform.position = originalPosition + new Vector3(Screen.width, -Screen.height, 0);
            leftBg.transform.position = leftoriginalPosition + new Vector3(-Screen.width, Screen.height, 0);

            // 同时执行移动和渐显动画
            Sequence sequence = DOTween.Sequence();
            bg.alpha = 1;
            leftBg.alpha = 1;
            sequence.Append(bg.transform.DOMove(originalPosition, 1f).SetEase(Ease.OutQuint))
                .Join(leftBg.transform.DOMove(leftoriginalPosition, 1f).SetEase(Ease.OutQuint));
            startBtn.gameObject.SetActive(false);
        });
        createBtn.onClick.AddListener(() =>
        {
            LoginPlugin.Instance.Name = NameInput.text;
            YOTOFramework.netMgr.CreateHost(ushort.Parse(PortInput.text));
        });
    }

    public override void OnShow()
    {
        leftBg.alpha = 0;
        bg.alpha = 0;
        startBtn.gameObject.SetActive(true);
    }

    public override void OnHide()
    {
        // 同时执行移动和渐显动画
        Sequence sequence = DOTween.Sequence();
        bg.alpha = 1;
        leftBg.alpha = 1;
        bg.transform.position = originalPosition;
        leftBg.transform.position = leftoriginalPosition;
        sequence.Append(bg.transform.DOMove(new Vector3(Screen.width, -Screen.height, 0), 1f).SetEase(Ease.OutQuint))
            .Join(leftBg.transform.DOMove(new Vector3(-Screen.width, Screen.height, 0), 1f).SetEase(Ease.OutQuint))
            .OnComplete(() =>
            {
            });

        startBtn.gameObject.SetActive(false);
    }
}