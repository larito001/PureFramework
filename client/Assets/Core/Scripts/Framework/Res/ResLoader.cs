using System;
using System.Collections;
using UnityEngine;

namespace YOTO
{
    public class ResLoader<T> : PoolItem<Vector3> where T : UnityEngine.Object
    {
        public static DataObjPool<ResLoader<T>, Vector3> pool = new DataObjPool<ResLoader<T>, Vector3>("ResLoader", 3);

        public long ID { get; private set; }
        private static long index = 0;

        private string currentPath;
        private Action<T> currentCallback;
        private bool canceled = false;

        public ResLoader()
        {
            ID = index++;
        }

        /// <summary>
        /// 异步加载单个资源
        /// </summary>
        public void LoadAsync(string path, Action<T> callback)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("ResLoader LoadAsync path is null or empty");
                return;
            }

            Cancel(); // 如果之前有加载，先取消

            currentPath = path;
            currentCallback = callback;
            canceled = false;

            YOTOFramework.Instance.StartCoroutine(LoadResourceAsync(path));
        }

        private IEnumerator LoadResourceAsync(string path)
        {
            ResourceRequest request = Resources.LoadAsync<T>(path);
            yield return request;

            if (canceled)
            {
                Debug.Log($"ResLoader: 资源加载被取消 {path}");
                yield break;
            }

            T loadedAsset = request.asset as T;

            if (loadedAsset == null)
            {
                Debug.LogError($"ResLoader: 资源加载失败 {path}");
            }
            else
            {
                Debug.Log($"ResLoader: 资源加载完成 {loadedAsset.name}");
            }

            currentCallback?.Invoke(loadedAsset);

            // 清理状态
            currentPath = null;
            currentCallback = null;
        }

        /// <summary>
        /// 取消当前加载
        /// </summary>
        public void Cancel()
        {
            canceled = true;
            currentCallback = null;
            currentPath = null;
        }

        public void AfterIntoObjectPool()
        {
            ID = -1;
            Cancel();
        }

        public void SetData(Vector3 serverData)
        {
            // 可以存储或初始化数据
        }

        ~ResLoader()
        {
            // Resources.LoadAsync 不需要手动释放
        }
    }
}
