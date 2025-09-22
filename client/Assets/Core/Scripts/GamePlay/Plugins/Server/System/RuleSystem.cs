using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RuleSystem:ServerSystemBase
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
        // losePlayers.Add(players[0].playerId);
        // var rule = ServerDataPlugin.Instance.CurrentRule;
        // if (rule != null)
        // {
        //     // notify.rule = rule;
        //     var players = ServerDataPlugin.Instance.GetPlayerList();
        //     int winId = 0;
        //     if (rule.ruleId == 1)
        //     {
        //         //todo:读取数据，根据规则发放数据
        //
        //         int maxNum = 0;
        //         foreach (var player in players)
        //         {
        //             // && player.playerId != ServerDataPlugin.Instance.RulePlayerId
        //             if (!hosterIsLose)
        //             {
        //                 if (player.SatisfactionValue >= maxNum)
        //                 {
        //                     winId = player.playerId;
        //                     maxNum = player.lootNum;
        //                 }
        //                 else
        //                 {
        //                     losePlayers.Add(player.playerId);
        //                 }
        //             }
        //             else
        //             {
        //                 losePlayers.Add(player.playerId);
        //             }
        //         }
        //     }
        //     else if (rule.ruleId == 2)
        //     {
        //         int minNum = 999999;
        //         foreach (var player in players)
        //         {
        //             //&& player.playerId != ServerDataPlugin.Instance.RulePlayerId
        //             if (!hosterIsLose )
        //             {
        //                 if (player.SatisfactionValue <= minNum)
        //                 {
        //                     winId = player.playerId;
        //                     minNum = player.lootNum;
        //                 }
        //                 else
        //                 {
        //                     losePlayers.Add(player.playerId);
        //                 }
        //             }
        //             else
        //             {
        //                 losePlayers.Add(player.playerId);
        //             }
        //         }
        //     }
        //     
        //     Debug.Log("结算时规则：" + rule.roleName);
        // }

        return losePlayers;
        // return notify;
    }

    #endregion
}