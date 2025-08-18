using System;
using System.Collections;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace YOTO
{
    #region 示例

    [System.Serializable]
    public class TestPlayerData
    {
        public string playerName;
        public int coins;
    }

    public class TestPlayerDataContaner : DataContaner<TestPlayerData>
    {
        public static TestPlayerDataContaner Instance;

        public TestPlayerDataContaner()
        {
            Instance = this;
        }

        private TestPlayerData _data = new();
        public override string SaveKey => "player_save";

        public override TestPlayerData GetData() => _data;
        public override void __SetData(TestPlayerData data) => _data = data;
    }

    public class ExampleUsage : MonoBehaviour
    {
        void Start()
        {
            var save = new TestPlayerDataContaner();
            save.GetData().coins = 999;
            save.GetData().playerName = "YOTO";
            save.Save(() =>
            {
                save.Load(() => { Debug.Log($"读取成功：{save.GetData().playerName} - {save.GetData().coins}"); });
            });
        }
    }

    #endregion

    /// <summary>
    /// 数据类的操作接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DataContaner<T> : IDataContainerBase where T : class, new()
    {
        public abstract string SaveKey { get; }
        public abstract T GetData();
        public abstract void __SetData(T data);

        public void Save(Action onComplete = null)
        {
            YOTOFramework.storeMgr.Save(this, onComplete);
        }

        public void Load(Action onComplete = null)
        {
            YOTOFramework.storeMgr.Load(this, onComplete);
        }
    }

    public interface IDataContainerBase
    {
        string SaveKey { get; }
        void Save(Action onComplete = null);
        void Load(Action onComplete = null); // 可以拓展为带 callback 的泛型接口，但这里简化处理
    }

    /// <summary>
    /// 保存策略，默认使用json
    /// </summary>
    public interface ISaveStrategy
    {
        string Serialize<T>(T data);
        T Deserialize<T>(string json);
    }

    /// <summary>
    /// 驱动策略，默认使用文件
    /// </summary>
    public interface IStorageDriver
    {
        IEnumerator WriteCoroutine(string key, string content, Action onComplete = null);
        IEnumerator ReadCoroutine<T>(string key, ISaveStrategy strategy, Action<T> onComplete) where T : class;
        void Delete(string key);
    }

    /// <summary>
    /// 默认驱动
    /// </summary>
    public class FileStorageDriver : IStorageDriver
    {
        private string GetPath(string key)
        {
            Debug.Log(Application.persistentDataPath);
            return Path.Combine(Application.persistentDataPath, $"{key}.json");
        }

        public IEnumerator WriteCoroutine(string key, string content, Action onComplete = null)
        {
            string path = GetPath(key);

            yield return null;

            try
            {
                File.WriteAllText(path, content, Encoding.UTF8);
                onComplete?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"[Save Error] {e.Message}");
            }
        }

        public IEnumerator ReadCoroutine<T>(string key, ISaveStrategy strategy, Action<T> onComplete) where T : class
        {
            string path = GetPath(key);

            yield return null;

            if (!File.Exists(path))
            {
                onComplete?.Invoke(null);
                yield break;
            }

            try
            {
                string json = File.ReadAllText(path, Encoding.UTF8);
                T data = strategy.Deserialize<T>(json);
                onComplete?.Invoke(data);
            }
            catch (Exception e)
            {
                Debug.LogError($"[Load Error] {e.Message}");
                onComplete?.Invoke(null);
            }
        }

        public void Delete(string key)
        {
            string path = GetPath(key);
            if (File.Exists(path)) File.Delete(path);
        }
    }

    /// <summary>
    /// 默认策略
    /// </summary>
    public class JsonSaveStrategy : ISaveStrategy
    {
        public string Serialize<T>(T data)
        {
            return JsonUtility.ToJson(data, true);
        }

        public T Deserialize<T>(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }
    }

    public class StoreMgr
    {
        private ISaveStrategy _strategy;
        private IStorageDriver _storage;

        public void Init()
        {
            //默认json和文件读写方式存储（后续可以改为联网存储到云端）
            _strategy = new JsonSaveStrategy();
            _storage = new FileStorageDriver();
        }

        public void Save<T>(DataContaner<T> dataContaner, Action onComplete = null) where T : class, new()
        {
            string json = _strategy.Serialize(dataContaner.GetData());
            YOTOFramework.Instance.StartCoroutine(_storage.WriteCoroutine(dataContaner.SaveKey, json, onComplete));
        }

        public void Load<T>(DataContaner<T> dataContaner, Action onComplete) where T : class, new()
        {
            YOTOFramework.Instance.StartCoroutine(_storage.ReadCoroutine<T>(dataContaner.SaveKey, _strategy, data =>
            {
                if (data == null) data = new T();
                dataContaner.__SetData(data);
                Debug.Log("数据加载完成:"+dataContaner.SaveKey+":"+data);
                onComplete?.Invoke();
            }));
        }

        public void Delete(string key)
        {
            _storage.Delete(key);
        }
    }
}