using System.Collections.Generic;
using UnityEngine;
using UXF;

public class KeyboardLocomotion : Locomotion
{
    public float rotateSpeed;

    void Update()
    {
        Rotate(Input.GetAxis("Horizontal") * rotateSpeed);
        MoveForward(Input.GetAxis("Vertical"));
        EndButton(Input.GetButtonDown("Fire1"));
    }
}
