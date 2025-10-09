using System;
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
[RequireComponent(typeof(YOTOUIShow))]
public abstract class UIPageBase : MonoBehaviour
{
    private void Start()
    {
        tweenList.Add(GetComponent<YOTOUIShow>());
    }

    public List<YOTOUIChangeBase> tweenList=new List<YOTOUIChangeBase>();
    public UIEnum uiType;
    public bool TweenEnter=false;
    public bool TweenExit = false;
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
    }

    public void Exit()
    {
        for (var i = 0; i < tweenList.Count; i++)
        {
            tweenList[i].OnExist();
        }
    }
    
    public void CloseSelf()
    {
        YOTOFramework.uIMgr.Hide(uiType);
    }

}
