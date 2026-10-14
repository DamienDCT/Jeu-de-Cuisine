using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDatabase : MonoBehaviour
{
    public static ObjectDatabase Instance;

    [SerializeField] private ObjectDatabaseSO database;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetPrefabByIndex(int index)
    {
        GameObject prefabSelected = null;
        ObjectData objData = database.objectsData.Find(obj => obj.ID == index);
        if(objData != null)
        {
            prefabSelected = objData.prefab;
        }

        return prefabSelected;
    }

    public ObjectData GetObjectDataOfIndex(int index)
    {
        ObjectData objData = database.objectsData.Find(obj => obj.ID == index);
        return objData;
    }
}
