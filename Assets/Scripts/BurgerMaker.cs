using UnityEngine;
using System.Collections.Generic;
using System;

public class BurgerMaker : KitchenTool
{
    private List<IngredientData> ingredientsOn;

    private float currentYoffset;
    private int currentIngredientIndexNeeded;

    public RecipeData currentSelectedRecipeData { get; private set; }

    public event EventHandler<RecipeArgs> OnRecipeSelected;
    public event EventHandler OnNextBurgerLayerCompleted;

    public event EventHandler OnShow;
    public event EventHandler OnHide;

    [SerializeField] private CanvasModifier canvasModifier;
    [SerializeField] private GameObject bottomBurgerBunPrefab;

    private Dish currentDish;

    private void Awake()
    {
        ingredientsOn                   = new List<IngredientData>();
        currentYoffset                  = 0f;
        currentIngredientIndexNeeded    = 0;
        currentDish                     = null; 
    }

    public override void Interact()
    {
        if(!BurgerMakerUI.Instance.isEnabled && !PlayerPickup.Instance.HasItemInHand() && ingredientsOn.Count == 0)
        {
            BurgerMakerUI.Instance.SetBurgerMaker(this);
            BurgerMakerUI.Instance.Show();

            return;
        }

        // Si on utilise un ingrédient sur le burger maker mais aucune recette sélectionnée
        if (currentSelectedRecipeData == null)
            return;

        GameObject _ingredient = PlayerPickup.Instance.GetCurrentPickedUpItem()?.gameObject;

        // Si on a bien un ingrédient en main
        if(_ingredient != null && _ingredient.TryGetComponent<Ingredient>(out Ingredient ingredient))
        {
            int ingredientInHandID = ingredient.GetID();

            IngredientData ingredientData = IngredientDatabase.Instance.SearchIngredientByIndex(ingredientInHandID);
            
            // Si l'ingrédient n'est pas dans la DB
            if (ingredientData == null)
                return;

            if (!IsIngredientCorrect(ingredientInHandID))
            {
                Debug.Log("Ingredient incorrect");
                return;
            }

            ingredientsOn.Add(ingredientData);
 

            OnNextBurgerLayerCompleted?.Invoke(this, EventArgs.Empty);

            PlayerPickup.Instance.EmptyHand(false);



            PlaceIngredientOn(ingredient);

            // Pain à l'envers
            if(currentIngredientIndexNeeded == 0)
            {
                _ingredient.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
            }
            currentIngredientIndexNeeded++;
            //ingredientData.ingredientPrefab;



        } else if(_ingredient == null)
        {
            if(CheckRecipeComplete())
            {
                Debug.Log("we've completed the recipe !!!");
            } else
            {
                Debug.Log("recipe not completed yet");
            }
        }

    }

    private bool CheckRecipeComplete()
    {
        if (currentSelectedRecipeData.ingredientsNeeded.Count != ingredientsOn.Count)
            return false;

        
        for(int i = 0; i < currentSelectedRecipeData.ingredientsNeeded.Count; i++)
        {
            if (ingredientsOn[i].ID != currentSelectedRecipeData.ingredientsNeeded[i].ingredientID)
                return false;
        }
        return true;
    }

    private void PlaceIngredientOn(Ingredient ingredient)
    {
        // if(currentIngredientIndexNeeded > 0)
        /*        // {
                    currentYoffset += 0.4f;
                    ingredient.EnableScaleOnKitchenObject();
                    ingredient.EnablePhysics();
              //  }

                
                ingredient.transform.localPosition = new Vector3(0f, currentYoffset, 0f);*/
        if (currentDish == null)
            return;
        currentDish.UpdateDishVisuals(currentIngredientIndexNeeded);

    }

    public override void InteractHold()
    {
        return;
    }

    private bool IsIngredientCorrect(int currentIngredientID)
    {
        if (currentIngredientIndexNeeded >= currentSelectedRecipeData.ingredientsNeeded.Count)
            return false;
        int ingredientID = currentSelectedRecipeData.ingredientsNeeded[currentIngredientIndexNeeded].ingredientID;
        return currentIngredientID == ingredientID;
    }
    private void SetupDishOn()
    {
        if (currentSelectedRecipeData?.DishPrefab != null)
        {
            GameObject instantiatedDish = Instantiate(currentSelectedRecipeData.DishPrefab);
            instantiatedDish.transform.SetParent(contentInputParent, false);
            instantiatedDish.transform.localPosition = new Vector3(0f, currentYoffset, 0f);
            currentDish = instantiatedDish.GetComponent<Dish>();
        }
    }

    public void SetBurgerRecipe(RecipeData recipeData)
    {
        this.currentSelectedRecipeData = recipeData;
        this.currentDish = recipeData?.DishPrefab.GetComponent<Dish>();
        OnRecipeSelected?.Invoke(this, new RecipeArgs
        {
            outputRecipeData = recipeData
        });
        OnSelect();
        EnableWorldSpaceCanvas();
        //SetupDishOn();
    }

    private void DisableWorldSpaceCanvas()
    {
        OnHide?.Invoke(this, EventArgs.Empty);

    }

    private void EnableWorldSpaceCanvas()
    {
        OnShow?.Invoke(this, EventArgs.Empty);
    }

    public override void OnSelect()
    {
        if(currentSelectedRecipeData != null)
        {
            /*  screenSpaceCanvas.gameObject.SetActive(true);*/
            canvasModifier.ToggleZoomIn();
        }
    }

    public override void OnDeselect()
    {
        /*    screenSpaceCanvas.gameObject.SetActive(false);*/
        if(currentSelectedRecipeData != null)
        {
            canvasModifier.ToggleZoomOut();
        }
    }
}
