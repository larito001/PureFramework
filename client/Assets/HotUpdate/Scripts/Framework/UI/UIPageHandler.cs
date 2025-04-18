using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using YOTO;
using System;

public enum PageState
{
    UnLoad,//ʲôû
    Loading,//ڼ
    Show,//ʾ
    Hide,//
}
public class UIPageHandler 
{
    UIPageBase uIPageBase;
    Transform parent;
    private string key;
    private bool isStatic;
    private Action onLoadComplete;  // 加载完成的回调
    public PageState curState = PageState.UnLoad;
    private bool shouldBeHidden = false; // 新增：标记是否应该被隐藏

    public void Init(string key,bool isStatic)
    {
        Debug.Log($"[UIPageHandler] Init: key={key}, isStatic={isStatic}");
        this.isStatic=isStatic; 
        this.key = key;
        shouldBeHidden = false; // 初始化时重置标记
        
        if (uIPageBase != null && uIPageBase.gameObject != null)
        {
            Debug.Log($"[UIPageHandler] Init: UI already exists, enabling it. key={key}");
            Enable();
        }
    }

    public void SetLoadCallback(Action callback)
    {
        Debug.Log($"[UIPageHandler] SetLoadCallback: key={key}");
        onLoadComplete = callback;
    }

    public void Load(Transform parent)
    {
        Debug.Log($"[UIPageHandler] Load Start: key={key}, currentState={curState}");
        this.parent = parent;

        // 检查现有UI是否可用
        if (uIPageBase != null && uIPageBase.gameObject != null)
        {
            Debug.Log($"[UIPageHandler] Reusing existing UI: key={key}");
            uIPageBase.transform.SetParent(parent, false);
            onLoadComplete?.Invoke();
            Show();
            return;
        }

        if (curState != PageState.Loading)
        {
            curState = PageState.Loading;
            if (isStatic)
            {
                Debug.Log($"[UIPageHandler] Loading Static UI: key={key}");
                GameObject prefab = Resources.Load<GameObject>(key);
                if (prefab == null)
                {
                    Debug.LogError($"[UIPageHandler] Failed to load static UI prefab: key={key}");
                    return;
                }
                OnLoaded(prefab);
            }
            else
            {
                Debug.Log($"[UIPageHandler] Loading Dynamic UI: key={key}");
                YOTOFramework.resMgr.LoadUI(key, OnLoaded);
            }
        }
        else
        {
            Debug.Log($"[UIPageHandler] UI already loading: key={key}");
        }
    }

    private void OnLoaded(GameObject prefab)
    {
        Debug.Log($"[UIPageHandler] OnLoaded Start: key={key}");
        
        if (prefab == null)
        {
            Debug.LogError($"[UIPageHandler] Failed to load UI prefab: key={key}");
            return;
        }

        try
        {
            UIPageBase page = UnityEngine.Object.Instantiate(prefab, parent).GetComponent<UIPageBase>();
            if (page == null)
            {
                Debug.LogError($"[UIPageHandler] Failed to get UIPageBase component: key={key}");
                return;
            }
            
            uIPageBase = page;
            uIPageBase.canvasGroup = uIPageBase.GetComponent<CanvasGroup>();
            if (uIPageBase.canvasGroup == null)
            {
                Debug.LogError($"[UIPageHandler] Failed to get CanvasGroup component: key={key}");
                return;
            }

            Debug.Log($"[UIPageHandler] UI instantiated successfully: key={key}");
            
            // 确保UI初始状态是隐藏的
            Disable();
            
            uIPageBase.OnLoad();
            YOTOFramework.uIMgr.OnUILoaded(uIPageBase.gameObject);
            
            Debug.Log($"[UIPageHandler] Before callback: key={key}");
            onLoadComplete?.Invoke();
            Debug.Log($"[UIPageHandler] Callback executed: key={key}");
            onLoadComplete = null;

            // 只有在不应该被隐藏的情况下才显示UI
            if (!shouldBeHidden)
            {
                Debug.Log($"[UIPageHandler] Before showing new UI: key={key}");
                Show();
            }
            else
            {
                Debug.Log($"[UIPageHandler] UI was marked as hidden during loading, keeping it hidden: key={key}");
            }
            Debug.Log($"[UIPageHandler] OnLoaded Complete: key={key}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[UIPageHandler] Exception in OnLoaded: key={key}, error={e}");
        }
    }

    private void Show()
    {
        Debug.Log($"[UIPageHandler] Show Start: key={key}, hasUI={uIPageBase != null}, hasCanvasGroup={uIPageBase?.canvasGroup != null}");
        if (uIPageBase != null && uIPageBase.gameObject != null && uIPageBase.canvasGroup != null)
        {
            Enable();
            uIPageBase.OnShow();
            curState = PageState.Show;
            shouldBeHidden = false; // 显示时重置标记
            Debug.Log($"[UIPageHandler] Show Complete: key={key}");
        }
        else
        {
            Debug.LogError($"[UIPageHandler] Failed to show UI: key={key}, UI or CanvasGroup is null");
        }
    }

    public void OnHide()
    {
        Debug.Log($"[UIPageHandler] OnHide Start: key={key}, hasUI={uIPageBase != null}, hasCanvasGroup={uIPageBase?.canvasGroup != null}");
        shouldBeHidden = true; // 设置隐藏标记
        if (uIPageBase != null && uIPageBase.gameObject != null && uIPageBase.canvasGroup != null)
        {
            Disable();
            uIPageBase.OnHide();
            curState = PageState.Hide;
            Debug.Log($"[UIPageHandler] OnHide Complete: key={key}");
        }
    }

    private void Disable()
    {
        if (uIPageBase != null && uIPageBase.gameObject != null && uIPageBase.canvasGroup != null)
        {
            uIPageBase.canvasGroup.alpha = 0;
            uIPageBase.canvasGroup.interactable = false;
            uIPageBase.canvasGroup.blocksRaycasts = false;
            uIPageBase.isEnable = false;
            Debug.Log($"[UIPageHandler] UI Disabled: key={key}");
        }
    }

    private void Enable()
    {
        if (uIPageBase != null && uIPageBase.gameObject != null && uIPageBase.canvasGroup != null)
        {
            uIPageBase.isEnable = true;
            uIPageBase.canvasGroup.alpha = 1;
            uIPageBase.canvasGroup.interactable = true;
            uIPageBase.canvasGroup.blocksRaycasts = true;
            Debug.Log($"[UIPageHandler] UI Enabled: key={key}");
        }
    }

    public void Distory()
    {
        OnHide();
        GameObject.Destroy(uIPageBase.gameObject);
    }
}
