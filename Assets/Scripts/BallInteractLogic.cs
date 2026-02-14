using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BallInteractLogic : MonoBehaviour, IPointerClickHandler
{
    public string ballID;
    public Image destination;

    private void Start()
    {
        destination.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("This is Ball No." + ballID);

        if (destination.isActiveAndEnabled == false)
        {
            Debug.Log("Let's go");
            destination.enabled = true;
        }

        else if (destination.isActiveAndEnabled == true)
        {
            Debug.Log("Let's not go");
            destination.enabled = false;
        }
    }
}
