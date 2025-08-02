using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using YOTO;

public class StartScene : VirtualSceneBase
{
    private TrainEntity _trainEntity;
    public override void OnAdd()
    {
  
    }

    public override void Onload(SceneParam param)
    {

    }

    public override void OnInit()
    {
        _trainEntity = new TrainEntity();
        YOTOFramework.sceneMgr.cameraCtrl.UseStartCamera();
        YOTOFramework.uIMgr.Show(UIEnum.StartPanel);
    }

    public override void UnLoad()
    {
        _trainEntity?.Free();
    }

    public override void Update(float dt)
    {

    }
}
