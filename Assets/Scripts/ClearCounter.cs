using Unity.VisualScripting;
using UnityEngine;

public class ClearCounter : BaseCounter
{
/*    [SerializeField] private Transform parentTransformCounter;
    private Transform currentItemOnCounter;*/


    private void Awake()
    {
    
    }   

    public override void Interact()
    {

        if (kitchenTool == null)
        {
            if(base.CanPlaceIngredientsOn() && PlayerPickup.Instance.HasIngredientInHand())
            {
                // Put item on it
                Ingredient itemOnHand        = PlayerPickup.Instance.GetCurrentPickedUpItem().GetComponent<Ingredient>();

                base.PlaceIngredientOn(itemOnHand);

            } else if(base.HasIngredientsOn() && !PlayerPickup.Instance.HasItemInHand())
            {
                // Take item on it
                base.PickupCurrentSelectedIngredientOn();
            }
        }
        else
        {
            kitchenTool.Interact();
        }
    }

    public override void Open()
    {


        if (this.gameObject.TryGetComponent<StorageSystem>(out StorageSystem storageSystem))
        {
            PlayerController.Instance.SetCanPlayerMove(false);

            Transform _playerTransform = PlayerPickup.Instance.GetCurrentPickedUpItem();
            if (_playerTransform != null && _playerTransform.TryGetComponent<DeliveryBox>(out DeliveryBox deliveryBox))
            {
                if (deliveryBox != null)
                {
                    storageSystem.LoadDeliveryBox(deliveryBox, true);
                    StorageSystemUI.Instance.SetDeliveryBoxVisibility(true);
                }
            } else
            {
                StorageSystemUI.Instance.SetDeliveryBoxVisibility(false);
            }
            storageSystem.OpenStorage();

        }
    }

}
