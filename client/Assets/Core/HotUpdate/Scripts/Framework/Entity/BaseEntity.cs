using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using YOTO;

public abstract  class BaseEntity 
{
    private static int  ids=0;
    public int  _entityID;
    private bool _isLoaded=false;
    public bool IsLoaded
    {
       get { return _isLoaded; } 
    }
    public  BaseEntity()
    {
        Init();
        _isLoaded = true;
    }

    public void Init()
    {
        if (_isLoaded) return;
        _entityID=ids++;
        YOTOOnload();
        _isLoaded = true;
        YOTOFramework.entityMgr._AddEntity(this);
    }
    protected abstract void YOTOOnload();
    public abstract void YOTOStart();
    public abstract void YOTOUpdate(float deltaTime);
    public abstract void YOTONetUpdate();
    public abstract void YOTOFixedUpdate(float deltaTime);
    public abstract void YOTOOnHide();

    public virtual void Free()
    {
        YOTOOnHide();
        _isLoaded = false;
        YOTOFramework.entityMgr._RemoveEntity(this);
    }

    public void RemoveThis()
    {
        YOTOOnHide();
        _isLoaded = false;
        YOTOFramework.entityMgr._RemoveEntity(this);
    }


}
