using System;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace YOTO
{
    public class ResMgr
    {
        private ResLoader<T> CreateLoader<T>()
        {
            return ResLoader<T>.pool.GetItem(Vector3.zero);
        }

        private void RecycleLoader<T>(ResLoader<T> baseLoader)
        {
            ResLoader<T>.pool.RecoverItem(baseLoader);
        }

        public void Init()
        {
        }

        public void LoadUI(string key, Action<GameObject> callBack)
        {
            ResLoader<GameObject> loader = CreateLoader<GameObject>();
            loader.LoadAsync<GameObject>(key,
                (t) =>
                {
                    if (t.Status == AsyncOperationStatus.Succeeded)
                    {
                        callBack(t.Result);
                    }

                    RecycleLoader(loader);
                });
        }

        public void LoadGameObject(string path, Action<GameObject> callBack)
        {
            ResLoader<GameObject> loader = CreateLoader<GameObject>();

            loader.LoadAsync<GameObject>(path,
                (t) =>
                {
                    if (t.Status == AsyncOperationStatus.Succeeded)
                    {
                        callBack(t.Result);
                    }
                    else
                    {
                        Debug.LogError("加载失败");
                    }

                    RecycleLoader(loader);
                });
        }

        public void ReleasePack(string path)
        {
            
        }
    }
}