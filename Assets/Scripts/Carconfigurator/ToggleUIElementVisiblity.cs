using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleUIElementVisiblity : MonoBehaviour,IToggle
{

    void Start()
    {
        transform.gameObject.SetActive(false);
    }

    public void Toggle()
    {
        transform.gameObject.SetActive(!transform.gameObject.activeSelf);
    }
}
