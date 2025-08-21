
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
// 请求基类
public interface IRequest : NetworkMessage { }

// 响应基类
public interface IResponse : NetworkMessage { }

public class PlayerData
{
    public int playerId;
    public string playerName;
    
}
public struct LoginRequest : IRequest
{
    public string playerName;
}
public struct LoginNotify : IResponse
{
    public List<PlayerData> playerDatas;
}
public struct LinkConfig: IRequest
{
    public int connectId;
}
public struct LinkConfigResponse: IResponse
{
    public bool isSuccess;
}
// 例子：移动响应
public struct LoginResponse : IResponse
{
    public PlayerData playerData;
    public bool isSuccess;
}
public class MessageData 
{
    // 例子：移动请求


}
