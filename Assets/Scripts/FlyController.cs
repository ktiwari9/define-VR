using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyController : MonoBehaviour
{
    public GameObject Dynamic;

    // Settings to be received from experiment creator
    private float speed;
    private float radius;
    private float offset;
    private float minHeight;
    private float maxHeight;
    private float offsetStepSize;

    // Helper variables
    private float targetHeight;
    private float targetOffset;
    private float currentHeight;
    private float currentOffset;
    private bool goingUp;
    private bool goingOut;
    private Vector3 oldpos;

  



    public void setConfigurations(float flySpeed, float flyRadius, float flyOffset, float flyMinHeight, float flyMaxHeight, float flyOffsetStepSize)
    {
        speed = flySpeed;
        setRadius(flyRadius);
        offset = flyOffset;
        minHeight = flyMinHeight;
        maxHeight = flyMaxHeight;
        offsetStepSize = flyOffsetStepSize;
    }

    public void setRadius(float rad)
    {
        if(rad < 0)
        {
            Dynamic.SetActive(false);
        }
        else
        {
            Dynamic.SetActive(true);
            radius = rad;
        }
    }


    
    // Update is called once per frame
    void FixedUpdate()
    {

        foreach (Transform child in Dynamic.transform) { 

            float x = Mathf.Cos(Time.time * speed);
            float z = Mathf.Sin(Time.time * speed);
            Vector3 newpos = new Vector3(x * radius, child.localPosition.y, z * radius);

            // Height offset
            // If target reached, select new one
            if ((goingUp && currentHeight > targetHeight) || (!goingUp && currentHeight < targetHeight))
            {
                targetHeight = UnityEngine.Random.Range(minHeight, maxHeight);
                goingUp = targetHeight > currentHeight;
            }
            currentHeight = newpos.y + (goingUp ? offsetStepSize : -offsetStepSize);

            // Horizontal offset
            currentOffset = (new Vector2(child.localPosition.x, child.localPosition.z)).magnitude - radius;
            if ((goingOut && currentOffset > targetOffset) || (!goingOut && currentOffset < targetOffset))
            {
                targetOffset = UnityEngine.Random.Range(-offset, offset);
                goingOut = targetOffset > currentOffset;
            }

            // Set modifications
            newpos.x = newpos.x + offsetStepSize * (goingOut ? x : -x) + currentOffset * x;
            newpos.y = currentHeight;
            newpos.z = newpos.z + offsetStepSize * (goingOut ? z : -z) + currentOffset * z;

            // Rotate to face correct direction
            child.localRotation = Quaternion.LookRotation(newpos - child.localPosition);

            oldpos = newpos;

            // Set the new position
            child.localPosition = newpos;
        }
    }
}
