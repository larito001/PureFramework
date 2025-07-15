using System.Collections;
using System.Collections.Generic;
using Proto;
using UnityEngine;
using YOTO;
using EventType = YOTO.EventType;

public class NormalScene : VirtualSceneBase
{
    private GameObject _sceneObj;
    public class NormalSceneParam:SceneParam
    {
        public int level = 0;
    }
    //注册事件
    public override void OnAdd()
    {
    }

    //
    public override void Onload(SceneParam param)
    {
        var p = param as NormalSceneParam;
        Debug.Log("YTLOG;加载了场景"+p.level);
        string path;
        path = "Assets/HotUpdate/prefabs/Level/Level_"+p.level+".prefab";
        YOTOFramework.resMgr.LoadGameObject(path, (obj) =>
        {
            _sceneObj=   GameObject.Instantiate(obj);
        });
    }


    //加载常用系统
    public override void OnInit()
    {
     Debug.Log( "获取数据："+TestPlayerDataContaner.Instance.GetData().playerName);  
        YOTOFramework.uIMgr.Hide(UIEnum.StartPanel);
        YOTOFramework.sceneMgr.cameraCtrl.UsePlayerCamera();
        var org = GameObject.Find("PlayerOrgPos");
        WeatherManager.Instance.Init();
        //加载紧急事件系统
        // EmergencyManager.Instance.Init();
        YOTOFramework.uIMgr.Show(UIEnum.FightingPanel);
        EnemyManager.Instance.Init();
        PlayerManager.Instance.Init(org.transform);
        TowerManager.Instance.Init();
        SceneResManager.Instance.Init();
        YOTOFramework.uIMgr.Show(UIEnum.AimUI);
    }

    public override void UnLoad()
    {
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