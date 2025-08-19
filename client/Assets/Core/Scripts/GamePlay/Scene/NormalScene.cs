using System.Collections;

using UnityEngine;
using YOTO;
using EventType = YOTO.EventType;



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
     
    }
    

    //加载常用系统
    public override void OnInit()
    {
        YOTOFramework.sceneMgr.cameraCtrl.UsePlayerCamera();  
    }

  
    private void LevelTimeFinish()
    {
    }
    
    public override void UnLoad()
    {
       
    }

    public override void Update(float dt)
    {
        FlyTextMgr.Instance.Update(dt);
    }
}