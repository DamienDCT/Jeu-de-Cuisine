using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngredientDatabase : MonoBehaviour
{
    public static IngredientDatabase Instance;

    public const int BREAD_INGREDIENT_ID = 0;

    [SerializeField] private IngredientDataSO ingredientsDatabase;

    [SerializeField] private IngredientData unknownIngredientData;

    private void Awake()
    {
        Instance = this;
    }

    public IngredientData SearchIngredientByIndex(int index)
    {
        return ingredientsDatabase.ingredientsData.Where(i => i.ID == index).FirstOrDefault();
    }

    public IngredientData GetUnknownIngredientData()
    {
        return unknownIngredientData;
    }

    public List<IngredientData> GetAllIngredients()
    {
        return ingredientsDatabase.ingredientsData;
    }

    public List<IngredientData> FilterIngredients(string filterIngredientName)
    {
        return ingredientsDatabase.ingredientsData
            .Where(t => t.ingredientName != null &&
                        t.ingredientName.IndexOf(filterIngredientName, StringComparison.OrdinalIgnoreCase) >= 0)
            .ToList();
    }
}
