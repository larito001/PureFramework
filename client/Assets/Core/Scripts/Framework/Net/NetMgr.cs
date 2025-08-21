using UnityEngine;
using Mirror;

namespace YOTO
{
    public class NetMgr
    {
        private enum NetState
        {
            Idle,           // 空闲状态，未连接
            Hosting,        // 本地作为主机（服务器+客户端）
            Client,         // 仅客户端
            ServerClient    // 服务器模式下的客户端
        }

        private NetState currentState = NetState.Idle;

        private GameServerBase server;
        private GameClientBase client;
        public YOTOMirrorNetworkManager mirrorManager;

        public void Init()
        {
            server = new DemoGameServer();
            client = new DemoGameClient();
            mirrorManager = YOTOMirrorNetworkManager.singleton as YOTOMirrorNetworkManager;
            mirrorManager.Init();
        }

        // 创建主机 = 服务器 + 客户端
        public void CreateHost(ushort port)
        {
            switch (currentState)
            {
                case NetState.Idle:
                    currentState = NetState.Hosting;
                    JoinHost("", 0); // 本地客户端连接
                    server.StartServer(port);
                    break;
                default:
                    Debug.LogWarning("创建房间异常: 当前状态=" + currentState);
                    break;
            }
        }

        public void StopHost()
        {
            switch (currentState)
            {
                case NetState.Hosting:
                    server.StopServer();
                    currentState = NetState.Idle;
                    break;
                default:
                    Debug.LogWarning("关闭房间异常: 当前状态=" + currentState);
                    break;
            }
        }

        // 加入主机 = 客户端
        public void JoinHost(string ip, ushort port)
        {
            switch (currentState)
            {
                case NetState.Idle:
                    client.StartClient(ip, port);
                    currentState = NetState.Client;
                    break;
                case NetState.Hosting:
                    client.StartHostClient();
                    currentState = NetState.ServerClient;
                    break;
                default:
                    Debug.LogWarning("加入host异常: 当前状态=" + currentState);
                    break;
            }
        }

        public void LeaveHost()
        {
            switch (currentState)
            {
                case NetState.Client:
                case NetState.ServerClient:
                    client.StopClient();
                    if (currentState == NetState.ServerClient)
                        currentState = NetState.Hosting; // 保留服务器
                    else
                        currentState = NetState.Idle;
                    break;
                default:
                    Debug.LogWarning("离开host异常: 当前状态=" + currentState);
                    break;
            }
        }

        public void FixUpdate(float dt)
        {
            server.Update();
            client.Update();
        }
    }
}
