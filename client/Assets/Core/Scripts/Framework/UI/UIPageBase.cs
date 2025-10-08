using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using YOTO;

/// <summary>
/// UI�ű���Ҫ�̳���
/// </summary>
/// 
[RequireComponent(typeof(CanvasGroup))]
public abstract class UIPageBase : MonoBehaviour
{
    public List<YOTOUIMove> tweenList=new List<YOTOUIMove>();
    public UIEnum uiType;
    public bool Tween=false;
    public bool isEnable = false;
    public CanvasGroup canvasGroup;
    public abstract void OnLoad();
    public abstract void OnShow();
    public abstract void OnHide();

    public abstract void OnResize();
    public void Enter()
    {
        for (var i = 0; i < tweenList.Count; i++)
        {
            tweenList[i].OnEnter();
        }
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            float currentAlpha = 0f;
            canvasGroup.alpha = 0f;
            if (Tween)
            {
                DOTween.To(
                        () => currentAlpha,
                        x => {
                            currentAlpha = x;
                            if (canvasGroup != null)
                                canvasGroup.alpha = x;
                        },
                        1f,
                        1f
                    ).SetEase(Ease.OutQuad)
                    .SetTarget(this); // 绑定 tween，方便管理 
            }
            else
            {
                canvasGroup.alpha = 1;
            }
         

            isEnable = true;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void EnterFromTop()
    {
        GetComponent<RectTransform>().anchoredPosition=new Vector2( GetComponent<RectTransform>().anchoredPosition.x,    100f);
        GetComponent<RectTransform>().DOAnchorPosY(0, 0.5f)
            .SetEase(Ease.OutQuad);
    }

    public void Exit()
    {
        for (var i = 0; i < tweenList.Count; i++)
        {
            tweenList[i].OnExist();
        }
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            if (Tween)
            {
                float currentAlpha = canvasGroup.alpha;

                DOTween.To(
                        () => currentAlpha,
                        x => {
                            currentAlpha = x;
                            if (canvasGroup != null) // 防止目标失效
                                canvasGroup.alpha = x;
                        },
                        0f,
                        1f
                    ).SetEase(Ease.OutQuad)
                    .SetTarget(this); // 推荐加上，方便 Kill 或管理
            }
            else
            {
                canvasGroup.alpha = 0;
            }

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            isEnable = false;
        }
    }



    public void CloseSelf()
    {
        YOTOFramework.uIMgr.Hide(uiType);
    }

}
