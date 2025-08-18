
using System;
using UnityEngine;

public class StaticLoadingPage :MonoBehaviour
{
    public RectTransform progressImg;
    private Vector2 size;
    public Canvas loadingCanvas;
    private void Start()
    {
        size = progressImg.sizeDelta;
    }

    public void Loading(float progress)
    {
        Debug.Log($"当前进度： {progress * 100}%");
        size.x = progress * 1000;
        progressImg.sizeDelta = size;
    }
}
