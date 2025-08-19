using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using YOTO;

public class ServerMessageManager : Singleton<ServerMessageManager>
{
    // 请求处理器字典（服务端用）
    private Dictionary<Type, Func<IRequest, IResponse>> requestHandlers = new();
    public void RegisterRequestHandler<T>(Func<T, IResponse> handler) where T : struct, IRequest
    {
        requestHandlers[typeof(T)] = (req) => handler((T)req);

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
            IResponse response = handler(request);
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
