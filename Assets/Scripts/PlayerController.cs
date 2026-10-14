using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;

    public static PlayerController Instance { get; private set; }

    [SerializeField] private float speed;

    private Vector2 movementVector;

    [SerializeField] private bool canPlayerMove;

    private BaseCounter currentBaseCounterSelected;

    public event EventHandler<SelectedCounterArgs> OnSelectedCounterChanged;

    private bool isHolding = false;

    private void Awake()
    {
        movementVector = Vector2.zero;
        Instance = this;
        canPlayerMove = true;
        currentBaseCounterSelected = null;
    }

    private void Start()
    {
        this.playerInput = GetComponent<PlayerInput>();
        if(playerInput != null)
        {
            // Movement events
                playerInput.actions["Movement"].performed       += OnMovement;
                playerInput.actions["Movement"].canceled        += OnMovement;

            // Player Interactions events
                playerInput.actions["Interact"].performed       += OnInteractCounter;

                playerInput.actions["InteractHold"].performed   += OnInteractHoldCounter;
                playerInput.actions["InteractHold"].canceled   += OnInteractHoldCounter;

                playerInput.actions["OpenContainer"].performed  += OnContainerOpen;


                playerInput.actions["SwapIngredientCounter"].performed  += OnContainerScroll;

            
        }
    }

    private void OnContainerScroll(InputAction.CallbackContext context)
    {
        if (currentBaseCounterSelected == null)
            return;

        Vector2 currentMouseWheelValue = context.ReadValue<Vector2>();
        currentBaseCounterSelected.SwapSelectedItem(currentMouseWheelValue.y);
    }

    private void OnInteractHoldCounter(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(!isHolding)
            {
                currentBaseCounterSelected.InteractHold();
                isHolding = true;
            }
        } else if(context.canceled)
        {
            if(isHolding)
            {
                isHolding = false;
                currentBaseCounterSelected.CancelHoldInteraction();
            }
        }
   /*     } else if(context.canceled)
        {
            currentBaseCounterSelected.CancelHoldInteraction();
        }*/
    }

    private void OnContainerOpen(InputAction.CallbackContext context)
    {
        if(currentBaseCounterSelected != null)
        {
            if (transform.forward == currentBaseCounterSelected.gameObject.transform.forward)
            {
                currentBaseCounterSelected.Open();
            }
        }
    }

    private void OnInteractCounter(InputAction.CallbackContext context)
    {
        if (currentBaseCounterSelected != null)
        {
            currentBaseCounterSelected.Interact();
        }
    }

    private void Update()
    {
        PerformMovement(movementVector);
        PerformInteractions();
    }

    public void SetCanPlayerMove(bool canPlayerMove)
    {
        this.canPlayerMove = canPlayerMove;
    }

    private void PerformMovement(Vector2 movementVector)
    {
        if (!canPlayerMove)
            return;
        if (movementVector == Vector2.zero)
            return;

        transform.position  = transform.position + new Vector3(movementVector.x, 0f, movementVector.y) * speed * Time.deltaTime;
        transform.forward   = new Vector3(movementVector.x, 0f, movementVector.y);
    }

    private void PerformInteractions()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 2f))
        {
            if (hit.transform.TryGetComponent<BaseCounter>(out BaseCounter baseCounter))
            {


                if(baseCounter != currentBaseCounterSelected)
                {
                    if (currentBaseCounterSelected != null)
                    {
                        currentBaseCounterSelected.DisableOutline();
                        if (currentBaseCounterSelected.GetKitchenTool() != null)
                        {
                            currentBaseCounterSelected.GetKitchenTool().OnDeselect();
                        }
                    }
                    currentBaseCounterSelected = baseCounter;
                    currentBaseCounterSelected.EnableOutline();
                    if(currentBaseCounterSelected.GetKitchenTool() != null)
                    {
                        currentBaseCounterSelected.GetKitchenTool().OnSelect();
                    }
                    OnSelectedCounterChanged?.Invoke(this, new SelectedCounterArgs
                    {
                        baseCounter = baseCounter,
                    });
                }
            }
            else
            {
                if(currentBaseCounterSelected != null)
                {
                    currentBaseCounterSelected.DisableOutline();
                    if (currentBaseCounterSelected.GetKitchenTool() != null)
                    {
                        currentBaseCounterSelected.GetKitchenTool().OnDeselect();
                    }
                }

                currentBaseCounterSelected = null;
                OnSelectedCounterChanged?.Invoke(this, new SelectedCounterArgs
                {
                    baseCounter = null,
                });
            }
        }
        else
        {
            if (currentBaseCounterSelected != null)
            {
                currentBaseCounterSelected.DisableOutline();
                if (currentBaseCounterSelected.GetKitchenTool() != null)
                {
                    currentBaseCounterSelected.GetKitchenTool().OnDeselect();
                }
            }
            currentBaseCounterSelected = null;
            OnSelectedCounterChanged?.Invoke(this, new SelectedCounterArgs
            {
                baseCounter = null,
            });
        }
    }

    private void OnMovement(InputAction.CallbackContext ctx)
    {
        movementVector = ctx.ReadValue<Vector2>();
    }
}

[System.Serializable]
public class SelectedCounterArgs : EventArgs
{
    public BaseCounter baseCounter;
}