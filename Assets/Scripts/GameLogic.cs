using UnityEngine;

public class GameLogic : MonoBehaviour
{
    //When the training begins:
        //Set current state to 1
    
    // STATE 1: Packgage Handling
        // Title: Warehouse Tranining:"
        // Instructions: "Pick up packages from storage
    //                    and place all 5 on the packing table."
    // Checklist: Show Package 1 through Package 5
    // Progress: "Completed: 0/5"
    // Complettion: When all 5 packages are on a collision object on the table
    //          Goto STATE 2
    
    // STATE 2: Verify the Order
         // Instructions: "Check there are 5 packages"
         //Checklsit:  shows all 5 packages as checked
         // hit confirm button
         //     Goto STATE 3
    
    // STATE 3: Package Prep
    // Instructions: Prepare the packages by making sure they are taped
    // Confirm all 5 are taped
    //          Goto STATE 4

    // STATE 4: Get packages ready to deliver
        //Instructions: "Put all the packages in the truck to be deliverd"
        // Progress: "Ready to Deliver: 0/5"
    
    



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
