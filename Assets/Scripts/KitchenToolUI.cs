using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.LightTransport;
using TMPro;

public class KitchenToolUI : MonoBehaviour
{
    [SerializeField] private KitchenTool kitchenTool;

    [Header("Transparent Sprite")]
    [SerializeField] private Sprite transparentSprite;
    

    [Space]

    [Header("UI Progress Bar")]
    [SerializeField] private Image progressionImage;

    [SerializeField] private float fillAmount        = 0f;

    [SerializeField] private bool isProgressing      = false;
    [SerializeField] private float progressObjective = 0f;

    [SerializeField] private Gradient colorGradient;
    [SerializeField] private RectTransform progressBarParent;

    [Header("UI Main Attributes")]
    [SerializeField] private RectTransform globalParent;

    [Space]
    [Space]
    [Header("UI Ingredient On")]
    [SerializeField] private Image ingredientSpriteImage;
    [SerializeField] private Image outputIngredientSprite;

    [Space]
    [Header("UI Camembert")]
    [SerializeField] private Image imageProgressive;
    [SerializeField] private Image imagePieChart;

    [Header("UI Pickup Remaining")]
    [SerializeField] private TextMeshProUGUI amountIngredientRemainingText;

    

    private IngredientData currentIngredientData;
    private int currentIngredientNbUse;
    

    private void Awake()
    {
        fillAmount              = 0f;
        progressObjective       = 0f;
        currentIngredientData   = null;
        currentIngredientNbUse  = 0;
    }

    private void Start()
    {
        kitchenTool.OnProgressChanged += KitchenTool_OnProgressChanged;
        kitchenTool.OnIngredientChanged += KitchenTool_OnIngredientChanged;
        kitchenTool.OnProgressionCancelled += KitchenTool_OnProgressionCancelled;
        kitchenTool.OnStartPickup += KitchenTool_OnStartPickup;
        kitchenTool.OnCraftIngredient += KitchenTool_OnCraftIngredient;
        kitchenTool.OnPickupIngredient += KitchenTool_OnPickupIngredient;
    }



    private void OnDisable()
    {
        kitchenTool.OnProgressChanged -= KitchenTool_OnProgressChanged;
        kitchenTool.OnIngredientChanged -= KitchenTool_OnIngredientChanged;
        kitchenTool.OnProgressionCancelled -= KitchenTool_OnProgressionCancelled;
        kitchenTool.OnStartPickup -= KitchenTool_OnStartPickup;
        kitchenTool.OnCraftIngredient -= KitchenTool_OnCraftIngredient;
        kitchenTool.OnPickupIngredient -= KitchenTool_OnPickupIngredient;
    }

    private void KitchenTool_OnPickupIngredient(object sender, int nbIngredientRemaining)
    {
        ChangeAmountIngredientText(nbIngredientRemaining);
    }

    private void KitchenTool_OnCraftIngredient(object sender, int ingredientRemaining)
    {
        currentIngredientNbUse--;
        float progression = (float)currentIngredientNbUse / (float)this.currentIngredientData.maxUseIngredient;
        imageProgressive.fillAmount = progression;

        ChangeAmountIngredientText(currentIngredientData.maxUseIngredient - currentIngredientNbUse);
    }

    private void KitchenTool_OnIngredientChanged(object sender, KitchenTool.OnIngredientChangedEventArgs args)
    {
        if (isProgressing)
            return;

        IngredientData ingredientData = args.ingredientData;
        this.currentIngredientData = ingredientData;
        this.currentIngredientNbUse = currentIngredientData.maxUseIngredient;

        ingredientSpriteImage.sprite = ingredientData.ingredientSprite;

        Material _material = new Material(imagePieChart.material);
        _material.SetInt("_partAmount", ingredientData.maxUseIngredient);

        imagePieChart.material = _material;

        globalParent.gameObject.SetActive(true);
        imageProgressive.fillAmount = 1f;

        // output ingredient
        outputIngredientSprite.sprite = args.outputIngredientData.ingredientSprite;

        //  amountIngredientRemaining.text = "" + ingredientData.maxUseIngredient;
    }

    private void KitchenTool_OnProgressChanged(object sender, KitchenTool.OnProgressChangedEventArgs e)
    {
        if (!isProgressing)
        {
            StartProgression(e.progressNormalized);
        }
    }

    public void StartProgression(float objectiveProgress = 0.5f)
    {
        isProgressing       = true;
        //fillAmount          = 0f;
        progressObjective   = objectiveProgress;
    }

    private void KitchenTool_OnProgressionCancelled(object sender, EventArgs e)
    {
        isProgressing = false;
        ResetUIProgression();
    }

    private void KitchenTool_OnStartPickup(object sender, int nbItemRemaining)
    {
        isProgressing = false;
        ResetUIProgression();
        ResetUI();
        ChangeAmountIngredientText(nbItemRemaining);
       // amountIngredientRemaining.text = "0";
    }

    private void Update()
    {
        if (!isProgressing)
            return;

        UpdateUIProgressBar();

    }

    private void UpdateUIProgressBar()
    {
        // Increment the fill amount proportionally to the progress duration
        fillAmount += Time.deltaTime / progressObjective;
        float lerpedPercentage = Mathf.Clamp(fillAmount, 0f, 1f);

        // Update the fill amount and color of the progress bar
        progressionImage.fillAmount = lerpedPercentage;
        Color color = colorGradient.Evaluate(lerpedPercentage);
        progressionImage.color = color;

        // Stop progressing if we've completed the progress
        if (lerpedPercentage >= 1f)
        {
           // isProgressing = false;
            fillAmount = 0f; // Reset fillAmount to prepare for the next progress
            kitchenTool.EndProgression();


        }
    }

    private void ResetUIProgression()
    {
        progressionImage.fillAmount = fillAmount;
        Color color = colorGradient.Evaluate(fillAmount);
        progressionImage.color = color;
    }

    private void ResetUI()
    {
        ingredientSpriteImage.sprite = transparentSprite;
    }

    private void ChangeAmountIngredientText(int amountIngredientRemaining)
    {
        amountIngredientRemainingText.text = "" + amountIngredientRemaining;
    }

    // TODO : Foutre ça sur chat gpt, pcq les serveurs ont crash sa mère la pute j'suis dépité
}
