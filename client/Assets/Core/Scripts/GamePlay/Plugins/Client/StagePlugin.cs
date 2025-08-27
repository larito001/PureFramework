
using System.Collections.Generic;
using YOTO;


public class StagePlugin : LogicPluginBase
{
    public static StagePlugin Instance;
    public StagePlugin()
    {
        Instance = this;
    }
    private int playerId=-1;
    private List<PlayerData> players = new List<PlayerData>();
    public bool GameStart = false;
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
        GameStart = false;
    }
    public void OnNetUninstall()
    {
        GameStart = false;
    }
    public void OnGameStart(int playerId,List<PlayerData> playerDatas)
    {
        GameStart = true;
        YOTOFramework.uIMgr.ClearUI();
        this.playerId=playerId;
        players=playerDatas;
        PlayerPlugin.Instance.GeneratePlayers(players);
    }

    public void OnGameEnd()
    {
        
    }
}
