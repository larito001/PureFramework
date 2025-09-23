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

    /// <summary>
    /// 游戏抢夺阶段
    /// </summary>
    public void StartFoodSystem()
    {
        _server.commonSystem.OnFlyTextNotify("Go!", FlyTextType.Normal);
        ServerDataPlugin.Instance.OnGameReStart();
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
        index++;
       var stage= stageQueue.Peek();
       if (index < stage.randomTime)
       {
           return;
       }

       stageQueue.Dequeue();
        List<FoodData> foods = new List<FoodData>();
        for (int i = 0; i < stage.dropCount; i++)
        {
            Quality qualityRandom = ServerDataPlugin.Instance.RandomFood(stage);
            var food = new FoodData();
            food.foodId = FoodData.idIndex++;
            food.position = new Vector3(Random.Range(-0.5f, 0.5f), 0.8f, Random.Range(-0.5f, 0.5f));
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
                var food = ServerDataPlugin.Instance.GetFoodById(arg1.foodId);
                food.StartCatch(ServerDataPlugin.Instance.GetPlayerById(arg1.playerId));
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