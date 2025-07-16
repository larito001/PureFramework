using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMapPlugin : LogicPluginBase
{
  public static GameMapPlugin Instance;

  protected override void OnInstall()
  {
    base.OnInstall();
  }
  protected override void OnUninstall()
  {
    base.OnUninstall();
  }

  public int level;
  

  public GameMapPlugin()
  {
    Instance = this;
  }
}
