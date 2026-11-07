using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    // Connect the packing-table sensor.
    public PackingTableSensor tableSensor;

    // Connect Box1 through Box5 in that order.
    public TrainingPackage[] packages = new TrainingPackage[5];

    // Connect the five state objects.
    public GameObject state1Group;
    public GameObject state2Group;
    public GameObject state3Group;
    public GameObject state4Group;
    public GameObject state5Group;

    // Connect the title, instructions, and checklist.
    public TMP_Text trainingTitle;
    public TMP_Text taskInstructions;
    public TMP_Text progressChecklist;

    // Other scripts can read which step is active.
    public int CurrentState { get; private set; }

    // These sets remember which boxes finished each task.
    // The same box cannot be added twice.
    private readonly HashSet<TrainingPackage> filledBoxes =
        new HashSet<TrainingPackage>();

    private readonly HashSet<TrainingPackage> sealedBoxes =
        new HashSet<TrainingPackage>();

    private readonly HashSet<TrainingPackage> deliveredBoxes =
        new HashSet<TrainingPackage>();

    // Stops training completion from being reported repeatedly.
    private bool trainingComplete;


    // When the training begins:
    // Clear the previous progress.
    // Set current state to 1.

    private void Start()
    {
        filledBoxes.Clear();
        sealedBoxes.Clear();
        deliveredBoxes.Clear();
        trainingComplete = false;

        StartState1();
    }


    // STATE 1: Package Handling
    // Title: "Warehouse Training"
    // Instructions: "Pick up packages from storage
    //                and place all 5 on the packing table."
    // Checklist: Show Package 1 through Package 5.
    // Progress: "Completed: 0/5"

    private void StartState1()
    {
        ActivateState(1);

        SetInstructions(
            "TASK 01 — PACKAGE HANDLING\n\n" +
            "Pick up packages from storage and place " +
            "all 5 on the packing table.");

        // Begin with an empty checklist.
        // The table sensor updates it as boxes are placed.
        ShowChecklist(
            "Placed on Table",
            new HashSet<TrainingPackage>());
    }

    // Completion: All 5 packages are released on the table.
    // Go to STATE 2.

    public void CompletePackageHandling()
    {
        // Ignore this event outside State 1.
        if (CurrentState != 1)
        {
            return;
        }

        // Wait until the table sensor confirms all five boxes.
        if (tableSensor == null || !tableSensor.AllPackagesPlaced)
        {
            return;
        }

        StartState2();
    }


    // STATE 2: Verify the Order
    // Instructions: "Check there are 5 packages."
    // Checklist: Shows all 5 packages as placed.
    // Wait for the Confirm button.

    private void StartState2()
    {
        ActivateState(2);

        SetInstructions(
            "TASK 02 — VERIFY THE ORDER\n\n" +
            "Check that all 5 packages are on the table.\n\n" +
            "Press Confirm when you are ready.");

        // These boxes completed table placement only.
        // Filling, sealing, and delivery are still unfinished.
        if (progressChecklist != null)
        {
            progressChecklist.text =
                "Package 1 — On table\n\n" +
                "Package 2 — On table\n\n" +
                "Package 3 — On table\n\n" +
                "Package 4 — On table\n\n" +
                "Package 5 — On table\n\n" +
                "Awaiting order confirmation";
        }
    }

    // Hit Confirm button:
    // Check all 5 packages are still placed.
    // Go to STATE 3.

    public void ConfirmOrder()
    {
        // The Confirm button only works during State 2.
        if (CurrentState != 2)
        {
            return;
        }

        // Do not advance if a box has been picked up or removed.
        if (tableSensor == null || !tableSensor.AllPackagesPlaced)
        {
            SetInstructions(
                "TASK 02 — VERIFY THE ORDER\n\n" +
                "Place all 5 packages on the table and release them.\n\n" +
                "Then press Confirm again.");

            return;
        }

        StartState3();
    }


    // STATE 3: Fill the Boxes
    // Put one item inside each box — 5 items total.
    // Checklist: Track each filled box separately.
    // Our closed box models use the area above the lid for packing.

    private void StartState3()
    {
        ActivateState(3);

        SetInstructions(
            "TASK 03 — FILL THE BOXES\n\n" +
            "Place and release one item on top of each box " +
            "to pack it.\n\n" +
            "Fill all 5 boxes.");

        ShowChecklist("Boxes Filled", filledBoxes);
    }

    // A contents sensor calls this with its box and detected item.

    public void RegisterPackedItem(
        TrainingPackage box, PackingItem item)
    {
        // Packing only works during State 3.
        if (CurrentState != 3)
        {
            return;
        }

        // Accept only an assigned training box and a valid item.
        if (!IsTrainingBox(box) || item == null)
        {
            return;
        }

        // Do not fill a held box or add another item to a filled box.
        if (box.IsHeld || filledBoxes.Contains(box))
        {
            return;
        }

        // TryPack refuses held items and items already packed.
        // An accepted item disappears to represent going inside the box.
        if (!item.TryPack())
        {
            return;
        }

        filledBoxes.Add(box);

        ShowChecklist("Boxes Filled", filledBoxes);

        // Check whether this was the fifth filled box.
        CompleteFilling();
    }

    // Completion: All 5 different boxes contain an item.
    // Go to STATE 4.

    public void CompleteFilling()
    {
        if (CurrentState != 3)
        {
            return;
        }

        // Stay in State 3 until every box has its own record.
        if (!AllFiveRecorded(filledBoxes))
        {
            return;
        }

        StartState4();
    }


    // STATE 4: Package Prep
    // Prepare the packages by taping them.
    // Use the tape dispenser and scissors.
    // Track all 5 sealed boxes.

    private void StartState4()
    {
        ActivateState(4);

        SetInstructions(
            "TASK 04 — PACKAGE PREP\n\n" +
            "Use the tape dispenser and scissors " +
            "to seal all 5 boxes.");

        ShowChecklist("Boxes Sealed", sealedBoxes);
    }

    // The sealing system calls this after the tape
    // and scissors actions are finished for a box.

    public void RegisterSealedBox(TrainingPackage box)
    {
        if (CurrentState != 4 || !IsTrainingBox(box))
        {
            return;
        }

        // A box must contain an item before it can be sealed.
        if (!filledBoxes.Contains(box))
        {
            return;
        }

        sealedBoxes.Add(box);

        ShowChecklist("Boxes Sealed", sealedBoxes);

        // Check whether all five boxes are now sealed.
        CompleteTaping();
    }

    // Completion: All 5 filled boxes are sealed.
    // Go to STATE 5.

    public void CompleteTaping()
    {
        if (CurrentState != 4)
        {
            return;
        }

        // Stay in State 4 until every box is filled and sealed.
        if (!AllFiveRecorded(filledBoxes) ||
            !AllFiveRecorded(sealedBoxes))
        {
            return;
        }

        StartState5();
    }


    // STATE 5: Get Packages Ready to Deliver
    // Put all the packages in the truck to be delivered.
    // Progress: "Ready to Deliver: 0/5"

    private void StartState5()
    {
        ActivateState(5);

        SetInstructions(
            "TASK 05 — DELIVERY\n\n" +
            "Place and release all 5 sealed packages " +
            "inside the truck.");

        ShowChecklist("Ready to Deliver", deliveredBoxes);
    }

    // The truck sensor reports whether a box is inside.
    // It must report when a box is released, picked up, or removed.

    public void SetBoxInTruck(TrainingPackage box, bool isInside)
    {
        if (CurrentState != 5 ||
            trainingComplete ||
            !IsTrainingBox(box))
        {
            return;
        }

        // Count only released, filled, and sealed boxes inside the truck.
        if (isInside &&
            !box.IsHeld &&
            filledBoxes.Contains(box) &&
            sealedBoxes.Contains(box))
        {
            deliveredBoxes.Add(box);
        }
        else
        {
            // Before training finishes, removing a box removes its count.
            deliveredBoxes.Remove(box);
        }

        ShowChecklist("Ready to Deliver", deliveredBoxes);

        CompleteDelivery();
    }

    // Completion: All 5 sealed packages are loaded.
    // Show "Training Complete."

    public void CompleteDelivery()
    {
        if (CurrentState != 5 || trainingComplete)
        {
            return;
        }

        // Stay in State 5 until all five boxes finished every task.
        if (!AllFiveRecorded(filledBoxes) ||
            !AllFiveRecorded(sealedBoxes) ||
            !AllFiveRecorded(deliveredBoxes))
        {
            return;
        }

        trainingComplete = true;

        SetInstructions(
            "TRAINING COMPLETE\n\n" +
            "All 5 packages are ready for delivery.");

        ShowChecklist("Ready to Deliver", deliveredBoxes);
    }


    // SHARED HELPERS
    // These do common jobs so each state does not repeat the same code.


    // Checks whether a box is one of the five assigned in the Inspector.
    // This prevents unrelated objects from counting toward training.
    private bool IsTrainingBox(TrainingPackage box)
    {
        if (box == null || packages == null)
        {
            return false;
        }

        foreach (TrainingPackage package in packages)
        {
            if (package == box)
            {
                return true;
            }
        }

        // The box was not in our training list.
        return false;
    }


    // Checks whether all five different boxes finished a task.
    // "records" can be the filled, sealed, or delivered box list.
    private bool AllFiveRecorded(HashSet<TrainingPackage> records)
    {
        if (packages == null || packages.Length != 5)
        {
            return false;
        }

        // Use this temporary set to spot duplicate assignments.
        HashSet<TrainingPackage> uniqueBoxes =
            new HashSet<TrainingPackage>();

        foreach (TrainingPackage package in packages)
        {
            // Stop if a field is empty or the same box appears twice.
            if (package == null || !uniqueBoxes.Add(package))
            {
                return false;
            }

            // Stop if this box has not finished the task.
            if (!records.Contains(package))
            {
                return false;
            }
        }

        // Every field contains a different box that finished the task.
        return true;
    }


    // Changes the current step and activates its object group.
    // Shared objects, such as boxes and the door, stay outside these groups.
    private void ActivateState(int nextState)
    {
        // Remember the new state before turning its objects on.
        CurrentState = nextState;

        // Only the matching state's group receives true.
        SetGroupActive(state1Group, nextState == 1);
        SetGroupActive(state2Group, nextState == 2);
        SetGroupActive(state3Group, nextState == 3);
        SetGroupActive(state4Group, nextState == 4);
        SetGroupActive(state5Group, nextState == 5);

        if (trainingTitle != null)
        {
            trainingTitle.text = "WAREHOUSE TRAINING";
        }

        // Helps developers follow state changes during testing.
        Debug.Log("Training entered State " + CurrentState);
    }


    // Turns a group and its children on or off.
    // Skips an empty Inspector field instead of causing an error.
    private void SetGroupActive(GameObject group, bool active)
    {
        if (group != null)
        {
            group.SetActive(active);
        }
    }


    // Replaces the directions on the training panel.
    // This changes text only, not the current state.
    private void SetInstructions(string message)
    {
        if (taskInstructions != null)
        {
            taskInstructions.text = message;
        }
    }


    // Builds the checklist using the records for a particular task.
    // Filling a box does not also mark it sealed or delivered.
    private void ShowChecklist(
        string counterLabel,
        HashSet<TrainingPackage> records)
    {
        if (progressChecklist == null || packages == null)
        {
            return;
        }

        string checklist = "";
        int completedCount = 0;

        // Inspector order determines the displayed package numbers.
        // The learner can still handle packages in any order.
        for (int i = 0; i < packages.Length; i++)
        {
            bool complete =
                packages[i] != null && records.Contains(packages[i]);

            if (complete)
            {
                completedCount++;
            }

            string status = complete ? " — Complete" : "";

            checklist +=
                "Package " + (i + 1) + status + "\n\n";
        }

        // Add the total underneath the package lines.
        checklist += counterLabel + ": " + completedCount + "/5";

        progressChecklist.text = checklist;
    }
}