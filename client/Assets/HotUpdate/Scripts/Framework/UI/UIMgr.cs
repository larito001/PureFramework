using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YOTO;

public enum UILayerEnum
{
    Normal,
    Top,
    Tips,
    PopText,
}

public class UILayer
{
    
    public Dictionary<UIEnum, UIPageHandler> handlers = new Dictionary<UIEnum, UIPageHandler>();
    GameObject uiRoot;
    public GameObject layerRoot;
    private UILayerEnum layer;
    public void Init(GameObject root,UILayerEnum layerEnum)
    {
        layer = layerEnum;
        uiRoot = root;
        layerRoot = new GameObject(layer.ToString());
        layerRoot.layer = LayerMask.NameToLayer("UI");
        layerRoot.transform.SetParent(root.transform, false);
        // 添加Canvas组件
        Canvas canvas = layerRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.worldCamera = YOTOFramework.cameraMgr.getUICamera();
        canvas.overrideSorting = true; // 重要：启用排序覆盖
        canvas.sortingOrder = ((int)layer * 100);
        CanvasScaler scaler = layerRoot.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0;
        // 添加其他必要组件
        layerRoot.AddComponent<GraphicRaycaster>();
    }
    
    public void Show(UIInfo info)
    {
        UIPageHandler newHandler;
         if (!handlers.TryGetValue(info.uiEnum, out newHandler) || newHandler == null)
         {
             // Debug.LogError($"[UILayer] Show: Failed to get handler for {info.uiEnum}");
             newHandler = new UIPageHandler();
             newHandler.Init(info.key,info.uiEnum);
             handlers.Add(info.uiEnum,newHandler);
         }
         // 设置加载完成的回调
         newHandler.SetLoadCallback(() =>
         {
         });
         // 开始加载新UI
         newHandler.Load(this);
    }

    public void Hide(UIEnum uIEnum)
    {
        if (!handlers.ContainsKey(uIEnum))
        {
            return;
        }

        if (handlers.TryGetValue(uIEnum, out UIPageHandler handler))
        {
            handler.OnHide();
        }
    }

    public void Clear()
    {
        foreach (var uiPageHandler in handlers)
        {
            uiPageHandler.Value.Distory();
        }
        handlers.Clear();
    }
}

public class UIMgr
{
    // 使用UIEnum作为键，因为它是枚举类型，不会有值相等性的问题
    UIConfig uIConfig = null;
    public GameObject UIRoot;

    // 用于存储不同层级的Canvas引用
    private Dictionary<UILayerEnum, UILayer> uiLayers = new Dictionary<UILayerEnum, UILayer>();

    public UILayer GetLayer(UILayerEnum layerEnum)
    {
        if (uiLayers.ContainsKey(layerEnum))
        {
            return uiLayers[layerEnum];
        }

        return null;
    }
    private void SetUILayer(GameObject obj)
    {
        obj.layer = LayerMask.NameToLayer("UI");
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            SetUILayer(obj.transform.GetChild(i).gameObject);
        }
    }

    public void Init()
    {
        uIConfig = new UIConfig();
        UIRoot = new GameObject("UIRoot");
        UIRoot.layer = LayerMask.NameToLayer("UI");
        GameObject.DontDestroyOnLoad(UIRoot);
        foreach (UILayerEnum layer in System.Enum.GetValues(typeof(UILayerEnum)))
        {
            UILayer layertemp = new UILayer();
            layertemp.Init(UIRoot,layer);
            uiLayers.Add(layer,layertemp);
        }

    }

    public void Show(UIEnum uiEnum)
    {
        Debug.Log($"[UIMgr] Show: uiEnum={uiEnum}");
        UIInfo point = uIConfig.uiConfigDic[uiEnum];

        if (uiLayers.ContainsKey(point.layer))
        {
            uiLayers[point.layer].Show(point);
        }
    }

    public void OnUILoaded(GameObject uiObject)
    {
        if (uiObject != null)
        {
            SetUILayer(uiObject);
        }
    }

    public void Hide(UIEnum uiEnum)
    {
        Debug.Log($"[UIMgr] Hide: uiEnum={uiEnum}");
        if (uIConfig.uiConfigDic.TryGetValue(uiEnum, out UIInfo point))
        {
            if (uiLayers.TryGetValue(point.layer, out UILayer type))
            {
                type?.Hide(uiEnum);
            }
        }
    }

    public void ClearUI()
    {
        Debug.Log("[UIMgr] Clearing all UIs");

        // 遍历所有UI类型，调用它们的Clear方法
        foreach (var typeBase in uiLayers.Values)
        {
            if (typeBase != null)
            {
                typeBase.Clear();
            }
        }
    }
}