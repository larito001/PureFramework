using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

/// <summary>
/// �������롢����ʵ�塣����ʵ���update�ȷ��������Կ��Ƶ���ʵ���ʱ������
/// </summary>
public class EntityMgr
{
    Dictionary<int,BaseEntity> entities = new Dictionary<int,BaseEntity>();
    List<BaseEntity> baseEntities = new List<BaseEntity>();
    private float UpdateTime=0.1f;
    private float currentUpdateTime=0;
    int i;
    public void Init()
    {
        entities.Clear();
        baseEntities.Clear();
    }

    public void  _AddEntity(BaseEntity baseEntity)
    {
        entities[baseEntity._entityID]=baseEntity;
        baseEntities.Add(baseEntity);
    }
    public void _RemoveEntity(BaseEntity baseEntity) {
        if (entities.ContainsKey(baseEntity._entityID))
        {
            entities.Remove(baseEntity._entityID);
            baseEntities.Remove(baseEntity); 
        }
   
    }

    public void _Update(float deltaTime)
    {
        currentUpdateTime+=deltaTime;
        while (currentUpdateTime>=UpdateTime)
        {
            currentUpdateTime -= UpdateTime;
            NetUpdate();
        }
        for (i = 0; i < baseEntities.Count; i++)
        {
            if (baseEntities[i].IsLoaded)
            {
                baseEntities[i].YOTOUpdate(deltaTime);
            }
         
        }
    }

    private void NetUpdate()
    {
        for (i = 0; i < baseEntities.Count; i++)
        {
            if (baseEntities[i].IsLoaded)
            {
                baseEntities[i].YOTONetUpdate();
            }
         
        }
    }
    public void _FixedUpdate(float deltaTime)
    {
        for (i = 0; i < baseEntities.Count; i++)
        {
            if (baseEntities[i].IsLoaded)
            {
                baseEntities[i].YOTOFixedUpdate(deltaTime);
            }
        }
    }
}
