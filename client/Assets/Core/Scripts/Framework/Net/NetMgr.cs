
using UnityEngine;
using Mirror;

namespace  YOTO
{



    public class NetMgr
    {
        private bool isServer=false;
        private bool isClient=false;
        private bool isServerClient = false;
        GameServerBase server;
        GameClientBase client;
        public  YOTOMirrorNetworkManager mirrorManager;
        public void Init()
        {
            server = new DemoGameServer();
            client = new DemoGameClient();
            mirrorManager=YOTOMirrorNetworkManager.singleton as YOTOMirrorNetworkManager;
            mirrorManager.Init();
            CreateHost(7777);
        }

        public void CreateHost(ushort port)
        {
            if (!isServer&&!isClient)
            {
                isServer = true;
                JoinHost("", 0);
                server.StartServer(port); 
            }
            else
            {
                Debug.Log("创建房间异常");
            }
        }

        public void StopHost()
        {
            if (isServer)
            {
                server.StopServer();
                isServer = false;
            }
            else
            {
                Debug.Log("关闭房间异常");
            }
            
 
        }

        public void JoinHost(string ip, ushort port)
        {
            if (!isClient&&!isServer)
            {
                client.StartClient(ip,port);
                isClient = true;
                
            }else if(isServer&&!isServerClient){
                client.StartHostClient();
                isServerClient = true;
            }
            else
            {
                Debug.Log("加入host异常");
            }
        }
        public void LeaveHost()
        {
            if (isClient)
            {
                client.StopClient();   
                isClient = false;
            }
            else
            {
                Debug.Log("离开host异常");
            }
        }
        public void FixUpdate(float dt)
        {

            server.Update();
            client.Update();
        }


    }


}
