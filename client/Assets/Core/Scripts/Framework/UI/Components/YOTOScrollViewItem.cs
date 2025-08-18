using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class YOTOScrollViewItem : MonoBehaviour
{
    private YOTOScrollViewDataBase _currentData;
    private bool _isVisible;
    private int _dataIndex = -1;
    private void Awake()
    {
        _isVisible = false;
        _dataIndex = -1;
    }

    public virtual void OnRenderItem(YOTOScrollViewDataBase data)
    {
        if (data == null) return;
        
        _currentData = data;
        _isVisible = true;
    }

    public virtual void OnHidItem(YOTOScrollViewDataBase data)
    {
        _currentData = null;
        _isVisible = false;
        _dataIndex = -1;
    }

    public bool IsVisible => _isVisible;
    public YOTOScrollViewDataBase CurrentData => _currentData;
    public int DataIndex
    {
        get => _dataIndex;
        set => _dataIndex = value;
    }
}
