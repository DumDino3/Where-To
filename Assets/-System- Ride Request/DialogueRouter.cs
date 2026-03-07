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
        //DuringRide();
    }
    //--------Temporary Brute State Machine-----------

    void Awake()
    {
        dialogueRunner = FindFirstObjectByType<DialogueRunner>();
    }

    // public void duringride()
    // {
    //     riderequest ride = cabineventmanager.instance.currentride;
    //     if (ride == null)
    //     {
    //         debug.logwarning("no request to pick up");
    //         return;
    //     }
        
    //     int diagid = ride.dialogue;
    //     dialoguepool dialoguepool = dataparser.getdialoguepool(diagid);
    //     dialoguerunner.startdialogue(dialoguepool.ride);
    // }

    // private void handleridestarted() //subscribed to onridestarted
    // {
    //     riderequest ride = cabineventmanager.instance.currentride;
    //     if (ride == null)
    //     {
    //         debug.logwarning("no request to pick up");
    //         return;
    //     }
        
    //     int diagid = ride.dialogue;
    //     dialoguepool dialoguepool = dataparser.getdialoguepool(diagid);
    //     dialoguerunner.startdialogue(dialoguepool.geton);

    //     startcoroutine(startyap(10));
    // }

    // private void handlerideended() //subscribed to onrideended
    // {
    //     riderequest ride = cabineventmanager.instance.currentride;
    //     if (ride == null)
    //     {
    //         debug.logwarning("no passenger to drop");
    //         return;
    //     }
        
    //     int diagid = ride.dialogue;
    //     dialoguepool dialoguepool = dataparser.getdialoguepool(diagid);
    //     dialoguerunner.startdialogue(dialoguepool.end);
    // }
}
