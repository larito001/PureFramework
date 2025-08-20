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
    public void AddStopHostListener(UnityAction listener) => OnStopHostEvent += listener;
    public void AddStartServerListener(UnityAction listener) => OnStartServerEvent += listener;
    public void AddStopServerListener(UnityAction listener) => OnStopServerEvent += listener;
    public void AddServerConnectListener(UnityAction listener) => OnServerConnectEvent += listener;
    public void AddServerDisconnectListener(UnityAction listener) => OnServerDisconnectEvent += listener;
    public void AddServerReadyListener(UnityAction listener) => OnServerReadyEvent += listener;
    public void AddServerAddPlayerListener(UnityAction listener) => OnServerAddPlayerEvent += listener;
    public void AddStartClientListener(UnityAction listener) => OnStartClientEvent += listener;
    public void AddStopClientListener(UnityAction listener) => OnStopClientEvent += listener;
    public void AddClientConnectListener(UnityAction listener) => OnClientConnectEvent += listener;
    public void AddClientDisconnectListener(UnityAction listener) => OnClientDisconnectEvent += listener;
    public void AddClientNotReadyListener(UnityAction listener) => OnClientNotReadyEvent += listener;
    public void AddClientChangeSceneListener(UnityAction listener) => OnClientChangeSceneEvent += listener;
    public void AddClientSceneChangedListener(UnityAction listener) => OnClientSceneChangedEvent += listener;

    // ==== Mirror 内置生命周期重写 ====

    // ---------------- Host ----------------
    /// <summary>Host 启动时（包含本地 Server + 本地 Client）</summary>
    public override void OnStartHost()
    {
        base.OnStartHost();
        Debug.Log("✅ Host started");
        OnStartHostEvent?.Invoke();
    }

    /// <summary>Host 停止时</summary>
    public override void OnStopHost()
    {
        base.OnStopHost();
        Debug.Log("🛑 Host stopped");
        OnStopHostEvent?.Invoke();
    }

    // ---------------- Server ----------------
    /// <summary>服务器启动（监听端口成功）</summary>
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("✅ Server started");
        OnStartServerEvent?.Invoke();
    }

    /// <summary>服务器关闭</summary>
    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.Log("🛑 Server stopped");
        OnStopServerEvent?.Invoke();
    }

    /// <summary>有客户端连接到服务器（握手成功，但未必 ready）</summary>
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        Debug.Log($"📡 Client {conn.connectionId} connected to server");
        OnServerConnectEvent?.Invoke();
    }

    /// <summary>客户端断开连接</summary>
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log($"📴 Client {conn.connectionId} disconnected from server");
        OnServerDisconnectEvent?.Invoke();
    }

    /// <summary>当客户端发送 ready 消息（进入 ready 状态）</summary>
    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);
        Debug.Log($"✅ Client {conn.connectionId} is ready");
        OnServerReadyEvent?.Invoke();
    }

    /// <summary>当服务器为客户端添加玩家对象时（默认基于 PlayerPrefab）</summary>
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log($"👤 Player added for client {conn.connectionId}");
        OnServerAddPlayerEvent?.Invoke();
    }
    // ---------------- Client ----------------
    /// <summary>客户端启动（开始尝试连接服务器）</summary>
    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("✅ Client started (attempting connection...)");
        OnStartClientEvent?.Invoke();
    }

    /// <summary>客户端停止</summary>
    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log("🛑 Client stopped");
        OnStopClientEvent?.Invoke();
    }

    /// <summary>客户端连接到服务器成功</summary>
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("✅ Client connected to server");
        OnClientConnectEvent?.Invoke();
    }

    /// <summary>客户端从服务器断开</summary>
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        Debug.Log("🛑 Client disconnected from server");
        OnClientDisconnectEvent?.Invoke();
    }

    /// <summary>客户端被标记为 NotReady</summary>
    public override void OnClientNotReady()
    {
        base.OnClientNotReady();
        Debug.Log("⚠️ Client is not ready");
        OnClientNotReadyEvent?.Invoke();
    }

    /// <summary>客户端切换场景时调用</summary>
    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    {
        base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
        Debug.Log($"🌍 Client changing scene to {newSceneName}");
        OnClientChangeSceneEvent?.Invoke();
    }

    /// <summary>客户端场景切换完成</summary>
    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
        Debug.Log("🌍 Client scene changed");
        OnClientSceneChangedEvent?.Invoke();
    }

    // ---------------- 初始化 ----------------
    public void Init()
    {
        autoCreatePlayer = false; // 禁止自动创建玩家，由你手动处理
    }
}
