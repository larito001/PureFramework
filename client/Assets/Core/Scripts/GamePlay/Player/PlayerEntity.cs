using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YOTO;

public class PlayerEntity :ObjectBase,PoolItem<PlayerData>
{
    public static  DataObjPool<PlayerEntity,PlayerData> pool=new DataObjPool<PlayerEntity, PlayerData>("PlayerEntity", 4);
    private PlayerData data;

    public bool isSelf
    {
        get;
        private set;
    }

    private EyesCtrl eyesCtrl;
    private HandCtrl handCtrl;
    protected override void YOTOOnload()
    {
        
    }

    public override void YOTOStart()
    {
        
    }
    
    public override void YOTOUpdate(float deltaTime)
    {
    
      
    }
    
    public void SetEyesMove(Vector2 input)
    {
        eyesCtrl?.SetEyesMove(input);
    }
    
    public void SetEyesMove(Vector3 pos)
    {
        eyesCtrl.ForceMove(pos);
    }

    public void CatchFood(int foodId,bool success)
    {
        var food = StagePlugin.Instance.GetFoodEntityById(foodId);
        handCtrl.ExtendLeftHand(food.ObjTrans,success);
    }
    public void EndCatch()
    {
        handCtrl.RetractLeftHand();
    }

    public void StartLooting(int foodId)
    {
        var food = StagePlugin.Instance.GetFoodEntityById(foodId);
        handCtrl.ExtendLeftHand(food.ObjTrans,true);

        if (isSelf)
        {
             //todo：进入特殊视角
        }
        else
        {
            
        }
        
    }
    public void EndLooting(bool win,int foodId)
    {
        if (win)
        {
            var food = StagePlugin.Instance.GetFoodEntityById(foodId);
            handCtrl.ExtendLeftHand(food.ObjTrans,true);
        }
        handCtrl.RetractLeftHand();
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
        RecoverObject();
    }

    public void SetData(PlayerData data)
    {
        this.data=data;
        isSelf=data.playerId==LoginPlugin.Instance.PlayerId;
        SetInVision(true);
        SetPrefabBundlePath("Player/Player");
        Debug.LogError("生成player："+data.playerName);

    }

    protected override void AfterInstanceGObj()
    {
        objTrans.gameObject.SetActive(true);
        eyesCtrl = objTrans.GetComponent<EyesCtrl>();
        handCtrl = objTrans.GetComponent<HandCtrl>();
        eyesCtrl.Init(this);
        handCtrl.Init( this);
    }



}
