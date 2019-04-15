using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace UXF
{
    /// <summary>
    /// Attach this component to a gameobject and assign it in the trackedObjects field in an ExperimentSession to automatically record position/rotation of the object at each frame.
    /// </summary>
    public class PlayerTracker : Tracker
    {

        public GameObject TrackedButtons;
        
        
        
        /// <summary>
        /// Sets measurementDescriptor and customHeader to appropriate values
        /// </summary>
        protected override void SetupDescriptorAndHeader()
        {
            measurementDescriptor = "movement";
            
            customHeader = new string[]
            {
                "pos_x",
                "pos_z",
                "rot_y",
                "Sound On",
                "Lights On"
            };
        }

        /// <summary>
        /// Returns current position and rotation values
        /// </summary>
        /// <returns></returns>
        protected override string[] GetCurrentValues()
        {
            // get position and rotation
            Vector3 p = gameObject.transform.position;
            Vector3 r = gameObject.transform.eulerAngles;


            SoundLightController soundlight = TrackedButtons.GetComponent<SoundLightController>();
            bool soundOn = soundlight.soundOn;
            bool lightOn = soundlight.lightOn;

            string format = "0.####";

            // return position, rotation (x, y, z) as an array
            var values =  new string[]
            {
                p.x.ToString(format),
                p.z.ToString(format),
                r.y.ToString(format),
                soundOn.ToString(),
                lightOn.ToString()
            };

            return values;
        }
    }
}