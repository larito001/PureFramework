using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine;

// 请求基类
public interface IRequest : NetworkMessage { }

// 响应基类
public interface IResponse : NetworkMessage { }

// 例子：移动请求
public struct MoveRequest : IRequest
{
    public uint playerId;
    public Vector3 targetPos;
}

// 例子：移动响应
public struct MoveResponse : IResponse
{
    public uint playerId;
    public Vector3 confirmedPos;
}

public class NetworkMessageManager : MonoBehaviour
{
    // 请求处理器字典（服务端用）
    private Dictionary<Type, Func<IRequest, IResponse>> requestHandlers = new();

    // 客户端响应回调（可选）
    private Dictionary<Type, Action<IResponse>> responseCallbacks = new();

    public static NetworkMessageManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    #region 注册
    public void RegisterRequestHandler<T>(Func<T, IResponse> handler) where T : struct, IRequest
    {
        requestHandlers[typeof(T)] = (req) => handler((T)req);

        if (NetworkServer.active)
            NetworkServer.RegisterHandler<T>(OnServerReceiveRequest);
    }

    public void RegisterResponseHandler<T>(Action<T> callback) where T : struct, IResponse
    {
        responseCallbacks[typeof(T)] = (res) => callback((T)res);

        if (NetworkClient.active)
            NetworkClient.RegisterHandler<T>(OnClientReceiveResponse);
    }
    #endregion

    #region 发送
    public void SendRequest<T>(T request) where T : struct, IRequest
    {
        NetworkClient.Send(request);
    }

    private void SendResponse<T>(T response, NetworkConnectionToClient conn) where T : struct, IResponse
    {
        conn.Send(response);
    }
    #endregion

    #region 接收
    private void OnServerReceiveRequest<T>(NetworkConnectionToClient conn, T request) where T : struct, IRequest
    {
        if (requestHandlers.TryGetValue(typeof(T), out var handler))
        {
            IResponse response = handler(request);
            if (response != null)
                SendResponse((dynamic)response, conn);
        }
    }

    private void OnClientReceiveResponse<T>(T response) where T : struct, IResponse
    {
        if (responseCallbacks.TryGetValue(typeof(T), out var callback))
        {
            callback(response);
        }
    }
    #endregion
}