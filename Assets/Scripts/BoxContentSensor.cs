using System.Collections.Generic;
using UnityEngine;

public class BoxContentsSensor : MonoBehaviour
{
    // Connect the configured GameLogic scene object.
    public GameLogic gameLogic;

    // Connect the box this sensor belongs to.
    public TrainingPackage package;

    // Remember which item colliders are inside this sensor.
    private readonly HashSet<Collider> touchingColliders =
        new HashSet<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        touchingColliders.Add(other);
    }

    private void OnTriggerStay(Collider other)
    {
        // Also finds items already inside when the sensor starts.
        touchingColliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        touchingColliders.Remove(other);
    }

    private void Update()
    {
        // Wait until both Inspector fields are connected.
        if (gameLogic == null || package == null)
        {
            return;
        }

        // Only accept items during the filling step.
        if (gameLogic.CurrentState != 3)
        {
            return;
        }

        // Do not pack a box while someone is holding it.
        if (package.IsHeld)
        {
            return;
        }

        PackingItem candidate = null;

        foreach (Collider itemCollider in touchingColliders)
        {
            // Ignore missing or inactive colliders.
            if (itemCollider == null ||
                !itemCollider.enabled ||
                !itemCollider.gameObject.activeInHierarchy)
            {
                continue;
            }

            // Find PackingItem on the collider or its parent.
            PackingItem item =
                itemCollider.GetComponentInParent<PackingItem>();

            // The item must be released and not already packed.
            if (item != null && !item.IsHeld && !item.IsPacked)
            {
                candidate = item;
                break;
            }
        }

        if (candidate != null)
        {
            // GameLogic checks whether this box is already filled.
            // If accepted, the item disappears and the count updates.
            gameLogic.RegisterPackedItem(package, candidate);
        }
    }

    private void OnDisable()
    {
        touchingColliders.Clear();
    }
} 