using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SceneResType
{
    Wood=0,
    Iron,
}
public class SceneResManager : Singleton<SceneResManager>
{
 
    Dictionary<int, SceneResEntity> resList = new Dictionary<int, SceneResEntity>();

    public void Init()
    {
        var org = GameObject.Find("SceneResPos");
        for (int i = 0; i < 20; i++)
        {
            // 在 50 米半径内随机生成位置
            Vector3 randomOffset = Random.insideUnitSphere * 50f;
            randomOffset.y = 0; // 如果不需要 Y 轴变化（保持在地面）
    
            Vector3 spawnPos = org.transform.position + randomOffset;
    
            // 随机选择资源类型（Wood 或 Iron）
            SceneResType randomResType = Random.Range(0, 2) == 0 ? SceneResType.Wood : SceneResType.Iron;
    
            GenerateResAt(randomResType, spawnPos);
        }
    }
    public void GenerateResAt(SceneResType resType,Vector3 pos)
    {
        SceneResEntity res=null;
        if (resType == SceneResType.Wood)
        {
            res=WoodResEntity.pool.GetItem(pos);
        }else if (resType == SceneResType.Iron)
        {
            res=IronResEntity.pool.GetItem(pos);
        }

        if (res != null)
        {
            res.Location = pos;
            res.InstanceGObj();
            resList.Add(res._entityID, res); 
        }

    }

    public SceneResEntity GetResById(int id)
    {
        if (resList.ContainsKey(id))
        {
            return resList[id];
        }

        return null;
    }

    public override void Unload()
    {
        base.Unload();
        foreach (var sceneResEntity in resList)
        {
            sceneResEntity.Value.Free();
        }
    }
}
