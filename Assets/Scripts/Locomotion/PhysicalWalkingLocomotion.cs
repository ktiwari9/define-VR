using System.Collections.Generic;
using UnityEngine;
using UXF;
using Valve.VR;

public class PhysicalWalkingLocomotion : Locomotion
{
    public Camera mainCamera;
    public GameObject cameraBase;
    Vector3 lastHeadRot;
    bool freezeYCameraRotation;


    void FixedUpdate()
    {
        //characterController.center = mainCamera.transform.localPosition;
        if (freezeYCameraRotation)
        {
            player.transform.RotateAround(mainCamera.transform.position, Vector3.up, -(mainCamera.transform.eulerAngles.y - lastHeadRot.y));
            lastHeadRot = mainCamera.transform.eulerAngles;
        }
        else
        {
            player.transform.position = new Vector3(mainCamera.transform.position.x, 0, mainCamera.transform.position.z);
            cameraBase.transform.localPosition = new Vector3(-mainCamera.transform.localPosition.x, 0, -mainCamera.transform.localPosition.z);
        }

        // Check buttons
        if (SteamVR_Actions.vr_interaction.TriggerClick.GetStateDown(SteamVR_Input_Sources.Any))
        {
            BlurAndFreezeCamera();
        }
        else if (SteamVR_Actions.vr_interaction.TriggerClick.GetStateUp(SteamVR_Input_Sources.Any)){
            UnblurAndDefreezeCamera();
        }

        // Check trial ending input
        EndButton(SteamVR_Actions.vr_interaction.EndTrial.GetStateDown(SteamVR_Input_Sources.Any));
    }

    void BlurAndFreezeCamera()
    {
        Valve.VR.OpenVR.Chaperone.ForceBoundsVisible(true);
        lastHeadRot = mainCamera.transform.eulerAngles;
        freezeYCameraRotation = true;
        SteamVR_Fade.Start(new Color(0f, 0f, 0f, 0.92f), 0.5f);
        mainCamera.transform.Find("Blur Canvas").gameObject.SetActive(true);    // TODO: do this smarter
    }

    void UnblurAndDefreezeCamera()
    {
        mainCamera.transform.Find("Blur Canvas").gameObject.SetActive(false);   // TODO: do this smarter
        SteamVR_Fade.Start(new Color(0f, 0f, 0f, 0f), 0.5f);
        freezeYCameraRotation = false;
        Valve.VR.OpenVR.Chaperone.ForceBoundsVisible(false);
    }
}
