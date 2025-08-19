
using UnityEngine;
using Mirror;

namespace  YOTO
{
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
    public class NetMgr
    {
        //todo:Server
        //todo:Client
        public void Init()
        {
            
        }
        public void FixUpdate(float dt)
        {


        }


    }


}
