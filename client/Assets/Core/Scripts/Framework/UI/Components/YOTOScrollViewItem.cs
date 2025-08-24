using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class YOTOScrollViewItem: MonoBehaviour
{
    private int _dataIndex = -1;
    private void Awake()
    {
        _dataIndex = -1;
    }

    public virtual void OnRenderItem()
    {
    }

    public virtual void OnHidItem()
    {
     
    }
    
    public int DataIndex
    {
        get => _dataIndex;
        set => _dataIndex = value;
    }
    
}
