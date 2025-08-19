
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
// 请求基类
public interface IRequest : NetworkMessage { }

// 响应基类
public interface IResponse : NetworkMessage { }
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
public class MessageData 
{
    // 例子：移动请求


}
