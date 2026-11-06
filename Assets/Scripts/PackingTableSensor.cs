using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PackingTableSensor : MonoBehaviour
{
    // Connect the GameLogic scene object .
    public GameLogic gameLogic;

    // Connect Box1 through Box5 .
    public TrainingPackage[] packages = new TrainingPackage[5];

    // Connect the ProgressCheckList .
    public TMP_Text progressChecklist;

   
    public UnityEvent onAllPackagesPlaced = new UnityEvent();

   
    public bool AllPackagesPlaced { get; private set; }


    private readonly HashSet<Collider> touchingColliders =
        new HashSet<Collider>();

  
    private string previousText;

    // Makes sure we only report completion once
    private bool completionReported;

    private void OnTriggerEnter(Collider other)
    {
        touchingColliders.Add(other);
    }

    private void OnTriggerStay(Collider other)
    {
        // Also detects boxes already inside when the sensor turns on.
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

        // Check each package separately.
        for (int i = 0; i < packages.Length; i++)
        {
            TrainingPackage package = packages[i];
            bool isPlaced = false;

            // holding a box must is not counted as placed.
            if (package != null && !package.IsHeld)
            {
                foreach (Collider boxCollider in touchingColliders)
                {
                    // Ignore colliders that are missing or turned off.
                    if (boxCollider == null ||
                        !boxCollider.enabled ||
                        !boxCollider.gameObject.activeInHierarchy)
                    {
                        continue;
                    }

                    // Find the package on this collider or its parent.
                    TrainingPackage detectedPackage =
                        boxCollider.GetComponentInParent<TrainingPackage>();

                    if (detectedPackage == package)
                    {
                        isPlaced = true;

                        // Count this box only once.
                        break;
                    }
                }
            }

            if (isPlaced)
            {
                completedCount++;
            }

            // Show Complete beside each placed package.
            string status = isPlaced ? " — Complete" : "";

            checklist += "Package " + (i + 1) + status + "\n\n";
        }

        // Update this in every state so Confirm can check it.
        AllPackagesPlaced =
            packages.Length == 5 && completedCount == 5;

        // Only State 1 can update the panel or report completion.
        if (gameLogic == null || gameLogic.CurrentState != 1)
        {
            return;
        }

        checklist += "Completed: " + completedCount + "/5";

        // Change the panel only when the text changes.
        if (progressChecklist != null && checklist != previousText)
        {
            progressChecklist.text = checklist;
            previousText = checklist;
        }

        if (AllPackagesPlaced && !completionReported)
        {
            completionReported = true;

            Debug.Log("State 1 complete: all five packages are placed.");

            // Tell GameLogic to enter State 2.
            onAllPackagesPlaced.Invoke();
        }
    }

    private void OnDisable()
    {
        // Clear old info when the sensor turns off.
        touchingColliders.Clear();
        AllPackagesPlaced = false;
        previousText = null;
        completionReported = false;
    }
}