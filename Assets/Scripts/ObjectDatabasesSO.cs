using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectDatabaseSO : ScriptableObject
{
    public List<ObjectData> objectsData;
}

[System.Serializable]
public class ObjectData
{
    public string objectName;
    public int ID;
    public Vector2Int size;
    public GameObject prefab;
}
