using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StorageSystem : MonoBehaviour
{
    // Nb amount items 
    // List storage items 
    // OnOpen -> get the storage item to load UI

    // Possibilité de prendre un item depuis le counter inventory (mais pas d'en poser depuis l'UI)
    // Possibilité d'en rajouter un dans le counter si interact avec une bind en question & item mettable dans rangement
    // & y'a de la place etc etc.... 

    [SerializeField] private int nbAmountItems;

    [SerializeField] private List<ItemSlotInfo> ingredients;

    private void Start()
    {
        ingredients = new();
        IngredientData unknownIngredient = IngredientDatabase.Instance.GetUnknownIngredientData();
        for (int i = 0; i < nbAmountItems; i++)
        {
            ingredients.Add(new ItemSlotInfo(unknownIngredient, 0, i));
        }
    }


    public void LoadDeliveryBox(DeliveryBox deliveryBox, bool isAlone)
    {
        StorageSystemUI.Instance.LoadDeliveryBox(deliveryBox, false);
    }

    public void OpenStorage()
    {
        StorageSystemUI.Instance.Load(nbAmountItems, ingredients, this);
    }

    public void Close()
    {
        // SFX
    }

    public void SwapSlotContent(ItemSlotInfo _slotPickedUp, ItemSlotInfo _slotFilledIn)
    {
        ItemSlotInfo slotPickedUp = ingredients.Where(t => t.GetID() == _slotPickedUp.GetID()).FirstOrDefault();
        ItemSlotInfo slotFilledIn = ingredients.Where(t => t.GetID() == _slotFilledIn.GetID()).FirstOrDefault();

        int indexSlotPickedUp = ingredients.IndexOf(slotPickedUp);
        int indexSlotFilledIn = ingredients.IndexOf(slotFilledIn);

        int slotPickedUpID = ingredients[indexSlotPickedUp].GetID();
        int slotFilledInID = ingredients[indexSlotFilledIn].GetID();

     //   Debug.Log(slotPickedUp.ToString());
     //   Debug.Log(slotFilledIn.ToString());

      //  Debug.Log($"Index of pickedup = {indexSlotPickedUp} | Index of filled in = {indexSlotFilledIn}");

        ItemSlotInfo _copy = new ItemSlotInfo(slotPickedUp);
        ingredients[indexSlotPickedUp] = slotFilledIn;

        ingredients[indexSlotFilledIn] = _copy;

        ingredients[indexSlotPickedUp].SetID(slotPickedUpID); 
        ingredients[indexSlotFilledIn].SetID(slotFilledInID);
        Debug.Log("coucou in swapslotContent method");

        /*Debug.Log(ingredients[indexSlotPickedUp].ToString());
        Debug.Log(ingredients[indexSlotFilledIn].ToString());*/
    }

    public bool HasSameItemIn(IngredientData ingredientData, out ItemSlotInfo itemSlotInfo)
    {
        if (ingredientData == null)
        {
            itemSlotInfo = null;
            return false;
        }
        if(ingredientData.ingredientName == IngredientDatabase.Instance.GetUnknownIngredientData().ingredientName)
        {
            itemSlotInfo = null;
            return false;
        }

        foreach (ItemSlotInfo itemSlot in ingredients)
        {
            if(itemSlot.ingredientData != null && itemSlot.ingredientData.ingredientName == ingredientData.ingredientName)
            {
                itemSlotInfo = itemSlot;
                return true;
            }
        }

        itemSlotInfo = null;
        return false;
    }
}

[System.Serializable]
public class ItemSlotInfo
{
    public IngredientData ingredientData;
    public int amountSlot;
    public int ID;

    public ItemSlotInfo(IngredientData ingredientData, int amountSlot, int ID)
    {
        this.ingredientData = ingredientData;
        this.amountSlot = amountSlot;
        this.ID = ID;
    }

    public ItemSlotInfo(ItemSlotInfo itemSlotInfo)
    {
        this.ingredientData = itemSlotInfo.GetIngredientData();
        this.amountSlot = itemSlotInfo.GetAmount();
        this.ID = itemSlotInfo.ID;
    }

    public void SetAmount(int amount)
    {
        this.amountSlot = amount;
    }

    public int GetAmount()
    {
        return amountSlot;
    }

    public IngredientData GetIngredientData()
    {
        return this.ingredientData;
    }

    public int GetID()
    {
        return this.ID;
    }

    public void SetID(int id)
    {
        this.ID = id;
    }

    public void SetIngredientData(IngredientData ingredientData)
    {
        this.ingredientData = ingredientData;
    }

    public override string ToString()
    {
        String ingredientName = "null";
        if(ingredientData != null)
        {
            ingredientName = ingredientData.ingredientName;
        }

        return ingredientName + " | Qte : " + this.amountSlot;
    }

    public void MakeEmpty()
    {
        this.ingredientData = IngredientDatabase.Instance.GetUnknownIngredientData();
        this.amountSlot = 0;
    }
}