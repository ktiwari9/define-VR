using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using Valve.VR;

public class TeleportLocomotion : Locomotion
{
    public TrackedPoseDriver Left;
    public TrackedPoseDriver Right;
    public LineRenderer Line;
    public Transform TargetCircle;
    public LayerMask teleportLayer;
    public float surfaceNormalThreshold;
    public int seg;
    public Color good;
    public Color bad;
    private TrackedPoseDriver pressing;

    // VARIABLES
    public float grav;
    public float dt;

    private bool initialized = false;


    // Update is called once per frame
    void FixedUpdate()
    {
        if (!initialized)
        {
            Line.gameObject.SetActive(false);
            Line.positionCount = seg;
            TargetCircle.gameObject.SetActive(false);
            initialized = true;
        }

        if (SteamVR_Actions.vr_interaction.TriggerClick.GetStateUp(SteamVR_Input_Sources.Any) && TargetCircle.gameObject.activeInHierarchy)
        {
            player.transform.position = TargetCircle.transform.position;
        }

        if (SteamVR_Actions.vr_interaction.TriggerClick.GetStateDown(SteamVR_Input_Sources.Any))
        {
            pressing = SteamVR_Actions.vr_interaction.TriggerClick.GetStateDown(SteamVR_Input_Sources.LeftHand) ? Left : Right;
        }

        if (SteamVR_Actions.vr_interaction.TriggerClick.GetState(SteamVR_Input_Sources.Any))
        {
            Line.gameObject.SetActive(true);
            Vector3 x = pressing.transform.position;
            Vector3 v = pressing.transform.forward * velocityMultiplier;
            Vector3 g = Vector3.down * grav;
            Line.SetPosition(0, x);
            Vector3 hitPosition = transform.position;
            bool hitted = false;
            RaycastHit hit = new RaycastHit();
            for (int i = 1; i < seg; ++i)
            {
                Ray ray = new Ray(x, v);
                if (Physics.Raycast(ray, out hit, dt * v.magnitude * 1.01f, teleportLayer))
                {
                    x = hit.point;
                    hitted = true;
                    hitPosition = x;
                    Line.positionCount = i + 1;
                    Line.SetPosition(i, x);
                    break;
                }
                else
                {
                    x += v * dt;
                    v += g * dt;
                    Line.positionCount = i + 1;
                    Line.SetPosition(i, x);
                }
            }
            if (hitted && hit.normal.y > surfaceNormalThreshold)
            {
                Line.startColor = good;
                Line.endColor = good;
                TargetCircle.gameObject.SetActive(true);
                TargetCircle.transform.position = hitPosition;
            }
            else
            {
                Line.startColor = bad;
                Line.endColor = bad;
                TargetCircle.gameObject.SetActive(false);
            }

        }
        else
        {
            Line.gameObject.SetActive(false);
            TargetCircle.gameObject.SetActive(false);
        }

        // Check the trial ending button for user input
        EndButton(SteamVR_Actions.vr_interaction.EndTrial.GetStateDown(SteamVR_Input_Sources.Any));
    }
}
