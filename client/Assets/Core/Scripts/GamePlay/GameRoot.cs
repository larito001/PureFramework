
using System.Collections.Generic;
using Proto;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using YOTO;
using EventType = YOTO.EventType;

//控制游戏模式（场景）
public class GameRoot : SingletonMono<GameRoot>
{

    public void Init()
    {

        // Application.targetFrameRate = 60;
        FlyTextMgr.Instance.Init();
        // TouchSimulation.Enable();
        YOTOFramework.sceneMgr.LoadScene<StartScene>();
        Debug.Log("GameRoot 加载完成");
    }

    public void StartGame()
    {
        Debug.Log("GameRoot StartGame");

    }
    public void Gaming()
    {
        Debug.Log("GameRoot Gaming");
    }
    private void OnDrawGizmos()
    {
     
    }

}
