using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MapButton:MonoBehaviour
{
    public Button btn;
    public int level;
    private UnityAction<int> callback;
    public void SetCallBack(UnityAction<int> cb)
    {
        callback = cb;
    }
    private void Start()
    {
        btn.onClick.AddListener(BtnClick);
    }

    private void BtnClick()
    {
        callback?.Invoke(level);
    }
}