
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProjectDawn.Navigation.Hybrid;
using UnityEngine;
using YOTO;
using Random = UnityEngine.Random;

public class EnemyManager : SingletonMono<EnemyManager>
{

    private Dictionary<int, ZombieEntity> zombieEntities = new Dictionary<int, ZombieEntity>();//僵尸id，僵尸实体
    private Dictionary<int ,List<ZombieEntity>> zombieAreaList= new Dictionary<int , List<ZombieEntity>>();//地区id，地区内的僵尸列表
    private Dictionary<int ,int> zombieAreaDic= new Dictionary<int ,int>();//僵尸id，地区id
    private int num = 0;
    private List<EnemyRangeTrigger> triggers;
    private bool isInit = false;
    public void Init()
    {
        zombieEntities.Clear();
        var poss = GameObject.Find("EnemyOrgPos");
        triggers = poss.GetComponentsInChildren<EnemyRangeTrigger>().ToList();
        // 设置每个触发器的位置在poss的30米半径内随机分布
        foreach (var trigger in triggers)
        {
            // 生成随机方向并缩放到30米半径内的随机距离
            Vector3 randomOffset = Random.insideUnitSphere * 50f;
            // 保持y轴不变（如果需要）
            randomOffset.y = 0; // 如果不需要垂直方向的随机分布，可以去掉这行
    
            // 设置触发器的位置为原始位置加上随机偏移
            trigger.transform.position = poss.transform.position + randomOffset;
    
            // 如果需要触发器始终朝向中心点
            // trigger.transform.LookAt(poss.transform);
        }
        for (int i = 0; i < triggers.Count; i++)
        {
            triggers[i].Init(i);
            triggers[i].SetRange(50);
            zombieAreaList.Add(i, new List<ZombieEntity>());
        }
   

        isInit = true;
    }
    public void GenerateEnemy(int num)
    { 
        // 初始化 trigger 区域信息
     
        for (int i = 0; i < num; i++)
        {
            // 随机选择一个 trigger
            int triggerIndex = UnityEngine.Random.Range(0, triggers.Count);
            var trigger = triggers[triggerIndex];
            var list = zombieAreaList[triggerIndex];

            // 随机生成一个位置：基于 trigger 的位置，加一定范围内的偏移
            Vector3 basePos = trigger.transform.position;
            float range = 10f; // 控制分散半径
            Vector3 offset = new Vector3(
                UnityEngine.Random.Range(-range, range),
                0,
                UnityEngine.Random.Range(-range, range)
            );
            Vector3 spawnPos = basePos + offset;

            // 创建并初始化僵尸
            ZombieEntity zombieEntity = ZombieEntity.pool.GetItem(Vector3.zero);
            zombieEntity.Location = spawnPos;
            zombieEntity.InstanceGObj();
            zombieEntity.SetGroup(trigger.GetComponent<CrowdGroupAuthoring>());
  
            zombieEntity.SetTarget(null);
        
            zombieEntities.Add(zombieEntity._entityID, zombieEntity);
            list.Add(zombieEntity);
            zombieAreaDic.Add(zombieEntity._entityID, triggerIndex);
        }
    }
    private void FixedUpdate()
    {
        if (isInit)
        {
            if (zombieEntities.Count <= 0)
            {
                isInit = false;
                YOTOFramework.uIMgr.Show(UIEnum.FightingEndPanel);
            }
            
        }
    }

    public override void Unload()
    {
        isInit=false;
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
