using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YOTO;

public class PlayerEntity :ObjectBase,PoolItem<PlayerData>
{
    public static  DataObjPool<PlayerEntity,PlayerData> pool=new DataObjPool<PlayerEntity, PlayerData>("PlayerEntity", 4);
    private PlayerData data;
    private Vector2 lookInput;
    private float xRotation = 0f;
    private float yRotation = 0f;

    public float verticalClamp = 90f;
    public float horizontalClamp = 90f;
    public float sensitivity = 10f;
    public float smoothTime = 0.5f; // 平滑时间
    private bool isSelf = false;
    private float timerTemp = 0;
    Transform eyes = null;
    
    protected override void YOTOOnload()
    {
        
    }

    public override void YOTOStart()
    {
        
    }
    
    public override void YOTOUpdate(float deltaTime)
    {
        if (eyes != null)
        {
            // Lerp 平滑移动到 targetEyesPos
            float lerpSpeed = 10f; // 控制平滑速度，可调
            eyes.localPosition = Vector3.Lerp(eyes.localPosition, targetEyesPos, Time.deltaTime * lerpSpeed);
        }
        if (isSelf)
        {
            timerTemp+=deltaTime;
            if (timerTemp > 0.2f)
            {
                if (eyes!=null)
                {
                    PlayerPlugin.Instance.RotatePlayerRequest(eyes.localPosition);
                }
                timerTemp-=0.2f;
            }
        }
      
    }

    private Vector3 EyesOrgPos; // 初始位置，需要在 Start 或 Awake 中保存

    private Vector3 targetEyesPos;
    public void SetEyesMove(Vector2 input)
    {
        if (eyes != null)
        {
            float moveRange = 0.05f;

            // 在当前 localPosition 上叠加偏移
            Vector3 newPos = eyes.localPosition + new Vector3(input.x * 0.01f, input.y * 0.01f, 0); // 调整速度

            // 限制在 EyesOrgPos ± moveRange 内
            float limitX = Mathf.Clamp(newPos.x, EyesOrgPos.x - moveRange, EyesOrgPos.x + moveRange);
            float limitY = Mathf.Clamp(newPos.y, EyesOrgPos.y - moveRange, EyesOrgPos.y + moveRange);

            targetEyesPos = new Vector3(limitX, limitY, EyesOrgPos.z);
        }
    }
    

    public void SetEyesMove(Vector3 pos)
    {
        if (eyes != null)
        {
            targetEyesPos= pos;
        }
    }
    public override void YOTONetUpdate()
    {
        
    }

    public override void YOTOFixedUpdate(float deltaTime)
    {
        
    }

    public override void YOTOOnHide()
    {
        
    }

    public void AfterIntoObjectPool()
    {
        
    }

    public void SetData(PlayerData data)
    {
        this.data=data;
        SetInVision(true);
        SetPrefabBundlePath("Player/Player");
        Debug.LogError("生成player："+data.playerName);
        isSelf=data.playerId==LoginPlugin.Instance.PlayerId;
    }

    private Vector2 forward;
    protected override void AfterInstanceGObj()
    {
        var t = GameObject.Find("table");
        forward = t.transform.position - objTrans.position;
        objTrans.gameObject.SetActive(true);
        objTrans.forward = forward;
        eyes=  objTrans.Find("Eyes");
        if (data.playerId==LoginPlugin.Instance.PlayerId)
        { 
            //相机跟随
         var camera=   YOTOFramework.cameraMgr.getVirtualCamera("MainCameraVirtual");
         camera.gameObject.transform.position = objTrans.position+new Vector3(0,0.5f,0);
         //forward = 当前位置到t的位置

         objTrans.forward = forward;
     
         EyesOrgPos=eyes.localPosition;
         if (LoginPlugin.Instance.PlayerId==data.playerId)
         {
             YOTOFramework.sceneMgr.cameraCtrl.cameraDir.transform.forward=objTrans.forward;
         }
        }
        
    }

}
