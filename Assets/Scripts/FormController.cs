using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using SimpleWebBrowser;
using MessageLibrary;
using System;

public class FormController : MonoBehaviour
{
    public LineRenderer lineLeft;
    public LineRenderer lineRight;
    public TrackedPoseDriver Left;
    public TrackedPoseDriver Right;
    public WebBrowser browser;

    public List<string> stage1Addresses;
    public List<string> stage2Addresses;
    public List<string> stage3Addresses;

    public bool useSeparateCamera;

    private bool usingVR;
    private int formIndex = -1;
    private string partID;
    private int activeStage = 1;

    private bool formLoaded = true;

    // A public flag that can be checked from environment controller etc to see if forms are still being filled.
    [HideInInspector]
    public bool fillingForms = false;

    // Start is called before the first frame update
    void Start()
    {
        usingVR = UnityEngine.XR.XRDevice.isPresent;
        browser.OnPageLoaded += OnPageLoaded;
    }

    public void resetForms()
    {
        browser.StopAllCoroutines();
        formIndex = -1;
        formLoaded = false;
        fillingForms = false;
    }


    /// <summary>
    /// Show the forms. Id is participant id.
    /// </summary>
    /// <param name="id"></param>
    public void showForms(string id, int listStage)
    {
        activeStage = listStage;
        partID = id;
        browser.show(useSeparateCamera);
        fillingForms = true;
        formIndex = -1;
        if (usingVR)
        {
            lineRight.enabled = true;
            lineLeft.enabled = true;
        }
        if (!formLoaded)
        {
            nextForm();
        }
    }

    public void hideForms()
    {
        browser.hide(useSeparateCamera);
        activeStage = -1;
        lineRight.enabled = false;
        lineLeft.enabled = false;
    }

    public void OnPageLoaded(string url)
    {
        // Loaded next form
        if (!formLoaded)
        {
            formLoaded = true;
        }

        // Form was answered (since it was already loaded)
        else
        {
            formLoaded = false;
            nextForm();
        }
    }

    public void nextForm()
    {
        List<string> addresses = null;
        switch(activeStage)
        {
            case 1:
                addresses = stage1Addresses;
                break;
            case 2:
                addresses = stage2Addresses;
                break;
            case 3:
                addresses = stage3Addresses;
                break;
        }

        if (addresses != null)
        {
            if (formIndex + 1 == addresses.Count)
            {
                fillingForms = false;
                hideForms();
                return;
            }
            formIndex += 1;
            browser.LoadPage(addresses[formIndex]);
            return;
        }
        hideForms();
        fillingForms = false;
        return;
    }

    internal void setForms(List<string> formsToSet, int v)
    {
        switch (v)
        {
            case 1:
                stage1Addresses = formsToSet;
                break;
            case 2:
                stage2Addresses = formsToSet;
                break;
            case 3:
                stage3Addresses = formsToSet;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        LineRenderer line;
        TrackedPoseDriver controller;
        Valve.VR.SteamVR_Input_Sources source;
        for (int cont = 1; cont < 3; cont++)
        {
            if (cont == 1)
            {
                line = lineLeft;
                controller = Left;
                source = Valve.VR.SteamVR_Input_Sources.LeftHand;
            }
            else
            {
                line = lineRight;
                controller = Right;
                source = Valve.VR.SteamVR_Input_Sources.RightHand;
            }
            Vector3 x = controller.transform.position;
            Vector3 v = controller.transform.forward;
            line.SetPosition(0, x);
            Vector3 hitPosition = transform.position;
            RaycastHit hit = new RaycastHit();
            Ray ray = new Ray(x, v);
            for (int i = 1; i < 15; ++i)
            {
                if (Physics.Raycast(ray, out hit, v.magnitude * 1.01f))
                {
                    x = hit.point;
                    hitPosition = x;
                    line.positionCount = i + 1;
                    line.SetPosition(i, x);
                    break;
                }
                else
                {
                    x += v;
                    line.positionCount = i + 1;
                    line.SetPosition(i, x);
                }
            }

            if (!usingVR || !fillingForms)   // Not using VR or browser not visible, no point in checking VR inputs
            {
                return;
            }
            Vector2 pixelUV = browser.GetScreenCoords(ray);

            // User pressed the button down
            if (Valve.VR.SteamVR_Actions.vr_interaction.EndTrial.GetStateDown(source))
            {
                browser.SendMouseButtonEvent((int)pixelUV.x, (int)pixelUV.y, MouseButton.Left, MouseEventType.ButtonDown);
            }

            // User released the button
            if (Valve.VR.SteamVR_Actions.vr_interaction.EndTrial.GetStateUp(source))
            {
                browser.SendMouseButtonEvent((int)pixelUV.x, (int)pixelUV.y, MouseButton.Left, MouseEventType.ButtonUp);
            }

            // Grip, copy paste participant ID to browser
            if (Valve.VR.SteamVR_Actions.vr_interaction.Grip.GetLastStateDown(source))
            {
                sendParticipantID();
            }

            // User scrolled
            float scroll = 0;
            if (Valve.VR.SteamVR_Actions.vr_interaction.TrackpadUp.GetStateDown(source))
            {
                scroll = 0.25f;
            }
            if (Valve.VR.SteamVR_Actions.vr_interaction.TrackpadDown.GetStateDown(source))
            {
                scroll = -0.25f;
            }
            browser.ProcessScrollInput((int)pixelUV.x, (int)pixelUV.y, scroll);
        }
    }

    public void sendParticipantID()
    {
        if (fillingForms)
        {
            browser.sendString(partID);
        }
    }
}
