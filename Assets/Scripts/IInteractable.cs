using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public abstract void Interact(PlayerPickup playerPickup);

    public abstract void Open(PlayerPickup playerPickup);
}
