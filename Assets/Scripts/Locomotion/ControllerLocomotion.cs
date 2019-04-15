using System.Collections.Generic;
using UnityEngine;
using UXF;
using Valve.VR;

public class ControllerLocomotion : Locomotion
{
    public bool useForRotation;
    public float rotateSpeed;

    void Update()
    {
        float movement = SteamVR_Actions.vr_interaction.Movement.GetAxis(SteamVR_Input_Sources.Any);
        int direction = SteamVR_Actions.vr_interaction.Grip.GetState(SteamVR_Input_Sources.Any) ? -1 : 1;

        MoveForward(movement * direction);
        if (useForRotation)
        {
            float rotation = 0;
            if (SteamVR_Actions.vr_interaction.TrackpadLeft.GetState(SteamVR_Input_Sources.Any))
            {
                rotation = -1;
            }
            if (SteamVR_Actions.vr_interaction.TrackpadRight.GetState(SteamVR_Input_Sources.Any)){
                rotation = 1;
            }
            Rotate(rotation * rotateSpeed);
        }
        EndButton(SteamVR_Actions.vr_interaction.EndTrial.GetStateDown(SteamVR_Input_Sources.Any));
    }
}
