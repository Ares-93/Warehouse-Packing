using UnityEngine;

public class PackingItem : MonoBehaviour
{
    // Remembers whether the player is holding this item.
    public bool IsHeld { get; private set; }

    // Stops the same item from filling more than one box.
    public bool IsPacked { get; private set; }

    // The item's When Select event calls this.
    public void OnGrabbed()
    {
        IsHeld = true;
    }

    // The item's When Unselect event calls this.
    public void OnReleased()
    {
        IsHeld = false;
    }

    // GameLogic calls this when a box tries to accept the item.
    public bool TryPack()
    {
        // Wait until the item is released.
        // Reject items that were already packed.
        if (IsHeld || IsPacked)
        {
            return false;
        }

        IsPacked = true;

        // Hide the item to represent putting it inside the box.
        gameObject.SetActive(false);

        // Tell GameLogic that packing succeeded.
        return true;
    }
}