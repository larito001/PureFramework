using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicPluginBase
{
      
      public virtual void Init()
      {
            OnInstall();
      }

      public virtual void Release()
      {
            OnUninstall();
      }


      protected virtual void OnInstall()
      {
      }

      protected virtual void OnUninstall()
      {
      }
}
public class PluginManager 
{
      private List<LogicPluginBase> mPlugins = new List<LogicPluginBase>();

      public void InitPlugins()
      {
      }
      public void InstallPlugin<T>() where T : LogicPluginBase, new()
      {
            var plugin = new T();
            mPlugins.Add(plugin);
            plugin.Init();
      }
      public  void Unload()
      {
            for (var i = 0; i < mPlugins.Count; i++)
            {
                  mPlugins[i].Release();
            }
      }
}
