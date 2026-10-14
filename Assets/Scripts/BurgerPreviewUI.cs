using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class BurgerPreviewUI : MonoBehaviour
{
    [SerializeField] private BurgerMaker burgerMaker;

    [SerializeField] private float offsetArrowByLayer;
    [SerializeField] private float offsetArrowByLayerOverlay;

    [SerializeField] private RectTransform content;

    [Space]
    [Header("Prefabs")]
    [SerializeField] private RectTransform burgerLayerUIPrefab;

    [Space]
    [Header("UI Variables")]
    [SerializeField] private RectTransform listParent;
    [SerializeField] private RectTransform arrowLayer;
    [Header("UI Screen Space Overlay")]
    [SerializeField] private RectTransform listParentOverlay;
    [SerializeField] private RectTransform arrowLayerOverlay;

    private List<BurgerLayerUI> burgerLayers;
    private List<BurgerLayerUI> burgerLayersOverlay;
    private int currentLayerIndex;



    private void Awake()
    {
        burgerLayers            = new List<BurgerLayerUI>();
        burgerLayersOverlay     = new List<BurgerLayerUI>();
        currentLayerIndex       = 0;
    }

    private void Start()
    {
        burgerMaker.OnRecipeSelected += BurgerMaker_OnRecipeSelected;
        burgerMaker.OnNextBurgerLayerCompleted += BurgerMaker_OnNextBurgerLayerCompleted;

        burgerMaker.OnHide += BurgerMaker_OnHide;
        burgerMaker.OnShow += BurgerMaker_OnShow;
    }

    private void BurgerMaker_OnHide(object sender, EventArgs e)
    {
        Hide();
    }

    private void BurgerMaker_OnShow(object sender, EventArgs e)
    {
        Show();  
    }

    private void BurgerMaker_OnNextBurgerLayerCompleted(object sender, EventArgs args)
    {
        // On met à completed le prochain layer
        BurgerLayerUI burgerLayerUI = burgerLayers[currentLayerIndex];
        burgerLayerUI.SetCompleted();
        // On regarde le prochain layer

        // On met à completed le prochain layer
        burgerLayerUI = burgerLayersOverlay[currentLayerIndex];
        burgerLayerUI.SetCompleted();


        if (currentLayerIndex + 1 < burgerLayers.Count)
        {
            burgerLayerUI = burgerLayers[currentLayerIndex + 1];
            SetArrowPosition(burgerLayerUI);
        }

        currentLayerIndex++;
    }

    private void BurgerMaker_OnRecipeSelected(object sender, RecipeArgs args)
    {
        SetupUI(args.outputRecipeData);
    }

    private void SetupUI(RecipeData recipeData)
    {
        if (recipeData == null)
            return;

        DestroyUI();
        burgerLayers.Clear();
        burgerLayersOverlay.Clear();

        for(int i = recipeData.ingredientsNeeded.Count - 1; i >= 0; i--)
        {
            IngredientRequirement ingredientRequirement = recipeData.ingredientsNeeded[i];
            IngredientData ingredientData = IngredientDatabase.Instance.SearchIngredientByIndex(ingredientRequirement.ingredientID);
            if(ingredientData == null)
            {
                Debug.Log(ingredientRequirement.ingredientID + " not exists in DB");
                continue;
            }

            BurgerLayerUI layer = Instantiate(burgerLayerUIPrefab, listParent).GetComponent<BurgerLayerUI>();
            burgerLayers.Add(layer);
            BurgerLayerUI _layer = Instantiate(burgerLayerUIPrefab, listParentOverlay).GetComponent<BurgerLayerUI>();
            burgerLayersOverlay.Add(_layer);

            layer.SetupLayer(ingredientData);
            _layer.SetupLayer(ingredientData);
        }
        burgerLayers.Reverse();
        burgerLayersOverlay.Reverse();

        if(burgerLayers.Count > 0)
        {
            burgerLayers.First().Reverse();
            burgerLayersOverlay.First().Reverse();
        }
    }

    private void SetArrowPosition(BurgerLayerUI burgerLayerUI)
    {
        //  arrowLayer.localPosition = new Vector3(arrowLayer.localPosition.x, burgerLayerUI.transform.localPosition.y, arrowLayer.localPosition.z);
        arrowLayer.localPosition += new Vector3(0f, offsetArrowByLayer, 0f);
        arrowLayerOverlay.localPosition += new Vector3(0f, offsetArrowByLayerOverlay, 0f);
    }

    private void DestroyUI()
    {
        foreach(RectTransform rectTransform in listParent.GetComponentInChildren<RectTransform>())
        {
            Destroy(rectTransform.gameObject);
        }

        foreach (RectTransform rectTransform in listParentOverlay.GetComponentInChildren<RectTransform>())
        {
            Destroy(rectTransform.gameObject);
        }
    }

    private void Hide()
    {
        content.gameObject.SetActive(false);
    }

    private void Show()
    {
        content.gameObject.SetActive(true);
    }
}
