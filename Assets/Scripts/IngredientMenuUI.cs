using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientMenuUI : MonoBehaviour
{
    [SerializeField] private RectTransform gridDisplayLayout;
    [SerializeField] private RectTransform listDisplayLayout;



    [SerializeField] private Button swapGridLayoutButton;

    [SerializeField] private RectTransform popupQte;
    [SerializeField] private RectTransform popupOverlayBcg;

    [Header("Buttons")]
    [SerializeField] private Button minusQteButton;
    [SerializeField] private Button addQteButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button addTenQteButton;
    [SerializeField] private Button minusTenQteButton;

    [Header("Popup Fields")]
    [SerializeField] private TextMeshProUGUI titleIngredientText;
    [SerializeField] private TextMeshProUGUI priceIngredientText;
    [SerializeField] private TextMeshProUGUI qteIngredientText;
    [SerializeField] private TextMeshProUGUI totalPriceText;
    [SerializeField] private Image ingredientSprite;

    [Header("Prefab Ingredient Template")]
    [SerializeField] private RectTransform ingredientTemplateGridPrefab;
    [SerializeField] private RectTransform ingredientTemplateListPrefab;

    [Space]
    [Header("Panel for prefabs")]
    [SerializeField] private RectTransform gridPanelParent;
    [SerializeField] private RectTransform linePanelParent;

    [Space]
    [Header("Search references")]
    [SerializeField] private TMP_InputField searchTextField;
    [SerializeField] private Button searchButton;

    private int currentQuantityNeeded;
    private float totalPrice;
    private IngredientData currentIngredientToBuy;

    private bool isGridLayout;
    private bool isPopupOpened;

    private bool hasAllItems;

    private void Awake()
    {
        // Initialization
        isGridLayout = true;
        isPopupOpened = false;
        currentQuantityNeeded = 0;
        totalPrice = 0f;
        currentIngredientToBuy = null;

        // Add listenres
        minusQteButton.onClick.AddListener(() => RemoveQuantity());
        addQteButton.onClick.AddListener(() => AddQuantity());

        minusTenQteButton.onClick.AddListener(() => RemoveQuantity(10));
        addTenQteButton.onClick.AddListener(() => AddQuantity(10));
      //  swapGridLayoutButton.onClick.AddListener(SwapLayoutDisplay);
        buyButton.onClick.AddListener(BuyIngredient);



        searchButton.onClick.AddListener(SearchIngredients);
    }

    private void Start()
    {
        SetupUI(IngredientDatabase.Instance.GetAllIngredients());
    }

    private void SearchIngredients()
    {
        string searchFilter = searchTextField.text;
        List<IngredientData> allIngredients = IngredientDatabase.Instance.GetAllIngredients();
        if (string.IsNullOrEmpty(searchFilter) && !hasAllItems)
        {
            SetupUI(allIngredients); return;
        } else
        {
            hasAllItems = false;
            List<IngredientData> allIngredientsFiltered = IngredientDatabase.Instance.FilterIngredients(searchFilter);
            int allIngredientsSize = allIngredients.Count;

            SetupUI(allIngredientsFiltered, allIngredientsFiltered.Count == allIngredientsSize);
        }
    }

    private void SetupUI(List<IngredientData> ingredientsToDisplay, bool _hasAllItems = true)
    {
        // Clear both UI
        ClearUI();

        foreach(IngredientData ingredient in ingredientsToDisplay)
        {
            if (!ingredient.canBeBuy)
                continue;
            // Setup grid layout
            //IngredientTemplate ingredientTemplateGrid = Instantiate(ingredientTemplateGridPrefab, gridPanelParent)
            //                                                        .GetComponent<IngredientTemplate>();

            // Setup line layout
            IngredientTemplate ingredientTemplateList = Instantiate(ingredientTemplateListPrefab, linePanelParent)
                                                                    .GetComponent<IngredientTemplate>();

          //  ingredientTemplateGrid.Setup(this, ingredient);

            ingredientTemplateList.Setup(this, ingredient);
        }

        hasAllItems = _hasAllItems;



    }

    private void ClearUI()
    {
        //foreach(RectTransform _transform in gridPanelParent.GetComponentInChildren<RectTransform>())
        //{
        //    Destroy(_transform.gameObject);
        //}

        foreach (RectTransform _transform in linePanelParent.GetComponentInChildren<RectTransform>())
        {
            Destroy(_transform.gameObject);
        }
    }

    private void BuyIngredient()
    {
        if (currentIngredientToBuy == null)
            return;

        ShoppingCart.Instance.AddItemToCart(currentIngredientToBuy, currentQuantityNeeded);
        ClosePopup();
    }

    private void RemoveQuantity(int quantity = 1)
    {
        if (currentIngredientToBuy == null)
            return;
        if (currentQuantityNeeded == 0)
            return;
        currentQuantityNeeded = Mathf.Max(0, currentQuantityNeeded - quantity);

        qteIngredientText.text = "" + currentQuantityNeeded;
        totalPrice = currentQuantityNeeded * currentIngredientToBuy.ingredientPrice;
        totalPriceText.text = "$" + totalPrice + " / u";
    }

    private void AddQuantity(int quantity = 1)
    { 
        if (currentIngredientToBuy == null)
            return;
        currentQuantityNeeded += quantity;

        qteIngredientText.text = "" + currentQuantityNeeded;
        totalPrice = currentQuantityNeeded * currentIngredientToBuy.ingredientPrice;
        totalPriceText.text = "$" + totalPrice + " / u";
    }

    private void Update()
    {
        if (!isPopupOpened)
            return;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();
        }
    }

    private void PreparePopup(int ingredientID)
    {
        IngredientData data = IngredientDatabase.Instance.SearchIngredientByIndex(ingredientID);
        currentIngredientToBuy = data;
        titleIngredientText.text = data.ingredientName;
        priceIngredientText.text = "$" + data.ingredientPrice + " / u";
        qteIngredientText.text = "0";
        totalPriceText.text = "Total : $0";
        currentQuantityNeeded = 0;
        totalPrice = 0f;
        ingredientSprite.sprite = data.ingredientSprite;
    }

    public void OpenPopup(int ingredientID)
    {
        PreparePopup(ingredientID);
        LeanTween.scale(popupQte.gameObject, Vector3.one, 0.4f).setEaseOutBounce();
        popupOverlayBcg.gameObject.SetActive(true);
        TabletManager.Instance.SetFlagPopupOpen(true);
        isPopupOpened = true;
    }


    public void ClosePopup()
    {
        LeanTween.scale(popupQte.gameObject, Vector3.zero, 0.3f).setEaseOutQuint();
        popupOverlayBcg.gameObject.SetActive(false);
        isPopupOpened = false;
      //  yield return new WaitForEndOfFrame();
        TabletManager.Instance.SetFlagPopupOpen(false);
    }

    //private void SwapLayoutDisplay()
    //{
    //    gridDisplayLayout.gameObject.SetActive(!isGridLayout);
    //    listDisplayLayout.gameObject.SetActive(isGridLayout);
    //    isGridLayout = !isGridLayout;
    //}
}
