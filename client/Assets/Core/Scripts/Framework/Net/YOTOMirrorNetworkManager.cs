using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class YOTOMirrorNetworkManager : NetworkManager
{
    // ==== 事件定义 ====
    private event UnityAction OnStartHostEvent;
    private event UnityAction OnStopHostEvent;
    private event UnityAction OnStartServerEvent;
    private event UnityAction OnStopServerEvent;
    private event UnityAction OnServerConnectEvent;
    private event UnityAction OnServerDisconnectEvent;
    private event UnityAction OnServerReadyEvent;
    private event UnityAction OnServerAddPlayerEvent;
    private event UnityAction OnStartClientEvent;
    private event UnityAction OnStopClientEvent;
    private event UnityAction OnClientConnectEvent;
    private event UnityAction OnClientDisconnectEvent;
    private event UnityAction OnClientNotReadyEvent;
    private event UnityAction OnClientChangeSceneEvent;
    private event UnityAction OnClientSceneChangedEvent;

    // ==== 对外注册接口 ====
    public void AddStartHostListener(UnityAction listener) => OnStartHostEvent += listener;
    public void RemoveStartHostListener(UnityAction listener) => OnStartHostEvent -= listener;

    public void AddStopHostListener(UnityAction listener) => OnStopHostEvent += listener;
    public void RemoveStopHostListener(UnityAction listener) => OnStopHostEvent -= listener;

    public void AddStartServerListener(UnityAction listener) => OnStartServerEvent += listener;
    public void RemoveStartServerListener(UnityAction listener) => OnStartServerEvent -= listener;

    public void AddStopServerListener(UnityAction listener) => OnStopServerEvent += listener;
    public void RemoveStopServerListener(UnityAction listener) => OnStopServerEvent -= listener;

    public void AddServerConnectListener(UnityAction listener) => OnServerConnectEvent += listener;
    public void RemoveServerConnectListener(UnityAction listener) => OnServerConnectEvent -= listener;

    public void AddServerDisconnectListener(UnityAction listener) => OnServerDisconnectEvent += listener;
    public void RemoveServerDisconnectListener(UnityAction listener) => OnServerDisconnectEvent -= listener;

    public void AddServerReadyListener(UnityAction listener) => OnServerReadyEvent += listener;
    public void RemoveServerReadyListener(UnityAction listener) => OnServerReadyEvent -= listener;

    public void AddServerAddPlayerListener(UnityAction listener) => OnServerAddPlayerEvent += listener;
    public void RemoveServerAddPlayerListener(UnityAction listener) => OnServerAddPlayerEvent -= listener;

    public void AddStartClientListener(UnityAction listener) => OnStartClientEvent += listener;
    public void RemoveStartClientListener(UnityAction listener) => OnStartClientEvent -= listener;

    public void AddStopClientListener(UnityAction listener) => OnStopClientEvent += listener;
    public void RemoveStopClientListener(UnityAction listener) => OnStopClientEvent -= listener;

    public void AddClientConnectListener(UnityAction listener) => OnClientConnectEvent += listener;
    public void RemoveClientConnectListener(UnityAction listener) => OnClientConnectEvent -= listener;

    public void AddClientDisconnectListener(UnityAction listener) => OnClientDisconnectEvent += listener;
    public void RemoveClientDisconnectListener(UnityAction listener) => OnClientDisconnectEvent -= listener;

    public void AddClientNotReadyListener(UnityAction listener) => OnClientNotReadyEvent += listener;
    public void RemoveClientNotReadyListener(UnityAction listener) => OnClientNotReadyEvent -= listener;

    public void AddClientChangeSceneListener(UnityAction listener) => OnClientChangeSceneEvent += listener;
    public void RemoveClientChangeSceneListener(UnityAction listener) => OnClientChangeSceneEvent -= listener;

    public void AddClientSceneChangedListener(UnityAction listener) => OnClientSceneChangedEvent += listener;
    public void RemoveClientSceneChangedListener(UnityAction listener) => OnClientSceneChangedEvent -= listener;

    // ==== Mirror 内置生命周期重写 ====

    // ---------------- Host ----------------
    public override void OnStartHost()
    {
        base.OnStartHost();
        OnStartHostEvent?.Invoke();
    }

    public override void OnStopHost()
    {
        base.OnStopHost();
        OnStopHostEvent?.Invoke();
    }

    // ---------------- Server ----------------
    public override void OnStartServer()
    {
        base.OnStartServer();
        OnStartServerEvent?.Invoke();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        OnStopServerEvent?.Invoke();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        OnServerConnectEvent?.Invoke();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        OnServerDisconnectEvent?.Invoke();
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);
        OnServerReadyEvent?.Invoke();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        OnServerAddPlayerEvent?.Invoke();
    }

    // ---------------- Client ----------------
    public override void OnStartClient()
    {
        base.OnStartClient();
        OnStartClientEvent?.Invoke();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        OnStopClientEvent?.Invoke();
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        OnClientConnectEvent?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        OnClientDisconnectEvent?.Invoke();
    }

    public override void OnClientNotReady()
    {
        base.OnClientNotReady();
        OnClientNotReadyEvent?.Invoke();
    }

    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    {
        base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
        OnClientChangeSceneEvent?.Invoke();
    }

    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
        OnClientSceneChangedEvent?.Invoke();
    }

    // ---------------- 初始化 ----------------
    public void Init()
    {
        autoCreatePlayer = false; // 禁止自动创建玩家，由你手动处理
    }
}
