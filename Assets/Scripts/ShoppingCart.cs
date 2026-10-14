using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoppingCart : MonoBehaviour
{
    public event EventHandler OnListChanged;

    public static ShoppingCart Instance;

    

    public Dictionary<IngredientData, int> listOfIngredients { get; private set; }

    private void Awake()
    {
        listOfIngredients = new();
        Instance = this;
    }


    public void DeleteFromCart(IngredientData ingredientData)
    {
        if(listOfIngredients.ContainsKey(ingredientData))
        {
            listOfIngredients.Remove(ingredientData);
            OnListChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void AddItemToCart(IngredientData ingredient, int quantity)
    {
        if(listOfIngredients.ContainsKey(ingredient))
        {
            listOfIngredients[ingredient] += quantity;
        } else
        {
            //Debug.Log("count of list " + listOfIngredients.Count);
            //Debug.Log("const int = " + DeliveryManager.MAX_DELIVERY_BOX_SIZE);
            if(listOfIngredients.Count > (DeliveryManager.MAX_DELIVERY_BOX_SIZE - 1))
            {
                Debug.LogWarning("Plus de place dans le cart");
                return;
            }
            listOfIngredients.Add(ingredient, quantity);
            Debug.Log("quantity = " + listOfIngredients[ingredient]);
        }

        OnListChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsCartEmpty()
    {
        return this.listOfIngredients.Count == 0;
    }

    public void OrderCart()
    { 

        float priceTotal = CalculateTotalCartPrice();

        if(PlayerStats.Instance.CanPlayerBuyWithCost(priceTotal))
        {
            TabletManager.Instance.OrderIngredient(listOfIngredients);
        }
        
    }

    private float CalculateTotalCartPrice()
    {
        float total = 0;
        foreach(KeyValuePair<IngredientData, int> datas in listOfIngredients)
        {
            total += (datas.Key.ingredientPrice * datas.Value);
        }

        return total;
    }
}


