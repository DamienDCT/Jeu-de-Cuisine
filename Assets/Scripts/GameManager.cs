using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject deliveryBoxPrefab;

    public const int MAX_DELIVERY_BOX_SIZE = 8;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {

            Instantiate(deliveryBoxPrefab);
        }
    }
}
