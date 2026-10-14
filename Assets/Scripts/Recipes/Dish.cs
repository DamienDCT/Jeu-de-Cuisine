using UnityEngine;
using System.Collections.Generic;

public class Dish : MonoBehaviour
{
    [SerializeField] private List<Ingredient> ingredientList;

    public void UpdateDishVisuals(int currentIndex)
    {
        if (currentIndex >= ingredientList.Count)
            return;

        ingredientList[currentIndex].gameObject.SetActive(true);
    }
}
