using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Google.Protobuf;
using Proto;
using System.Threading.Tasks;
using Mirror;

namespace  YOTO
{
    public struct MoveMessage : NetworkMessage
    {
        public uint NetId;
        public Vector3 Position;
    }
    public abstract class BaseResponse
    {
        public abstract void Init();
    }
    public class NetData
    {
        public MessageType type;
        public  IMessage message;
    }

    public enum NetDataType
    {
        Room,
        GamePlay
    }

    public class NetMgr
    {


        public void FixUpdate(float dt)
        {


        }

        public void Init()
        {
            NetworkClient.RegisterHandler<MoveMessage>(OnMoveMessage);
            SendMove(123, Vector3.one);
        }
        public void SendMove(uint netId, Vector3 pos)
        {
            MoveMessage msg = new MoveMessage { NetId = netId, Position = pos };
            NetworkClient.Send(msg);
        }

        private void OnMoveMessage(MoveMessage msg)
        {
        Debug.LogError("收到消息："+msg.NetId);
        }
    }


}
