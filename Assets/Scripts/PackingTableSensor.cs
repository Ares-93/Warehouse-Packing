using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PackingTableSensor : MonoBehaviour
{
   
    [SerializeField] private TrainingPackage[] packages =
        new TrainingPackage[5];

   
    [SerializeField] private TMP_Text progressChecklist;

    
    [SerializeField] private UnityEvent onAllPackagesPlaced;

    // Remembers the colliders currently touching our sensor.
   
    private readonly HashSet<Collider> touchingColliders =
        new HashSet<Collider>();

    // Remembers the last displayed text to avoid updating it every frame.
    private string previousText;

    // Prevents the completion event from firing repeatedly.
    private bool completionReported;

    private void OnTriggerEnter(Collider other)
    {
        touchingColliders.Add(other);
    }

    private void OnTriggerStay(Collider other)
    {
        
        touchingColliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        touchingColliders.Remove(other);
    }

    private void Update()
    {
        int completedCount = 0;
        string checklist = "";

        // Check each of our five packages separately.
        for (int i = 0; i < packages.Length; i++)
        {
            TrainingPackage package = packages[i];
            bool isPlaced = false;

          
            if (package != null && !package.IsHeld)
            {
                foreach (Collider boxCollider in touchingColliders)
                {
                   
                    if (boxCollider == null ||
                        !boxCollider.enabled ||
                        !boxCollider.gameObject.activeInHierarchy)
                    {
                        continue;
                    }

                    // Look for TrainingPackage on the collider
                    
                    TrainingPackage detectedPackage =
                        boxCollider.GetComponentInParent<TrainingPackage>();

                    if (detectedPackage == package)
                    {
                        isPlaced = true;

                        // We found this package.
                        
                        break;
                    }
                }
            }

            if (isPlaced)
            {
                completedCount++;
            }

            
            string mark = isPlaced ? "[X]" : "[ ]";

            checklist += mark + " Package " + (i + 1) + "\n\n";
        }

        checklist += "Completed: " + completedCount + "/5";

        // Update the panel 
        if (progressChecklist != null && checklist != previousText)
        {
            progressChecklist.text = checklist;
            previousText = checklist;
        }

        // All five must be inside the sensor
        if (packages.Length == 5 &&
            completedCount == 5 &&
            !completionReported)
        {
            completionReported = true;
            Debug.Log("State 1 complete: all five packages are placed.");

          
            onAllPackagesPlaced.Invoke();
        }
    }

    private void OnDisable()
    {
       
        touchingColliders.Clear();
        previousText = null;
        completionReported = false;
    }
}