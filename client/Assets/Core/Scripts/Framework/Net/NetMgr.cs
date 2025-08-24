using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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
        private static bool IsPortInUse(int port)
        {
            try
            {
                // 尝试绑定到指定端口
                var listener = new TcpListener(IPAddress.Loopback, port);
                listener.Start();
                listener.Stop();
                return false; // 端口可用
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
            {
                return true; // 端口已被使用
            }
        }
        public static bool IsUdpPortInUse(int port)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] udpListeners = ipGlobalProperties.GetActiveUdpListeners();
    
            return udpListeners.Any(endpoint => endpoint.Port == port);
        }

        // 创建主机 = 服务器 + 客户端
        public void CreateHost(ushort port)
        {
            switch (currentState)
            {
                case NetState.Idle:
                    if (!IsPortInUse(port)&&!IsUdpPortInUse(port))
                    {
                        currentState = NetState.Hosting;
                        JoinHost("", 0); // 本地客户端连接
                        server.StartServer(port);  
                    }
                    else
                    {
                        Debug.LogError("端口已被使用");
                    }
               
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
                    client.StopHostClient();
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
                    client.StopClient();
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
