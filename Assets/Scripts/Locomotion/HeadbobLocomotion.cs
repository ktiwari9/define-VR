using UnityEngine;
using UnityEngine.SpatialTracking;
using Valve.VR;

public class HeadbobLocomotion : Locomotion {
	public TrackedPoseDriver HMD;

    // Private variables, used internally
    private float oldPos;      // The previous position
    private float endPos;       // The last end position
    private float oldRot;
    private bool goingUp;       // Are we going up (or down)
    private float bobtime;
    private float endRot;

    // Variables to be read from JSON
    public float bobThreshold;
    public float bobDeadzone;
    public float holdBob;
    public float rotThreshold;

    private bool initialized = false;

    bool isBob()
    {
        float newPos = HMD.transform.localPosition.y;
        float newRot = HMD.transform.localRotation.x;   // Forward rotation
        bool toReturn = false;
        // Change of direction at top
        if (goingUp && newPos < oldPos && oldPos - newPos > bobDeadzone)
        {
            // is the bob significant enough? Is rotation difference too high?
            if((oldPos - endPos) > bobThreshold && Mathf.Abs(oldRot - endRot) < rotThreshold)
            {
                toReturn = true;
            }
            endRot = oldRot;
            goingUp = false;
            endPos = oldPos;
        }

        // Change of direction at bottom
        else if (!goingUp && newPos > oldPos && newPos - oldPos > bobDeadzone)
        {
            // is the bob significant enough? Is rotation difference too high?
            if ((endPos - oldPos) > bobThreshold && Mathf.Abs(oldRot - endRot) < rotThreshold)
            {
                toReturn = true;
            }
            endRot = oldRot;
            goingUp = true;
            endPos = oldPos;
        }

        oldRot = newRot;
        oldPos = newPos;
        return toReturn;
    }


	void FixedUpdate ()
    {
        if (!initialized)
        {
            oldPos = HMD.transform.localPosition.y;
            endPos = HMD.transform.localPosition.y;
            initialized = true;
        }

        if (isBob())
        {
            // Set the expiration time to current + hold
            bobtime = Time.time + holdBob;
        }

        // If bob expiration time is more than current, move
        if(bobtime > Time.time)
        {
            MoveForward(1);
        }

        // Check the trial ending button
        EndButton(SteamVR_Actions.vr_interaction.EndTrial.GetStateDown(SteamVR_Input_Sources.Any));
    }
}

