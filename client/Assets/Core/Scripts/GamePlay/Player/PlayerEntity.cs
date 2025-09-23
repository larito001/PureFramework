using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YOTO;

public class PlayerEntity : ObjectBase, PoolItem<PlayerData>
{
    public static DataObjPool<PlayerEntity, PlayerData> pool =
        new DataObjPool<PlayerEntity, PlayerData>("PlayerEntity", 4);

    private PlayerData staticData;
    public bool leftHandDoing = false;
    public bool rightHandDoing = false;
    public bool isSelf { get; private set; }
    private EyesCtrl eyesCtrl;
    private HandCtrl handCtrl;
    private AnimatorCtrl animCtrl;
    public int SatietyValue;
    public int SatisfactionValue;
    public const int maxStatiety = 20;//最高饱腹值
    public bool CheckCanCatch()
    {
        bool isEnd = SatietyValue >= maxStatiety;
        if (isEnd)
        {
            // 将屏幕中心的世界坐标转换为屏幕坐标
            Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
            FlyTextMgr.Instance.AddText("Satiety Over", screenCenter, FlyTextType.Normal, TextPosType.Screen);
        }
        return !leftHandDoing&&!isEnd;
    }
    protected override void YOTOOnload()
    {
    }

    public void Drink()
    {
        animCtrl.OnDrink();
    }
    public void Dead()
    {
        animCtrl.OnDead();
    }
    public override void YOTOStart()
    {
    }

    public override void YOTOUpdate(float deltaTime)
    {
    }

    public void SetEyesMove(Vector2 input)
    {
        if(eyesCtrl!=null)
        eyesCtrl.SetEyesMove(input);
    }

    public void SetEyesMove(Vector3 pos)
    {
        if(eyesCtrl!=null)
        eyesCtrl.ForceMove(pos);
    }

    public void CatchFood(int foodId, bool success)
    {
        var food = StagePlugin.Instance.GetFoodEntityById(foodId);
        if (success)
        {
            handCtrl.ExtendLeftHand(food);  
        }

    }

    public void EndCatch()
    {
        handCtrl.RetractLeftHand();
    }

    public void StartLooting(int foodId)
    {
        var food = StagePlugin.Instance.GetFoodEntityById(foodId);
        handCtrl.ExtendLeftHand(food);

        if (isSelf)
        {
            YOTOFramework.sceneMgr.cameraCtrl.UseSpecialCamera(food.ObjTrans);

        }
    }

    public void EndLooting(bool win, int foodId)
    {
        
        if (win)
        {
            var food = StagePlugin.Instance.GetFoodEntityById(foodId);
            handCtrl.ExtendLeftHand(food);
            handCtrl.RetractLeftHand();
        }
        else
        {
            handCtrl.RetractLeftHand();
        }
     
        if (isSelf)
        {
            YOTOFramework.sceneMgr.cameraCtrl.UsePlayerCamera();
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
        handCtrl.OnUnLoad();
        RecoverObject();
    }

    public void RefreshPlayerProperty(int satiety, int satisfaction)
    {
        SatietyValue = satiety;
        SatisfactionValue = satisfaction;
        YOTOFramework.eventMgr.TriggerEvent(YOTOEventType.RefreshPlayerProperty);
    }
    public void SetData(PlayerData data)
    {
        this.staticData = data;
        isSelf = data.playerId == LoginPlugin.Instance.PlayerId;
        SetInVision(true);
        SetPrefabBundlePath("Player/Player");
        Debug.LogError("生成player：" + data.playerName);
    }

    public PlayerData GetPlayerDta()
    {
        return staticData;
    }

    protected override void AfterInstanceGObj()
    {
        leftHandDoing = false;
        rightHandDoing = false;
        objTrans.gameObject.SetActive(true);
        eyesCtrl = objTrans.GetComponent<EyesCtrl>();
        handCtrl = objTrans.GetComponent<HandCtrl>();
        animCtrl = objTrans.GetComponent<AnimatorCtrl>();
        eyesCtrl.Init(this);
        handCtrl.Init(this);
        animCtrl.Init(this);
    }
}