using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using YOTO;

public class ServerMessageManager : Singleton<ServerMessageManager>
{
    // 请求处理器字典（服务端用）
    private Dictionary<Type, Func<IRequest, int, IResponse>> requestHandlers = new();

    // 注册时传入 Func<T, int, IResponse>
    public void RegisterRequestHandler<T>(Func<T, int, IResponse> handler) where T : struct, IRequest
    {
        requestHandlers[typeof(T)] = (req, connId) => handler((T)req, connId);

        if (NetworkServer.active)
            NetworkServer.RegisterHandler<T>(OnServerReceiveRequest);
    }

    public void UnRegisterRequestHandler<T>() where T : struct, IRequest
    {
        requestHandlers.Remove(typeof(T));
    }

    private void OnServerReceiveRequest<T>(NetworkConnectionToClient conn, T request) where T : struct, IRequest
    {
        if (requestHandlers.TryGetValue(typeof(T), out var handler))
        {
            int connectionId = conn.connectionId;

            // 调用处理器并传 connectionId
            IResponse response = handler(request, connectionId);

            if (response != null)
                SendResponse((dynamic)response, conn);
        }
    }

    private void SendResponse<T>(T response, NetworkConnectionToClient conn) where T : struct, IResponse
    {
        conn.Send(response);
    }

    public void SendNotify<T>(T notify) where T : struct, IResponse
    {
        NetworkServer.SendToReady(notify);
    }
}