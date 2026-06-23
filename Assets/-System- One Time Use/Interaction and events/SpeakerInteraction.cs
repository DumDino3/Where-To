using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;
using Yarn.Unity;

public class SpeakerInteraction : PlayerInteractableBase
{
    public DialogueRunner diagRunner;
    [SerializeField] private float rotationDuration = 0.25f;
    [SerializeField] private Ease rotationEase = Ease.OutQuad;
    private Tween rotationTween;

    [SerializeField] GameObject vCam; 

    void Awake()
    {
        diagRunner = GameObject.FindFirstObjectByType<DialogueRunner>();
        vCam = GameObject.FindGameObjectWithTag("MainCam");
    }


    public override void Highlight(GameObject highlightedObject)
    {
        //Debug.Log("looked at button");
    }

    public override void Unhighlight(GameObject highlightedObject)
    {
        //Debug.Log("looked away from button");
    }

    public override void OnInteractDown(GameObject highlightedObject)
    {
        //Debug.Log("pressed");
    }

    public override void OnInteractUp(GameObject highlightedObject)
    {
        Debug.Log("released");

        rotationTween?.Kill();

        Vector3 targetRotation = vCam.transform.localEulerAngles;
        targetRotation.x = 8.3f;
        rotationTween = vCam.transform.DOLocalRotate(targetRotation, rotationDuration).SetEase(rotationEase);

        diagRunner.StartDialogue("Question");
    }
}