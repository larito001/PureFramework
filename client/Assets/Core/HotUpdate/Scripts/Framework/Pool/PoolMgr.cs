using System;
using System.Collections.Generic;
using UnityEngine;

namespace YOTO
{
    // public abstract class PoolBase
    // {
    //     protected int outerCount = 0; // 对象池外对象的数量
    //     protected bool enableAutoShrink = false; // 是否启用自动收缩
    //     protected int idleMaxCount = 5; // 池中最大闲置数量
    //     protected int frameShrinkCount = 1; // 单次收缩数量
    //     protected float shrinkInterval = 0; // 收缩间隔
    //
    //     protected float curTime = 0; // 计时器使用
    //     public int OuterCount => outerCount;
    //     public bool EnableAutoShrink => enableAutoShrink;
    //     public int IdleMaxCount => idleMaxCount;
    //     public int FrameShrinkCount => frameShrinkCount;
    //     public float ShrinkInterval => shrinkInterval;
    //
    //     public void Update(float dt)
    //     {
    //         if (EnableAutoShrink)
    //         {
    //             curTime += dt;
    //             if (curTime >= shrinkInterval)
    //             {
    //                 curTime = 0;
    //                 // 尝试收缩
    //                 //AutoShrink();
    //             }
    //         }
    //     }
    //
    //     protected abstract void AutoShrink();
    //
    //     /// <summary>
    //     /// 初始化
    //     /// </summary>
    //     /// <param name="enableAutoShrink">是否自动收缩</param>
    //     /// <param name="idleMaxCount">最大闲置数量</param>
    //     /// <param name="frameShrinkCount">单次收缩数量</param>
    //     /// <param name="shrinkInterval">收缩间隔，建议30-60</param>
    //     public void Init(bool enableAutoShrink, int idleMaxCount, int frameShrinkCount, float shrinkInterval)
    //     {
    //         this.enableAutoShrink = enableAutoShrink;
    //         this.idleMaxCount = idleMaxCount;
    //         this.frameShrinkCount = frameShrinkCount;
    //         this.shrinkInterval = shrinkInterval;
    //         this.outerCount = 0;
    //         this.curTime = 0;
    //     }
    // }
    //
    // public class GameObjectPool : PoolBase
    // {
    //     private PoolBaseGameObject prefab;
    //
    //     /// <summary>
    //     /// 对象队列
    //     /// </summary>
    //     private Queue<PoolBaseGameObject> pool = new Queue<PoolBaseGameObject>(); // 初始化队列
    //
    //     public void SetPrefab(PoolBaseGameObject prefab)
    //     {
    //         this.prefab = prefab;
    //     }
    //
    //     /// <summary>
    //     /// 取出对象
    //     /// </summary>
    //     /// <param name="count">数量</param>
    //     /// <param name="parent">父物体</param>
    //     /// <typeparam name="T">对象类型</typeparam>
    //     /// <returns></returns>
    //     public T[] OnFetch<T>(int count, Transform parent) where T : PoolBaseGameObject
    //     {
    //         var objs = new List<T>();
    //         curTime = 0;
    //         if (prefab == null)
    //         {
    //             throw new InvalidOperationException("没设置prefab.");
    //         }
    //         // 扩容池子
    //         while (pool.Count < count)
    //         {
    //             PoolBaseGameObject cloneObj = UnityEngine.Object.Instantiate(prefab, parent);
    //             cloneObj.__isRecycle = true;
    //             pool.Enqueue(cloneObj);
    //         }
    //
    //         // 取出对象并初始化
    //         for (int i = 0; i < count; i++)
    //         {
    //             var deItem = pool.Peek();
    //             if (deItem==null)
    //             {
    //                 Debug.LogError("取出对象为空");
    //                 break;
    //             }
    //             if (!deItem.__isRecycle)
    //             {
    //                 Debug.LogError("对象未被回收，尝试取出");
    //                 break;
    //             }
    //             deItem.transform.SetParent(parent);
    //             deItem.transform.eulerAngles = Vector3.zero;
    //             deItem.transform.localScale = Vector3.one;
    //             deItem.transform.localPosition = Vector3.zero;
    //             try
    //             {
    //                 deItem.OnStart();
    //                 deItem.gameObject.SetActive(true);
    //                 deItem.__isRecycle = false;
    //                 objs.Add(deItem.GetComponent<T>());
    //                 pool.Dequeue(); // 取出后将其移出队列
    //                 outerCount++;
    //             }
    //             catch (Exception ex)
    //             {
    //                 Debug.LogError("初始化对象时错误: " + ex);
    //             }
    //
    //
    //         }
    //
    //         return objs.ToArray();
    //     }
    //
    //     public T Get<T>(Transform parent=null) where T : PoolBaseGameObject
    //     {
    //         var getItem = OnFetch<T>(1, parent);
    //         if (getItem.Length > 0)
    //         {
    //             return getItem[0];
    //         }
    //
    //         return null;
    //     }
    //
    //     /// <summary>
    //     /// 回收对象
    //     /// </summary>
    //     /// <param name="objects">需要回收的对象</param>
    //     /// <typeparam name="T">对象类型</typeparam>
    //     public void Set<T>(params T[] objects) where T : PoolBaseGameObject
    //     {
    //         if (objects == null)
    //         {
    //             Debug.LogError("设置对象数组为空");
    //             return;
    //         }
    //
    //         foreach (var obj in objects)
    //         {
    //             if (obj == null)
    //             {
    //                 Debug.LogError("回收对象为空");
    //                 break;
    //             }
    //             if (obj.__isRecycle)
    //             {
    //                 Debug.LogError("尝试回收已经回收的对象");
    //                 break;
    //             }
    //             try
    //             {
    //                 obj.ResetAll();
    //                 //obj.gameObject.SetActive(false);
    //                 obj.transform.localPosition = Vector3.one * 9999f;
    //                 pool.Enqueue(obj);
    //                 obj.__isRecycle = true;
    //                 outerCount--;
    //             }
    //             catch (Exception ex)
    //             {
    //                 Debug.LogError("回收时错误: " + ex);
    //             }
    //
    //
    //
    //         }
    //
    //     }
    //
    //     /// <summary>
    //     /// 清理对象池
    //     /// </summary>
    //     public void ClearPool()
    //     {
    //         while (pool != null && pool.Count > 0)
    //         {
    //             var item = pool.Peek();
    //             if (item == null)
    //             {
    //                 Debug.LogError("回收空对象");
    //                 pool.Dequeue();
    //                 break;
    //             }
    //             if (!item.__isRecycle)
    //             {
    //                 Debug.Log("尝试回收未回收的对象");
    //                 break;
    //             }
    //             try
    //             {
    //                 item.ResetAll();
    //                 pool.Dequeue(); // 将队列中第一个对象移除
    //                 UnityEngine.Object.Destroy(item);
    //             }
    //             catch (Exception ex)
    //             {
    //                 Debug.LogError("删除时错误: " + ex);
    //             }
    //
    //
    //         }
    //     }
    //
    //     //todo:删除队列
    //     protected override void AutoShrink()
    //     {
    //         int ShrinkCount = frameShrinkCount;
    //         Debug.Log("回收gameObj");
    //         while (pool != null && pool.Count > idleMaxCount && ShrinkCount > 0)
    //         {
    //             var item = pool.Peek();
    //      
    //             if (item == null)
    //             {
    //                 Debug.LogError("回收空对象");
    //                 break;
    //             }
    //             if (!item.__isRecycle)
    //             {
    //                 Debug.Log("尝试回收未回收的对象");
    //                 break;
    //             }
    //             try
    //             {
    //                 Debug.Log("回收game" + item.name);
    //                 item.ResetAll();
    //                 pool.Dequeue(); // 将队列中第一个对象移除
    //                 UnityEngine.GameObject.Destroy(item.gameObject);
    //                 ShrinkCount--;
    //             }
    //             catch (Exception ex)
    //             {
    //                 Debug.LogError("删除时错误: " + ex);
    //             }
    //
    //
    //         }
    //     }
    // }
    //
    // public class ObjectPool : PoolBase
    // {
    //     private Queue<PoolBaseObject> pool = new Queue<PoolBaseObject>(); // 初始化队列
    //
    //     public T[] OnFetch<T>(int count) where T : PoolBaseObject, new()
    //     {
    //         var objs = new List<T>();
    //         curTime = 0;
    //         for (int i = 0; i < count; i++)
    //         {
    //             if (pool.Count > 0)
    //             {
    //                 T obj = (T)pool.Peek();
    //                 if (obj == null)
    //                 {
    //                     Debug.LogError("取出对象为空");
    //                     break;
    //                 }
    //                 if (!obj.__isRecycle)
    //                 {
    //                     Debug.LogError("错误，obj已被使用");
    //                     break;
    //                 }
    //
    //                 try
    //                 {
    //                     obj.OnStart();
    //                     obj.__isRecycle = false;
    //                     objs.Add(obj);
    //                     pool.Dequeue(); // 取出后将其移出队列
    //                     outerCount++;
    //                 }
    //                 catch (Exception e)
    //                 {
    //                     Debug.LogError("resetall时错误: " + e);
    //                 }
    //
    //             }
    //             else
    //             {
    //                 T obj = new T();
    //                 try
    //                 {
    //                     obj.OnStart();
    //                     obj.__isRecycle = false;
    //                     objs.Add(obj);
    //                     outerCount++;
    //                 }
    //                 catch (Exception e)
    //                 {
    //                     Debug.LogError("resetall时错误: " + e);
    //                 }
    //             }
    //         }
    //
    //       //  Debug.Log("拿出对象池");
    //         return objs.ToArray();
    //     }
    //
    //     public T Get<T>() where T : PoolBaseObject, new()
    //     {
    //         return OnFetch<T>(1)[0];
    //     }
    //
    //     public void Set<T>(params T[] objects) where T : PoolBaseObject, new()
    //     {
    //         if (objects == null)
    //         {
    //             return;
    //         }
    //
    //         foreach (var obj in objects)
    //         {
    //             if (obj == null)
    //             {
    //                 Debug.LogError("回收对象为空");
    //                 break;
    //             }
    //             if (obj.__isRecycle)
    //             {
    //                 Debug.LogError("对象已被回收");
    //                 break;
    //             }
    //
    //             try
    //             {
    //                 obj.ResetAll();
    //                 pool.Enqueue(obj);
    //                 obj.__isRecycle = true;
    //                 outerCount--;
    //               //  Debug.Log("存入对象池");
    //             }
    //             catch (Exception e)
    //             {
    //                 Debug.LogError("resetall时错误: " + e);
    //             }
    //
    //
    //         }
    //     }
    //
    //     public void ClearPool()
    //     {
    //         while (pool != null && pool.Count > 0)
    //         {
    //             var obj = pool.Peek();
    //             if (obj == null)
    //             {
    //                 Debug.LogError("回收对象为空");
    //                 break;
    //             }
    //             if (!obj.__isRecycle)
    //             {
    //                 Debug.LogError("尝试释放未被回收对象");
    //                 break;
    //             }
    //             try
    //             {
    //                 obj.ResetAll();
    //                 pool.Dequeue(); // 将队列中第一个对象移除
    //             }
    //             catch (Exception e)
    //             {
    //                 Debug.LogError("清空时尝试删除错误: " + e);
    //             }
    //
    //         }
    //     }
    //
    //     protected override void AutoShrink()
    //     {
    //         int ShrinkCount = frameShrinkCount;
    //
    //         while (pool != null && pool.Count > idleMaxCount && ShrinkCount > 0)
    //         {
    //
    //             var obj = pool.Peek();
    //             if (obj == null)
    //             {
    //                 Debug.Log("释放了空对象");
    //                 break;
    //             }
    //             if (!obj.__isRecycle)
    //             {
    //                 Debug.LogError("删除失败，请注意：请勿把未回收对象丢入对象池");
    //                 break;
    //             }
    //             try
    //             {
    //                 obj.ResetAll();
    //                 pool.Dequeue(); // 将队列中第一个对象移除
    //                 ShrinkCount--;
    //
    //
    //             }
    //             catch (Exception e)
    //             {
    //                 Debug.LogError("resetall时错误: " + e);
    //             }
    //         }
    //         Debug.Log("清理对象池" + pool.Count);
    //     }
    // }
    //
    // public class PoolMgr
    // {
    //     //private GameObjectPool gameObjectPool = new GameObjectPool();
    //     //private ObjectPool objectPool = new ObjectPool();
    //
    //     //public GameObjectPool GameObjectPool => gameObjectPool;
    //     //public ObjectPool ObjectPool => objectPool;
    //
    //     Dictionary<GameObjectPoolType, GameObjectPool> gameObjectPoolDic = new Dictionary<GameObjectPoolType, GameObjectPool>();
    //     Dictionary<ObjectPoolType, ObjectPool> objectPoolDic = new Dictionary<ObjectPoolType, ObjectPool>();
    //
    //     public ObjectPool GetObjectPool(ObjectPoolType t)
    //     {
    //         ObjectPool pool = null;
    //         if (objectPoolDic.ContainsKey(t))
    //         {
    //             pool = objectPoolDic[t];
    //         }
    //         else
    //         {
    //             pool=new ObjectPool();
    //             pool.Init(true, 2, 100, 5);
    //             objectPoolDic.Add(t, pool);
    //         }
    //         return pool;
    //     }
    //     public GameObjectPool GetGameObjectPool(GameObjectPoolType t)
    //     {
    //         GameObjectPool pool = null;
    //         if (gameObjectPoolDic.ContainsKey(t))
    //         {
    //             pool = gameObjectPoolDic[t];
    //         }
    //         else
    //         {
    //             pool = new GameObjectPool();
    //             pool.Init(true, 2, 100, 5);
    //             gameObjectPoolDic.Add(t, pool);
    //         }
    //         return pool;
    //     }
    //     public void Init()
    //     {
    //     }
    //
    //     public void Update(float dt)
    //     {
    //         foreach (var item in gameObjectPoolDic)
    //         {
    //             item.Value.Update(dt);
    //         }
    //         foreach (var item in objectPoolDic)
    //         {
    //             item.Value.Update(dt);
    //         }
    //
    //     }
    // }
}
