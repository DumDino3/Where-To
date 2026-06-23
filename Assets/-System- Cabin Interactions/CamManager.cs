using Unity.Cinemachine;
using System.Collections.Generic;
using UnityEngine;
public class CamManager : MonoBehaviour
{
    public List<CinemachineCamera> CamAngles;

    [Range(0.0f, 0.1f)]
    public float edgeThickness = 0.02f;
    private int screenW;
    private int screenH;

    public int currentAngle = 1;

    public CinemachineBrain cineBrain;

    public bool stopTurning;
    private bool disengaged = true;

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
        bool isOnEdge = leftEdge || rightEdge || bottomEdge || topEdge;

        //When mouse moves away from an edge, allow change cam again. This is to make sure mouse held at an edge does not move the cam all the way to the end.
        if (!isOnEdge)
        {
            disengaged = true;
        }

        if (!stopTurning)
        {
            // Check mouse position to detect edge touches -> update currentAngle
            if (disengaged && !cineBrain.IsBlending)
            {
                switch (currentAngle)
                {
                    case 4:
                        if (topEdge)
                        {
                            currentAngle = 1;
                            disengaged = false;
                        }
                        else if (leftEdge)
                        {
                            currentAngle = 0;
                            disengaged = false;
                        }
                        else if (rightEdge)
                        {
                            currentAngle = 2;
                            disengaged = false;
                        }
                        break;
                    default:
                        if (rightEdge && currentAngle < 3)
                        {
                            currentAngle++;
                            disengaged = false;
                        }
                        else if (leftEdge && currentAngle > 0)
                        {
                            currentAngle--;
                            disengaged = false;
                        }
                        else if (bottomEdge)
                        {
                            currentAngle = 4;
                            disengaged = false;
                        }
                        break;
                }
            }
        }

        SwitchToCurrentCam();
    }

    private void SwitchToCurrentCam()
    {
        // Lower all cam priority
        foreach (var p in CamAngles)
        {
            p.Priority.Value = 2;
        }

        // Boost current camera's priority
        CamAngles[currentAngle].Priority.Value = 10;
    }

    public void SwitchToCam(int angle)
    {
        currentAngle = angle;
    }
}
