using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static UnityEditor.Progress;

public class BehaviourManager : MonoBehaviour
{
    public Rig sitRig;
    public Rig handRig;

    public bool sit = true;
    public bool hand= false;
    private bool holdingItem = false;

    private GameObject questItem;
    public GameObject questItemPrefab;
    public Transform socket;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Update()
    {

    }

    public void SwitchToSit()
    {
        sitRig.weight = 1f;
        handRig.weight = 0f;

        StopAllCoroutines();

        holdingItem = false;
        Destroy(questItem);
        questItem = null;
    }

    public void SwitchToHand()
    {
        sitRig.weight = 0f;
        handRig.weight = 1f;

        holdingItem = true;

        StartCoroutine(SpawnItemNextFrame());
    }

    IEnumerator SpawnItemNextFrame()
    {
        // Wait for the pose to update
        yield return null;

        // NOW the hand rig is in its new pose
        if (questItem == null)
        {
            questItem = Instantiate(questItemPrefab);
            questItem.transform.position = socket.position;
            questItem.transform.rotation = Quaternion.identity;
        }
    }
}
