using UnityEngine;

public class TaxiInput : MonoBehaviour
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