using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YOTO;

public class PlayerEntity :ObjectBase,PoolItem<PlayerData>
{
    public static  DataObjPool<PlayerEntity,PlayerData> pool=new DataObjPool<PlayerEntity, PlayerData>("PlayerEntity", 1);
    private PlayerData data;
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

    protected override void AfterInstanceGObj()
    {
        if (data.playerId==LoginPlugin.Instance.PlayerId)
        {
            //相机跟随
         var camera=   YOTOFramework.cameraMgr.getVirtualCamera("MainCameraVirtual");
         camera.gameObject.transform.position=objTrans.position;
        }
        
    }
}
