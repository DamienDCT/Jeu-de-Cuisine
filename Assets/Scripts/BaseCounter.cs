using UnityEngine;
using System.Collections.Generic;


public class BaseCounter : MonoBehaviour
{
    [SerializeField] protected KitchenTool kitchenTool;

    [SerializeField] protected List<Ingredient> ingredientsOnTop;
    [SerializeField] protected List<Transform> ingredientsPositionOnTop;

    [SerializeField] private int currentIngredientSelectedIndex;
    protected bool isOccupied;

    [SerializeField] protected Direction placedDirection;

    private void Awake()
    {
        isOccupied = false;
        currentIngredientSelectedIndex = 0;
    }
    
    public virtual void Interact()
    {
        Debug.LogError("BaseCounter.Interact");
    }

    public virtual void Open()
    {
        Debug.LogError("BaseCounter.Open");
    }

    public virtual void InteractHold()
    {
        kitchenTool.InteractHold();
    }

    public virtual void CancelHoldInteraction()
    {
        kitchenTool.CancelHoldInteraction();
    }

    public void PutItemOn(Ingredient ingredient)
    {

    }

    public void SetPlacedDirection(Direction placedDirection)
    {
        this.placedDirection = placedDirection;
    }

    protected bool CanPlaceIngredientsOn()
    {
        return ingredientsOnTop.Count < ingredientsPositionOnTop.Count;
    }

    protected bool HasIngredientsOn()
    {
        return ingredientsOnTop.Count > 0;
    }

    protected virtual void PlaceIngredientOn(Ingredient ingredient)
    {
        /*    this.currentItemOnCounter = itemOnHand;
            itemOnHand.position = parentTransformCounter.position;
            itemOnHand.SetParent(parentTransformCounter);*/
        if (!CanPlaceIngredientsOn())
            return;

        ingredientsOnTop.Add(ingredient);
        if (ingredientsOnTop.Count > 0)
        {
            currentIngredientSelectedIndex = 0;
            AddOutline(ingredientsOnTop[currentIngredientSelectedIndex].gameObject);
        }

        foreach (Transform _transform in ingredientsPositionOnTop)
        {
            if (_transform.childCount == 0)
            {
                ingredient.transform.SetParent(_transform);
                ingredient.transform.transform.localPosition = Vector3.zero;

                PlayerPickup.Instance.EmptyHand(false);
                isOccupied = true;
                break;
            }
        }
    }

    protected void PickupCurrentSelectedIngredientOn()
    {
        if (!HasIngredientsOn())
            return;

        Ingredient ingredient = ingredientsOnTop[currentIngredientSelectedIndex];

        PlayerPickup.Instance.SetCurrentItem(ingredient.gameObject);
        RemoveOutline(ingredient.gameObject);

        ingredientsOnTop.Remove(ingredient);

        if(ingredientsOnTop.Count > 0)
        {
            currentIngredientSelectedIndex = (currentIngredientSelectedIndex - 1 + ingredientsOnTop.Count) % ingredientsOnTop.Count;
            AddOutline(ingredientsOnTop[currentIngredientSelectedIndex].gameObject);
        }
    }

    public void SwapSelectedItem(float value)
    {
        if (value == 0)
            return;
        if (!HasIngredientsOn())
            return;

        GameObject currentIngredient = ingredientsOnTop[currentIngredientSelectedIndex].gameObject;
        Debug.Log($"Current Index Before: {currentIngredientSelectedIndex}");
         
        if (ingredientsOnTop.Count > 1)
        {
            RemoveOutline(currentIngredient);

            if (value == 1f)
            {
                // Incrémenter et s'assurer que l'index ne dépasse pas la taille de la liste
                currentIngredientSelectedIndex = (currentIngredientSelectedIndex + 1) % ingredientsOnTop.Count;
            }
            else if (value == -1f)
            {
                // Décrémenter et gérer le débordement négatif
                currentIngredientSelectedIndex = (currentIngredientSelectedIndex - 1 + ingredientsOnTop.Count) % ingredientsOnTop.Count;
            }

            Debug.Log($"Current Index After: {currentIngredientSelectedIndex}");
            currentIngredient = ingredientsOnTop[currentIngredientSelectedIndex].gameObject;
        }

        AddOutline(currentIngredient);
    }


    private void RemoveOutline(GameObject gameObject)
    {
        MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        
        foreach(MeshRenderer _renderer in meshRenderers)
        {
            // Ensure the meshRenderer has more than one material
            if (_renderer.materials.Length > 1)
            {
                Material[] materials = _renderer.materials;
                Material outlineMaterial = materials[1];

                // Ensure the material has the "_Scale" property
                if (outlineMaterial.HasProperty("_Scale"))
                {
                    outlineMaterial.SetFloat("_Scale", 0f);
                }
            }
        }
    }

    private void AddOutline(GameObject gameObject)
    {
        MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        
        foreach(MeshRenderer _renderer in meshRenderers)
        {
            // Ensure the meshRenderer has more than one material
            if (_renderer.materials.Length > 1)
            {
                Material[] materials = _renderer.materials;
                Material outlineMaterial = materials[1];

                // Ensure the material has the "_Scale" property
                if (outlineMaterial.HasProperty("_Scale"))
                {
                    outlineMaterial.SetFloat("_Scale", 1.2f);
                }
            }
        }
    }

    public void DisableOutline()
    {
        if(ingredientsOnTop.Count <= 0)
            return;

        GameObject gameObject = ingredientsOnTop[currentIngredientSelectedIndex].gameObject;

        if (gameObject != null)
        {
            RemoveOutline(gameObject);
        }
    }

    public void EnableOutline()
    {
        if (ingredientsOnTop.Count <= 0)
            return;

        GameObject gameObject = ingredientsOnTop[currentIngredientSelectedIndex].gameObject;

        if (gameObject != null)
        {
            AddOutline(gameObject);
        }
    }

    public KitchenTool GetKitchenTool()
    {
        return kitchenTool;
    }
}
