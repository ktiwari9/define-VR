using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLightController : MonoBehaviour
{

    public GameObject curtain;

    public bool soundOn = true;
    public bool lightOn = true;

    public void ToggleSound() {

        if (soundOn)
        {

            AudioListener.volume = 0;
        }

        else {

            AudioListener.volume = 1;

        }

        soundOn = !soundOn;        

    }


    public void ToggleLight()
    {

        if (lightOn)
        {

            curtain.SetActive(true);
        }

        else
        {

            curtain.SetActive(false);

        }

        lightOn = !lightOn;

    }


    public void LightSoundReset() {


        curtain.SetActive(false);
        AudioListener.volume = 0;
        soundOn = false;
        lightOn = true;

    }



}
