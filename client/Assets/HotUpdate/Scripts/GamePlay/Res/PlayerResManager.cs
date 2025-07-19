using System.Collections;
using System.Collections.Generic;
using YOTO;

[System.Serializable]
public class PlayerResData
{
    public int woodNum = 0;
    public int ironNum = 0;
}

public class PlayerResDataContaner : DataContaner<PlayerResData>
{
    public static PlayerResDataContaner Instance;

    public PlayerResDataContaner()
    {
        Instance = this;
    }

    private PlayerResData _data = new PlayerResData();

    public bool RemoveWoodInBag(int value)
    {
        if ((_data.woodNum - value) > 0)
        {
            _data.woodNum -= value;
            return true;
        }

        return false;
    }

    public void AddWoodInBag(int value)
    {
        _data.woodNum += value;
    }

    public bool RemoveOIronInBag(int value)
    {
        if ((_data.ironNum - value) > 0)
        {
            _data.ironNum -= value;
            return true;
        }

        return false;
    }

    public void AddIronInBag(int value)
    {
        _data.ironNum += value;
    }

    public override string SaveKey => "PlayerResData";

    public override PlayerResData GetData()
    {
        return _data;
    }

    public override void __SetData(PlayerResData data) => _data = data;
}

public class PlayerResManager : LogicPluginBase
{
    public static PlayerResManager Instance;

    public PlayerResManager()
    {
        Instance = this;
    }
    
    private int _woodNum = 0;
    private int _ironNum = 0;

    protected override void OnInstall()
    {
        base.OnInstall();
    }

    protected override void OnUninstall()
    {
        base.OnUninstall();
    }

    public void Save()
    {
        PlayerResDataContaner.Instance.Save();
    }
    public void SetUseWood(int num)
    {
        if (num >= 0)
        {
            if (PlayerResDataContaner.Instance.RemoveWoodInBag(num))
            {
                AddWoodNum(num);
            }   
        }
        else
        {
            if ((_woodNum + num) > 0)
            {
                UseWoodRes(-num);
                PlayerResDataContaner.Instance.AddWoodInBag(-num);
            }
        }
    
    }

    public void SetUseIron(int num)
    {
        if (num >= 0)
        {
            if (PlayerResDataContaner.Instance.RemoveOIronInBag(num))
            {
                AddIronNum(num);
            } 
        }
        else
        {
            if ((_ironNum+num)>0)
            {
                UseIronRes(-num);
                PlayerResDataContaner.Instance.AddIronInBag(-num);
            }
            
        }
   
    }

    public void SaveRes()
    {
        PlayerResDataContaner.Instance.Save();
    }


    public bool CheckWoodEnough(int num)
    {
        return _woodNum >= num;
    }

    public bool CheckIronEnough(int num)
    {
        return _ironNum >= num;
    }

    public void UseWoodRes(int num)
    {
        if (CheckWoodEnough(num))
        {
            _woodNum -= num;
            YOTOFramework.eventMgr.TriggerEvent(EventType.RefreshResInfo);
        }
    }

    public void UseIronRes(int num)
    {
        if (CheckIronEnough(num))
        {
            _ironNum -= num;
            YOTOFramework.eventMgr.TriggerEvent(EventType.RefreshResInfo);
        }
    }

    public void  AddIronNum(int num)
    {
        if (num >= 0)
        {
            _ironNum += num;
        }

        YOTOFramework.eventMgr.TriggerEvent(EventType.RefreshResInfo);
    }

    public int GetIronNum()
    {
        return _ironNum;
    }

    public void AddWoodNum(int num)
    {
        if (num >= 0)
        {
            _woodNum += num;
        }

        YOTOFramework.eventMgr.TriggerEvent(EventType.RefreshResInfo);
    }
    public int GetWoodNum()
    {
        return _woodNum;
    }
}