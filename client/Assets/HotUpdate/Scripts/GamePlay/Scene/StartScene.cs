using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using YOTO;

public class StartScene : VirtualSceneBase
{
 
    public override void OnAdd()
    {
  
    }

    public override void Onload(SceneParam param)
    {

    }

    public override void OnInit()
    {
        YOTOFramework.sceneMgr.cameraCtrl.UseStartCamera();
        YOTOFramework.uIMgr.Show(UIEnum.StartPanel);
    }

    public override void UnLoad()
    {

    }

    public override void Update(float dt)
    {

    }
}
