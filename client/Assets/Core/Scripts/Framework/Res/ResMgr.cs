using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace YOTO
{
    public class ResMgr
    {
        private ResLoader<T> CreateLoader<T>() where T : Object
        {
            return ResLoader<T>.pool.GetItem(Vector3.zero);
        }

        private void RecycleLoader<T>(ResLoader<T> baseLoader) where T : Object
        {
            ResLoader<T>.pool.RecoverItem(baseLoader);
        }

        public void Init()
        {
        }

        public void LoadUI(string key, Action<GameObject> callBack)
        {
            LoadGameObject(key, callBack);
        }

        public void LoadGameObject(string path, Action<GameObject> callBack)
        {
            ResLoader<GameObject> loader = CreateLoader<GameObject>();

            loader.LoadAsync(path,
                (t) =>
                {
                    callBack(t);
        
                });
        }

        public void ReleasePack(string path)
        {
            
        }
    }
}