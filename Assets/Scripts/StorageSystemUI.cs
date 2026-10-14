using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageSystemUI : MonoBehaviour
{
    public static StorageSystemUI Instance;

    [Header("Highlight parent")]
    [SerializeField] private Transform highlightParent;
    [Space]
    [Space]
    [SerializeField] private RectTransform mainContentUI;


    [SerializeField] private RectTransform mainInventoryContent;
    [SerializeField] private Transform slotsParent;
    [SerializeField] private GameObject slotPrefab;

    [Space]
    [SerializeField] private RectTransform dragParent;
    [SerializeField] private Sprite transparentSprite;



    [Header("Delivery Box fields")]
    [SerializeField] private Transform deliveryContent;
    [SerializeField] private Transform deliverySlotsParent;

    [Header("Buttons transfer all")]
    [SerializeField] private RectTransform buttonParent;
    [SerializeField] private Button transferToDeliveryBoxButton;
    [SerializeField] private Button transferToContentButton;


    private List<StorageSlot> currentSlots;
    private List<StorageSlot> deliveryBoxSlots;

    // Store variables to get afterwards to remove on their inventory
    private StorageSystem currentStorageBlock;
    private DeliveryBox currentDeliveryBox;

    private void Awake()
    {
        Instance            = this;
        currentStorageBlock = null;
        currentDeliveryBox  = null;
        currentSlots        = new();
        deliveryBoxSlots    = new();

       // transferToDeliveryBoxButton.onClick.AddListener(TransferToDeliveryBox);
        transferToContentButton.onClick.AddListener(TransferToContent);
        buttonParent.gameObject.SetActive(false);
    }

    private void TransferToContent()
    {
        if (currentStorageBlock == null || currentDeliveryBox == null)
            return;

        List<ItemSlotInfo> allContentDeliveryBox = currentDeliveryBox.GetContent();

        foreach(ItemSlotInfo itemSlot in allContentDeliveryBox)
        {
            if(currentStorageBlock.HasSameItemIn(itemSlot.GetIngredientData(), out ItemSlotInfo itemSlotInfo))
            {
                if(itemSlotInfo != null)
                {
                    int quantity = itemSlot.GetAmount();
                    if(quantity > 0)
                    {
                        itemSlotInfo.SetAmount(itemSlotInfo.GetAmount() + quantity);
                        itemSlot.MakeEmpty();
                    }
                }
            }
        }


        UpdateSlotsVisual();

    }

    private void UpdateSlotsVisual()
    {
        foreach (StorageSlot slot in currentSlots)
        {
            slot.UpdateQuantityText();
        }

        foreach (StorageSlot slot in deliveryBoxSlots)
        {
            slot.UpdateQuantityText();
        }
    }

   /* private void TransferToDeliveryBox()
    {
        if (currentStorageBlock == null || currentDeliveryBox == null)
            return;
    }*/

    private void Update()
    {
        if(currentStorageBlock == null && currentDeliveryBox == null)
            return;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
    }

    private void EmptySlots()
    {
        // TODO : Change recttransform -> storageSlot (??)
        foreach(Transform transform in slotsParent.GetComponentInChildren<RectTransform>())
        {
            Destroy(transform.gameObject);
        }
        currentSlots = new();


    }

    public void Load(int nbIngredients, List<ItemSlotInfo> ingredients, StorageSystem storage)
    {
        if (currentStorageBlock != null)
            return;

        EmptySlots();

        this.currentStorageBlock = storage;

        int nbIngredientLoaded = 0;
        foreach(ItemSlotInfo ingredientPerQuantity in ingredients)
        {
            StorageSlot slot = Instantiate(slotPrefab, slotsParent).GetComponent<StorageSlot>();
            slot.gameObject.name = "Slot" + nbIngredientLoaded;
            slot.SetupSlot(ingredientPerQuantity, transparentSprite, nbIngredientLoaded);
            slot.SetDragParent(dragParent);
            nbIngredientLoaded++;
            currentSlots.Add(slot);
        }
        Debug.Log("we loaded " + nbIngredientLoaded + " on the UI");
        this.mainInventoryContent.gameObject.SetActive(true);

        Show();
    }

    public void LoadDeliveryBox(DeliveryBox deliveryBox, bool isAlone)
    {
        if(isAlone)
        {
            EmptySlots();
        }

        foreach (Transform transform in deliverySlotsParent.GetComponentInChildren<RectTransform>())
        {
            Destroy(transform.gameObject);
        }

        this.currentDeliveryBox = deliveryBox;

        int nbIngredientLoaded = 0;
        foreach (ItemSlotInfo ingredientPerQuantity in deliveryBox.GetContent())
        {
            StorageSlot slot = Instantiate(slotPrefab, deliverySlotsParent).GetComponent<StorageSlot>();
            slot.SetupSlot(ingredientPerQuantity, transparentSprite, nbIngredientLoaded);
            slot.SetDragParent(dragParent);
            nbIngredientLoaded++;
            deliveryBoxSlots.Add(slot);
        }


        deliveryContent.gameObject.SetActive(true);

        buttonParent.gameObject.SetActive(!isAlone);

        if (isAlone)
        {
            mainInventoryContent.gameObject.SetActive(false);
            Show();
        }
    }

    public void Show()
    {
        mainContentUI.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if(currentStorageBlock != null)
        {
            currentStorageBlock.Close();
        }
        currentStorageBlock = null;
        currentDeliveryBox  = null;
        currentSlots        = new();
        deliveryBoxSlots    = new();
        HideHighlight();
        mainContentUI.gameObject.SetActive(false);
        deliveryContent.gameObject.SetActive(false);
        PlayerController.Instance.SetCanPlayerMove(true);
        buttonParent.gameObject.SetActive(false);
    }

    public void SetItemToSlot(StorageSlot storageSlot, StorageSlot droppedSlot, ItemSlotInfo storageSlotInfo, ItemSlotInfo droppedSlotInfo)
    {
        // currentStorageBlock.SwapItemsToSlots()f
        if (storageSlot.transform.parent.gameObject.name == "StorageGrid" && droppedSlot.transform.parent.gameObject.name == "StorageGrid")
        {
            // Just swap from two items in the same list
            if (this.currentStorageBlock != null)
            {
                // Get the actual reference from the ingredients list for both slots
                ItemSlotInfo actualStorageSlotInfo = storageSlot.GetContent();
                ItemSlotInfo actualDroppedSlotInfo = droppedSlot.GetContent();

                currentStorageBlock.SwapSlotContent(actualStorageSlotInfo, actualDroppedSlotInfo);
            }
        }
        else if (storageSlot.transform.parent.gameObject.name == "StorageGrid" && droppedSlot.transform.parent.gameObject.name == "DeliveryGrid")
        {
            // Set null to the index at the first list, and put it on the other list
            if (this.currentStorageBlock != null && currentDeliveryBox != null)
            {
                SwapTwoItems(storageSlot, droppedSlot);
            }
        }
        else if (storageSlot.transform.parent.gameObject.name == "DeliveryGrid" && droppedSlot.transform.parent.gameObject.name == "StorageGrid")
        {
            // Set null to the index at the first list, and put it on the other list
            if (this.currentStorageBlock != null && currentDeliveryBox != null)
            {
                SwapTwoItems(storageSlot, droppedSlot);
            }
        }
        else if (storageSlot.transform.parent.gameObject.name == "DeliveryGrid" && droppedSlot.transform.parent.gameObject.name == "DeliveryGrid")
        {
            if(this.currentDeliveryBox != null)
            {
                currentDeliveryBox.SwapSlotContent(storageSlotInfo, droppedSlotInfo);
            }
        }
    }

    private void SwapTwoItems(StorageSlot slotA, StorageSlot slotB)
    {
        ItemSlotInfo actualStorageSlotInfo = slotA.GetContent();
        ItemSlotInfo actualDroppedSlotInfo = slotB.GetContent();

        IngredientData storageIngredientData = actualStorageSlotInfo.GetIngredientData();
        IngredientData droppedIngredientData = actualDroppedSlotInfo.GetIngredientData();
        int storageAmount = actualStorageSlotInfo.GetAmount();
        int droppedAmount = actualDroppedSlotInfo.GetAmount();

        slotA.SetIngredientData(droppedIngredientData);
        slotB.SetIngredientData(storageIngredientData);

        slotA.SetAmount(droppedAmount);
        slotB.SetAmount(storageAmount);
    }

    public void SetClickMenuHighlight(RectTransform highlight)
    {
        highlight.transform.SetParent(this.highlightParent, true);
    }

    private void HideHighlight()
    {
        for(int i = highlightParent.childCount - 1; i >= 0; i--)
        {
            Destroy(highlightParent.GetChild(i).transform.gameObject);
        }
    }

    public void SetDeliveryBoxVisibility(bool isVisible)
    {
        deliveryContent.gameObject.SetActive(isVisible);
    }

    /**
     * TODO next time : - Faire l'interaction sur la planche à découper
     *                  - Fairel'interaction avec les comptoirs vides (pour poser l'item sur le comptoir
     *                  
     */
}
