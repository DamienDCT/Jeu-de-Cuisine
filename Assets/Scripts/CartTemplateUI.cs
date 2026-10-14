using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CartTemplateUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private int indexToDelete;

    private IngredientData ingredientData;


    [Header("Buttons")]
    [SerializeField] private Button deleteButton;

    [Header("UI Fields")]
    [SerializeField] private TextMeshProUGUI ingredientText;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Image ingredientSprite;

    private void Awake()
    {
        indexToDelete = -1;
    }

    public void SetupCartTemplate(int index, IngredientData ingredientData)
    {
        indexToDelete = index;
        this.ingredientData = ingredientData;
        Debug.Log("we setup the template");
        Debug.Log(this.ingredientData);
        deleteButton.onClick.AddListener(DeleteFromCart);
    }

    public void DeleteFromCart()
    {
        if (ingredientData == null)
        {
            Debug.Log("it's null");
            return;
        }

        Debug.Log("we destroy");

        ShoppingCart.Instance.DeleteFromCart(ingredientData);
        Destroy(this.gameObject);
    }

    public IngredientData GetIngredientData()
    {
        return this.ingredientData;
    }
    // Utiliser ces templates pour reformer le cart. 
    // Ajouter un event pour supprimer un élément du cart sans pb avec l'index stored

    public void UpdateVisual(IngredientData ingredientData, int quantity)
    {
        ingredientText.text = ingredientData.ingredientName;
        quantityText.text = "x" + quantity;
        ingredientSprite.sprite = ingredientData.ingredientSprite;
        this.ingredientData = ingredientData;
        Debug.Log(this.ingredientData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        deleteButton.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        deleteButton.gameObject.SetActive(false);
    }
}
