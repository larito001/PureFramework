
using System.Collections.Generic;


public class StagePlugin : LogicPluginBase
{
    public static StagePlugin Instance;
    public StagePlugin()
    {
        Instance = this;
    }
    private int playerId=-1;
    private List<PlayerData> players = new List<PlayerData>();
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
    }
    public void OnNetUninstall()
    {

    }
    public void OnGameStart(int playerId,List<PlayerData> playerDatas)
    {
        this.playerId=playerId;
        players=playerDatas;
        PlayerPlugin.Instance.GeneratePlayers(players);
    }

    public void OnGameEnd()
    {
        
    }
}
