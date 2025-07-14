    using System;
    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YOTO
{
    public abstract class SceneParam{
    
    }
    public class SceneMgr
    {

       public CameraCtrl cameraCtrl;
       
        private VirtualSceneBase currentScene=null;
        public void Init()
        {
            cameraCtrl = new CameraCtrl();
        }
        public void LoadScene<T>(SceneParam param=null)where T :VirtualSceneBase, new()
        {
            if (currentScene!=null)
            {
                currentScene.UnLoad();
            }



            currentScene = new T();
            // YOTOFramework.uIMgr.ClearUI();
            currentScene.Onload(param);
            currentScene.OnAdd();
            currentScene.OnInit();
         

        }

        public VirtualSceneBase GetCurrentScene()
        {
            return currentScene;
        }
        public void Update(float dt)
        {
            if (cameraCtrl!=null)
            {
                cameraCtrl.Update(UnityEngine.Time.deltaTime);
            }

            currentScene.Update(dt);
        }
    }
}

