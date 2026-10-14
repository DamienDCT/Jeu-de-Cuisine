using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Ingredient : MonoBehaviour
{
    [SerializeField] private int ingredientID;

    [SerializeField] private Vector3 scaleOnKitchenObject;
    [SerializeField] private Vector3 scaleOnHand;

    public int GetID()
    {
        return this.ingredientID;
    }

    public void EnablePhysics()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.isKinematic   = false;
            rb.useGravity    = true;
        }

        GetComponent<BoxCollider>().enabled     = true;
    }

    public void DisablePhysics()
    {
        GetComponent<Rigidbody>().isKinematic   = true;
        GetComponent<Rigidbody>().useGravity    = false;
        GetComponent<BoxCollider>().enabled     = false;
    }

    public void EnableScaleOnKitchenObject()
    {
        transform.localScale = scaleOnKitchenObject;
    }

    public void DisableScaleOnKitchenObject()
    {
        transform.localScale = scaleOnHand;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("BurgerMaker") || collision.gameObject.CompareTag("Ingredient"))
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
