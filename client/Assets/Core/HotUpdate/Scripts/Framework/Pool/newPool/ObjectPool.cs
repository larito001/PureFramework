using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using YOTO;

namespace HotUpdate.Scripts.Framework.Pool.newPool
{
    public class ObjectPool : SingletonMono<ObjectPool>
    {
        public class PoolBuffer
        {
            public class BufferItem
            {
                public Transform refTrans;
                public float lastActiveTime;

                public void Init()
                {
                }

                public void Reset()
                {
                    if (refTrans != null)
                    {
                        Destroy(refTrans.gameObject);
                        refTrans = null;
                    }

                    lastActiveTime = 0;
                }
            }

            public struct BufferConfig
            {
                public float loopCheckCDTime;
                public uint inactiveTimeMax;
                public uint perFrameDisposeCountMax;
                public uint poolSizeMax;
                public Vector3 recoverWorldPos;
                public bool isNotCheckRecover;
            }

            public string name;
            public Transform rootTrans;
            private string _resPath;

            // 对象池实例化模板
            // 注意：这个模板缓存的是第一个加载出来的Prefab，整个池子使用时必须保持Prefab是干净的。
            private GameObject template;

            // 是否处于loading状态
            private bool _isLoading;
            private event Action<GameObject, string, bool> _getItemCompleteCallbacks;
            private event Action _loadCompleteCallbacks;

            private Queue<BufferItem> items;

            public int BufferSize
            {
                get
                {
                    if (items == null)
                    {
                        return 0;
                    }

                    return items.Count;
                }
            }

            public BufferConfig config;
            private float lastLoopCheckTime = 0;
            private uint usingCount;



            public void Init()
            {
                items = new Queue<BufferItem>();
                lastLoopCheckTime = 0;
                config = new BufferConfig();
                config.loopCheckCDTime = 5;
                config.inactiveTimeMax = 5;
                config.perFrameDisposeCountMax = 10;
                config.recoverWorldPos = new Vector3(1000, 1000, 1000);
                config.isNotCheckRecover = false;
                _isLoading = false;
            }

            public void SetupResPath(string resPath)
            {
                _resPath = resPath;
            }

            public void SetupIsNotCheckRecover(bool isNotCheckRecover)
            {
                config.isNotCheckRecover = isNotCheckRecover;
            }

            private void AddUsingCount()
            {
                usingCount++;
            }

            private void ReduceUsingCount()
            {
                if (usingCount > 0)
                    usingCount--;
            }

            public void Reset()
            {
                name = string.Empty;

                if (template != null)
                {
                    Addressables.Release(template);
                    template = null;
                }

                if (items != null)
                {
                    while (items.Count > 0)
                    {
                        var curItem = items.Dequeue();
                        curItem.Reset();
                    }
                }

                if (rootTrans != null)
                {
                    RemoveLifeRef();
                    Destroy(rootTrans.gameObject);
                    rootTrans = null;
                    _resPath = string.Empty;
                }

                _getItemCompleteCallbacks = null;
                _loadCompleteCallbacks = null;

                _isLoading = false;
                config.isNotCheckRecover = false;
            }

            public bool LoopCheck()
            {

                if (_isLoading)
                {
                    return false;
                }

                if (items == null)
                {
                    return true;
                }

                if (Time.time - lastLoopCheckTime < config.loopCheckCDTime)
                {
                    return false;
                }

                lastLoopCheckTime = Time.time;

                var curFrameDisposeCount = 0;
                while (items.Count > 0 && curFrameDisposeCount <= config.perFrameDisposeCountMax)
                {
                    var curItem = items.Peek();
                    if ((Time.time - curItem.lastActiveTime) > config.inactiveTimeMax)
                    {
                        curFrameDisposeCount++;
                        items.Dequeue();
                        curItem.Reset();
                    }
                    else
                    {
                        break;
                    }
                }

                return (!config.isNotCheckRecover) && items.Count <= 0 && usingCount == 0;
            }

            public void AsyncLoadAndGetItem(Action<GameObject, string, bool> callback)
            {
                _getItemCompleteCallbacks += callback;
                if (template == null)
                {
                    AsyncLoadItem(InvokeGetItemCompleteCallbacks);
                }
                else
                {
                    InvokeGetItemCompleteCallbacks();
                }
            }

            private void AsyncLoadItem(Action callback = null)
            {
                _loadCompleteCallbacks += callback;
                if (template == null)
                {
                    if (!_isLoading)
                    {
                        _isLoading = true;
                        YOTOFramework.resMgr.LoadGameObject(_resPath,(templateObj) =>
                        {
                            if (templateObj == null)
                            {
                                Debug.LogError("没有找到path = " + _resPath + "的资源....");
                                return;
                            }
                    
                            if (rootTrans == null)
                            {
                                Destroy(templateObj);
                                return;
                            }
                    
                            // templateObj.transform.SetParent(rootTrans, false);
                            templateObj.SetActive(false);
                            template = templateObj;
                            _isLoading = false;
                            InvokeLoadCompleteCallbacks();
                        });
      
                    }
                }
                else
                {
                    InvokeLoadCompleteCallbacks();
                }
            }

