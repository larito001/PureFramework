using Mirror;
using UnityEngine;
using YOTO;

public class GameServer : MonoBehaviour
{
    void Start()
    {
        #if UNITY_EDITOR
        NetworkManager.singleton.StartHost();
        #endif


        var mgr = ServerMessageManager.Instance;
        // NetworkManager.singleton.networkAddress = serverIP;
        // 注册移动请求处理
        mgr.RegisterRequestHandler<MoveRequest>((req) =>
        {
            Debug.Log($"Player {req.playerId} wants to move to {req.targetPos}");

            // 服务器校验和逻辑
            Vector3 finalPos = req.targetPos; // 可以做阻挡检测/修正
            // 广播给所有客户端
            var res = new MoveResponse
            {
                playerId = req.playerId,
                confirmedPos = finalPos
            };
            mgr.SendNotify(res);
            
            return  res;
        });
    }
}