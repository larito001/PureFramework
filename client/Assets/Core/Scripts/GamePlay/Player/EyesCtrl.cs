using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YOTO;

public class EyesCtrl : MonoBehaviour
{
    private PlayerEntity playerEntity;
    
    public Transform eyes = null;
    private Vector3 targetEyesPos;
    private Vector3 EyesOrgPos; // 初始位置，需要在 Start 或 Awake 中保存
    private float timerTemp = 0;
    public Transform headGameObject;
    private Vector3 forward;

    private void Update()
    {
        if (playerEntity.isShow) return;
        if (eyes != null)
        {
            // Lerp 平滑移动到 targetEyesPos
            float lerpSpeed = 10f; // 控制平滑速度，可调
            eyes.localPosition = Vector3.Lerp(eyes.localPosition, targetEyesPos, Time.deltaTime * lerpSpeed);
        }
        if (playerEntity.isSelf)
        {
            timerTemp+=Time.deltaTime;
            if (timerTemp > 0.1f)
            {
                if (eyes!=null)
                {
                    PlayerPlugin.Instance.RotatePlayerRequest(eyes.localPosition);
                }
                timerTemp-=0.1f;
            }
        }
    }

    public void SetIsSelf(bool isSelf)
    {
        headGameObject.gameObject.SetActive(!isSelf);
    }
    public void ForceMove(Vector3 pos)
    {
        if (eyes != null)
        {
            targetEyesPos= pos;
        }
    }
    public void SetEyesMove(Vector2 input)
    {
        if (eyes != null)
        {
            float moveRange = 0.5f;

            // 在当前 localPosition 上叠加偏移
            Vector3 newPos = eyes.localPosition + new Vector3(input.x * 0.01f, input.y * 0.01f, 0); // 调整速度

            // 限制在 EyesOrgPos ± moveRange 内
            float limitX = Mathf.Clamp(newPos.x, EyesOrgPos.x - moveRange, EyesOrgPos.x + moveRange);
            float limitY = Mathf.Clamp(newPos.y, EyesOrgPos.y - moveRange, EyesOrgPos.y + moveRange);

            targetEyesPos = new Vector3(limitX, limitY, EyesOrgPos.z);
        }
    }

    public void Init(PlayerEntity playerEntity)
    {
        this.playerEntity=playerEntity;
        var t = GameObject.Find("table");
        forward = t.transform.position - transform.position;
        transform.forward = forward;
        if (playerEntity.isSelf)
        {
            
            SetIsSelf(!playerEntity.isShow);  
            //相机跟随
            var camera=   YOTOFramework.cameraMgr.getVirtualCamera("MainCameraVirtual");
     
            //forward = 当前位置到t的位置
    
            transform.forward = forward;
            camera.gameObject.transform.position =transform.forward*0.2f+transform.position+new Vector3(0,0.5f,0);
            EyesOrgPos=eyes.localPosition;
            if (playerEntity.isSelf)
            {
                YOTOFramework.sceneMgr.cameraCtrl.cameraDir.transform.forward=transform.forward;
            }
        }
        else
        {
            SetIsSelf(false);
        }
    }
}
