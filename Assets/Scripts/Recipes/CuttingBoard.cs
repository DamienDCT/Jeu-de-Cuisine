using UnityEngine;

public class CuttingBoard : KitchenTool
{

    public override void InteractHold()
    {
        if (PlayerPickup.Instance.HasItemInHand())
            return;

        if (currentIngredientInput == null)
            return;


        base.StartProgression(this.currentIngredient.ingredientCookTime);
    }
}
