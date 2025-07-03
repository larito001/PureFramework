
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProjectDawn.Navigation.Hybrid;
using UnityEngine;
using YOTO;

public class EnemyManager : SingletonMono<EnemyManager>
{

    private Dictionary<int, ZombieEntity> zombieEntities = new Dictionary<int, ZombieEntity>();//僵尸id，僵尸实体
    private Dictionary<int ,List<ZombieEntity>> zombieAreaList= new Dictionary<int , List<ZombieEntity>>();//地区id，地区内的僵尸列表
    private Dictionary<int ,int> zombieAreaDic= new Dictionary<int ,int>();//僵尸id，地区id
    private int num = 0;
    private List<EnemyRangeTrigger> triggers; 
    public void Init()
    {
        zombieEntities.Clear();
        var poss = GameObject.Find("EnemyOrgPos");
        triggers = poss.GetComponentsInChildren<EnemyRangeTrigger>().ToList();
        for (var i = 0; i < triggers.Count; i++)
        {
            triggers[i].Init(i);
            triggers[i].SetRange(50);
            var list = new List<ZombieEntity>();
            zombieAreaList.Add(i,list);
            
            var pos = triggers[i].transform.position;
            for (int j = 0; j < 3; j++)
            {
                ZombieEntity zombieEntity=  ZombieEntity.pool.GetItem(Vector3.zero);
                zombieEntity.InstanceGObj();
                zombieEntity.SetGroup(triggers[i].GetComponent<CrowdGroupAuthoring>());
                // 设置位置，假设僵尸在地面上（y轴为0）
                zombieEntity.Location=pos;
                zombieEntity.SetTarget(triggers[i].transform);
                zombieEntities.Add(zombieEntity._entityID, zombieEntity);
                list.Add(zombieEntity);
                zombieAreaDic.Add(zombieEntity._entityID,i);
            }
        }
        
    }

    public override void Unload()
    {
        foreach (var e in zombieEntities)
        {
            e.Value.Free();
        }
        zombieEntities.Clear();
        zombieAreaList.Clear();
        zombieAreaDic.Clear();
        base.Unload();
    }
    
    public void TriggerEnmeies(int id,Transform target)
    {
        if (zombieAreaList.ContainsKey(id))
        {
            for (var i = 0; i < zombieAreaList[id].Count; i++)
            {
                zombieAreaList[id][i].SetTarget(target);
            }
        }
    }

    public void ExitEnmeies(int id)
    {
        if (zombieAreaList.ContainsKey(id))
        {
            for (var i = 0; i < zombieAreaList[id].Count; i++)
            {
                zombieAreaList[id][i].SetTarget(null);
            }
        }
    }

    public void Hurt(int id, float hurt)
    {
        if (zombieEntities.ContainsKey(id))
        {
            // YOTOFramework.poolMgr.GetObjectPool(ObjectPoolType.NormalZombieEntity).Set(zombieEntities[id]);
            zombieEntities[id].Hurt(hurt);
            
        }
    }

    public ZombieEntity GetRecentEnemy(Vector3 pos)
    {
        ZombieEntity res=null;
        float distance = 0;
        foreach (var value in zombieEntities.Values)
        {
            if (res==null)
            {
                res = value;
            }
            else 
            {
               var dis= (res.zombieBase.transform.position -value.zombieBase.transform.position).magnitude;
               if (distance > dis)
               {
                   res = value;
                   distance = dis;
               }
            }
        }

        return res;
    }

    public bool  CheckZombieAlive(int id)
    {
       return zombieEntities.ContainsKey(id);
    }
    public void RemoveZombie(int id)
    {
        if (zombieEntities.ContainsKey(id))
        {
            zombieEntities.Remove(id);
            if (zombieAreaDic.ContainsKey(id))
            {
                var area = zombieAreaDic[id];
                for (var i = zombieAreaList[area].Count-1; i >0; i--)
                {
                    if (zombieAreaList[area][i]._entityID == id)
                    {
                        zombieAreaList[area].RemoveAt(i);
                    }
                }
                zombieAreaDic.Remove(id);
            }
        }
    }
    
    
}
