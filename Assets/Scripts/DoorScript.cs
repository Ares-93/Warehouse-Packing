
using System.Collections;


using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [Header("Scene References")]

  
    [SerializeField] private Transform doorPanels;

    [Header("Door Movement")]

    // How far up the panels move when the door opens.
    [SerializeField] private float openHeight = 4f;

    // How many seconds opening or closing takes.
    [SerializeField] private float moveDuration = 2f;

    // Remembers where the panels are when the door is closed.
    private Vector3 closedPosition;

    // Remembers where the panels should be when the door is open.
    private Vector3 openPosition;

   

    private bool isOpen = false;

    private bool isMoving = false;

    // Unity runs Awake when this object starts up.
    private void Awake()
    {
        // Check that we connected DoorPanels in the Inspector.
        if (doorPanels == null)
        {
         
            Debug.LogError("Assign DoorPanels in the Inspector.", this);

          
            enabled = false;

          
            return;
        }

    
        closedPosition = doorPanels.localPosition;

     
        // starting position + upward direction × opening height.
        openPosition = closedPosition + Vector3.up * openHeight;
    }


  
    public void ToggleDoor()
    {
    
        if (!enabled || isMoving)
        {
            return;
        }

        // Start moving the door over time.
        StartCoroutine(MoveDoor());
    }

 
    private IEnumerator MoveDoor()
    {
 
        isMoving = true;


        Vector3 startPosition = doorPanels.localPosition;

        Vector3 targetPosition;

        if (isOpen)
        {
        
            targetPosition = closedPosition;
        }
        else
        {
            
            targetPosition = openPosition;
        }


        float duration = Mathf.Max(0.01f, moveDuration);

   
        float elapsed = 0f;

     
        while (elapsed < duration)
        {
          
            elapsed += Time.deltaTime;

          
            float progress = Mathf.Clamp01(elapsed / duration);

     
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

     
            doorPanels.localPosition = Vector3.Lerp(
                startPosition,
                targetPosition,
                smoothProgress
            );

   
            yield return null;
        }

 
        doorPanels.localPosition = targetPosition;

    
        isOpen = !isOpen;

       
        isMoving = false;
    }

 
    [ContextMenu("Test Toggle Door")]
    private void TestToggleDoor()
    {
   
        if (Application.isPlaying)
        {
            ToggleDoor();
        }
    }
}