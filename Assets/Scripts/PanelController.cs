using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public GameObject panel;

    public void EnablePanel() {

        panel.transform.Find("Score").gameObject.SetActive(true);
        panel.transform.Find("Timer").gameObject.SetActive(true);
        panel.GetComponent<Image>().enabled = true;


    }

    public void DisablePanel()
    {

        panel.transform.Find("Score").gameObject.SetActive(false);
        panel.transform.Find("Timer").gameObject.SetActive(false);
        panel.GetComponent<Image>().enabled = false;


    }

}
