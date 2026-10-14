using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Unity.VisualScripting;

public class RecipeDatabase : MonoBehaviour
{
    public static RecipeDatabase Instance;
    [SerializeField] private List<RecipeData> allRecipes;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LoadRecipesFromFile("Recipes");
    }

    private void LoadRecipesFromFile(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);
        if (jsonFile == null)
        {
            Debug.LogError($"JSON introuvable : {fileName}");
            return;
        }

        allRecipes = JsonConvert.DeserializeObject<RecipeDataListWrapper>(jsonFile.text).recipes;
        Debug.Log($"✅ {allRecipes.Count} recettes chargées");

        foreach (var recipe in allRecipes)
        {
            recipe.LoadPrefab();
        //    Debug.Log($"Recette {recipe.outputIngredientId} — prefab : {(recipe.DishPrefab != null ? "✅ OK" : "❌ NULL")}");
        }
    }

    public bool FindRecipe(KitchenToolType kitchenToolType, IngredientRequirement[] ingredientRequired, out RecipeData outputIngredient)
    {
        // Erreur de chargement des données
        if(allRecipes.Count <= 0)
        {
            Debug.LogError("Erreur lors de chargement des données");
            outputIngredient = null;
            return false;
        }

        // On récup les recettes valides par le type demandé
            List<RecipeData> listFiltered = allRecipes.Where(r => r.requiredToolType == kitchenToolType).ToList();

        // Si on en a pas, on renvoie false
            if (listFiltered.Count == 0)
            {
                outputIngredient = null;
                return false;
            }

        // On regarde dans la liste s'il y a une recette adéquate
            foreach(RecipeData recipe in listFiltered)
            {
                if(ingredientRequired.Length == recipe.ingredientsNeeded.Count)
                {
                    bool isCorrect = true;
                    for(int i = 0; i <  ingredientRequired.Length; i++)
                    {
                        if (recipe.ingredientsNeeded[i].ingredientID != ingredientRequired[i].ingredientID
                            || recipe.ingredientsNeeded[i].quantity != ingredientRequired[i].quantity)
                        {
                            isCorrect = false;
                            break;
                        }
                    }
                    if(isCorrect)
                    {
                        outputIngredient = recipe;
                        return true;
                    }
                }
            }

        outputIngredient = null;
        return false;

    }

    public List<RecipeData> FindRecipeFromKitchenTool(KitchenToolType kitchenToolType)
    {
        return allRecipes.Where(recipe => recipe.requiredToolType == kitchenToolType).ToList();
    }

    // Classe wrapper pour désérialiser une liste avec JsonUtility
    [System.Serializable]
    private class RecipeDataListWrapper
    {
        public List<RecipeData> recipes;
    }


}
[System.Serializable]
public class RecipeData
{
    public KitchenToolType requiredToolType;
    public int outputIngredientId;
    public List<IngredientRequirement> ingredientsNeeded;
    public string dishPrefabName;         

    // Chargé au runtime, pas depuis le JSON
    [JsonIgnore] public GameObject DishPrefab { get; private set; }

    public void LoadPrefab()
    {
        if (string.IsNullOrEmpty(dishPrefabName)) return;
        DishPrefab = Resources.Load<GameObject>($"Dishes/{dishPrefabName}");
        if (DishPrefab == null)
            Debug.LogWarning($"Prefab introuvable : Dishes/{dishPrefabName}");
    }
}


[System.Serializable]
public class IngredientRequirement
{
    public int ingredientID;   // Ingredient ID
    public int quantity; // Required quantity
}