using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [SerializeField] private float amountDollars;


    private void Awake()
    {
        Instance = this;
        amountDollars = float.MaxValue;
    }

    public void ChangeAmountDollars(float amount)
    {
        amountDollars += amount;
    }

    public bool CheckNegativeMoney()
    {
        return amountDollars < 0f;
    }

    public bool CanPlayerBuyWithCost(float cost)
    {
        return (amountDollars - cost) >= 0f;
    }
}
