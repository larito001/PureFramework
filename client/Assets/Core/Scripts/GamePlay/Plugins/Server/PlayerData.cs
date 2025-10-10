public enum PlayerState
{
    Idle, //无
    Catching, //抓取中
    Looting, //抢夺中
    Dead, //死亡
}

public class PlayerData
{
    public int playerId;
    public string playerName;
    private PlayerState State = PlayerState.Idle;
    public int SatietyValue = 0; //饱腹值
    public int SatisfactionValue = 0; //满意度
    public int lootNum = 0;
    public int useFoodId = -1;
    public int currentAlcohol = 0;
    public bool needPlayParticle = true;

    public void PlayerLose()
    {
        if (State != PlayerState.Dead)
        {
            PlayerNeedDrinkNotify drinkNotify = new PlayerNeedDrinkNotify();
            drinkNotify.playerId = playerId;
            currentAlcohol += 10;
            drinkNotify.currentRate = currentAlcohol;
            ServerMessageManager.Instance.SendNotify(drinkNotify);

            if (currentAlcohol >= 20)
            {
                State = PlayerState.Dead;
                //todo:广播角色死亡
                PlayerDeadNotify notify = new PlayerDeadNotify();
                notify.playerId = playerId;
                ServerMessageManager.Instance.SendNotify(notify);
                currentAlcohol = 0;
            }
        }
     
    }
    
    public PlayerState GetState()
    {
        return State;
    }

    public bool OnCatchFood(int foodid)
    {
        if (State == PlayerState.Idle)
        {
            State = PlayerState.Catching;
            useFoodId = foodid;
            return true;
        }

        return false;
    }

    public bool OnLootFood(int foodid)
    {
        if (State == PlayerState.Idle)
        {
            lootNum = 0;
            State = PlayerState.Looting;
            useFoodId = foodid;
            return true;
        }

        return false;
    }

    public bool OnLootAfterCatch(int foodid)
    {
        if (State == PlayerState.Catching)
        {
            lootNum = 0;
            State = PlayerState.Looting;
            useFoodId = foodid;
            return true;
        }

        return false;
    }

    public bool AddLoot()
    {
        if (State == PlayerState.Looting)
        {
            lootNum++;
            return true;
        }

        return false;
    }

    public bool  OnCatchFoodEnd()
    {
        if (State == PlayerState.Catching)
        {
            State = PlayerState.Idle;
            useFoodId = -1;
            return true;
        }
        return false;
    }

    public void EatFood(int satiety, int satisfaction)
    {
        SatietyValue += satiety;
        SatisfactionValue += satisfaction;
        RefreshPlayerProperty();
    }

    public void RefreshPlayerProperty()
    {
        PlayerPropertyNotify notify = new PlayerPropertyNotify()
        {
            playerId = playerId,
            satiety = SatietyValue,
            satisfaction = SatisfactionValue
        };
        ServerMessageManager.Instance.SendNotify(notify);
    }

    public bool OnLootFoodEnd()
    {
        if (State == PlayerState.Looting)
        {
            State = PlayerState.Idle;
            useFoodId = -1;
            return true;
        }

        return false;
    }

    public void OnRoundEnd()
    {
        SatietyValue = 0;
        SatisfactionValue = 0;
        State = PlayerState.Idle;
    }
    public void ClearProperty()
    {
        currentAlcohol = 0;
        OnRoundEnd();
    }
}