using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class YOTOMirrorNetworkManager : NetworkManager
{
 // ==== Host 相关事件 ====
    /// <summary>
    /// 调用 StartHost() 后触发（Host = Server + 本地 Client）  
    /// 用途：初始化房间、分配本地玩家
    /// </summary>
    private event UnityAction OnStartHostEvent;

    /// <summary>
    /// 调用 StopHost() 后触发  
    /// 用途：关闭房间、清理 Host 资源
    /// </summary>
    private event UnityAction OnStopHostEvent;

    // ==== Server 相关事件 ====
    /// <summary>
    /// 服务器启动后触发（包括 Host）  
    /// 用途：初始化服务器逻辑、加载场景
    /// </summary>
    private event UnityAction OnStartServerEvent;

    /// <summary>
    /// 服务器停止后触发（包括 Host）  
    /// 用途：清理服务器资源
    /// </summary>
    private event UnityAction OnStopServerEvent;

    // ==== Server 与客户端连接相关事件 ====
    /// <summary>
    /// 客户端连接服务器时触发  
    /// 参数 connectionId：客户端连接 ID（Host 本地客户端 = 0）  
    /// 用途：身份验证、初始化连接数据
    /// </summary>
    private event UnityAction<int> OnServerConnectEvent;

    /// <summary>
    /// 客户端断开连接时触发  
    /// 参数 connectionId：断开客户端 ID  
    /// 用途：清理玩家数据、广播断线消息
    /// </summary>
    private event UnityAction<int> OnServerDisconnectEvent;

    /// <summary>
    /// 客户端 Ready 后触发  
    /// 参数 connectionId：客户端 ID  
    /// 用途：确认客户端状态、同步初始数据
    /// </summary>
    private event UnityAction<int> OnServerReadyEvent;

    /// <summary>
    /// 为客户端创建玩家对象时触发  
    /// 参数 connectionId：玩家连接 ID  
    /// 用途：初始化玩家对象、绑定玩家数据
    /// </summary>
    private event UnityAction<int> OnServerAddPlayerEvent;

    // ==== Client 生命周期事件 ====
    /// <summary>
    /// 客户端启动后触发  
    /// 用途：初始化客户端状态
    /// </summary>
    private event UnityAction OnStartClientEvent;

    /// <summary>
    /// 客户端停止后触发  
    /// 用途：清理客户端资源、返回主界面
    /// </summary>
    private event UnityAction OnStopClientEvent;

    // ==== Client 与服务器连接相关事件 ====
    /// <summary>
    /// 客户端连接到服务器时触发  
    /// 用途：请求登录、加载大厅数据
    /// </summary>
    private event UnityAction OnClientConnectEvent;

    /// <summary>
    /// 客户端断开服务器连接时触发  
    /// 用途：提示断线、返回主菜单
    /// </summary>
    private event UnityAction OnClientDisconnectEvent;

    /// <summary>
    /// 服务器标记客户端 NotReady 时触发  
    /// 用途：禁用交互、显示加载提示
    /// </summary>
    private event UnityAction OnClientNotReadyEvent;

    /// <summary>
    /// 客户端开始切换场景时触发  
    /// 用途：显示加载界面
    /// </summary>
    private event UnityAction OnClientChangeSceneEvent;

    /// <summary>
    /// 客户端完成场景切换时触发  
    /// 用途：初始化场景 UI 和对象
    /// </summary>
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

    public void AddServerConnectListener(UnityAction<int> listener) => OnServerConnectEvent += listener;
    public void RemoveServerConnectListener(UnityAction<int> listener) => OnServerConnectEvent -= listener;

    public void AddServerDisconnectListener(UnityAction<int> listener) => OnServerDisconnectEvent += listener;
    public void RemoveServerDisconnectListener(UnityAction<int> listener) => OnServerDisconnectEvent -= listener;

    public void AddServerReadyListener(UnityAction<int> listener) => OnServerReadyEvent += listener;
    public void RemoveServerReadyListener(UnityAction<int> listener) => OnServerReadyEvent -= listener;

    public void AddServerAddPlayerListener(UnityAction<int> listener) => OnServerAddPlayerEvent += listener;
    public void RemoveServerAddPlayerListener(UnityAction<int> listener) => OnServerAddPlayerEvent -= listener;

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
        OnServerConnectEvent?.Invoke(conn.connectionId);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        OnServerDisconnectEvent?.Invoke(conn.connectionId);
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);
        OnServerReadyEvent?.Invoke(conn.connectionId);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        OnServerAddPlayerEvent?.Invoke(conn.connectionId);
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
