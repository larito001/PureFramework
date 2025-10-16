using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YOTO;

public class FoodSystem : ServerSystemBase
{
    Queue<FoodDropStage> stageQueue = new Queue<FoodDropStage>(); //掉落队列
    private int index = 0;
    public override void AddEvent()
    {
        ServerMessageManager.Instance.RegisterRequestHandler<CatchFoodRequest>(OnCatchFoodRequest);
        ServerMessageManager.Instance.RegisterRequestHandler<LootingInputRequest>(OnLootingInputRequest);
    }

    public override void RemoveEvent()
    {
        ServerMessageManager.Instance.UnRegisterRequestHandler<CatchFoodRequest>();
        ServerMessageManager.Instance.UnRegisterRequestHandler<LootingInputRequest>();
    }

    #region 食物
    
    public void StartFoodSystem()
    {
        index = 0;
        _server.commonSystem.OnFlyTextNotify("Go!", FlyTextType.Normal);
  
        ServerDataPlugin.Instance.SetRandomPattern();
        stageQueue.Clear();
        var stages = ServerDataPlugin.Instance.CurrentPattern.stages;
        
        for (var i = 0; i < stages.Count; i++)
        {
            var start = stages[i].startTime;
            var end = stages[i].endTime;
            var randomDropTime = UnityEngine.Random.Range(start, end);

            stages[i].randomTime = randomDropTime;
            stageQueue.Enqueue(stages[i]);
        }

        YOTOFramework.timeMgr.LoopCall(GenerateFoods,1);

    }

    

    public void EndGenerateFood()
    {
        YOTOFramework.timeMgr.RemoveTimer(GenerateFoods);
    }
    
    
    /// <summary>
    /// 广播生成食物
    /// </summary>
    private void GenerateFoods()
    {
        if ( stageQueue.Count<=0)
        {
            return;
        }
        index++;
       var stage= stageQueue.Peek();
       if (index < stage.randomTime)
       {
           return;
       }

       stageQueue.Dequeue();
       
        switch ( ServerDataPlugin.Instance.CurrentRule.ruleId)
        {
            case 1:
                //砸金蛋
                GenerateEggs(stage);
                break;
            case 2:
                GenerateNormal(stage);
                break;
            case 3:
                GenerateNumberAdd(stage);
                break;
            case 4:
                GenerateNormal(stage);
                break;
            case 5:
                GenerateBet(stage);
                break;
            case 6:
                GenerateBet2(stage);
                break;
        }
     

    }

    private void GenerateEggs(FoodDropStage stage)
    {
        List<FoodData> foods = new List<FoodData>();
        
        for (int i = 0; i < stage.dropCount; i++)
        {
            Quality qualityRandom = ServerDataPlugin.Instance.RandomFood(stage);
            var food = new FoodData();
            food.path = stage.name;
            food.foodId = FoodData.idIndex++;
            food.position = new Vector3(Random.Range(-0.5f, 0.5f), 0.8f, Random.Range(-0.5f, 0.5f));

            var random = Random.Range(-0.5f, 0.5f);
            food.SatisfactionValue = random>0?3:-3;
            food.quality = qualityRandom;
            food.Init();
            ServerDataPlugin.Instance.AddFood(food);
            foods.Add(food);
        }
        
        FoodNotify notify = new FoodNotify();
        notify.foodList = foods;
        ServerMessageManager.Instance.SendNotify(notify);
    }

    
    private void GenerateNormal(FoodDropStage stage)
    {
        List<FoodData> foods = new List<FoodData>();
        
        for (int i = 0; i < stage.dropCount; i++)
        {
            Quality qualityRandom = ServerDataPlugin.Instance.RandomFood(stage);
            var food = new FoodData();
            food.path = stage.name;
            food.foodId = FoodData.idIndex++;
            food.position = new Vector3(Random.Range(-0.5f, 0.5f), 0.8f, Random.Range(-0.5f, 0.5f));
            food.SatisfactionValue = 2*(int)(qualityRandom+1);
            food.quality = qualityRandom;
            food.Init();
            ServerDataPlugin.Instance.AddFood(food);
            foods.Add(food);
        }
        
        FoodNotify notify = new FoodNotify();
        notify.foodList = foods;
        ServerMessageManager.Instance.SendNotify(notify);
    }
    private void GenerateNumberAdd(FoodDropStage stage)
    {
        List<FoodData> foods = new List<FoodData>();
        
        for (int i = 0; i < stage.dropCount; i++)
        {
            Quality qualityRandom = ServerDataPlugin.Instance.RandomFood(stage);
            var food = new FoodData();
            food.path = stage.name;
            food.foodId = FoodData.idIndex++;
            food.position = new Vector3(Random.Range(-0.5f, 0.5f), 0.8f, Random.Range(-0.5f, 0.5f));
            food.SatisfactionValue = 2*(int)(qualityRandom+1);
            food.catchEnum = CatchEnum.NumberCul;
            food.quality = qualityRandom;
            food.Init();
            ServerDataPlugin.Instance.AddFood(food);
            foods.Add(food);
        }
        
        FoodNotify notify = new FoodNotify();
        notify.foodList = foods;
        ServerMessageManager.Instance.SendNotify(notify);
    }
    
