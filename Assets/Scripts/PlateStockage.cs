using UnityEngine;
using System.Collections.Generic;

public class PlateStockage : BaseCounter
{
    private PlateCounter linkedPlateCounter;
    private int amountPlateOn;
    private List<Plate> plateList;

    [Header("Amount of plates")]
    [SerializeField] private int amountPlateTotal = 4;

    [Header("Parent transform")]
    [SerializeField] private Transform parentTransform;

    private void Awake()
    {
        amountPlateOn = 0;
        plateList = new List<Plate>();
    }

    public override void Interact()
    {
        if (!HasPlateOn())
            return;
        if (PlayerPickup.Instance.HasHandEmpty())
        {
            Plate plate = plateList[^1];
            plateList.RemoveAt(plateList.Count - 1);
            amountPlateOn--;
            PlayerPickup.Instance.SetCurrentItem(plate.gameObject);
        }
    }

    private bool HasPlateOn()
    {
        return plateList.Count > 0;
    }

    public void SetPlateCounter( PlateCounter linkedPlateCounter )
    {
        this.linkedPlateCounter = linkedPlateCounter;
    }

    public bool IsStockageAvailable()
    {
        return plateList.Count < amountPlateTotal;
    }

    public void AddPlate(Plate plate)
    {
        amountPlateOn++;
        plateList.Add(plate);
    }

    private void RemovePlate()
    {
        amountPlateOn--;
    }

    public Transform GetParentTransform()
    {
        return this.parentTransform;
    }
}
