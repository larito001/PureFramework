using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodEntity :ObjectBase,PoolItem<FoodData>
{
    public static  DataObjPool<FoodEntity,FoodData> pool=new DataObjPool<FoodEntity, FoodData>("FoodEntity", 10);
    private FoodData data;
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
        if (objTrans.TryGetComponent<FoodBase>(out FoodBase food))
        {
            food.foodId=data.foodId;
        }
    }

    public void AfterIntoObjectPool()
    {
        if (objTrans.TryGetComponent<FoodBase>(out FoodBase food))
        {
            food.foodId=-1;
      
        }
        RecoverObject();
    }

    public void RefreshState(FoodData newdata)
    {
        data=newdata;
    }
    public void SetData(FoodData serverData)
    {
        this.data=serverData;
        SetInVision(true);
        SetPrefabBundlePath("Foods/TestFood");
    }
}
