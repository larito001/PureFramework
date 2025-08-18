using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YOTO;

public class GameDataManager 
{
    Dictionary<string ,IDataContainerBase> m_DataContainers = new Dictionary<string,IDataContainerBase>();

    public void Init()
    {
    }

    public void Unload()
    {
        m_DataContainers.Clear();
    }
    public void SaveAllData()
    {
        foreach (var mDataContainer in m_DataContainers)
        {
            mDataContainer.Value.Save();
        }
    }
    public void SaveDataByKey(string key)
    {
        if (m_DataContainers.ContainsKey(key))
        {
            m_DataContainers[key].Save();
        }
    }
    public void InitData<T>() where T : IDataContainerBase, new() 
    {
        var instance = new T();
   
        m_DataContainers.Add(instance.SaveKey,instance);
        instance.Load();
        
    }
}