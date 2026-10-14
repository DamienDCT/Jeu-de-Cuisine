using UnityEngine;
using UnityEngine.UI;

public class BurgerLayerUI : MonoBehaviour
{
    [SerializeField] private Image ingredientImage;

    [SerializeField] private Color layerNotCompletedColor;
    [SerializeField] private Color layerCompletedColor;

    public void SetupLayer(IngredientData ingredientData)
    {
        if (ingredientData.ingredientSprite == null)
        {
            ingredientImage.color = Random.ColorHSV();

            return;
        }

        ingredientImage.sprite = ingredientData.ingredientSprite;
        ingredientImage.color = layerNotCompletedColor;
        // Make opacity to 0.5 (not completed yet)
    }

    public void SetCompleted()
    {
        // Make opacity to 1
        //ingredientImage.color
        ingredientImage.color = layerCompletedColor;
    }

    public void Reverse()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
    }
}
