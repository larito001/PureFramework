
using UnityEngine;
using Mirror;
using Steamworks;

namespace YOTO
{
    public class NetMgr
    {
        private Callback<LobbyCreated_t> lobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> lobbyJoinRequested;
        protected Callback<LobbyEnter_t> lobbyEntered;
        private ELobbyType eLobbyType = ELobbyType.k_ELobbyTypePublic;
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
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            lobbyJoinRequested= Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            server = new DemoGameServer();
            client = new DemoGameClient();
            mirrorManager = YOTOMirrorNetworkManager.singleton as YOTOMirrorNetworkManager;
            mirrorManager.Init();
        }

        private void OnLobbyEntered(LobbyEnter_t param)
        {
            Debug.Log("✅ 已加入 Lobby: " + param.m_ulSteamIDLobby);

            CSteamID lobbyID = new CSteamID(param.m_ulSteamIDLobby);
            string hostAddress = SteamMatchmaking.GetLobbyData(lobbyID, "hostAddress");

            Debug.Log("👉 Host SteamID: " + hostAddress);

            JoinHost(hostAddress); // ✅ 使用host的SteamID连接
        }


        private void OnLobbyJoinRequested(GameLobbyJoinRequested_t param)
        {
            Debug.Log("收到好友邀请，加入 Lobby: " + param.m_steamIDLobby);
            SteamMatchmaking.JoinLobby(param.m_steamIDLobby);
   
        }

        private void OnLobbyCreated(LobbyCreated_t param)
        {
            if (param.m_eResult != EResult.k_EResultOK)
                return;

            CSteamID lobbyID = new CSteamID(param.m_ulSteamIDLobby);
            Debug.Log("✅ Lobby 创建成功: " + lobbyID);

            // 1️⃣ 设置房间信息
            SteamMatchmaking.SetLobbyData(lobbyID, "name", "My Game Room");
            SteamMatchmaking.SetLobbyData(lobbyID, "mode", "PVP");

            // 2️⃣ 设置主机SteamID
            string hostSteamID = SteamUser.GetSteamID().m_SteamID.ToString();
            SteamMatchmaking.SetLobbyData(lobbyID, "hostAddress", hostSteamID);
            
        }
        

        // 创建主机 = 服务器 + 客户端
        public void CreateHost()
        {

            switch (currentState)
            {
                case NetState.Idle:
                    currentState = NetState.Hosting;
                    // 3️⃣ 启动服务器（Mirror Host）
                    JoinHost("");
                    server.StartServer();
                    SteamMatchmaking.CreateLobby(eLobbyType, 4);
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
        public void JoinHost(string id)
        {
            switch (currentState)
            {
                case NetState.Idle:
                    client.StartClient(id);
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
            server.Update(dt);
            client.Update();
        }
    }
}