    private void GenerateBet(FoodDropStage stage)
    {
        List<FoodData> foods = new List<FoodData>();
        
        for (int i = 0; i < stage.dropCount; i++)
        {
            Quality qualityRandom = ServerDataPlugin.Instance.RandomFood(stage);
            var food = new FoodData();
            food.path = stage.name;
            food.foodId = FoodData.idIndex++;
            food.position = new Vector3(Random.Range(-0.5f, 0.5f), 0.8f, Random.Range(-0.5f, 0.5f));

            var random = Random.Range(-1f, 99f);
            food.SatisfactionValue = random>0?3:-9999;
            food.quality = qualityRandom;
            food.Init();
            ServerDataPlugin.Instance.AddFood(food);
            foods.Add(food);
        }
        
        FoodNotify notify = new FoodNotify();
        notify.foodList = foods;
        ServerMessageManager.Instance.SendNotify(notify);
    }
    private void GenerateBet2(FoodDropStage stage)
    {
        List<FoodData> foods = new List<FoodData>();
        
        for (int i = 0; i < stage.dropCount; i++)
        {
            Quality qualityRandom = ServerDataPlugin.Instance.RandomFood(stage);
            var food = new FoodData();
            food.path = stage.name;
            food.foodId = FoodData.idIndex++;
            food.position = new Vector3(Random.Range(-0.5f, 0.5f), 0.8f, Random.Range(-0.5f, 0.5f));

            var random = Random.Range(-100f, 2f);
            food.SatisfactionValue = random>0?9999:1;
            food.quality = qualityRandom;
            food.Init();
            ServerDataPlugin.Instance.AddFood(food);
            foods.Add(food);
        }
        
        FoodNotify notify = new FoodNotify();
        notify.foodList = foods;
        ServerMessageManager.Instance.SendNotify(notify);
    }
    
    /// <summary>
    /// 抓取食物的请求
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <returns></returns>
    private IResponse OnCatchFoodRequest(CatchFoodRequest arg1, int arg2)
    {
        if (!_server.stateCtrl.CheckIsPlaying()) return null;
        
        if (ServerDataPlugin.Instance.CheckHaveFood(arg1.foodId))
        {
            if (ServerDataPlugin.Instance.CheckHavePlayer(arg1.playerId))
            {
                if (ServerDataPlugin.Instance.GetPlayerById(arg1.playerId).GetState()!=PlayerState.Dead)
                {
                    var food = ServerDataPlugin.Instance.GetFoodById(arg1.foodId);
                    food.StartCatch(ServerDataPlugin.Instance.GetPlayerById(arg1.playerId));  
                }
                else
                {
                    Debug.Log("死了还想抓");
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 键盘输入抢夺
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <returns></returns>
    private IResponse OnLootingInputRequest(LootingInputRequest arg1, int arg2)
    {
        var player = ServerDataPlugin.Instance.GetPlayerById(arg1.playerId);
        if (player.AddLoot())
        {
            var food = ServerDataPlugin.Instance.GetFoodById(player.useFoodId);
            var proDic = food.GetProgress();
            LootingInputNotify notify = new LootingInputNotify();
            notify.playerProgress = proDic;
            ServerMessageManager.Instance.SendNotify(notify);
            _server.commonSystem.OnFlyTextNotify("!Space!", FlyTextType.Quick);
        }

        return null;
    }

    #endregion
}