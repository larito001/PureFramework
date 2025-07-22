
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : Singleton<PlayerManager> {
    private PlayerEntity player;
    private TrainEntity _trainEntity;
    public void Init(Transform org)
    {
        player = new PlayerEntity();
        player.DontMove();
        _trainEntity = new TrainEntity();
        player.Init(org.position);
        
    }

    public void Start()
    {
        player.CanMove();
    }
    public PlayerEntity GetPlayer()
    {
        return player;
    }
    public Vector3 GetPlayerPos()
    {
        return player.GetPlayerPos();
    }
    public Transform GetPlayerTrans()
    {
        return player.GetPlayerTrans();
    }
    public Transform GetTrainTrans()
    {
        return _trainEntity.GetAttackPos();
    }
    public override void Unload()
    {
        base.Unload();
        _trainEntity?.Free();
        _trainEntity = null;
        player?.Free();
        player = null;
    }
}
