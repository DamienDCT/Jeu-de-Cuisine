using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShoppingCartUI : MonoBehaviour
{
    [SerializeField] private ShoppingCart shoppingCart;

    [SerializeField] private RectTransform parentPanel;
    [SerializeField] private RectTransform cartItemPrefab;

    [SerializeField] private RectTransform contentPanel;

    [SerializeField] private TextMeshProUGUI noItemsText;
    [SerializeField] private TextMeshProUGUI totalText;

    [SerializeField] private Button orderCartButton;

    private void Awake()
    {
        shoppingCart.OnListChanged += ShoppingCart_OnListChanged;
        orderCartButton.onClick.AddListener(OrderCart);
       // Hide();
    }

    private void OrderCart()
    {
        if (shoppingCart != null)
        {
            shoppingCart.OrderCart();
        }
    }

    private void ShoppingCart_OnListChanged(object sender, EventArgs e)
    {
        ShoppingCart _shoppingCart = (ShoppingCart)sender;

        if (_shoppingCart == null)
            return;

        if(_shoppingCart.listOfIngredients.Count == 0)
        {
            // Disable the order button
            orderCartButton.interactable = false;
            return;
        } else
        {
            orderCartButton.interactable = true;
        }

        UpdateVisuals(_shoppingCart.listOfIngredients);
        noItemsText.gameObject.SetActive(shoppingCart.IsCartEmpty());
    }

    public void Hide()
    {
        contentPanel.gameObject.SetActive(false);
    }

    public void Show()
    {
        if(shoppingCart != null)
        {
            // Désactiver le bouton si aucun élément est à commander
            orderCartButton.interactable = shoppingCart.listOfIngredients.Count > 0;
        }
        contentPanel.gameObject.SetActive(true);
    }

    private void UpdateVisuals(Dictionary<IngredientData, int> cartList)
    {
        if (shoppingCart == null)
        {
            return;
        }


        CartTemplateUI[] rectTemplates = parentPanel.GetComponentsInChildren<CartTemplateUI>();
        float amountDollars = 0f;
        rectTemplates = parentPanel.GetComponentsInChildren<CartTemplateUI>();
        foreach (KeyValuePair<IngredientData, int> data in cartList)
        {
            amountDollars += (data.Key.ingredientPrice * data.Value);
            bool isOnUI = false;
            CartTemplateUI templateToUpdate = null;
            foreach(CartTemplateUI template in rectTemplates)
            {
                if(template.GetIngredientData() == data.Key)
                {
                    isOnUI = true;
                    templateToUpdate = template;
                }
            }

            // Update quantity
            if(isOnUI && templateToUpdate != null)
            {
                templateToUpdate.UpdateVisual(data.Key, data.Value);
            // Just add it
            } else
            {
                CartTemplateUI template = Instantiate(cartItemPrefab, parentPanel).GetComponent<CartTemplateUI>();
                Debug.Log(data.Key);
                template.SetupCartTemplate(data.Value, data.Key);
                template.UpdateVisual(data.Key, data.Value);
            }
        }

        totalText.text = "Total = $" + amountDollars;
    }

  
}
