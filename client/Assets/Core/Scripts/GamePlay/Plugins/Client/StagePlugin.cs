using System.Collections.Generic;
using UnityEngine;
using YOTO;


public class StagePlugin : LogicPluginBase
{
    public static StagePlugin Instance;
    public List<FoodData> foodList = new List<FoodData>();
    public Dictionary<int, FoodEntity> foodDict = new Dictionary<int, FoodEntity>();

    public StagePlugin()
    {
        Instance = this;
    }

    private int playerId = -1;
    private List<PlayerData> players = new List<PlayerData>();
    public bool GameStart = false;

    protected override void OnInstall()
    {
        base.OnInstall();
    }

    protected override void OnUninstall()
    {
        base.OnUninstall();
    }

    public void OnNetInstall()
    {
        GameStart = false;
        ClientMessageManager.Instance.RegisterResponseHandler<FoodNotify>(OnFoodNotify);
        ClientMessageManager.Instance.RegisterResponseHandler<RefreshFoodStateNotify>(OnRefreshFoodStateNotify);
        
    }

    private void OnRefreshFoodStateNotify(RefreshFoodStateNotify obj)
    {
        Debug.Log("RefreshFoodStateNotify");
        var list = obj.newDatas;
        for (var i = 0; i < list.Count; i++)
        {
         
            foodDict[list[i].foodId].RefreshState(list[i]);
        }
    }
    private void OnFoodNotify(FoodNotify obj)
    {
        foodList = obj.foodList;
        for (var i = 0; i < foodList.Count; i++)
        {
            GenerateFoodsOnMap(foodList[i]);
        }
    }

    private void GenerateFoodsOnMap(FoodData foodData)
    {
        var food = FoodEntity.pool.GetItem(foodData);
        foodDict.Add(foodData.foodId, food);
        food.Location = foodData.position;
        food.InstanceGObj();
    }

    public void OnNetUninstall()
    {
        GameStart = false;
    }

    public void OnGameStart(int playerId, List<PlayerData> playerDatas)
    {
        GameStart = true;
        YOTOFramework.uIMgr.ClearUI();
        this.playerId = playerId;
        players = playerDatas;
        PlayerPlugin.Instance.GeneratePlayers(players);
    }

   
    public void RemoveFood(int foodId)
    {
        for (var i = 0; i < foodList.Count; i++)
        {
            if (foodList[i].foodId == foodId)
            {
                foodList.RemoveAt(i);
                FoodEntity.pool.RecoverItem(foodDict[foodId]);
            
                foodDict.Remove(foodId);
                break;
            }
        }
    }

    public void OnGameEnd()
    {
    }
}