using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TabletManager : MonoBehaviour
{
    public static TabletManager Instance;

    public event EventHandler OnTabletOpen;
    public event EventHandler OnTabletClose;
    public event EventHandler<DeliveryManager.DeliveryArgs> OnDeliveryAsked;

    private PlayerInput playerInput;

    private bool isTabletOpened;
    private bool isPopupOpened;

    private void Awake()
    {
        Instance = this;
        isTabletOpened = false;
        isPopupOpened = false;
    }

    private void Start()
    {
        playerInput = PlayerManagerInput.Instance.GetPlayerInput();
        playerInput.actions["OpenTablet"].performed += OpenTablet;
    }

    private void Update()
    {
        if (!isTabletOpened)
            return;

        if (isPopupOpened)
            return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            CloseTablet();
        }
    }

    private void OpenTablet(InputAction.CallbackContext context)
    {
        if(!isTabletOpened)
        {
            OnTabletOpen?.Invoke(this, EventArgs.Empty);
            isTabletOpened = true;
            PlayerController.Instance.SetCanPlayerMove(false);
        }
    }

    public void SetFlagPopupOpen(bool flag)
    {
        this.isPopupOpened = flag;
    }

    public void CloseTablet()
    {
        isTabletOpened = false;
        PlayerController.Instance.SetCanPlayerMove(true);
        OnTabletClose?.Invoke(this, EventArgs.Empty);
    }

    public void OrderIngredient(Dictionary<IngredientData, int> datas)
    {
        OnDeliveryAsked?.Invoke(this, new DeliveryManager.DeliveryArgs
        {
            listOfIngredients = datas
        });
    }
}  
