using System.Collections.Generic;

/// <summary>
/// 池中的Item
/// </summary>
/// <typeparam name="S"></typeparam>
public interface PoolItem<S>
{
    /// <summary>
    /// 入池前重置属性
    /// </summary>
    void AfterIntoObjectPool();

    /// <summary>
    /// 设置Data
    /// </summary>
    /// <param name="serverData"></param>
    void SetData(S serverData);
}

/// <summary>
/// 数据对象专用池
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="S"></typeparam>
public class DataObjPool<T, S> where T : class, PoolItem<S>, new()
{
    private string name;
    private Stack<T> pool;
    private List<T> _usingList;

    public int BufferSize
    {
        get
        {
            if (pool == null)
            {
                return 0;
            }

            return pool.Count;
        }
    }

    private float sumReqTimes;
    private float catchReqTimes;

    public float HitRate
    {
        get
        {
            if (sumReqTimes.Equals(0))
            {
                return 0;
            }

            return catchReqTimes / sumReqTimes;
        }
    }

    public DataObjPool(string poolName, int capcity = 10)
    {
        name = poolName;
        pool = new Stack<T>(capcity);
        _usingList = new List<T>(capcity);
    }

    public void RecoverItem(T item)
    {
        item.AfterIntoObjectPool();
        pool.Push(item);
        if (_usingList.Contains(item))
        {
            if (item is BaseEntity)
            {
                (item as BaseEntity).RemoveThis();
            }

            _usingList.Remove(item);
        }
    }

    public T GetItem(S serverData)
    {
        sumReqTimes++;
        T returnData = null;
        if (pool.Count > 0)
        {
            catchReqTimes++;
            returnData = pool.Pop();
        }
        else
        {
            returnData = new T();
        }

        if (returnData is BaseEntity)
        {
            (returnData as BaseEntity).Init();
        }

        returnData.SetData(serverData);
        _usingList.Add(returnData);
        return returnData;
    }

    public void ClearPool()
    {
        for (int i = 0; i < _usingList.Count; i++)
        {
            _usingList[i].AfterIntoObjectPool();
            if (_usingList[i] is BaseEntity)
            {
                (_usingList[i] as BaseEntity).RemoveThis();
            }
        }

        pool.Clear();
        _usingList.Clear();
    }
}