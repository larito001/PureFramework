using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YOTO;

public class MainPlayerInfoCtrl : MonoBehaviour
{
    public Image rate;
    public TextMeshProUGUI name;

    private PlayerEntity _player;
    private bool isStart=false;
    private Camera cam;
    private Vector2 screenPadding = new Vector2(30f, 30f); 
    /// <summary>
    /// 设置进度条（rate）的比例，范围 0~1
    /// </summary>
    public void SetPlayer(PlayerEntity player)
    {
        gameObject.SetActive(true);
        _player = player;
        isStart = true;
        name.text=player.GetPlayerDta().playerName;
        // if (rate != null)
        // {
        //     // 确保 type 是 Filled，否则 fillAmount 无效
        //     rate.type = Image.Type.Filled;  
        //     rate.fillAmount = Mathf.Clamp01(value);  
        // }
        if (cam==null)
        {
            cam=YOTOFramework.cameraMgr.getUICamera();
        }
    }

// x 是左右留白, y 是上下留白

    private void FixedUpdate()
    {
        if (isStart)
        {
            rate.fillAmount = Mathf.Clamp01(_player.SatietyValue / (float)PlayerEntity.maxStatiety);
            Vector3 screenPosition = cam.WorldToScreenPoint(_player.Location);

            // 获取UI实际尺寸
            Vector2 uiSize = ((RectTransform)transform).sizeDelta * transform.lossyScale; 
            float halfWidth = uiSize.x / 2f;
            float halfHeight = uiSize.y / 2f;

            // 限制边界，留出 padding
            float minX = halfWidth + screenPadding.x;
            float maxX = Screen.width - halfWidth - screenPadding.x;
            float minY = halfHeight + screenPadding.y;
            float maxY = Screen.height - halfHeight - screenPadding.y;

            screenPosition.x = Mathf.Clamp(screenPosition.x, minX, maxX);
            screenPosition.y = Mathf.Clamp(screenPosition.y, minY, maxY);

            transform.position = screenPosition;
        }
    }


    public void Reset()
    {
        isStart = false;
        _player = null;
        gameObject.SetActive(false);
    }
}