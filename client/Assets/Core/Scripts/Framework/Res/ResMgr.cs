using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace YOTO
{
    public class ResMgr
    {
        private class CachedResource<T> where T : Object
        {
            public T asset;
            public int refCount;
        }

        private Dictionary<string, CachedResource<GameObject>> prefabCache = new();
        private Dictionary<string, CachedResource<AudioClip>> audioCache = new();

        // 创建 ResLoader
        private ResLoader<T> CreateLoader<T>() where T : Object
        {
            return ResLoader<T>.pool.GetItem(Vector3.zero);
        }

        private void RecycleLoader<T>(ResLoader<T> loader) where T : Object
        {
            ResLoader<T>.pool.RecoverItem(loader);
        }

        public void Init() { }

        // 加载UI或Prefab
        public void LoadUI(string key, Action<GameObject> callBack)
        {
            LoadGameObject(key, callBack);
        }

        public void LoadGameObject(string path, Action<GameObject> callBack)
        {
            // 检查缓存
            if (prefabCache.TryGetValue(path, out var cached))
            {
                cached.refCount++;
                GameObject go = Object.Instantiate(cached.asset);
                callBack(go);
                return;
            }

            ResLoader<GameObject> loader = CreateLoader<GameObject>();
            loader.LoadAsync(path, (t) =>
            {
                if (t == null)
                {
                    callBack(null);
                    RecycleLoader(loader);
                    return;
                }

                // 缓存Prefab
                prefabCache[path] = new CachedResource<GameObject> { asset = t, refCount = 1 };

                GameObject go = Object.Instantiate(t);
                callBack(go);

                RecycleLoader(loader);
            });
        }

        // 加载音频
        public void LoadAudio(string path, Action<AudioClip> callBack)
        {
            if (audioCache.TryGetValue(path, out var cached))
            {
                cached.refCount++;
                callBack(cached.asset);
                return;
            }

            ResLoader<AudioClip> loader = CreateLoader<AudioClip>();
            loader.LoadAsync(path, (t) =>
            {
                if (t == null)
                {
                    callBack(null);
                    RecycleLoader(loader);
                    return;
                }

                audioCache[path] = new CachedResource<AudioClip> { asset = t, refCount = 1 };
                callBack(t);

                RecycleLoader(loader);
            });
        }

        // 释放Prefab或Audio
        public void ReleasePack(string path, Object obj = null)
        {
            if (obj is GameObject)
            {
                if (prefabCache.TryGetValue(path, out var cached))
                {
                    cached.refCount--;
                    if (cached.refCount <= 0)
                    {
                        prefabCache.Remove(path);
                        Resources.UnloadUnusedAssets(); 
 
                    }
                }

                // 回收实例化对象
                Object.Destroy(obj);
            }
            else if (obj is AudioClip)
            {
                if (audioCache.TryGetValue(path, out var cached))
                {
                    cached.refCount--;
                    if (cached.refCount <= 0)
                    {
                        Resources.UnloadAsset(cached.asset);
                        audioCache.Remove(path);
                    }
                }
            }
        }
    }
}
