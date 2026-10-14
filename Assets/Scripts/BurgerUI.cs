using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BurgerUI : MonoBehaviour
{
    [Header("UI Variables")]
    [SerializeField] private TextMeshProUGUI burgerName;
    [SerializeField] private Button selectButton;
    [SerializeField] private Image burgerSprite;
    [SerializeField] private TextMeshProUGUI selectedText;

    public event EventHandler<RecipeArgs> OnRecipeSelected;

    private RecipeData currentRecipeData;

    public void Setup(RecipeData recipeData, bool isSelected)
    {
        Debug.Log(recipeData.outputIngredientId);
        IngredientData outputBurger = IngredientDatabase.Instance.SearchIngredientByIndex(recipeData.outputIngredientId);
        if(outputBurger == null)
        {
            Debug.Log("it's null");
            return;
        }

        if(!outputBurger.Equals(IngredientDatabase.Instance.GetUnknownIngredientData()))
        {
            burgerName.text = outputBurger.ingredientName;
        }
        
        if(isSelected)
        {
            selectButton.gameObject.SetActive(false);
            selectedText.gameObject.SetActive(true);
        }

        this.currentRecipeData = recipeData;
        Debug.Log(outputBurger.ID);
      //  Debug.Log("we active the burger of recipe data : " + currentRecipeData.dishPrefabName);
      //  selectButton.onClick.AddListener(SelectBurger);
        //burgerName.text = ingredientData.ingredientName;
    }

    public void SelectBurger()
    {
        Debug.Log("coucou");
        if(currentRecipeData != null)
        {
            Debug.Log("we send event onrecipeselected");
            OnRecipeSelected?.Invoke(this, new RecipeArgs
            {
                outputRecipeData = currentRecipeData,
            });
        } else
        {
            Debug.Log("current recipedata = null");
        }
 
    }
}

[Serializable]
public class RecipeArgs : EventArgs
{
    public RecipeData outputRecipeData;
}
