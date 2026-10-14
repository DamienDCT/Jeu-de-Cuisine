using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{

    [SerializeField] private GameObject boxPrefab;

    public const int MAX_DELIVERY_BOX_SIZE = 8;

    private DeliveryBox currentDeliveryBox;

    private void Awake()
    {
        currentDeliveryBox = null;
    }

    private void Start()
    {
        TabletManager.Instance.OnDeliveryAsked += DeliveryManager_OnDeliveryAsked;
    }

    private void DeliveryManager_OnDeliveryAsked(object sender, DeliveryArgs args)
    {
        if (currentDeliveryBox != null)
            return;


        Dictionary<IngredientData, int> listOfIngredients = args.listOfIngredients;
        // TODO : Instantiate la box avec les ingrédients à l'intérieur
        DeliveryBox box = Instantiate(boxPrefab, transform.position, Quaternion.identity).GetComponent<DeliveryBox>();

        box.FillUpWithOrder(args.listOfIngredients);

        // Faire un tapis roulant pour plusieurs orders à la fois ?
        this.currentDeliveryBox = box;
    }

    [System.Serializable]
    public class DeliveryArgs : EventArgs
    {
        public Dictionary<IngredientData, int> listOfIngredients;
    }
}

