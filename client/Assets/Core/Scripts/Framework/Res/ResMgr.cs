using System;
using UnityEngine;

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
                    callBack(t);

                    RecycleLoader(loader);
                });
        }

        public void LoadGameObject(string path, Action<GameObject> callBack)
        {
            ResLoader<GameObject> loader = CreateLoader<GameObject>();

            loader.LoadAsync<GameObject>(path,
                (t) =>
                {
                    callBack(t);

                    RecycleLoader(loader);
                });
        }

        public void ReleasePack(string path)
        {
            
        }
    }
}