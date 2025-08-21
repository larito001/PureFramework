using System.Collections;
using System.Collections.Generic;
using kcp2k;
using Mirror;
using UnityEngine;
using YOTO;

public abstract class GameClientBase
{
    protected ClientMessageManager messageMgr;

    // ==== 启动客户端 ====
    public void StartClient(string ip, ushort port)
    {
        messageMgr = ClientMessageManager.Instance;
        var mgr = YOTOFramework.netMgr.mirrorManager;

        RegisterClientEvents(mgr);

        var transport = mgr.transport as KcpTransport;
        if (transport != null)
        {
            transport.Port = port;
        }

        mgr.networkAddress = ip;
        mgr.StartClient();
    }

    // ==== 启动 Host 客户端（本地 Host） ====
    public void StartHostClient()
    {
        messageMgr = ClientMessageManager.Instance;
        var mgr = YOTOFramework.netMgr.mirrorManager;

        RegisterClientEvents(mgr);
    }

    // ==== 停止客户端 ====
    public void StopClient()
    {
        var mgr = YOTOFramework.netMgr.mirrorManager;

        RemoveClientEvents(mgr);

        mgr.StopClient();
    }

    // ==== 封装注册和移除方法 ====
    private void RegisterClientEvents(YOTOMirrorNetworkManager mgr)
    {
        mgr.AddStartClientListener(OnStartClient);
        mgr.AddStopClientListener(OnStopClient);
        mgr.AddClientConnectListener(OnClientConnect);
        mgr.AddClientDisconnectListener(OnClientDisconnect);
        mgr.AddClientNotReadyListener(OnClientNotReady);
        mgr.AddClientChangeSceneListener(OnClientChangeScene);
        mgr.AddClientSceneChangedListener(OnClientSceneChanged);
    }

    private void RemoveClientEvents(YOTOMirrorNetworkManager mgr)
    {
        mgr.RemoveStartClientListener(OnStartClient);
        mgr.RemoveStopClientListener(OnStopClient);
        mgr.RemoveClientConnectListener(OnClientConnect);
        mgr.RemoveClientDisconnectListener(OnClientDisconnect);
        mgr.RemoveClientNotReadyListener(OnClientNotReady);
        mgr.RemoveClientChangeSceneListener(OnClientChangeScene);
        mgr.RemoveClientSceneChangedListener(OnClientSceneChanged);
    }

    // ==== 抽象生命周期方法 ====
    public abstract void Update();
    public abstract void OnStartClient();       // 客户端启动
    public abstract void OnStopClient();        // 客户端停止
    public abstract void OnClientConnect();     // 连接成功
    public abstract void OnClientDisconnect();  // 断开连接
    public abstract void OnClientNotReady();    // 标记 NotReady
    public abstract void OnClientChangeScene(); // 切换场景
    public abstract void OnClientSceneChanged();// 场景切换完成
}
