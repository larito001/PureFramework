
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
        if (GameMapPlugin.Instance == null) return;

        Gizmos.color = Color.green;

        var cellSize = GameMapPlugin.Instance .cellSize;
        var center = GameMapPlugin.Instance .center;

        for (int x = 0; x <= GameMapPlugin.Instance .column; x++)
        {
            Gizmos.DrawLine(
                new Vector3(center.x + x * cellSize, 0, center.z),
                new Vector3(center.x + x * cellSize, 0, center.z + GameMapPlugin.Instance .row * cellSize));
        }

        for (int y = 0; y <= GameMapPlugin.Instance .row; y++)
        {
            Gizmos.DrawLine(
                new Vector3(center.x, 0, center.z + y * cellSize),
                new Vector3(center.x + GameMapPlugin.Instance .column * cellSize, 0, center.z + y * cellSize));
        }
    }

}
