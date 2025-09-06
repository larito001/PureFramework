using System.Collections;

using UnityEngine;
using YOTO;


public class NormalScene : VirtualSceneBase
{
    private GameObject _sceneObj;
    private IEnumerator generateEnemyIE;
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
        YOTOFramework.uIMgr.ClearUI();
        PlayerPlugin.Instance.GeneratePlayers(LoginPlugin.Instance.GetPlayerDatas());
    }
    

    //加载常用系统
    public override void OnInit()
    {
        
    }
    public override void UnLoad()
    {
        YOTOFramework.uIMgr.ClearUI();
        PlayerPlugin.Instance.RemoveAllPlayers();
        StagePlugin.Instance.RemoveAllFoods();
    }

    public override void Update(float dt)
    {
        FlyTextMgr.Instance.Update(dt);
    }
}