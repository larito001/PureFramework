using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectEntity :ObjectBase, PoolItem<FoodEntity>
{
    public static DataObjPool<SelectEntity, FoodEntity> pool = new DataObjPool<SelectEntity, FoodEntity>("SelectEntity", 10);
    FoodEntity _food;
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

    protected override void AfterInstanceGObj()
    {
        ObjTrans.position = _food.ObjTrans.transform.position;
    }

    public void AfterIntoObjectPool()
    {
        _food = null;
        RecoverObject();
    }

    public void SetData(FoodEntity entity)
    {
        _food = entity;
        SetPrefabBundlePath("Foods/SelectPanel");
        SetInVision(true);
        InstanceGObj();
    }
}
