using kcp2k;
using Mirror;
using UnityEngine;
using YOTO;

public abstract class GameServerBase
{
    protected ServerMessageManager messageMgr;
    private YOTOMirrorNetworkManager mgr;

    // === 启动服务器 ===
    public void StartServer(ushort port)
    {
        messageMgr = ServerMessageManager.Instance;
        mgr = YOTOFramework.netMgr.mirrorManager;

        RegisterServerCallbacks();

        if (mgr.transport is KcpTransport kcp)
        {
            kcp.Port = port;
            mgr.StartHost();  // Host = Server + 本地 Client，但我们只关心服务器
        }
        else
        {
            Debug.LogError("❌ Transport is not KcpTransport");
        }
    }

    // === 停止服务器 ===
    public void StopServer()
    {
        mgr.StopHost();
        UnregisterServerCallbacks();
    }

    // === 注册服务器生命周期事件 ===
    private void RegisterServerCallbacks()
    {
        mgr.AddStartHostListener(OnStartHost);
        mgr.AddStopHostListener(OnStopHost);
        mgr.AddStartServerListener(OnStartServer);
        mgr.AddStopServerListener(OnStopServer);

        // 这些事件现在是带 int 参数的
        mgr.AddServerConnectListener(OnServerConnect);
        mgr.AddServerDisconnectListener(OnServerDisconnect);
        mgr.AddServerReadyListener(OnServerReady);
        mgr.AddServerAddPlayerListener(OnServerAddPlayer);
    }

    private void UnregisterServerCallbacks()
    {
        mgr.RemoveStartHostListener(OnStartHost);
        mgr.RemoveStopHostListener(OnStopHost);
        mgr.RemoveStartServerListener(OnStartServer);
        mgr.RemoveStopServerListener(OnStopServer);

        mgr.RemoveServerConnectListener(OnServerConnect);
        mgr.RemoveServerDisconnectListener(OnServerDisconnect);
        mgr.RemoveServerReadyListener(OnServerReady);
        mgr.RemoveServerAddPlayerListener(OnServerAddPlayer);
    }

    // === 抽象方法：生命周期 ===
    public abstract void Update(float dt);

    public abstract void OnStartHost();
    public abstract void OnStopHost();
    public abstract void OnStartServer();
    public abstract void OnStopServer();

    // 这些方法改为接收 connectionId
    public abstract void OnServerConnect(int connectionId);
    public abstract void OnServerDisconnect(int connectionId);
    public abstract void OnServerReady(int connectionId);
    public abstract void OnServerAddPlayer(int connectionId);
}
