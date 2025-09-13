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
    public Button startBtn;

    public override void OnLoad()
    {
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
            // 获取Image组件
    
            // 保存初始位置和透明度
            Vector3 originalPosition = bg.transform.position;
            // 设置起始状态：屏幕下方且完全透明
            bg.transform.position = originalPosition - new Vector3(0, Screen.height, 0);
            bg.alpha = 0;
    
            // 同时执行移动和渐显动画
            Sequence sequence = DOTween.Sequence();
            sequence.Append(bg.transform.DOMove(originalPosition, 1f).SetEase(Ease.OutBack))
                .Join(bg.DOFade(1f, 1f).SetEase(Ease.OutQuad));
            bg.blocksRaycasts = true;
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
        bg.blocksRaycasts = false;
        bg.alpha = 0;
        startBtn.gameObject.SetActive(true);
        
    }

    public override void OnHide()
    {
    }
}