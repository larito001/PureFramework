using System.Collections;
using System.Collections.Generic;
using kcp2k;
using Mirror;
using UnityEngine;
using YOTO;

public abstract class GameClientBase
{
    protected ClientMessageManager messageMgr;
    public void StartClient(string ip, ushort prot)
    {
        messageMgr = ClientMessageManager.Instance;
        var mgr = YOTOFramework.netMgr.mirrorManager;
        mgr.AddStartClientListener(OnInit);
        mgr.AddStopClientListener(OnShutdown);
        var transport = mgr.transport;
        var kcp = transport as KcpTransport;
        if (kcp != null)
        {
            kcp.Port = prot;
            mgr.StartClient();
        }
    }
    public void StartHostClient()
    {
        messageMgr = ClientMessageManager.Instance;
        var mgr = YOTOFramework.netMgr.mirrorManager;
        mgr.AddStartClientListener(OnInit);
        mgr.AddStopClientListener(OnShutdown);
    }

    public void StopClient()
    {
        YOTOFramework.netMgr.mirrorManager.StopClient();
    }

    public abstract void OnInit();
    public abstract void Update();
    public abstract void OnShutdown();
}