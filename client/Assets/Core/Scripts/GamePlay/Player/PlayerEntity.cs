using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YOTO;

public class PlayerEntity :ObjectBase,PoolItem<PlayerData>
{
    public static  DataObjPool<PlayerEntity,PlayerData> pool=new DataObjPool<PlayerEntity, PlayerData>("PlayerEntity", 1);
    private PlayerData data;
    private Vector2 lookInput;
    private float xRotation = 0f;
    private float yRotation = 0f;

    public float verticalClamp = 90f;
    public float horizontalClamp = 90f;
    public float sensitivity = 10f;
    public float smoothTime = 0.5f; // 平滑时间
    protected override void YOTOOnload()
    {
        
    }

    public override void YOTOStart()
    {
        
    }

    public override void YOTOUpdate(float deltaTime)
    {
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
    }

    private Vector2 forward;
    protected override void AfterInstanceGObj()
    {
        var t = GameObject.Find("table");
        forward = t.transform.position - objTrans.position;
        objTrans.forward = forward;
        if (data.playerId==LoginPlugin.Instance.PlayerId)
        {
            //相机跟随
         var camera=   YOTOFramework.cameraMgr.getVirtualCamera("MainCameraVirtual");
         camera.gameObject.transform.position = objTrans.position+new Vector3(0,0.5f,0);

         //forward = 当前位置到t的位置

         objTrans.forward = forward;
         if (LoginPlugin.Instance.PlayerId==data.playerId)
         {
             YOTOFramework.sceneMgr.cameraCtrl.cameraDir.transform.forward=objTrans.forward;
         }
        }
        
    }

}
