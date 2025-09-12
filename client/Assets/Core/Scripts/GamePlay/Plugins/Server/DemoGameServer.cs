using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DemoGameServer : GameServerBase
{
    #region  配置属性

    private const float orgGameTime = 30; //游戏总时长
    private readonly int playerMaxNum = 4;
    private const float orgStateTimer = 2;
    private const float orgSelectingTimer = 5;

    #endregion
    
    #region 暂存属性
    
    private GameState gameState = GameState.Idle;
    private float readyTimer = orgStateTimer;
    private float selectingTimer = orgSelectingTimer;
    private float delayTimer = 1;
    private int delayIndex = (int)orgStateTimer;
    private float gameTimer = 0;
    Queue<FoodDropStage> stageQueue = new Queue<FoodDropStage>();
    
    #endregion

    #region 生命周期

    private void AddEvent()
    {
        ServerMessageManager.Instance.RegisterRequestHandler<LoginRequest>(OnLoginRequest);
        ServerMessageManager.Instance.RegisterRequestHandler<GameStartRequest>(OnGameReadyRequest);
        ServerMessageManager.Instance.RegisterRequestHandler<HeadPosRequest>(OnHeadRotationRequest);
        ServerMessageManager.Instance.RegisterRequestHandler<CatchFoodRequest>(OnCatchFoodRequest);
        ServerMessageManager.Instance.RegisterRequestHandler<LootingInputRequest>(OnLootingInputRequest);
        ServerMessageManager.Instance.RegisterRequestHandler<MainPlayerRuleSelectRequest>(OnMainPlayerRuleSelectRequest);
    }




    private void RemoveEvent()
    {
        ServerMessageManager.Instance.UnRegisterRequestHandler<LoginRequest>();
        ServerMessageManager.Instance.UnRegisterRequestHandler<GameStartRequest>();
        ServerMessageManager.Instance.UnRegisterRequestHandler<HeadPosRequest>();
        ServerMessageManager.Instance.UnRegisterRequestHandler<CatchFoodRequest>();
        ServerMessageManager.Instance.UnRegisterRequestHandler<LootingInputRequest>();
        ServerMessageManager.Instance.UnRegisterRequestHandler<MainPlayerRuleSelectRequest>();
    }

    public override void Update(float dt)
    {
        // 可以在这里处理服务器每帧逻辑
        foreach (var food in ServerDataPlugin.Instance.GetFoodList())
        {
            food.Update(dt);
        }

        if (gameState == GameState.Selecting)
        {
            selectingTimer -= dt;
            if (selectingTimer <= 0)
            {
                SetRandomRule();
            }
        }

        if (gameState == GameState.Ready)
        {
            readyTimer -= dt;
            delayTimer -= dt;
            if (delayTimer <= 0)
            {
                OnFlyTextNotify("Ready!", FlyTextType.Normal);
                delayIndex--;
                delayTimer = 1f; // 重置为1秒
            }

            if (readyTimer <= 0)
            {
                OnGameStart();
                ReSetTimers();
            }
        }

        if (gameState == GameState.Playing)
        {
            //生成食物
            if (stageQueue.Count > 0 && gameTimer >= stageQueue.Peek().randomTime)
            {
                var foodStage = stageQueue.Dequeue();
                GenerateFoods(foodStage);
            }


            gameTimer += dt;
            if (gameTimer >= orgGameTime)
            {
                gameTimer = 0;
                OnGameEndNotify();
            }
        }
    }

    private void ReSetTimers()
    {
        delayIndex = (int)orgStateTimer;
        readyTimer = orgStateTimer;
        selectingTimer = orgSelectingTimer;
        delayTimer = 1;
    }

    #region host

    public override void OnStartHost()
    {
        Debug.Log("✅ Host started");
    }

    public override void OnStopHost()
    {
        Debug.Log("🛑 Host stopped");
    }

    #endregion

    #region server

    public override void OnStartServer()
    {
        AddEvent();
        Debug.Log("✅ Server started");
    }

    public override void OnStopServer()
    {
        ClearPlayers();
        RemoveEvent();
        Debug.Log("🛑 Server stopped");
    }

    #endregion

    #region 客户端连接

    public override void OnServerConnect(int connectionId)
    {
        Debug.Log($"📡 Client {connectionId} connected to server");
    }

    public override void OnServerReady(int connectionId)
    {
        Debug.Log($"✅ Client {connectionId} is ready");
    }


    public override void OnServerDisconnect(int connectionId)
    {
        Debug.Log($"❌ Client {connectionId} disconnected from server");
        RemovePlayer(connectionId);
    }

    public override void OnServerAddPlayer(int connectionId)
    {
        Debug.Log($"Player {connectionId} added");
    }

    #endregion

    #endregion

    #region 游戏外业务:登录，登出

    private IResponse OnLoginRequest(LoginRequest req, int connectionId)
    {
        Debug.Log($"Player {req.playerName}");

        PlayerData playerDataTemp = null;
        LoginResponse res = new LoginResponse
        {
            isSuccess = false,
            playerData = null
        };
        if (ServerDataPlugin.Instance.GetPlayerList().Count < playerMaxNum && gameState == GameState.Idle)
        {
            playerDataTemp = new PlayerData();
            playerDataTemp.playerId = connectionId;
            playerDataTemp.playerName = req.playerName;
            ServerDataPlugin.Instance.AddPlayer(playerDataTemp);
            // 广播给所有客户端
            RefreshPlayerNotify();
            res.isSuccess = true;
            res.playerData = playerDataTemp;
        }

        return res;
    }

    private void RefreshPlayerNotify()
    {
        LoginNotify notify = new LoginNotify
        {
            playerDatas = ServerDataPlugin.Instance.GetPlayerList().ToList()
        };
        ServerMessageManager.Instance.SendNotify(notify);
    }

    private void RemovePlayer(int connectionId)
    {
        ServerDataPlugin.Instance.RemovePlayerById(connectionId);
        RefreshPlayerNotify();
        if (ServerDataPlugin.Instance.GetPlayerList().Count <= 1)
        {
            OnGameEndNotify();
        }
    }

    private void ClearPlayers()
    {
        ServerDataPlugin.Instance.RemoveAllPlayers();
        RefreshPlayerNotify();
    }

    #endregion

    #region 游戏业务

    #region 通用模块

    private void OnFlyTextNotify(string txt, FlyTextType flyType, List<int> elsePlayers = null)
    {
        FlyTextNotify notify = new FlyTextNotify();
        notify.txt = txt;
        notify.elsePlayers = elsePlayers;
        notify.flyType = flyType;
        ServerMessageManager.Instance.SendNotify(notify);
    }

    #endregion

    #region 游戏生命周期

    private IResponse OnGameReadyRequest(GameStartRequest arg1, int arg2)
    {
        if (gameState != GameState.Idle) return null;
        var notify = new GameStartNotify();
        notify.isSuccess = true;
        ServerMessageManager.Instance.SendNotify(notify);
        OnSelectHostPlayer();

        return null;
    }


    private void OnGameStart()
    {
        gameState = GameState.Playing;
        OnFlyTextNotify("Go!", FlyTextType.Normal);
        ServerDataPlugin.Instance.OnGameReStart();
        ServerDataPlugin.Instance.SetRandomPattern();
        stageQueue.Clear();
        var stages = ServerDataPlugin.Instance.CurrentPattern.stages;
        for (var i = 0; i < stages.Count; i++)
        {
            var start = stages[i].startTime;
            var end = stages[i].endTime;
            var randomDropTime = UnityEngine.Random.Range(start, end);

            stages[i].randomTime = randomDropTime;
            stageQueue.Enqueue(stages[i]);
        }
        
        RefreshAllPlayerProperty();
    }

    public void OnGameEndNotify()
    {
        gameState = GameState.Idle;

        GameEndNotify notify = OnFinishUseRule();
        ServerMessageManager.Instance.SendNotify(notify);
    }

    #endregion

    #region 游戏gamePlay逻辑

    #region 规则系统

    /// <summary>
    /// 游戏开始选择规则制定者
    /// </summary>
    private void OnSelectHostPlayer()
    {
        //todo:选择主玩家，给主玩家发送可选项，制定游戏规则
        var playerId = ServerDataPlugin.Instance.GetRandomPlayer();
        ServerDataPlugin.Instance.SetRulePlayerId(playerId);
        var rules= ServerDataPlugin.Instance.getRandomRules(3);
        RuleSelectNotify notify = new RuleSelectNotify
        {
            rules = rules,
            playerId = playerId
        };
        ServerMessageManager.Instance.SendNotify(notify);
        gameState = GameState.Selecting;
    }

    /// <summary>
    /// 接收玩家选择的rule
    /// </summary>
    /// <param name="param"></param>
    private IResponse OnMainPlayerRuleSelectRequest(MainPlayerRuleSelectRequest  param,int playerId)
    {
        if (gameState == GameState.Selecting)
        {
            int id = param.ruleId;
            ServerDataPlugin.Instance.SetCurrentRule(id);
            gameState = GameState.Ready;
        }
 
        return null;
    }

    public void SetRandomRule()
    {
        ServerDataPlugin.Instance.SetRandomRule();
        gameState = GameState.Ready;
    }

    /// <summary>
    /// 规则结算
    /// </summary>
    /// <returns></returns>
    private GameEndNotify OnFinishUseRule()
    {
        var notify = new GameEndNotify();
        var rule = ServerDataPlugin.Instance.CurrentRule;
        notify.rule = rule;
        if (rule.ruleId == 1)
        {
            //todo:读取数据，根据规则发放数据
        }
        Debug.Log("结算时规则："+rule.roleName);

        return notify;
    }

    #endregion

    #region 基础玩法

    /// <summary>
    /// 刷新所有玩家的属性
    /// </summary>
    private void RefreshAllPlayerProperty()
    {
        var tempList = ServerDataPlugin.Instance.GetPlayerList();
        foreach (var player in tempList)
        {
            player.ClearProperty();
            player.RefreshPlayerProperty();
        }
    }

    /// <summary>
    /// 广播生成食物
    /// </summary>
    private void GenerateFoods(FoodDropStage stage)
    {
        List<FoodData> foods = new List<FoodData>();
        for (int i = 0; i < stage.dropCount; i++)
        {
            Quality qualityRandom = ServerDataPlugin.Instance.RandomFood(stage);
            var food = new FoodData();
            food.foodId = FoodData.idIndex++;
            food.position = new Vector3(Random.Range(-0.5f, 0.5f), 0.8f, Random.Range(-0.5f, 0.5f));
            food.quality = qualityRandom;
            food.Init();
            ServerDataPlugin.Instance.AddFood(food);
            foods.Add(food);
        }

        FoodNotify notify = new FoodNotify();
        notify.foodList = foods;
        ServerMessageManager.Instance.SendNotify(notify);
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

    /// <summary>
    /// 抓取食物的请求
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <returns></returns>
    private IResponse OnCatchFoodRequest(CatchFoodRequest arg1, int arg2)
    {
        if (ServerDataPlugin.Instance.CheckHaveFood(arg1.foodId))
        {
            if (ServerDataPlugin.Instance.CheckHavePlayer(arg1.playerId))
            {
                var food = ServerDataPlugin.Instance.GetFoodById(arg1.foodId);
                food.StartCatch(ServerDataPlugin.Instance.GetPlayerById(arg1.playerId));
            }
        }

        return null;
    }

    /// <summary>
    /// 键盘输入抢夺
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <returns></returns>
    private IResponse OnLootingInputRequest(LootingInputRequest arg1, int arg2)
    {
        var player = ServerDataPlugin.Instance.GetPlayerById(arg1.playerId);
        if (player.AddLoot())
        {
            var food = ServerDataPlugin.Instance.GetFoodById(player.useFoodId);
            var proDic = food.GetProgress();
            LootingInputNotify notify = new LootingInputNotify();
            notify.playerProgress = proDic;
            ServerMessageManager.Instance.SendNotify(notify);
            OnFlyTextNotify("!Space!", FlyTextType.Quick);
        }

        return null;
    }

    #endregion

    #endregion

    #endregion
    
    public enum GameState
    {
        Idle, //未开始
        Selecting, //等待选择规则
        Ready, //ready
        Playing,//游戏开始
    }
}