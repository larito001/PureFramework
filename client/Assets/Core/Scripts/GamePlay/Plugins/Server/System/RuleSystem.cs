using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RuleSystem : ServerSystemBase
{
    public override void AddEvent()
    {
        ServerMessageManager.Instance
            .RegisterRequestHandler<MainPlayerRuleSelectRequest>(OnMainPlayerRuleSelectRequest);
    }

    public override void RemoveEvent()
    {
        ServerMessageManager.Instance.UnRegisterRequestHandler<MainPlayerRuleSelectRequest>();
    }

    #region 规则系统

    /// <summary>
    /// 进入规则指定阶段
    /// </summary>
    public void OnSelectHostPlayer()
    {
        ServerDataPlugin.Instance.ResetRule();
        //选择主玩家，给主玩家发送可选项，制定游戏规则
        var playerId = ServerDataPlugin.Instance.GetRandomPlayer();
        ServerDataPlugin.Instance.SetRulePlayerId(playerId);
        var rules = ServerDataPlugin.Instance.getRandomRules(3);

        RuleSelectNotify notify = new RuleSelectNotify
        {
            rules = rules,
            playerId = playerId
        };
        ServerMessageManager.Instance.SendNotify(notify);
    }

    /// <summary>
    /// 接收玩家选择的rule
    /// </summary>
    /// <param name="param"></param>
    private IResponse OnMainPlayerRuleSelectRequest(MainPlayerRuleSelectRequest param, int playerId)
    {
        int id = param.ruleId;
        ServerDataPlugin.Instance.SetCurrentRule(id);
        _server.stateCtrl.ForcePopCurrentState(GameState.Selecting);
        return null;
    }

    public void SetRandomRule()
    {
        ServerDataPlugin.Instance.SetRandomRule();
    }

    /// <summary>
    /// 规则结算
    /// </summary>
    /// <returns></returns>
    public List<int> OnFinishUseRule()
    {
        // var notify = new GameEndNotify();
        List<int> losePlayers = new List<int>();
        var players = ServerDataPlugin.Instance.GetPlayerList().ToList();

        if (players[0].GetState() != PlayerState.Dead)
        {
            losePlayers.Add(players[0].playerId);
        }
        else if (players.Count > 1 && players[1].GetState() != PlayerState.Dead)
        {
            losePlayers.Add(players[1].playerId);
        }
        else if (players.Count > 2 && players[2].GetState() != PlayerState.Dead)
        {
            losePlayers.Add(players[2].playerId);
        }


        var rule = ServerDataPlugin.Instance.CurrentRule;

        
        if (rule != null)
        {
            int winId = 0;
            
            int maxNum = 0;
            foreach (var player in players)
            {
                
                if (player.SatisfactionValue >= maxNum)
                {
                    winId = player.playerId;
                    maxNum = player.lootNum;
                }
            }

            foreach (var player in players)
            {
                if (winId != player.playerId)
                {
                    losePlayers.Add(player.playerId);
                }
            }
        
            Debug.Log("结算时规则：" + rule.roleName);
        }

        return losePlayers;
        // return notify;
    }

    #endregion
}