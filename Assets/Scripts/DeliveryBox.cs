using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DeliveryBox : MonoBehaviour, IInteractable
{
    [SerializeField] private List<ItemSlotInfo> content;

    public const int AMOUNT_DIFFERENT_ITEMS = 8;


    private void Awake()
    {
        IngredientData unknownIngredient = IngredientDatabase.Instance.GetUnknownIngredientData();
        content = new();
        for (int i = 0; i < AMOUNT_DIFFERENT_ITEMS; i++)
        {
            content.Add(new ItemSlotInfo(unknownIngredient, 0, i));
        }
/*        content[0].SetIngredientData(IngredientDatabase.Instance.SearchIngredientByIndex(0));
        content[0].SetAmount(10);
        content[1].SetIngredientData(IngredientDatabase.Instance.SearchIngredientByIndex(7));
        content[1].SetAmount(5);
        content[2].SetIngredientData(IngredientDatabase.Instance.SearchIngredientByIndex(29));
        content[2].SetAmount(5);
        content[3].SetIngredientData(IngredientDatabase.Instance.SearchIngredientByIndex(8));
        content[3].SetAmount(5);
        content[4].SetIngredientData(IngredientDatabase.Instance.SearchIngredientByIndex(2));
        content[4].SetAmount(5);
        content[5].SetIngredientData(IngredientDatabase.Instance.SearchIngredientByIndex(4));
        content[5].SetAmount(5);        
        content[6].SetIngredientData(IngredientDatabase.Instance.SearchIngredientByIndex(32));
        content[6].SetAmount(5);*/
    }

    public void AddIngredient(IngredientData ingredient, int quantity)
    {
        int unknownIngredientId = IngredientDatabase.Instance.GetUnknownIngredientData().ID;
        Debug.Log("length = " + content.Count);
        if (!content.Exists(i => i.ingredientData == ingredient))
        {
            int k = 0;
            bool hasBeenModified = false;
            while (k < AMOUNT_DIFFERENT_ITEMS && !hasBeenModified)
            {
                if (content[k].GetIngredientData().ID == unknownIngredientId)
                {
                    content[k] = new ItemSlotInfo(ingredient, quantity, content[k].GetID());
                    hasBeenModified = true;
                }

                k++;
            }
        }
    }

    public void RemoveIngredient(IngredientData ingredient, int quantity)
    {
        // Si on a un ingr�dient
        if (content.Exists(i => i.ingredientData == ingredient))
        {
            ItemSlotInfo slotInfo = content.Where(i => i.ingredientData == ingredient).FirstOrDefault();
            if(slotInfo != null)
            {
                int index = content.IndexOf(slotInfo);
                // On set sa quantit�
                this.content[index].SetAmount(quantity);
                // Si la quantit� est � 0 (slot vide)
                if (this.content[index].GetAmount() == 0)
                {
                    // On le retire du dico
                    this.content[index] = new ItemSlotInfo(null, 0, content[index].GetID());
                }
            }

        }
    }

    public void Interact(PlayerPickup playerPickup)
    {
        playerPickup.SetCurrentItem(this.gameObject);
    }

    public List<ItemSlotInfo> GetContent()
    {
        return this.content;
    }

    public void SwapSlotContent(ItemSlotInfo _slotPickedUp, ItemSlotInfo _slotFilledIn)
    {
        ItemSlotInfo slotPickedUp = content.Where(t => t.GetID() == _slotPickedUp.GetID()).FirstOrDefault();
        ItemSlotInfo slotFilledIn = content.Where(t => t.GetID() == _slotFilledIn.GetID()).FirstOrDefault();

        int indexSlotPickedUp = content.IndexOf(slotPickedUp);
        int indexSlotFilledIn = content.IndexOf(slotFilledIn);

        int slotPickedUpID = content[indexSlotPickedUp].GetID();
        int slotFilledInID = content[indexSlotFilledIn].GetID();

        ItemSlotInfo _copy = new ItemSlotInfo(slotPickedUp);
        content[indexSlotPickedUp] = slotFilledIn;

        content[indexSlotFilledIn] = _copy;

        content[indexSlotPickedUp].SetID(slotPickedUpID);
        content[indexSlotFilledIn].SetID(slotFilledInID);
    }

    public void Open(PlayerPickup playerPickup)
    {
        StorageSystemUI.Instance.LoadDeliveryBox(this, true);
    }

    public void FillUpWithOrder(Dictionary<IngredientData, int> ingredientDatas)
    {
        foreach (KeyValuePair<IngredientData, int> data in ingredientDatas)
        {
            AddIngredient(data.Key, data.Value);
        }
    }
}
