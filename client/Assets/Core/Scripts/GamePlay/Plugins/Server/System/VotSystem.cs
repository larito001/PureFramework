using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VotSystem : ServerSystemBase
{
    private Dictionary<int, int> playerVotNum = new Dictionary<int, int>(); //投票数
    HashSet<int> playerVotHash = new HashSet<int>(); //是否投过票
    private bool hosterIsLose = false;
    private bool GameHasVoted = false;

    public override void AddEvent()
    {
        ServerMessageManager.Instance.RegisterRequestHandler<VotRequest>(OnVotRequest);
        ServerMessageManager.Instance.RegisterRequestHandler<SomeOneFindHostPlayerRequest>(
            OnSomeOneFindHostPlayerRequest);
    }

    public override void RemoveEvent()
    {
        ServerMessageManager.Instance.UnRegisterRequestHandler<VotRequest>();

        ServerMessageManager.Instance.UnRegisterRequestHandler<SomeOneFindHostPlayerRequest>();
    }

    #region 投票

    /// <summary>
    /// 开启投票阶段
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <returns></returns>
    private IResponse OnSomeOneFindHostPlayerRequest(SomeOneFindHostPlayerRequest arg1, int arg2)
    {
        if (!GameHasVoted)
        {
            playerVotNum.Clear();
            playerVotHash.Clear();
            GameHasVoted = true;
            _server.stateCtrl.StartVoting();
            SomeOneFindHostPlayerNotifyt notify = new SomeOneFindHostPlayerNotifyt
            {
                playerId = arg2
            };
            ServerMessageManager.Instance.SendNotify(notify);
        }

        return null;
    }

    /// <summary>
    /// 玩家投票
    /// </summary>
    private IResponse OnVotRequest(VotRequest arg1, int playerId)
    {
        if (!playerVotHash.Contains(arg1.playerId))
        {
            playerVotHash.Add(arg1.playerId);
            if (playerVotNum.ContainsKey(playerId))
            {
                playerVotNum[playerId]++;
            }
            else
            {
                playerVotNum.Add(playerId, 1);
            }
        }


        return null;
    }

    /// <summary>
    /// 投票结束
    /// </summary>
    public void VotingEnd()
    {
        VotEndNotify notify = new VotEndNotify();
        notify.pidAndvots = new List<Vector2Int>();
        notify.isSuccess = false;
        if (playerVotNum.Count > 0)
        {
            int maxNum = 0;
            int maxId = 0;
            foreach (var keyValuePair in playerVotNum)
            {
                notify.pidAndvots.Add(new Vector2Int(keyValuePair.Key, keyValuePair.Value));
                if (keyValuePair.Value > maxNum)
                {
                    maxNum = keyValuePair.Value;
                    maxId = keyValuePair.Key;
                }
            }

            var hostId = ServerDataPlugin.Instance.RulePlayerId;
            if (hostId == maxId)
            {
                //todo:投票成功
                _server.commonSystem.OnFlyTextNotify(
                    "bingo! hoster is " + ServerDataPlugin.Instance.GetPlayerById(hostId).playerName,
                    FlyTextType.Normal);
                notify.isSuccess = true;
                hosterIsLose = true;
            }
            else
            {
                _server.commonSystem.OnFlyTextNotify("shit,guess wrong!", FlyTextType.Normal);
            }
        }
        else
        {
            _server.commonSystem.OnFlyTextNotify("what? are you sure?", FlyTextType.Normal);
        }

        ServerMessageManager.Instance.SendNotify(notify);
    }

    #endregion

    public void Reset()
    {
        GameHasVoted = false;
        hosterIsLose = false;
    }
}