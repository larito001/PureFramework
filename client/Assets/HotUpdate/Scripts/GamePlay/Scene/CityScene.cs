using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YOTO;
using EventType = YOTO.EventType;

public class CityScene : VirtualSceneBase
{
    //注册事件
    public override void OnAdd()
    {
    }

    //
    public override void Onload(SceneParam param)
    {
        YOTOFramework.eventMgr.AddEventListener(EventType.GameStart,GameStart);

    }

    private Queue<LevelWeatherInfoData> datas = new Queue<LevelWeatherInfoData>();

    //加载常用系统
    public override void OnInit()
    {
        System.GC.Collect();
        System.GC.Collect();

        // Debug.Log( "获取数据："+TestPlayerDataContaner.Instance.GetData().playerName);  
        WeatherManager.Instance.Init();
        //加载紧急事件系统
        // EmergencyManager.Instance.Init();
        var org = GameObject.Find("PlayerOrgPos");
        PlayerManager.Instance.Init(org.transform);

        SceneResManager.Instance.Init();

        datas.Clear();
      
    }


    private void LevelTimeFinish()
    {
    }

    private void GameStart()
    {
        PlayerManager.Instance.Start();
        YOTOFramework.sceneMgr.cameraCtrl.UsePlayerCamera();
        YOTOFramework.uIMgr.Show(UIEnum.FightingPanel);
        YOTOFramework.uIMgr.Show(UIEnum.AimUI);
        TowerManager.Instance.Init();
        EnemyManager.Instance.Init();
        EnemyManager.Instance.GenerateEnemy(50);
    }

    public override void UnLoad()
    {
        YOTOFramework.eventMgr.RemoveEventListener(EventType.GameStart, GameStart);
        PlayerResManager.Instance.SaveRes();
        WeatherManager.Instance.Unload();
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