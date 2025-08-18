using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace YOTO
{
    public  class ResLoader<T>: PoolItem<Vector3>
    {
        public static  DataObjPool<ResLoader<T>,Vector3> pool=new DataObjPool<ResLoader<T>, Vector3>("ResLoader", 3);
        public long ID { get; private set; }
        static long index = 0;
        private List<string> loadKeys=new List<string>();
        public ResLoader()
        {
            ID = index++;
        }
        public    void Load<T>() { 
            
        }

        AsyncOperationHandle<T> handle;
        public void LoadAsync<T>(string path, Action<AsyncOperationHandle<T>> callback)
        {
            if (!loadKeys.Contains(path))
            {
                loadKeys.Add(path);

            }
            if (callback != null)
            {
                var handle = Addressables.LoadAssetAsync<T>(path);
      
                handle.Completed += callback;
            }
        }

        ~ ResLoader()
        {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
        }

        public void AfterIntoObjectPool()
        {
            ID =-1;
        }

        public void SetData(Vector3 serverData)
        {
         
        }
    }
   

}
