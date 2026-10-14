using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabletUI : MonoBehaviour
{
    [SerializeField] private RectTransform tabletTransform;

    [SerializeField] private RectTransform mainMenuTransform;
    [SerializeField] private RectTransform ingredientMenuTransform;
    
    private void Start()
    {
        TabletManager.Instance.OnTabletOpen += TabletManager_OnTabletOpen;
        TabletManager.Instance.OnTabletClose += TabletManager_OnTabletClose;
    }

    private void TabletManager_OnTabletOpen(object sender, EventArgs args)
    {
        OpenTablet();
    }

    private void TabletManager_OnTabletClose(object sender, EventArgs args)
    {
        CloseTablet();
    }

    private void OpenTablet()
    {
        SetMainMenu();
        LeanTween.moveY(tabletTransform, 0f, 1f).setEaseOutQuint();
    }

    public void CloseTablet()
    {
        LeanTween.moveY(tabletTransform, -1080f, 1f).setEaseOutQuint();
    }

    public void SetMainMenu()
    {
        mainMenuTransform.transform.gameObject.SetActive(true);
        ingredientMenuTransform.transform.gameObject.SetActive(false);
    }


    public void SetIngredientMenu()
    {
        Debug.Log("coucou on clique sur le menu");
        mainMenuTransform.transform.gameObject.SetActive(false);
        ingredientMenuTransform.transform.gameObject.SetActive(true);
    }

}
