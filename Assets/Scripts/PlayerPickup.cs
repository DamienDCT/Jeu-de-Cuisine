using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    public static PlayerPickup Instance;

    [SerializeField] private bool isPlayerInRange;

    [SerializeField] private Transform currentObjectSelected;
    [SerializeField] private Transform currentItemInHand;
    [SerializeField] private Transform grabPoint;


    private void Awake()
    {
        isPlayerInRange = false;    
        currentItemInHand = null;
        currentObjectSelected = null;

        Instance = this;
    }

    private void Start()
    {
        PlayerInput playerInput = PlayerManagerInput.Instance.GetPlayerInput();

        if(playerInput != null)
        {
            playerInput.actions["Interact"].performed += GrabItem;
            playerInput.actions["OpenContainer"].performed += OpenContainer;
        }
    }

    private void GrabItem(InputAction.CallbackContext context)
    {
        // TODO : Drop item with another bind ?
        if (currentItemInHand != null)
        {
           DropItem();
        } else if(currentObjectSelected != null)
        {
            currentObjectSelected.GetComponent<IInteractable>().Interact(this);
        }
    }

    public bool HasHandEmpty()
    {
        return !HasItemInHand() && !HasIngredientInHand();
    }

    private void OpenContainer(InputAction.CallbackContext context)
    {
        if (currentObjectSelected != null && currentItemInHand == null)
        {
            currentObjectSelected.GetComponent<IInteractable>().Open(this);
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag("GrabbableItem"))
            return;

        isPlayerInRange = true;
        currentObjectSelected = collider.gameObject.transform;
    }

    public void OnTriggerExit(Collider collider)
    {
        if (!collider.CompareTag("GrabbableItem"))
            return;

        isPlayerInRange = false;
        currentObjectSelected = null;
    }

    private void DropItem()
    {
        if(this.currentItemInHand != null)
        {
            Debug.Log("we drop");
            if (this.currentItemInHand.CompareTag("Ingredient"))
                return;

            this.currentItemInHand.GetComponent<BoxCollider>().enabled = true;
            this.currentItemInHand.GetComponent<Rigidbody>().isKinematic = false;
            this.currentItemInHand.GetComponent<Rigidbody>().useGravity = true;
            this.currentItemInHand.SetParent(null);
            this.isPlayerInRange = false;
            this.currentItemInHand = null;
        }
    }

    public void EmptyHand(bool destroyItem = true)
    {
        if (currentItemInHand == null)
            return;

        if (destroyItem)
        {
            Destroy(currentItemInHand.gameObject);
        }

        this.isPlayerInRange    = false;
        this.currentItemInHand  = null;
    }

    public void SetCurrentItem(GameObject gameObject)
    {
        Debug.Log(gameObject.name);
        this.currentItemInHand = gameObject.transform;
        this.currentItemInHand.GetComponent<BoxCollider>().enabled = false;
        this.currentItemInHand.transform.position = grabPoint.position;

        Rigidbody rb = null;
        this.currentItemInHand.TryGetComponent<Rigidbody>(out rb);
        if(rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        this.currentItemInHand.SetParent(grabPoint);
        this.isPlayerInRange = false;
    }

    public bool HasItemInHand()
    {
        return currentItemInHand != null;
    }

    public Transform GetCurrentPickedUpItem()
    {
        return this.currentItemInHand;
    }

    public bool HasIngredientInHand()
    {
        return HasItemInHand() && this.currentItemInHand.CompareTag("Ingredient");
    }
}

