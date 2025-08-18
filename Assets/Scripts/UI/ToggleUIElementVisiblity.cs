using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleUIElementVisiblity : MonoBehaviour,IToggle
{
    [SerializeField] bool VisibleOnStart=false;
    [SerializeField] Transform UIElement;
    void Awake()
    {
    }
    void Start()
    {
        transform.gameObject.SetActive(VisibleOnStart);
    }

    public void Toggle()
    {
        transform.gameObject.SetActive(!transform.gameObject.activeSelf);
    }

    
}
