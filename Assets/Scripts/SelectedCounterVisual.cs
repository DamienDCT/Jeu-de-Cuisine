using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{

    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject selectedObject;
    [SerializeField] private GameObject unselectedObject;

    private void Start()
    {
        PlayerController.Instance.OnSelectedCounterChanged += PlayerController_OnSelectedCounterChanged;
    }

    private void PlayerController_OnSelectedCounterChanged(object sender, SelectedCounterArgs e)
    {
        if(e.baseCounter == baseCounter)
        {
            Show();
        } else
        {
            Hide();
        }
    }

    public void Hide()
    {
        selectedObject.SetActive(false);
        this.unselectedObject.SetActive(true);
    }

    public void Show()
    {
        selectedObject.SetActive(true);
        this.unselectedObject.SetActive(false);
    }
}
