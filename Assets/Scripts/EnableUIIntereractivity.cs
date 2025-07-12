using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnableUIIntereractivity : MonoBehaviour,IToggle
{
    [SerializeField] List<Button> objectsToToggle;

    public void Toggle()
    {
        foreach(var o in objectsToToggle)
        {
            o.interactable = !o.interactable;
        }
    }
}
