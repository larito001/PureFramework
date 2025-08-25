using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlugin : LogicPluginBase
{
    public static PlayerPlugin Instance;
    private List<PlayerEntity> players = new List<PlayerEntity>();
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

    public void GeneratePlayers(List<PlayerData> playerDatas)
    {
        for (var i = 0; i < playerDatas.Count; i++)
        {
            // playerDatas[i]
            
            var p= PlayerEntity.pool.GetItem(playerDatas[i]);
            p.Location = new Vector3();
            p.InstanceGObj();
            players.Add(p);
        }

    }

    public void RemoveAllPlayers()
    {
        
    }
    
}
