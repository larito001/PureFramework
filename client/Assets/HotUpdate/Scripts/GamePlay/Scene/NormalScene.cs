using System.Collections;
using System.Collections.Generic;
using Proto;
using UnityEngine;
using YOTO;
using EventType = YOTO.EventType;

[System.Serializable]
public class LevelInfoDatas
{
    public List<LevelInfoData> Datas = new List<LevelInfoData>();
}

[System.Serializable]
public class LevelInfoData
{
    public int level;
    public List<LevelWeatherInfoData> WeatherInfos = new List<LevelWeatherInfoData>();
    public string ScenePath;
}

[System.Serializable]
public class LevelWeatherInfoData
{
    public int during; //持续时间
    public int weather; //天气
    public int dayTime; //日期
    public int enemyNum; //敌人数量
}

public class LevelInfoDataContaner : DataContaner<LevelInfoDatas>
{
    public static LevelInfoDataContaner Instance;

    public LevelInfoDataContaner()
    {
        Instance = this;
    }

    private LevelInfoDatas _data = new LevelInfoDatas();
    public override string SaveKey => "LevelInfo";

    public override LevelInfoDatas GetData()
    {
        return _data;
    }

    public override void __SetData(LevelInfoDatas data) => _data = data;
}

public class NormalScene : VirtualSceneBase
{
    private GameObject _sceneObj;

    public class NormalSceneParam : SceneParam
    {
        public int level = 0;
    }

    private NormalSceneParam _param;

    //注册事件
    public override void OnAdd()
    {
    }

    //
    public override void Onload(SceneParam param)
    {
        _param = param as NormalSceneParam;
        Debug.Log("YTLOG;加载了场景" + _param.level);
        foreach (var levelInfoData in LevelInfoDataContaner.Instance.GetData().Datas)
        {
            if (levelInfoData.level == _param.level)
            {
                string path = levelInfoData.ScenePath;
                YOTOFramework.resMgr.LoadGameObject(path, (obj) => { _sceneObj = GameObject.Instantiate(obj); });
            }
        }
    }

    private Queue<LevelWeatherInfoData> datas = new Queue<LevelWeatherInfoData>();

    //加载常用系统
    public override void OnInit()
    {
        // Debug.Log( "获取数据："+TestPlayerDataContaner.Instance.GetData().playerName);  
        YOTOFramework.uIMgr.Hide(UIEnum.StartPanel);
        YOTOFramework.sceneMgr.cameraCtrl.UsePlayerCamera();
        var org = GameObject.Find("PlayerOrgPos");
        WeatherManager.Instance.Init();
        //加载紧急事件系统
        // EmergencyManager.Instance.Init();
        YOTOFramework.uIMgr.Show(UIEnum.FightingPanel);
        PlayerManager.Instance.Init(org.transform);
        TowerManager.Instance.Init();
        SceneResManager.Instance.Init();
        YOTOFramework.uIMgr.Show(UIEnum.AimUI);

        datas.Clear();
        foreach (var levelInfoDatas in LevelInfoDataContaner.Instance.GetData().Datas)
        {
            if (levelInfoDatas.level == _param.level)
            {
                var infos = levelInfoDatas.WeatherInfos;
                foreach (var levelInfoData in infos)
                {
                    datas.Enqueue(levelInfoData);
                }
            }
        }

        YOTOFramework.timeMgr.DelayCall(() =>
            {
                EnemyManager.Instance.Init();
                YOTOFramework.Instance.StartCoroutine(DayTimeCycle());
            }
            , 2);
    }

    IEnumerator DayTimeCycle()
    {
        while (datas.Count > 0)
        {
            var info = datas.Dequeue();
            WeatherManager.Instance.ChangeDayTime((DayTimeType)info.dayTime);
            WeatherManager.Instance.ChangeWeather((Weather)info.weather);
            EnemyManager.Instance.GenerateEnemy(info.enemyNum);
            yield return new WaitForSeconds(info.during);
        }

        LevelTimeFinish();
    }

    private void LevelTimeFinish()
    {
    }

    public override void UnLoad()
    {
        YOTOFramework.Instance.StopCoroutine(DayTimeCycle());
        GameObject.Destroy(_sceneObj);
        WeatherManager.Instance.Unload();
        YOTOFramework.uIMgr.ClearUI();
        EnemyManager.Instance.Unload();
        TowerManager.Instance.Unload();
        SceneResManager.Instance.Unload();
        PlayerManager.Instance.Unload();
        Debug.Log("yoto:unload");
    }

    public override void Update(float dt)
    {
        FlyTextMgr.Instance.Update(dt);
    }
}