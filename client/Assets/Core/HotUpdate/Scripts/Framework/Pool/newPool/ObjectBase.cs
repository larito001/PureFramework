using System;
using System.Collections;
using System.Collections.Generic;
using HotUpdate.Scripts.Framework.Pool.newPool;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

/// <summary>
/// 外城物件基类
/// 初始化调用逻辑
/// 1、从对象池获取对象，并通过SetData函数传入数据
/// 2、派生类中SetData函数中设置数据函数：SetPrefabBundlePath、SetBoxSize、SetOffsetY、SetInVision
/// 3、丢入 DrawObjectCmd 池中执行 ->InstanceGObj ->loadPrefab ->OnPrefabLoadFinished ->OnPrefabReadyUse ->OnInstanceHud ->AfterInstanceGObj
///                                              ->OnPrefabReadyUse ->AfterInstanceGObj
/// 回收调用逻辑
/// 外部回收借口调用 ->RecoverObject ->BeforeRecover ->RecoverHud ->RecoverObjTrans ->AfterRecover
/// </summary>
public abstract class ObjectBase:BaseEntity
{
    #region priavte私有
    private string prefabPath;
    private bool isInVision = false;
    private bool isDrawed = false;
    protected Transform objTrans;
    private ObjectPool.PoolBuffer poolBuffer;
    private Vector3 location;
    private Quaternion rotation;
    private Transform parent;
    private bool isRecover;

    private void loadPrefab()
    {
        poolBuffer.AsyncLoadAndGetItem(OnPrefabLoadFinished);
    }

    private void OnPrefabLoadFinished(GameObject origin, string name, bool isNew)
    {
        if (origin == null)
        {
            return;
        }

        if (!isInVision)
        {
            return;
        }

        if (isRecover)
        {
            objTrans = origin.transform;
            RecoverObjTrans();
            return;
        }

        objTrans = origin.transform;
        objTrans.gameObject.SetActive(true);

        OnPrefabReadyUse(objTrans);
    }

    private void OnPrefabReadyUse(Transform trans)
    {
        trans.position = Location;
        trans.rotation = Rotation;
        trans.SetParent(Parent);
        AfterInstanceGObj();
        
    }

    private void RecoverObjTrans()
    {
        if (poolBuffer != null)
        {
            if (objTrans != null)
            {
                poolBuffer.RecoverItem(objTrans.gameObject);
            }

            poolBuffer.RemoveGetItemCompleteCallback(OnPrefabLoadFinished);
            poolBuffer = null;
            objTrans = null;
        }
        else
        {
            if (objTrans != null)
            {
                Object.Destroy(objTrans.gameObject);
                objTrans = null;
            }
        }
    }

    

    #endregion

    #region  public外部调用

    public Transform ObjTrans
    {
        get { return objTrans; }
    }
    public virtual Vector3 Location
    {
        get { return location; }

        set { location = value; }
    }
    public virtual Quaternion Rotation
    {
        get { return rotation; }

        set { rotation = value; }
    }
    public virtual Transform Parent
    {
        get { return parent; }

        set { parent = value; }
    }
    /// <summary>
    /// 设置预制物路径
    /// </summary>
    /// <param name="resName"></param>
    /// <param name="hashcode"></param>
    protected void SetPrefabBundlePath(string prefabPath)
    {
        this.prefabPath = prefabPath;
    }

    /// <summary>
    /// 设置是否在显示范围内
    /// </summary>
    /// <param name="isInVision"></param>
    public void SetInVision(bool isInVision)
    {
        this.isInVision = isInVision;
    }
    /// <summary>
    /// 实例化对象
    /// </summary>
    public void InstanceGObj()
    {
        isRecover = false;
        if (isInVision && !string.IsNullOrEmpty(prefabPath) && isDrawed == false)
        {
            if (poolBuffer == null)
            {
                var loopCheckCdTime = GetPoolBufferLoopCheckCdTime();
                var inactiveTimeMax = GetPoolBufferInactiveTimeMax();
                var perFrameDisposeCountMax = GetPoolBufferPerFrameDisposeCountMax();
                var poolSizeMax = GetPoolBufferPoolSizeMax();
                poolBuffer = ObjectPool.Instance.GetBuffer(prefabPath, loopCheckCdTime, inactiveTimeMax,
                    perFrameDisposeCountMax, poolSizeMax);
            }

            loadPrefab();
            isDrawed = true;
        }
    }
    /// <summary>
    /// 回收
    /// </summary>
    public void RecoverObject(bool isDelete = false)
    {
        BeforeRecover(isDelete);
        RecoverObjTrans();

        isInVision = false;
        isDrawed = false;
        isRecover = true;
    }

    #endregion
    
    #region  Virtual待实现

    /// <summary>
    /// 对象池内对象卸载，多久检测一次。
    /// </summary>
    /// <returns></returns>
    protected virtual float GetPoolBufferLoopCheckCdTime()
    {
        return 0.5f;
    }

    /// <summary>
    /// 对象池内对象超过多久未使用，则卸载。
    /// </summary>
    /// <returns></returns>
    protected virtual uint GetPoolBufferInactiveTimeMax()
    {
        return 5;
    }

    /// <summary>
    /// 对象池检测一次，最对卸载多少个对象。
    /// </summary>
    /// <returns></returns>
    protected virtual uint GetPoolBufferPerFrameDisposeCountMax()
    {
        return 10;
    }

    /// <summary>
    /// 对象池内对象缓存最大数量
    /// </summary>
    /// <returns></returns>
    protected virtual uint GetPoolBufferPoolSizeMax()
    {
        return 30;
    }


    /// <summary>
    /// 实例化GObj之后调用
    /// </summary>
    protected abstract void AfterInstanceGObj();

    /// <summary>
    /// 回收之前调用
    /// </summary>
    protected virtual void BeforeRecover(bool isDelete)
    {
    }


    #endregion
    
}