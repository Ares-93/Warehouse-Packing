using TMPro;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    // Connects the packing-table sensor 
    public PackingTableSensor tableSensor;

    //connects the states
    public GameObject state1Group;
    public GameObject state2Group;
    public GameObject state3Group;
    public GameObject state4Group;
    public GameObject state5Group;

    //connects the title, insturctions and checklist
    public TMP_Text trainingTitle;
    public TMP_Text taskInstructions;
    public TMP_Text progressChecklist;


    public int CurrentState { get; private set; }


    // When the training begins:
    // Set current state to 1.

    private void Start()
    {
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

        // The table sensor updates this as boxes are placed.
        ShowChecklist("Completed", false);
    }
    // Completion: When all 5 packages are released
    //             inside the collision sensor on the table:
    //          Go to STATE 2.

      public void CompletePackageHandling()
    {
        // Only accept this event during State 1.
        if (CurrentState != 1)
        {
            return;
        }

        StartState2();
    }
   
    // STATE 2: Verify the Order
    // Instructions: "Check there are 5 packages."
    // Checklist: Shows all 5 packages as complete.

     private void StartState2()
    {
        ActivateState(2);

        SetInstructions(
            "TASK 02 — VERIFY THE ORDER\n\n" +
            "Check there are 5 packages on the table.\n\n" +
            "Press Confirm when you are ready.");

        ShowChecklist("Completed", true);
    }
    // Hit Confirm button:
    //          Go to STATE 3.

    public void ConfirmOrder()
{
    // The button only works during State 2.
    if (CurrentState != 2)
    {
        return;
    }

    // Do not continue if the sensor was not connected.
    if (tableSensor == null)
    {
        Debug.LogWarning("Connect Table Sensor on GameLogic.");
        return;
    }

    // All five boxes must still be placed on the table.
    if (!tableSensor.AllPackagesPlaced)
    {
        SetInstructions(
            "Place all 5 packages on the table and release them.\n\n" +
            "Then press Confirm again."
                       );

                return;
    }

    // The order is verified. Begin filling the boxes.
    StartState3();
}
   
    // STATE 3: Fill the Boxes
    // Instructions: Put one item inside each box.
    // Checklist: 5 items total packed, one in each box.

      private void StartState3()
    {
        ActivateState(3);

        SetInstructions(
            "TASK 03 — FILL THE BOXES\n\n" +
            "Put one item inside each box.\n\n" +
            "Fill all 5 boxes.");

        ShowChecklist("Boxes Filled", false);
    }
    // Completion: All 5 boxes contain an item.
    // Go to STATE 4.

        public void CompleteFilling()
    {
        // Only the filling step can advance to taping.
        if (CurrentState != 3)
        {
            return;
        }

        StartState4();
    }
  
    // STATE 4: Package Prep
    // Instructions: Prepare the packages by taping them.
    // Use the tape dispenser and scissors.

       private void StartState4()
    {
        ActivateState(4);

        SetInstructions(
            "TASK 04 — PACKAGE PREP\n\n" +
            "Use the tape dispenser and scissors " +
            "to seal all 5 boxes.");

        ShowChecklist("Boxes Sealed", false);
    }
    // Confirm all 5 are taped.
    // Go to STATE 5.

    public void CompleteTaping()
    {
        // Only the taping step can advance to delivery.
        if (CurrentState != 4)
        {
            return;
        }

        StartState5();
    }


    // STATE 5: Get Packages Ready to Deliver
    // Instructions: "Put all the packages in the truck
    //                to be delivered."
    // Progress: "Ready to Deliver: 0/5"

      private void StartState5()
    {
        ActivateState(5);

        SetInstructions(
            "TASK 05 — DELIVERY\n\n" +
            "Put all 5 sealed packages in the truck " +
            "to be delivered.");

        ShowChecklist("Ready to Deliver", false);
    }
    // Completion: All 5 sealed packages are in the truck.
    // Show "Training Complete."

    public void CompleteDelivery()
    {
        // Ignore delivery completion untill we get to delivery step
        if (CurrentState != 5)
        {
            return;
        }

        SetInstructions(
            "TRAINING COMPLETE\n\n" +
            "All 5 packages are ready for delivery.");

        ShowChecklist("Ready to Deliver", true);
    }



    private void ActivateState(int nextState)
    {
        
        CurrentState = nextState;

        // control which states are active.
        SetGroupActive(state1Group, nextState == 1);
        SetGroupActive(state2Group, nextState == 2);
        SetGroupActive(state3Group, nextState == 3);
        SetGroupActive(state4Group, nextState == 4);
        SetGroupActive(state5Group, nextState == 5);

        if (trainingTitle != null)
        {
            trainingTitle.text = "WAREHOUSE TRAINING";
        }

        Debug.Log("Training entered State " + CurrentState);
    }

    private void SetGroupActive(GameObject group, bool active)
    {
        
        if (group != null)
        {
            group.SetActive(active);
        }
    }

    private void SetInstructions(string message)
    {
        if (taskInstructions != null)
        {
            taskInstructions.text = message;
        }
    }

    private void ShowChecklist(string counterLabel, bool allComplete)
    {
        if (progressChecklist == null)
        {
            return;
        }

        string checklist = "";

        //check if all the packages are done
        for (int i = 1; i <= 5; i++)
        {
            string status = allComplete ? " — Complete" : "";

            checklist += "Package " + i + status + "\n\n";
        }

        int completedCount = allComplete ? 5 : 0;

        checklist += counterLabel + ": " + completedCount + "/5";

        progressChecklist.text = checklist;
    }
}