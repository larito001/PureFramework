using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YOTO;
public struct ParticleEntityData
{
    public string path;
    public Vector3 pos;
}
public class ParticleEntity :  PoolItem<ParticleEntityData>
{
    public static DataObjPool<ParticleEntity, ParticleEntityData> pool =
        new DataObjPool<ParticleEntity, ParticleEntityData>("ParticleEntity", 4);

    private ParticleEntityData _data;
    private GameObject _obj;
    private bool needPlay = false;
    private bool loaded = false;
    public void AfterIntoObjectPool()
    {
        YOTOFramework.resMgr.ReleasePack(_data.path,_obj);
        _obj = null;
        loaded = false;
        needPlay = false;
    }

    public void SetData(ParticleEntityData data)
    {
        _data = data;
      YOTOFramework.resMgr.LoadGameObject(data.path, OnLoad);
      loaded = false;
    }

    private void OnLoad(GameObject  obj)
    {
        
        _obj = obj;
       obj.transform.position= _data.pos;
       if (needPlay)
       {
           PlayParticle();
       }

       loaded = true;
       needPlay = false;
    }

    private void PlayParticle()
    {
      //获取obj及其子节点的所有粒子，然后播放
      var list = _obj.GetComponentsInChildren<ParticleSystem>();
      foreach (var item in list)
      {
          item.Play();
      }
    }
    public void Play()
    {
        if (loaded)
        {
            PlayParticle();
        }
        else
        {
            needPlay = true;
        }

    }
}
