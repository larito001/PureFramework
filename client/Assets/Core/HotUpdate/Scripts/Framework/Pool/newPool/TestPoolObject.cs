using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPoolObject : ObjectBase,PoolItem<Vector3>
{
   public static  DataObjPool<TestPoolObject,Vector3> pool=new DataObjPool<TestPoolObject, Vector3>("poolName", 20);
    public void AfterIntoObjectPool()
    {
        
    }

    public void SetData(Vector3 serverData)
    {
        SetInVision(true);
        SetPrefabBundlePath("Assets/PolygonApocalypse/Prefabs/Characters/SM_Chr_Teen_Male_01.prefab");
    }

    protected override void YOTOOnload()
    {
        throw new System.NotImplementedException();
    }

    public override void YOTOStart()
    {
        throw new System.NotImplementedException();
    }

    public override void YOTOUpdate(float deltaTime)
    {
        throw new System.NotImplementedException();
    }

    public override void YOTONetUpdate()
    {
        throw new System.NotImplementedException();
    }

    public override void YOTOFixedUpdate(float deltaTime)
    {
        throw new System.NotImplementedException();
    }

    public override void YOTOOnHide()
    {
        throw new System.NotImplementedException();
    }
    

    protected override void AfterInstanceGObj()
    {
        
    }
}
