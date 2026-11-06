using UnityEngine;

public class TrainingPackage : MonoBehaviour
{
    // check whether someone is holding this box.
    
    public bool IsHeld { get; private set; }

    // Called when the player grabs this box.
    public void OnGrabbed()
    {
        IsHeld = true;
        Debug.Log(gameObject.name + " was grabbed.");
    }

    // Called when the player lets go of this box.
    public void OnReleased()
    {
        IsHeld = false;
        Debug.Log(gameObject.name + " was released.");
    }
}