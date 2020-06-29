using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    #region Singleton

    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion


    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    public void FillPool(GameObject parent)
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        Queue<GameObject> objectPool = new Queue<GameObject>();

        foreach(Pool pool in pools)
        {
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
                GiveParent(obj.transform, parent.transform);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public void AddToPool(GameObject prefab, string tag, int size, GameObject parent)
    {
        Pool pool = new Pool();
        pool.prefab = prefab;
        pool.tag = tag;
        pool.size = size;
        pools.Add(pool);
        FillPool(parent);
    }

    public GameObject SpawnFromPool(string tag, Vector3 positon, Quaternion rotation)
    {
        if(!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag + " + tag + " doesn't exist.");
            return null;
        }
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = positon;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
    public void GiveParent(Transform obj, Transform parent)
    {
        obj.SetParent(parent, false);
    }

}
