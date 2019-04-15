using System.Collections.Generic;
using UnityEngine;
using UXF;

/// <summary>
/// The class that other locomotion methods inherit
/// </summary>


public class Locomotion : MonoBehaviour
{
    // Public variables, to be read from config file
    public float velocityMultiplier;
    public float maxForwardSpeed;

    // Variables for internal use
    protected GameObject player;
    protected GameObject experimentControls;
    protected CharacterController characterController;
    protected ExperimentController experimentController;
    protected bool pressed;

    public void Start()
    {
        player = GameObject.Find("Player");
        experimentControls = GameObject.Find("Experiment Controls");
        characterController = player.GetComponent<CharacterController>();
        experimentController = experimentControls.GetComponent<ExperimentController>();
    }

    protected void Rotate(float amount)
    {
        player.transform.Rotate(0, amount * Time.deltaTime, 0);
    }

    protected void MoveForward(float amount)
    {
        Vector3 forwardVector = player.transform.TransformDirection(Vector3.forward);
        float moveamount = velocityMultiplier * amount * maxForwardSpeed;

        // Limit speed to maximum
        if(moveamount > maxForwardSpeed)
        {
            moveamount = maxForwardSpeed;
        }
        // Backwards limit
        if(moveamount < -maxForwardSpeed)
        {
            moveamount = -maxForwardSpeed;
        }
        characterController.SimpleMove(forwardVector * moveamount);
    }

    protected void EndButton(bool button)
    {
        if (button && !pressed)
        {
            pressed = true;

            // Call ExperimentController to end the trial
            experimentController.tryToEnd();
        }
        if (!button && pressed)
        {
            pressed = false;
        }
    }
}
