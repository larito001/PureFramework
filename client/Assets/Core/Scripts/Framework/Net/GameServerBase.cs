using System.Collections;
using System.Collections.Generic;
using kcp2k;
using Mirror;
using UnityEngine;
using UnityEngine.PlayerLoop;
using YOTO;

public abstract class GameServerBase
{
    protected ServerMessageManager messageMgr;
    public void StartServer(ushort port)
    {
        messageMgr = ServerMessageManager.Instance; 
        var mgr = YOTOFramework.netMgr.mirrorManager;
        mgr.AddStartServerListener(OnInit);
        mgr.AddStopServerListener(OnShutdown);
        var transport = mgr.transport;
        var kcp = transport as KcpTransport;
        if (kcp != null)
        {
            kcp.Port = port;
            mgr.StartHost();
        }


    }

    public void StopServer()
    {
        YOTOFramework.netMgr.mirrorManager.StopHost();
    }

    public abstract void OnInit();
    public abstract void Update();
    public abstract void OnShutdown();
}