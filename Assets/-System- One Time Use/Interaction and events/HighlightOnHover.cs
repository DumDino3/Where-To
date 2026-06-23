using UnityEngine;
using UnityEngine.Events;

public class HighlightOnHover : MonoBehaviour
{
	[Header("Hover Events")]
	[SerializeField] private UnityEvent onHoverEnter;
	[SerializeField] private UnityEvent onHoverExit;

	public void TriggerHoverEnter()
	{
		onHoverEnter?.Invoke();
	}

	public void TriggerHoverExit()
	{
		onHoverExit?.Invoke();
	}
}
