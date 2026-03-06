using UnityEngine;

public class Taxi : MonoBehaviour
{
    private CabinStateMachine cabinStateMachine;
    private bool isPickingUp = false;
    private bool isInSpawnPoint = false;

    private void Awake()
    {
        cabinStateMachine = GetComponent<CabinStateMachine>();
    }

    private void Start()
    {
        cabinStateMachine.SetIdle();
    }

    private void Update()
    {
        if (isInSpawnPoint && Input.GetKeyDown(KeyCode.E))
        {
            if (!isPickingUp)
            {
                Debug.Log("pickingUp");
                cabinStateMachine.SetPickUp();
            }
            else
            {
                cabinStateMachine.SetDropOff();
            }
            isPickingUp = !isPickingUp;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("SpawnPoint"))
        {
            Debug.Log("Press E to interact");
            isInSpawnPoint = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("SpawnPoint"))
        {
            isInSpawnPoint = false;
        }
    }
}