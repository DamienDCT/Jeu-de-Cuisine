using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class IngredientDataSO : ScriptableObject
{
    public List<IngredientData> ingredientsData;
}

[System.Serializable]
public class IngredientData
{
    public string ingredientName;
    public float ingredientPrice;
    public int ID;
    public Sprite ingredientSprite;
    public GameObject ingredientPrefab;
    public float ingredientCookTime;
    public int maxUseIngredient;
    public bool canBeBuy;

    public override bool Equals(object obj)
    {
        IngredientData data = (IngredientData)obj;
        return data.ingredientName == this.ingredientName;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
