using System.Collections;
using UnityEngine;
using Yarn.Unity;

public class DialogueRouter : MonoBehaviour
{
    public DialogueRunner dialogueRunner;

    //--------Temporary Brute State Machine-----------
    IEnumerator StartYap (int time)
    {
        yield return new WaitForSeconds(time);
        DuringRide();
    }
    //--------Temporary Brute State Machine-----------

    void Awake()
    {
        dialogueRunner = FindFirstObjectByType<DialogueRunner>();
    }

    public void DuringRide()
    {
        RideRequest ride = CabinEventManager.Instance.currentRide;
        if (ride == null)
        {
            Debug.LogWarning("No request to pick up");
            return;
        }
        
        int diagID = ride.DIALOGUE;
        DialoguePool dialoguePool = DataParser.GetDialoguePool(diagID);
        dialogueRunner.StartDialogue(dialoguePool.RIDE);
    }

    private void HandleRideStarted() //Subscribed to OnRideStarted
    {
        RideRequest ride = CabinEventManager.Instance.currentRide;
        if (ride == null)
        {
            Debug.LogWarning("No request to pick up");
            return;
        }
        
        int diagID = ride.DIALOGUE;
        DialoguePool dialoguePool = DataParser.GetDialoguePool(diagID);
        dialogueRunner.StartDialogue(dialoguePool.GETON);

        StartCoroutine(StartYap(10));
    }

    private void HandleRideEnded() //Subscribed to OnRideEnded
    {
        RideRequest ride = CabinEventManager.Instance.currentRide;
        if (ride == null)
        {
            Debug.LogWarning("No passenger to drop");
            return;
        }
        
        int diagID = ride.DIALOGUE;
        DialoguePool dialoguePool = DataParser.GetDialoguePool(diagID);
        dialogueRunner.StartDialogue(dialoguePool.END);
    }

    private void Start()
    {
        CabinEventManager.Instance.OnRideStarted += HandleRideStarted;
        CabinEventManager.Instance.OnRideEnded += HandleRideEnded;
    }

    private void OnDisable()
    {
        CabinEventManager.Instance.OnRideStarted -= HandleRideStarted;
        CabinEventManager.Instance.OnRideEnded -= HandleRideEnded;
    }
}
