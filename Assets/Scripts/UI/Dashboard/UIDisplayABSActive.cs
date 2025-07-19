using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDisplayABSActive : MonoBehaviour
{
    [SerializeField] Color ActiveColor;
    [SerializeField] Color inactiveColor;
    [SerializeField] BoolVariable absVariable;
    [SerializeField] TextMeshProUGUI text;

    Color GetTextColor => absVariable.Value ? ActiveColor : inactiveColor;
    public void Update()
    {
        text.color = GetTextColor;
    }

}
