using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FoodEntity :ObjectBase,PoolItem<FoodData>
{
    public static  DataObjPool<FoodEntity,FoodData> pool=new DataObjPool<FoodEntity, FoodData>("FoodEntity", 10);
    private FoodData data;
    private Rigidbody rigidbody;
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
        rigidbody=objTrans.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        if (objTrans.TryGetComponent<FoodBase>(out FoodBase food))
        {
            food.foodId=data.foodId;
        }
    }

    public void AfterIntoObjectPool()
    {
        if (objTrans!=null&&objTrans.TryGetComponent<FoodBase>(out FoodBase food))
        {
            food.foodId=-1;
        }
        rigidbody.isKinematic=true;
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

    private Tweener shakeTween; // 保存抖动动画的引用

    public void OnCatch()
    {
        // 停止之前的抖动动画（如果有）
        if (shakeTween != null && shakeTween.IsActive())
        {
            shakeTween.Kill();
        }
    
        // 使用DOTween实现以z轴为中心的旋转抖动
        shakeTween = objTrans.DOShakeRotation(1f, strength: 15f, vibrato: 10, randomness: 90f, fadeOut: true)
            .SetEase(Ease.OutQuad).SetLoops(3, LoopType.Restart);
    }

    public void StopCatch()
    {
        // 停止抖动动画
        if (shakeTween != null && shakeTween.IsActive())
        {
            shakeTween.Kill();
            // 可选：重置对象旋转到原始状态
            objTrans.DOLocalRotate(Vector3.zero, 0.1f);
        }
    }
}
