using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerManagerInput : MonoBehaviour
{
    public static PlayerManagerInput Instance;

    private PlayerInput playerInput;

    private void Awake()
    {
        Instance = this;
        playerInput = GetComponent<PlayerInput>();
    }

    public PlayerInput GetPlayerInput()
    {
        return playerInput;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Au clic gauche
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var r in results)
                Debug.Log("Touché : " + r.gameObject.name);
        }
    }

    public void SwapActionMap(string actionMap)
    {
        if (playerInput == null)
            return;

        playerInput.SwitchCurrentActionMap(actionMap);
    }
}
