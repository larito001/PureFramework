using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameState
{
    Room, //未开始
    Rest, //中场休息
    Selecting, //等待选择规则
    Ready, //ready
    Playing, //游戏开始
    Voting, //正在投票
    End,
}

public class StateInfo
{
    public GameState State;
    public float stateOrgTime = 10;
    public float stateCurrentTime = 10;
    public int secondIndex = 0;
    public float timer = 0;

    public StateInfo(GameState state, float orgTime)
    {
        State = state;
        stateOrgTime = orgTime;
        stateCurrentTime = orgTime + 0.001f;
        timer = 0;
        secondIndex = (int)stateOrgTime;
    }

    public void ResetTimer()
    {
        stateCurrentTime = stateOrgTime;
        secondIndex = (int)stateOrgTime;
        timer = 0;
    }

    public void SetSecondsCallBack(UnityAction<int> secondsCallBack)
    {
        _secondsCallBack = secondsCallBack;
    }

    UnityAction<int> _secondsCallBack;
}

public class GameServerStateCtrl
{
    private const float orgGameTime = 20; //playing总时长
    private const float orgReadyTimer = 2; //ready倒计时
    private const float orgSelectingTimer = 5; //选择规则时间
    private const float orgvotingTimer = 3; //投票时间
    private const float orgRestTImer = 8; //休息时间
    private const float orgEndTImer = 13; //结算时间

    public UnityAction<StateInfo> OnStateEnd;
    public UnityAction<StateInfo> OnStateStart;
    public UnityAction<StateInfo, int> OnStateUpdate;
    private Stack<StateInfo> _stateStack = new Stack<StateInfo>();
    private int GameIndex = 0; //几轮

    public bool GameIsStart()
    {
        if (_stateStack.Count > 0)
        {
            return _stateStack.Peek().State != GameState.Room;
        }

        return false;
    }

    public void Update(float dt)
    {
        if (_stateStack.Count == 0) return;
        var state = _stateStack.Peek();
        if (state.State == GameState.Room) return;
        var currentTime = state.stateCurrentTime;
        if (currentTime >= 0)
        {
            state.timer += dt;
            if (state.timer >= 1)
            {
                state.timer -= 1;
                state.secondIndex--;
                OnStateUpdate?.Invoke(state,state.secondIndex);
            }

            state.stateCurrentTime -= dt;
        }
        else
        {
            NextState();
        }
    }

    private void NextState()
    {
        if (_stateStack.Count == 0) return;
        var current = _stateStack.Pop();
        current.ResetTimer();
        OnStateEnd?.Invoke(current);

        if (_stateStack.Count == 0) return;
        var next = _stateStack.Peek();
        if (!(next.stateOrgTime > next.stateCurrentTime))
        {
            OnStateStart?.Invoke(next);
        }
    }

    public void OnJoinRoom()
    {
        //room 
        RePush();
    }

    public void ReStartLevel()
    {
        var state = new StateInfo(GameState.Playing, orgGameTime);
        _stateStack.Push(state);
        state = new StateInfo(GameState.Ready, orgReadyTimer);
        _stateStack.Push(state);
        state = new StateInfo(GameState.Selecting, orgSelectingTimer);
        _stateStack.Push(state);
        state = new StateInfo(GameState.Rest, orgRestTImer);
        _stateStack.Push(state);
    }

    public void OnGameStart()
    {
        ForcePopCurrentState(GameState.Room);
    }

    private void RePush()
    {
        _stateStack.Clear();
        var state = new StateInfo(GameState.Playing, orgGameTime);
        _stateStack.Push(state);
        state = new StateInfo(GameState.Ready, orgReadyTimer);
        _stateStack.Push(state);
        state = new StateInfo(GameState.Selecting, orgSelectingTimer);
        _stateStack.Push(state);
        state = new StateInfo(GameState.Room, -1);
        _stateStack.Push(state);
    }

    public void ForcePopCurrentState(GameState state)
    {
        if (_stateStack.Count == 0) return;
        var topState = _stateStack.Peek();
        if (topState.State == state)
        {
            NextState();
        }
    }

    public void StartVoting()
    {
        var state = new StateInfo(GameState.Voting, orgvotingTimer);
        _stateStack.Push(state);
        OnStateStart?.Invoke(state);
    }

    public void GameEnd()
    {
        var state = new StateInfo(GameState.End, orgEndTImer);
        _stateStack.Push(state);
        OnStateStart?.Invoke(state);
    }
}