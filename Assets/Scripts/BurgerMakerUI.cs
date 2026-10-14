using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class BurgerMakerUI : MonoBehaviour
{
    public static BurgerMakerUI Instance;
    [SerializeField] private BurgerMaker currentBurgerMaker;
    [SerializeField] private RectTransform burgerUIPrefab;

    [Header("UI Variables")]
    [SerializeField] private Transform burgerParent;
    [SerializeField] private RectTransform listParent;

    [SerializeField] private RectTransform contentToScaleAnimation;

    [SerializeField] private Image backgroundUI;
    [SerializeField] private Color backgroundColor;

    public bool isEnabled { get; private set; }

    private void Awake()
    {
        Instance = this;
        isEnabled = false;
    }

    public void SetBurgerMaker(BurgerMaker burgerMaker)
    {
        this.currentBurgerMaker = burgerMaker;
        InitializeUI();
    }

    private void InitializeUI()
    {
        if(currentBurgerMaker == null)
        {
            return;
        }

        DestroyUI();

        List<RecipeData> recipesMaking = RecipeDatabase.Instance.FindRecipeFromKitchenTool(KitchenToolType.MAKING);

        RecipeData recipeData = currentBurgerMaker.currentSelectedRecipeData;
        Debug.Log("coucou" + recipeData);

        foreach(RecipeData recipe in recipesMaking)
        {
            BurgerUI burgerUI = Instantiate(burgerUIPrefab, listParent).GetComponent<BurgerUI>();
            bool isSelected = false;
            if(recipeData != null && recipeData.outputIngredientId == recipe.outputIngredientId)
            {
                isSelected = true;
            }

            burgerUI.Setup(recipe, isSelected);

            burgerUI.OnRecipeSelected += BurgerUI_OnRecipeSelected;
        }
        
    }

    private void BurgerUI_OnRecipeSelected(object sender, RecipeArgs args)
    {
        if (this.currentBurgerMaker != null)
        {
            currentBurgerMaker.SetBurgerRecipe(args.outputRecipeData);
            Debug.Log("We've chosen the recipe of output id " + args.outputRecipeData.outputIngredientId);
            Hide();
        }
        else
        {
            Debug.Log("current burger maker = null");
        }
    }

    private void DestroyUI()
    {
        foreach(RectTransform rectTransform in listParent.GetComponentInChildren<RectTransform>())
        {
            BurgerUI burgerUI = rectTransform.gameObject.GetComponent<BurgerUI>();

            burgerUI.OnRecipeSelected -= BurgerUI_OnRecipeSelected;

            Destroy(rectTransform.gameObject);
        }
    }

    public void Hide()
    {
        SetBurgerMaker(null);

        LeanTween.alphaCanvas(backgroundUI.GetComponent<CanvasGroup>(), 0f, 0.2f);

        contentToScaleAnimation.localScale = Vector3.one;

        // Scale animation
            LeanTween.scale(contentToScaleAnimation, Vector3.zero, 0.4f).setEase(LeanTweenType.easeOutQuint).setOnComplete(() => {
                this.burgerParent.gameObject.SetActive(false);
                isEnabled = false;
            });

    }

    public void Show()
    {

        this.burgerParent.gameObject.SetActive(true);
        contentToScaleAnimation.localScale = Vector3.zero;
        
        LeanTween.alphaCanvas(backgroundUI.GetComponent<CanvasGroup>(), 1f, 0.05f);

        // Scale animation
            LeanTween.scale(contentToScaleAnimation, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack).setOnComplete(() => {
                isEnabled = true;
            });
    }

    private void Update()
    {
        if(isEnabled && Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
    }

}
