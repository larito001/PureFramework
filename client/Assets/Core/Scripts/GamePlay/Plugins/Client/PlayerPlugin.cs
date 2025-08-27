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
        ClientMessageManager.Instance.RegisterResponseHandler<InputNotify>(OnInputNotify);
        ClientMessageManager.Instance.RegisterResponseHandler<InputResponse>(OnInputResponse);
    
    }

    private void OnInputResponse(InputResponse obj)
    {
        
    }

    private void OnInputNotify(InputNotify obj)
    {
        // players.RotatePlayer();
        if (LoginPlugin.Instance.PlayerId==obj.playerId)
        {
            YOTOFramework.sceneMgr.cameraCtrl.lookInput=obj.input;
        }
        if(players.ContainsKey(obj.playerId))
        players[obj.playerId].RoatePlayer(obj.input);
    }

    public void OnNetUninstall()
    {
        ClientMessageManager.Instance.UnRegisterResponseHandler<InputNotify>();
        ClientMessageManager.Instance.UnRegisterResponseHandler<InputResponse>();
    }
    public void RotatePlayerRequest(Vector2  ipt)
    {
        var pid = LoginPlugin.Instance.PlayerId;
        var mgr = ClientMessageManager.Instance;
        Debug.Log("RotateRequest");
        mgr.SendRequest(new InputRequest()
        {
          playerId =pid,
          input = ipt
        });
    }

    private void RatatePlayer(int pid)
    {
        //
        // players.Rotate();
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
