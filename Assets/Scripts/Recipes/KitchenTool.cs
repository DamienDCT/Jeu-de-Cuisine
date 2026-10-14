using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.VFX;

public class KitchenTool : MonoBehaviour
{
    [Space]
    [SerializeField] protected KitchenToolType kitchenToolType;

    [Space]
    protected IngredientData currentIngredient;
    protected IngredientData outputIngredient;
    [SerializeField] protected Transform contentInputParent;
    protected Ingredient currentIngredientInput;
    [SerializeField] private VisualEffect cuttingEffect;


    protected RecipeData currentRecipe;
    private int outputIngredientAmount;
    private int remainingItemsOnOutput;




    //[SerializeField] protected ProgressionBar progressionBar;
    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnIngredientChangedEventArgs> OnIngredientChanged;
    public event EventHandler OnProgressionCancelled;

    public event EventHandler<int> OnStartPickup;
    public event EventHandler<int> OnCraftIngredient;
    public event EventHandler<int> OnPickupIngredient;


    public class OnProgressChangedEventArgs : EventArgs
    {
        public float progressNormalized;
    }    
    
    public class OnIngredientChangedEventArgs : EventArgs
    {
        public IngredientData ingredientData;
        public IngredientData outputIngredientData;
    }


    private void Start()
    {
        currentIngredient       = IngredientDatabase.Instance.GetUnknownIngredientData();
        outputIngredient        = IngredientDatabase.Instance.GetUnknownIngredientData();

        currentRecipe           = null;
        outputIngredientAmount  = 0;
        remainingItemsOnOutput  = 0;
    }

    public virtual void Interact()
    {
        Transform itemInHand = PlayerPickup.Instance.GetCurrentPickedUpItem();
        if (!PlayerPickup.Instance.HasItemInHand() && !PlayerPickup.Instance.HasIngredientInHand() && remainingItemsOnOutput > 0)
        {
            Debug.Log("Pas d'ingr�dient en main");
            // Pas d'item en main
            remainingItemsOnOutput--;

            GameObject ingredient = Instantiate(outputIngredient.ingredientPrefab);
            PlayerPickup.Instance.SetCurrentItem(ingredient);
            OnPickupIngredient?.Invoke(this, remainingItemsOnOutput);
        }
        else if (!PlayerPickup.Instance.HasItemInHand() && remainingItemsOnOutput > 0)
        {

        }
        else
        {
            if (currentIngredient.Equals(IngredientDatabase.Instance.GetUnknownIngredientData()))
            {
                Ingredient ingredient = itemInHand.GetComponent<Ingredient>();

                if(ingredient != null)
                {
                    int ingredientID = ingredient.GetID();

                    IngredientData ingredientData = IngredientDatabase.Instance.SearchIngredientByIndex(ingredientID);

                    if (ingredientData == null)
                        Debug.LogError("Ingredient not found in database");

                    // Si un ingr�dient sur la planche est d�j� pr�sent
                    if(currentIngredient.Equals(IngredientDatabase.Instance.GetUnknownIngredientData()))
                    {
                        // S'il existe une recette avec l'ingr�dient en question
                        if(CheckForRecipe(ingredientData.ID, out RecipeData outputRecipe))
                        {
                            currentIngredient       = ingredientData;
                            currentRecipe           = outputRecipe;
                            outputIngredient        = IngredientDatabase.Instance.SearchIngredientByIndex(outputRecipe.outputIngredientId);
                            remainingItemsOnOutput  = 0;
                            outputIngredientAmount  = 0;

                            this.currentIngredientInput = Instantiate(currentIngredient.ingredientPrefab, contentInputParent, false).GetComponent<Ingredient>();
                            currentIngredientInput.transform.localPosition = new Vector3(0f, 0.1f, 0f);

                            OnIngredientChanged?.Invoke(this, new OnIngredientChangedEventArgs
                            {
                                ingredientData = ingredientData,
                                outputIngredientData = outputIngredient,
                            });

                            PlayerPickup.Instance.EmptyHand();
                        }
                    }

                } else
                {
                    Debug.LogError("An error has occured");
                }
                // We have no ingredient yet
                // Get the ingredient on the object with a script that contains the ID 
            }
        }
    }

    public virtual void EndProgression()
    {
        outputIngredientAmount++;
        remainingItemsOnOutput++;

        if(outputIngredientAmount == currentIngredient.maxUseIngredient)
        {
            Destroy(currentIngredientInput.gameObject);
            currentIngredient       = IngredientDatabase.Instance.GetUnknownIngredientData();
            currentIngredientInput  = null;
            currentRecipe           = null;
            OnStartPickup?.Invoke(this, outputIngredientAmount);
            OnCraftIngredient?.Invoke(this, currentIngredient.maxUseIngredient - outputIngredientAmount);
            cuttingEffect.SendEvent("OnFinishCut");
        } else
        {
            OnCraftIngredient?.Invoke(this, currentIngredient.maxUseIngredient - outputIngredientAmount);
        }
    }

    protected bool CheckForRecipe(int ingredientID, out RecipeData recipeData)
    {
        IngredientRequirement requirement = new IngredientRequirement() { ingredientID = ingredientID, quantity = 1 };

        return RecipeDatabase.Instance.FindRecipe(this.kitchenToolType, new IngredientRequirement[] { requirement }, out recipeData);
    }

    public virtual void InteractHold()
    {
        Debug.LogError("Interact Hold not implemented");
    }
    
    public void StartProgression(float progressNormalized)
    {
        cuttingEffect.SendEvent("OnStartCut");
        OnProgressChanged.Invoke(this, new OnProgressChangedEventArgs
        {
            progressNormalized = progressNormalized
        });
    }

    public void CancelHoldInteraction()
    {
        OnProgressionCancelled?.Invoke(this, EventArgs.Empty);
        cuttingEffect.SendEvent("OnFinishCut");
    }

    public void EmptyIngredients()
    {
        // ingredients.Clear();
        currentIngredient = IngredientDatabase.Instance.GetUnknownIngredientData();
    }

    public virtual void OnSelect()
    {

    }

    public virtual void OnDeselect()
    {

    }
}

[System.Serializable]
public enum KitchenToolType
{
    CUTTING = 0,
    COOKING = 1,
    MAKING = 2,
}
