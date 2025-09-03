using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServerDataPlugin : LogicPluginBase
{
    public static ServerDataPlugin Instance;
    public override void Init()
    {
        Instance = this;
    }
    private Dictionary<int,PlayerData> players = new Dictionary<int,PlayerData>();
    private Dictionary<int, FoodData> foods = new Dictionary<int, FoodData>();

    public Dictionary<int, PlayerData>.ValueCollection GetPlayerList()
    {
        return players.Values;
    }
    public Dictionary<int, FoodData>.ValueCollection GetFoodList()
    {
        return foods.Values;
    }

    public PlayerData GetPlayerById(int id)
    {
        if (players.ContainsKey(id))
        {
            return players[id];
        }
        return null;
    }
    public void AddPlayer(PlayerData playerData)
    {
        players.Add(playerData.playerId, playerData);
    }

    public bool CheckHavePlayer(int id)
    {
        return players.ContainsKey(id);
    }
    public void RemovePlayerById(int id)
    {
        if (players.ContainsKey(id))
        {
            players.Remove(id);
        }
    }
    public void RemoveAllPlayers()
    {
        players.Clear();
    }
    public FoodData GetFoodById(int id)
    {
        if (foods.ContainsKey(id))
        {
            return foods[id];
        }
        return null;
    }
    public void AddFood(FoodData foodData)
    {
        foods.Add(foodData.foodId, foodData);
    }
    public bool CheckHaveFood(int id)
    {
        return foods.ContainsKey(id);
    }
    public void RemoveFoodById(int id)
    {
        if (foods.ContainsKey(id))
        {
            foods.Remove(id);
        }
    }
    public void RemoveAllFoods()
    {
        foods.Clear();
    }


}
