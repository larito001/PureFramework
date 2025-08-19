using System.Collections;
using System.Collections.Generic;
using kcp2k;
using Mirror;
using UnityEngine;
using YOTO;

public abstract class GameClientBase
{
    public void StartClient(string ip, ushort prot,bool isHostPlayer=false)
    {
   
        var mgr = YOTOFramework.netMgr.mirrorManager;
        mgr.AddStartClientListener(OnInit);
        mgr.AddStopClientListener(OnShutdown);
        if (!isHostPlayer)
        {
            var transport = mgr.transport;
            var kcp = transport as KcpTransport;
            if (kcp != null)
            {
                kcp.Port = prot;
                mgr.StartClient();
            }
        }
   

    }

    public void StopClient()
    {
        YOTOFramework.netMgr.mirrorManager.StopClient();
    }

    public abstract void OnInit();
    public abstract void Update();
    public abstract void OnShutdown();
}