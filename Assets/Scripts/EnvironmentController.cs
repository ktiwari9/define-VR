using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


/// <summary>
/// The controller of VR navigation experiments
/// </summary>
public class EnvironmentController : MonoBehaviour {
    public GameObject trainingEnvironment;
    public GameObject testingEnvironment;
    public GameObject forms;

    public GameObject trainingDefault;
    public GameObject testingDefault;


    public GameObject trainingCustom;
    public GameObject testingCustom;

    public PostProcessVolume postprocessing;

    public void setRoomSize(float x, float y, float z)
    {
        trainingEnvironment.transform.localScale = new Vector3(x,y,z);
    }

    public void chooseTexture(string textureType)
    {

        if (textureType == "custom")
        {
            activateCustom();

        }
        

        else {

            activateDefault();

        }

    }

    public void toggleFormCamera()
    {
        if (forms.GetComponent<FormController>().browser.MainCamera.isActiveAndEnabled)
        {
            forms.GetComponent<FormController>().browser.MainCamera.enabled = false;
        }
        else
        {
            forms.GetComponent<FormController>().browser.MainCamera.enabled = true;
        }
    }

    public void hideTraining()
    {
        trainingEnvironment.SetActive(false);
    }

    public void showTraining()
    {
        trainingEnvironment.SetActive(true);
    }

    public void hideTesting()
    {
        testingEnvironment.SetActive(false);
    }

    public void showTesting()
    {
        testingEnvironment.SetActive(true);
    }

    public void positionForms(Transform transform)
    {
        forms.transform.position = transform.position;
        forms.transform.rotation = transform.rotation;
    }

    public void enablePostProcessing()
    {
        postprocessing.weight = 1;
    }

    public void disablePostProcessing()
    {
        postprocessing.weight = 0;
    }

    public void showForms(string id, int index)
    {
        forms.GetComponent<FormController>().showForms(id, index);
    }

    public void hideForms()
    {
        forms.GetComponent<FormController>().hideForms();
    }

    public void resetForms()
    {
        forms.GetComponent<FormController>().resetForms();
    }

    public void activateDefault()
    {

        trainingDefault.SetActive(true);
        testingDefault.SetActive(true);
      
        trainingCustom.SetActive(false);
        testingCustom.SetActive(false);
    }


   
    public void activateCustom()
    {

        trainingDefault.SetActive(false);
        testingDefault.SetActive(false);
       
        trainingCustom.SetActive(true);
        testingCustom.SetActive(true);
    }

    internal bool getFormsBeingFilled()
    {
        return forms.GetComponent<FormController>().fillingForms;
    }

    internal void setForms(List<string> formsToSet, int v)
    {
        forms.GetComponent<FormController>().setForms(formsToSet, v);
    }
}