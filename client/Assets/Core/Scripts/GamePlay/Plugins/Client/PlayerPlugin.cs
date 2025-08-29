using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YOTO;

public class PlayerPlugin : LogicPluginBase
{
    public static PlayerPlugin Instance;
    private Dictionary<int,PlayerEntity> players = new Dictionary<int,PlayerEntity>();
    public PlayerPlugin()
    {
        Instance = this;
    }
    protected override void OnInstall()
    {
        base.OnInstall();
    }
    
    protected override void OnUninstall()
    {
        base.OnUninstall();
    }
    public void OnNetInstall()
    {
        ClientMessageManager.Instance.RegisterResponseHandler<HeadPosNotify>(OnHeadPosNotify);
        ClientMessageManager.Instance.RegisterResponseHandler<HeadPosResponse>(OnHeadPosResponse);
    
    }

    private void OnHeadPosResponse(HeadPosResponse obj)
    {
        
    }

    private void OnHeadPosNotify(HeadPosNotify obj)
    {
        // players.RotatePlayer();
        if (LoginPlugin.Instance.PlayerId!=obj.playerId)
        {
            //todo:旋转眼球
            if (players.ContainsKey(obj.playerId))
            {
                Debug.Log("Player:"+obj.playerId+" 旋转眼球"+obj.pos);
                players[obj.playerId].SetEyesMove(obj.pos);
            }
      
        }
    }

    public void RotateSelfPlayerEyes(Vector2 input)
    {
        if (players.ContainsKey(LoginPlugin.Instance.PlayerId))
        {
            players[LoginPlugin.Instance.PlayerId].SetEyesMove(input); 
        }
    
    }
    public void OnNetUninstall()
    {
        ClientMessageManager.Instance.UnRegisterResponseHandler<HeadPosNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<HeadPosResponse>();
    }
    public void RotatePlayerRequest(Vector3  pos)
    {
        var pid = LoginPlugin.Instance.PlayerId;
        var mgr = ClientMessageManager.Instance;
        Debug.Log("RotateRequest");
        mgr.SendRequest(new HeadPosRequest()
        {
          playerId =pid,
          pos = pos
        });
    }
    
    public void GeneratePlayers(List<PlayerData> playerDatas)
    {
      
        for (var i = 0; i < playerDatas.Count; i++)
        {
            // playerDatas[i]
           var pp = GameObject.Find("p"+(i+1).ToString());
           if (pp != null)
           {
               var p= PlayerEntity.pool.GetItem(playerDatas[i]);
               p.Location = pp.transform.position;
               p.InstanceGObj();
               players.Add(playerDatas[i].playerId,p);
           }
          
        }

    }
    
    public void RemoveAllPlayers()
    {
        
    }
    
}
