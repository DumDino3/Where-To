using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;

public class CamManager : MonoBehaviour
{
    public List<CinemachineCamera> CamAngles;

    [Range(0.0f, 0.1f)]
    public float edgeThickness = 0.02f;
    private int screenW;
    private int screenH;

    public int currentAngle;

    public CinemachineBrain cineBrain;

    public bool stopTurning;

    void Start()
    {
        GameObject vCams = GameObject.FindGameObjectWithTag("VCams");
        for (int i = 0; i < vCams.transform.childCount; i++)
        {
            Transform vCam = vCams.transform.GetChild(i);
            CinemachineCamera vCamComponent = vCam.GetComponent<CinemachineCamera>();

            CamAngles.Add(vCamComponent);
        }

        //Determine where screen edges would be on set resolution
        screenW = Screen.width;
        screenH = Screen.height;


    }

    void Update()
    {
        Vector3 m = Input.mousePosition;

        bool leftEdge = m.x < screenW * edgeThickness;
        bool rightEdge = m.x > screenW * (1 - edgeThickness);
        bool bottomEdge = m.y < screenH * edgeThickness;
        bool topEdge = m.y > screenH * (1 - edgeThickness);

        if (!stopTurning)
        {
            // Check mouse position to detect edge touches -> update currentAngle
            if (rightEdge && currentAngle < CamAngles.Count - 2 && !cineBrain.IsBlending)
            {
                currentAngle++;      
            }
            else if (leftEdge && currentAngle > 0 && !cineBrain.IsBlending && currentAngle != 4)
            {
                currentAngle--;
            }
            else if (bottomEdge && currentAngle == 1 && !cineBrain.IsBlending)
            {
                currentAngle = 4;
            }

            else if (topEdge && currentAngle == 4 && !cineBrain.IsBlending)
            {
                currentAngle = 1;
            }
        }

        SwitchToCurrentCam();
    }

    private void SwitchToCurrentCam()
    {
        // Lower all cam priority
        foreach (var p in CamAngles)
        {
            p.Priority = 5;
        }

        // Boost current camera's priority
        CamAngles[currentAngle].Priority = 10;
    }
}
