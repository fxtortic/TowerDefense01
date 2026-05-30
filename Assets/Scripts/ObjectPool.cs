using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int initialSize = 20;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDict;
    private Dictionary<string, Pool> poolDataDict;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        InitializePools();
    }

    void InitializePools()
    {
        poolDict = new Dictionary<string, Queue<GameObject>>();
        poolDataDict = new Dictionary<string, Pool>();

        foreach (var pool in pools)
        {
            var queue = new Queue<GameObject>();
            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
            poolDict[pool.tag] = queue;
            poolDataDict[pool.tag] = pool;
        }
    }

    public GameObject Get(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDict.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool '{tag}' не знайдено!");
            return null;
        }

        GameObject obj;
        if (poolDict[tag].Count > 0)
        {
            obj = poolDict[tag].Dequeue();
        }
        else
        {
            obj = Instantiate(poolDataDict[tag].prefab, transform);
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
         PooledObject po = obj.GetComponent<PooledObject>();
        if (po == null) po = obj.AddComponent<PooledObject>();
        po.poolTag = tag;

        obj.SetActive(true);
        return obj;
    }

    public void Return(string tag, GameObject obj)
    {
        if (!poolDict.ContainsKey(tag))
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        poolDict[tag].Enqueue(obj);
    }

    public void ReturnAll(string tag)
        {
            if (!poolDataDict.ContainsKey(tag)) return;

            List<GameObject> toReturn = new List<GameObject>();
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                {
                    PooledObject po = child.GetComponent<PooledObject>();
                    if (po != null && po.poolTag == tag)
                        toReturn.Add(child.gameObject);
                }
            }
            foreach (var obj in toReturn)
                Return(tag, obj);
        }
}
