using UnityEngine;
using UnityEngine.SpatialTracking;
using Valve.VR;

public class ArmswingLocomotion : Locomotion {
	public TrackedPoseDriver Left;
	public TrackedPoseDriver Right;

    Vector3 oldL, oldR;

    // VARIABLES
    public bool movingBothRequired; // Should both hands move for movement to be counted?
    public float minMoveThreshold;  // Values lower than this is ignored
    public float maxMoveThreshold;  // Values exceeding this is assumed to be tracking errors and ignored
    public float hold;              // Amount of time to keep moving after movement was detected
    public float maximumSwing;      // Maximum amount of movement, this prevents too fast movements

    private float oldamount;
    private float holdtime;

    private bool initialized = false;

    /// <summary>
    /// Calculates the amount of swing in the arms based on the previous location of the controllers
    /// </summary>
    /// <returns>amount of movement detected</returns>
	float getArmSwingerAmount(){
		Vector3 newL,newR;
		newL = Left.transform.localPosition;
		newR = Right.transform.localPosition;
        float amountL = Vector3.Distance(newL, oldL);
        float amountR = Vector3.Distance(newR, oldR);
        float amount = amountL + amountR;
        oldL = newL;
        oldR = newR;
        if (movingBothRequired)
        {
            if(amountL < minMoveThreshold || amountR < minMoveThreshold || amountL > maxMoveThreshold || amountR > maxMoveThreshold)
            {
                return 0;
            }
        }
        else
        {
            if(amount < minMoveThreshold || amount > maxMoveThreshold)
            {
                return 0;
            }
        }
        if(amount > maximumSwing)
        {
            amount = maximumSwing;
        }
		return amount;
	}


	void FixedUpdate ()
    {
        if (!initialized)
        {
            oldL = Left.transform.localPosition;
            oldR = Right.transform.localPosition;
            initialized = true;
        }
		float armSwingerAmount = getArmSwingerAmount();
        holdtime -= Time.deltaTime;
        if(holdtime > 0 && oldamount > armSwingerAmount)
        {
            armSwingerAmount = oldamount;
        }
        else if(armSwingerAmount > oldamount)
        {
            holdtime = hold;
        }
        oldamount = armSwingerAmount;
        float move = armSwingerAmount;
        MoveForward(move);

        // Check the trial ending button
        EndButton(SteamVR_Actions.vr_interaction.EndTrial.GetStateDown(SteamVR_Input_Sources.Any));
     }
}

