using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 通用对象池 - 优化性能，减少GC
/// 对微信小游戏特别重要，减少卡顿
/// </summary>
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    
    // 预制件 -> 池中对象队列
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = 
        new Dictionary<GameObject, Queue<GameObject>>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 预生成一些对象到池中
    /// </summary>
    public void Prewarm(GameObject prefab, int count)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            poolDictionary[prefab] = new Queue<GameObject>();
        }
        
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            poolDictionary[prefab].Enqueue(obj);
        }
    }

    /// <summary>
    /// 从池中获取对象
    /// </summary>
    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            poolDictionary[prefab] = new Queue<GameObject>();
        }
        
        GameObject obj;
        
        if (poolDictionary[prefab].Count > 0)
        {
            obj = poolDictionary[prefab].Dequeue();
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(prefab, position, rotation, transform);
        }
        
        return obj;
    }

    /// <summary>
    /// 返还对象到池中
    /// </summary>
    public void Return(GameObject prefab, GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        
        if (!poolDictionary.ContainsKey(prefab))
        {
            poolDictionary[prefab] = new Queue<GameObject>();
        }
        
        poolDictionary[prefab].Enqueue(obj);
    }

    /// <summary>
    /// 清空池，释放内存
    /// </summary>
    public void ClearPool(GameObject prefab)
    {
        if (poolDictionary.ContainsKey(prefab))
        {
            foreach (GameObject obj in poolDictionary[prefab])
            {
                Destroy(obj);
            }
            poolDictionary[prefab].Clear();
        }
    }

    /// <summary>
    /// 清空所有池
    /// </summary>
    public void ClearAllPools()
    {
        foreach (var kvp in poolDictionary)
        {
            foreach (GameObject obj in kvp.Value)
            {
                Destroy(obj);
            }
        }
        poolDictionary.Clear();
    }
}
