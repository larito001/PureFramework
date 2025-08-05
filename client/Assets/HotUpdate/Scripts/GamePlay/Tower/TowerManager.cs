using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerManager : Singleton<TowerManager>
{
    Dictionary<int, TowerBaseEntity> towers = new Dictionary<int, TowerBaseEntity>();
    public void Init()
    {
        // var trainPos = GameObject.Find("BasePos");
        // var list = trainPos.GetComponentsInChildren<Transform>().ToList();
        // for (int i = 0; i < list.Count; i++)
        // {
        //     GenerateTowerBase(list[i].position); 
        // }
    }
    public ObjectBase GenerateTowerBase(Vector3 pos)
    {
        var tower = TowerBaseEntity.pool.GetItem(Vector3.zero);
        tower.Location = pos;
        tower.InstanceGObj();
        towers.Add(tower._entityID, tower);
        return tower;
    }
    
    public TowerBaseEntity GetTowerById(int id)
    {
        if (towers.ContainsKey(id))
        {
            return towers[id];
        }

        return null;
    }

    public override void Unload()
    {
        foreach (var towerBaseEntity in towers)
        {
            towerBaseEntity.Value.Free();
        }

        base.Unload();
    }
}