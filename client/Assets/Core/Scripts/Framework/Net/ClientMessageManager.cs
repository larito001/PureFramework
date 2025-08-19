using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using YOTO;

public class ClientMessageManager : Singleton<ClientMessageManager>
{
    // 客户端响应回调（可选）
    private Dictionary<Type, Action<IResponse>> responseCallbacks = new();
    public void RegisterResponseHandler<T>(Action<T> callback) where T : struct, IResponse
    {
        responseCallbacks[typeof(T)] = (res) => callback((T)res);

        if (NetworkClient.active)
            NetworkClient.RegisterHandler<T>(OnClientReceiveResponse);
    }
    public void  UnRegisterResponseHandler  <T>() where T : struct, IResponse
    {
        responseCallbacks.Remove(typeof(T));
    }
    
    public void SendRequest<T>(T request) where T : struct, IRequest
    {
        NetworkClient.Send(request);
    }

    
    private void OnClientReceiveResponse<T>(T response) where T : struct, IResponse
    {
        if (responseCallbacks.TryGetValue(typeof(T), out var callback))
        {
            callback(response);
        }
    }
}
