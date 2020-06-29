using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prepearer : MonoBehaviour {

    #region Singleton

    public static Prepearer Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion
    public string pistolPoolTag;
    public int pistolPoolSize;
    public GameObject pistolBulParent;
    public GameObject pistolBulPrefab;
    void Start()
    {
        ObjectPooler.Instance.AddToPool(pistolBulPrefab, pistolPoolTag, pistolPoolSize, pistolBulParent);
    }
   
}
