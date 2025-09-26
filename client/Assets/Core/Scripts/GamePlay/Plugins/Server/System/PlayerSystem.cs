using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSystem : ServerSystemBase
{
    public const int playerMaxNum = 4;

    public override void AddEvent()
    {
        ServerMessageManager.Instance.RegisterRequestHandler<LoginRequest>(OnLoginRequest);
        ServerMessageManager.Instance.RegisterRequestHandler<HeadPosRequest>(OnHeadRotationRequest);
    }

    public override void RemoveEvent()
    {
        ServerMessageManager.Instance.UnRegisterRequestHandler<LoginRequest>();
        ServerMessageManager.Instance.UnRegisterRequestHandler<HeadPosRequest>();
    }

    private IResponse OnLoginRequest(LoginRequest req, int connectionId)
    {
        Debug.Log($"Player {req.playerName}");

        PlayerData playerDataTemp = null;
        LoginResponse res = new LoginResponse
        {
            isSuccess = false,
            playerData = null
        };
        if (ServerDataPlugin.Instance.GetPlayerList().Count < playerMaxNum && !_server.stateCtrl.GameIsStart())
        {
            playerDataTemp = new PlayerData();
            playerDataTemp.playerId = connectionId;
            playerDataTemp.playerName = req.playerName;
            ServerDataPlugin.Instance.AddPlayer(playerDataTemp);
            // 广播给所有客户端
            RefreshPlayerDatas();
            res.isSuccess = true;
            res.playerData = playerDataTemp;
        }

        if (connectionId == 0)
        {
            _server.stateCtrl.OnJoinRoom();
        }

        return res;
    }

    private void RefreshPlayerDatas()
    {
        RefreshPlayerDatas notify = new RefreshPlayerDatas
        {
            playerDatas = ServerDataPlugin.Instance.GetPlayerList().ToList()
        };
        ServerMessageManager.Instance.SendNotify(notify);
    }

    public void RemovePlayer(int connectionId)
    {
        if (ServerDataPlugin.Instance.RemovePlayerById(connectionId))
        {
            RefreshPlayerDatas();
            if (ServerDataPlugin.Instance.GetPlayerList().Count <= 1)
            {
                _server.stateCtrl.OnJoinRoom();
                _server.OnGameEndNotify();
            }
        }
 
    }

    public void ClearPlayers()
    {
        ServerDataPlugin.Instance.RemoveAllPlayers();
        RefreshPlayerDatas();
    }

    /// <summary>
    /// 刷新所有玩家的属性
    /// </summary>
    public void OnGameStart()
    {
        var tempList = ServerDataPlugin.Instance.GetPlayerList();
        foreach (var player in tempList)
        {
            player.ClearProperty();
            player.RefreshPlayerProperty();
        }
    }

    public void OnRoundEnd()
    {
        var tempList = ServerDataPlugin.Instance.GetPlayerList();
        foreach (var player in tempList)
        {
            player.OnRoundEnd();
        }
    }

    /// <summary>
    /// 广播转发角色头的位置
    /// </summary>
    /// <param name="request"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    private IResponse OnHeadRotationRequest(HeadPosRequest request, int id)
    {
        HeadPosNotify notify = new HeadPosNotify()
        {
            playerId = request.playerId,
            pos = request.pos
        };
        ServerMessageManager.Instance.SendNotify(notify);

        return null;
    }
    
}