using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfotextController : MonoBehaviour
{
    public void ToggleVisibility()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.enabled = !canvas.enabled;
    }
}
