using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class YOTOMirrorNetworkManager : NetworkManager
{
    // 事件定义
    private event UnityAction OnStartHostEvent;
    private event UnityAction OnStopHostEvent;
    private event UnityAction OnStartServerEvent;
    private event UnityAction OnStopServerEvent;
    private event UnityAction OnStartClientEvent;
    private event UnityAction OnStopClientEvent;

    // 对外注册接口
    public void AddStartHostListener(UnityAction listener) => OnStartHostEvent += listener;
    public void AddStopHostListener(UnityAction listener) => OnStopHostEvent += listener;
    public void AddStartServerListener(UnityAction listener) => OnStartServerEvent += listener;
    public void AddStopServerListener(UnityAction listener) => OnStopServerEvent += listener;
    public void AddStartClientListener(UnityAction listener) => OnStartClientEvent += listener;
    public void AddStopClientListener(UnityAction listener) => OnStopClientEvent += listener;

    // 移除接口（建议有）
    public void RemoveStartHostListener(UnityAction listener) => OnStartHostEvent -= listener;
    public void RemoveStopHostListener(UnityAction listener) => OnStopHostEvent -= listener;
    public void RemoveStartServerListener(UnityAction listener) => OnStartServerEvent -= listener;
    public void RemoveStopServerListener(UnityAction listener) => OnStopServerEvent -= listener;
    public void RemoveStartClientListener(UnityAction listener) => OnStartClientEvent -= listener;
    public void RemoveStopClientListener(UnityAction listener) => OnStopClientEvent -= listener;

    // 初始化
    public void Init()
    {
        autoCreatePlayer = false; // 你之前的配置
    }

    // Mirror 内置生命周期回调
    public override void OnStartHost()
    {
        base.OnStartHost();
        Debug.Log("✅ Host started (Server + Local Client)");
        OnStartHostEvent?.Invoke();
    }

    public override void OnStopHost()
    {
        base.OnStopHost();
        Debug.Log("🛑 Host stopped");
        OnStopHostEvent?.Invoke();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("✅ Server started");
        OnStartServerEvent?.Invoke();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.Log("🛑 Server stopped");
        OnStopServerEvent?.Invoke();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("✅ Client started");
        OnStartClientEvent?.Invoke();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log("🛑 Client stopped");
        OnStopClientEvent?.Invoke();
    }
}
