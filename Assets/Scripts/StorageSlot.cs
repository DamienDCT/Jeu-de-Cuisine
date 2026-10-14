using NUnit.Framework.Internal.Execution;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StorageSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    private static StorageSlot lastSelectedSlot;

    private bool isSlotEnabled;
    private Transform originalParent;

    [SerializeField] private Image ingredientSprite;
    [SerializeField] private RectTransform dragParent;
    [SerializeField] private ItemSlotInfo slotInfo;

    [Space]
    [Header("Quantity fields")]
    [SerializeField] private GameObject quantityParentObject;
    [SerializeField] private TextMeshProUGUI quantityText;

    [Space]
    [Header("Click menu")]
    [SerializeField] private RectTransform clickMenuContent;
    [SerializeField] private Button pickupButton;

    private Sprite transparentSprite;

    private void Awake()
    {
        isSlotEnabled       = true;
        lastSelectedSlot    = null;

        pickupButton?.onClick.AddListener(PickupItem);
    }

    public void SetupSlot(ItemSlotInfo itemSlotInfo, Sprite transparentSprite, int ID)
    {
        this.transparentSprite = transparentSprite;
        this.slotInfo = itemSlotInfo;
        if (itemSlotInfo.GetIngredientData().ID == -1)
        {
            ingredientSprite.sprite = transparentSprite;
        } else
        {
            ingredientSprite.sprite = itemSlotInfo.ingredientData.ingredientSprite;
        }
        UpdateQuantityText();
    }

    public void DisableSlot()
    {
        isSlotEnabled = false;
        ingredientSprite.color = Color.black;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isSlotEnabled) return;
        ResetHighlight();
        originalParent = ingredientSprite.transform.parent;
        ingredientSprite.transform.SetParent(dragParent); // Move to the drag layer
        ingredientSprite.raycastTarget = false; // Disable raycast blocking for the sprite
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isSlotEnabled) return;
        ResetHighlight();
        ingredientSprite.transform.position = eventData.position; // Follow the mouse position
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isSlotEnabled) return;
        //  ingredientSprite.transform.SetParent(originalParent); // Return to the original parent
        ingredientSprite.raycastTarget = true; // Re-enable raycast blocking
        ingredientSprite.transform.SetParent(this.transform);
        ingredientSprite.transform.localPosition = Vector3.zero; // Reset position within the parent
        ResetHighlight();
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Check if the dragged object is valid
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null)
            return;
        
        // Check if there's a StorageSlot component on the dragged object
        StorageSlot droppedSlot = droppedObject.GetComponent<StorageSlot>();
        if (droppedSlot == null)
            return;

        //! BUG : Sauvegarde pas correcte dans les storages respectifs (peut être lors de la fusion d'items ?)

        // If both slots have no items, there's nothing to swap
        if (this.slotInfo.GetIngredientData() == null && droppedSlot.GetIngredientData() == null)
            return;

        if (this.slotInfo.GetIngredientData().ingredientName != droppedSlot.slotInfo.GetIngredientData().ingredientName)
        {
            droppedSlot.ingredientSprite.transform.SetParent(droppedSlot.transform);

            this.ingredientSprite.sprite = droppedSlot.GetIngredientData().ingredientSprite;

            // Swap the slot data
            if((this.transform.parent.gameObject.name == "StorageGrid" && droppedSlot.transform.parent.gameObject.name == "StorageGrid")
                || this.transform.parent.gameObject.name == "DeliveryGrid" && droppedSlot.transform.parent.gameObject.name == "DeliveryGrid")
            {
                int currentID = this.slotInfo.GetID();
                int droppedID = droppedSlot.slotInfo.GetID();

                ItemSlotInfo tempSlotInfo = new ItemSlotInfo(this.slotInfo);
                this.slotInfo = droppedSlot.slotInfo;
                droppedSlot.slotInfo = tempSlotInfo;

                this.slotInfo.SetID(droppedID);
                droppedSlot.slotInfo.SetID(currentID);
            }


            // Update the slot contents in the storage system using SetItemToSlot
            StorageSystemUI.Instance.SetItemToSlot(this, droppedSlot, this.slotInfo, droppedSlot.slotInfo);
            if (droppedSlot.slotInfo.GetIngredientData().ingredientName == "Unknown")
            {
                droppedSlot.ingredientSprite.sprite = transparentSprite;
            } else
            {
                droppedSlot.ingredientSprite.sprite = droppedSlot.slotInfo.GetIngredientData().ingredientSprite;
            }
            // Update the quantity text
            ResetHighlight();
            this.UpdateQuantityText();
            droppedSlot.UpdateQuantityText();
        } else
        {
            droppedSlot.ingredientSprite.transform.SetParent(droppedSlot.transform);
            droppedSlot.ingredientSprite.sprite = transparentSprite;
            this.ingredientSprite.sprite = droppedSlot.GetIngredientData().ingredientSprite;

            slotInfo.SetAmount(slotInfo.GetAmount() + droppedSlot.slotInfo.GetAmount());
            droppedSlot.SetIngredientData(IngredientDatabase.Instance.GetUnknownIngredientData());
            droppedSlot.SetAmount(0);

            ResetHighlight();

            // Update the quantity text
            this.UpdateQuantityText();
            droppedSlot.UpdateQuantityText();
        }
    }

    public void UpdateQuantityText()
    {
        if(slotInfo.ingredientData.Equals(IngredientDatabase.Instance.GetUnknownIngredientData()))
        {
            ingredientSprite.sprite = transparentSprite;
        }

        if(slotInfo.GetAmount() > 0)
        {
            quantityParentObject.SetActive(true);
            quantityText.text = "" + slotInfo.GetAmount();
        } else
        {
            quantityParentObject.SetActive(false);
        }
    }

    private void ClearSlot()
    {
        this.slotInfo = new ItemSlotInfo(null, 0, this.slotInfo.GetID());
        this.ingredientSprite.sprite = transparentSprite;
    }

    public IngredientData GetIngredientData()
    {
        if (this.slotInfo == null) return null;

        return this.slotInfo.GetIngredientData();
    }
    
    public ItemSlotInfo GetContent()
    {
        return this.slotInfo;
    }

    public void SetTransparentSprite(Sprite transparentSprite)
    {
        this.transparentSprite = transparentSprite;
        this.ingredientSprite.sprite = transparentSprite;
    }

    public void SetDragParent(RectTransform dragParent)
    {
        this.dragParent = dragParent;
    }

    public void SetIngredientData(IngredientData ingredientData)
    {
        this.slotInfo.SetIngredientData(ingredientData);
    }

    public void SetAmount(int amount)
    {
        this.slotInfo.SetAmount(amount);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ManageHighlight();
    }

    private void ResetHighlight()
    {
        if(lastSelectedSlot != null)
        {
            lastSelectedSlot.DisableHighlight();
            lastSelectedSlot = null;
        }
        DisableHighlight();
    }

    private void ManageHighlight()
    {
        if (GetIngredientData().Equals(IngredientDatabase.Instance.GetUnknownIngredientData()))
        {
            return;
        }

        if (lastSelectedSlot != null)
        {
            lastSelectedSlot.DisableHighlight();
            lastSelectedSlot = this;
            EnableHighlight();
        }
        else
        {
            lastSelectedSlot = this;
            EnableHighlight();
        }
    }

    private void DisableHighlight()
    {
        this.clickMenuContent.gameObject.SetActive(false);
        lastSelectedSlot?.clickMenuContent.transform.SetParent(this.transform, true);
    }

    private void EnableHighlight()
    {
        this.clickMenuContent.gameObject.SetActive(true);
        StorageSystemUI.Instance.SetClickMenuHighlight(clickMenuContent);
    }

    private void PickupItem()
    {
        if (this.slotInfo.GetAmount() <= 0)
            return;


        if (PlayerPickup.Instance.HasItemInHand())
            return;


        GameObject ingredient       = slotInfo.GetIngredientData().ingredientPrefab;
        GameObject objectInHand     = Instantiate(ingredient);

        if((slotInfo.GetAmount() - 1) == 0)
        {
            slotInfo.SetIngredientData(IngredientDatabase.Instance.GetUnknownIngredientData());
            slotInfo.SetAmount(0);
        } else
        {
            slotInfo.SetAmount(slotInfo.GetAmount() - 1);
        }
        this.UpdateQuantityText();

        PlayerPickup.Instance.SetCurrentItem(objectInHand);

        DisableHighlight();
        StorageSystemUI.Instance.Hide();
    }
}