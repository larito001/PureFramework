using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YOTO;

public class BloodFVXEntity : ObjectBase, PoolItem<Vector3>
{
    public static DataObjPool<BloodFVXEntity, Vector3> pool =
        new DataObjPool<BloodFVXEntity, Vector3>("BloodFVXEntity", 20);

    private ParticleSystem particle;
    private Vector3 firePos;
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

    public void StartPlay()
    {
    
    }
    protected override void AfterInstanceGObj()
    {
        objTrans.LookAt(firePos); 
        particle = objTrans.GetComponentInChildren<ParticleSystem>();
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particle.Play();
        YOTOFramework.timeMgr.DelayCall(DelayRemove, 1);
    }

    private void DelayRemove()
    {
        // Debug.Log("回收血"+_entityID+objTrans.position);
        RecoverObject();
        pool.RecoverItem(this);
    }

    public void AfterIntoObjectPool()
    {
        // Debug.Log("RemoveObj"+_entityID);
    }

    public void SetData(Vector3 data)
    {
        firePos = data;
        SetInVision(true);
        SetPrefabBundlePath("Assets/HotUpdate/prefabs/SimpleFX/Prefabs/FX_BloodSplatter.prefab");
    }
}