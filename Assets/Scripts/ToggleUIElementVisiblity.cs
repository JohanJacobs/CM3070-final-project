using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleUIElementVisiblity : MonoBehaviour,IToggle
{
    [SerializeField] bool VisibleOnStart=false;
    void Start()
    {
        transform.gameObject.SetActive(VisibleOnStart);
    }

    public void Toggle()
    {
        transform.gameObject.SetActive(!transform.gameObject.activeSelf);
    }
}
