using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using YOTO;

public class TestSender : MonoBehaviour
{
    private float timer = 0;
    private bool isInit = false;
    public bool Send = false;
    void Start()
    {
        


    }

    private void Init()
    {
        var mgr = ClientMessageManager.Instance;
#if !UNITY_EDITOR
NetworkManager.singleton.StartClient();
#endif
        if (Transport.active == null)
        {
            Debug.LogError("No Transport active!");
        }

        // 注册响应监听
        mgr.RegisterResponseHandler<MoveResponse>((res) =>
        {
            Debug.Log($"Server confirmed player {res.playerId} moved to {res.confirmedPos}");
            // 更新本地表现
        });
    }
    private void Update()
    {
        if (isInit&&Send)
        {
            RequestMove(123, new Vector3(1, 2, 3));
        }

        timer+=Time.deltaTime;
        if (timer > 5&&!isInit)
        {
            Init();
            isInit = true;
        }

   
    }

    public void RequestMove(uint playerId, Vector3 targetPos)
    {
        var mgr = ClientMessageManager.Instance;
        mgr.SendRequest(new MoveRequest
        {
            playerId = playerId,
            targetPos = targetPos
        });
    }
}