            private void InvokeGetItemCompleteCallbacks()
            {
                if (_getItemCompleteCallbacks == null)
                {
                    return;
                }

                var completeList = _getItemCompleteCallbacks.GetInvocationList();
                if (completeList.Length > 0)
                {
                    for (int i = 0; i < completeList.Length; i++)
                    {
                        var complete = completeList[i] as Action<GameObject, string, bool>;
                        var isNew = GetItem(out var resultObj);
                        if (resultObj != null)
                        {
                            complete?.Invoke(resultObj.gameObject, name, isNew);
                        }
                    }
                }

                _getItemCompleteCallbacks = null;
            }

            private void InvokeLoadCompleteCallbacks()
            {
                if (_loadCompleteCallbacks == null)
                {
                    return;
                }

                var completeList = _loadCompleteCallbacks.GetInvocationList();
                if (completeList.Length > 0)
                {
                    for (int i = 0; i < completeList.Length; i++)
                    {
                        var complete = completeList[i] as Action;
                        complete?.Invoke();
                    }
                }

                _loadCompleteCallbacks = null;
            }

            //同步方法
            public bool GetItem(out Transform target)
            {

                bool isNew = false;
                target = null;
                if (items.Count > 0)
                {
                    var curItem = items.Dequeue();
                    target = curItem.refTrans;
                    curItem.refTrans = null;
                    curItem.Reset();
                }
                else
                {
                    target = Instantiate(template).transform;
                    target.SetParent(rootTrans, false);
                    isNew = true;
                }

                AddUsingCount();
                return isNew;
            }

            //注意在销毁buffer时remove一下
            public void RemoveGetItemCompleteCallback(Action<GameObject, string, bool> getItemCompleteCallback)
            {
                _getItemCompleteCallbacks -= getItemCompleteCallback;
            }

            public void RemoveLoadCompleteCallback(Action loadCompleteCallback)
            {
                _loadCompleteCallbacks -= loadCompleteCallback;
            }

            public void RecoverItem(GameObject target)
            {
                if (target == null)
                {
                    return;
                }

                if (items.Count >= config.poolSizeMax)
                {
                    Destroy(target);
                    ReduceUsingCount();
                    return;
                }

                target.transform.SetParent(rootTrans);
                target.transform.position = config.recoverWorldPos;
                if (template != null)
                {
                    target.transform.localScale = template.transform.localScale;
                }

                var bufferItem = new BufferItem();
                bufferItem.Init();
                bufferItem.refTrans = target.transform;
                bufferItem.lastActiveTime = Time.time;
                items.Enqueue(bufferItem);
                ReduceUsingCount();
            }

            private void RemoveLifeRef()
            {
                if (!string.IsNullOrEmpty(_resPath))
                {
                    //todo:释放资源 ResourceFunc.RemoveLifeRef(rootTrans, _resPath, false);
                    _resPath = null;
                }
            }
        }

        private List<PoolBuffer> buffers;
        private readonly float LOOP_CHECK_TIME = 0.5f;


        private void Awake()
        {
            buffers = new List<PoolBuffer>();
        }

        private void Start()
        {
            InvokeRepeating("LoopCheck", 0, LOOP_CHECK_TIME);
        }

        private void LoopCheck()
        {
            if (buffers != null)
            {
                for (int i = buffers.Count - 1; i >= 0; i--)
                {
                    var curBuffer = buffers[i];
                    var isEmpty = curBuffer.LoopCheck();
                    if (!isEmpty)
                    {
                        continue;
                    }

                    buffers[i].Reset(); //todo:回收buffer
                    buffers.RemoveAt(i);
                }
            }
        }
    
        public PoolBuffer GetBuffer(string bufferName, float loopCheckCDTime, uint inactiveTimeMax,
            uint perFrameDisposeCountMax, uint poolSizeMax)
        {
            PoolBuffer target = null;
            for (int i = 0; i < buffers.Count; i++)
            {
                var curBuffer = buffers[i];
                if (curBuffer.name == bufferName)
                {
                    target = curBuffer;
                }
            }

            if (target != null) return target;

            target = new PoolBuffer();
            target.Init(); //todo:初始化
            BufferInitSet(bufferName, target, loopCheckCDTime, inactiveTimeMax, perFrameDisposeCountMax, poolSizeMax);
            buffers.Add(target);

            return target;
        }
    
        private void BufferInitSet(string bufferName, PoolBuffer target, float loopCheckCDTime, uint inactiveTimeMax,
            uint perFrameDisposeCountMax, uint poolSizeMax)
        {
            target.name = bufferName;
            target.rootTrans = new GameObject().transform;
#if UNITY_EDITOR
            target.rootTrans.name = bufferName;
#endif

            //todo:设置场景根节点
            target.rootTrans.parent = transform;

            target.SetupResPath(bufferName);

            target.config.loopCheckCDTime = loopCheckCDTime;
            target.config.inactiveTimeMax = inactiveTimeMax;
            target.config.perFrameDisposeCountMax = perFrameDisposeCountMax;
            target.config.poolSizeMax = poolSizeMax;
        }


        public void Clear()
        {
            CancelInvoke("LoopCheck");
            for (int i = buffers.Count - 1; i >= 0; i--)
            {
                var curBuffer = buffers[i];
                buffers[i].Reset();//todo:回收
            }

            buffers.Clear();
            buffers = null;
        }
    }
}