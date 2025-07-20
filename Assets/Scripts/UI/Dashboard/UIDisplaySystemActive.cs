using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDisplaySystemActive : MonoBehaviour
{
    [SerializeField] Color ActiveColor;
    [SerializeField] Color inactiveColor;
    [SerializeField] BoolVariable Variable;
    [SerializeField] TextMeshProUGUI text;

    Color GetTextColor => Variable.Value ? ActiveColor : inactiveColor;
    public void Update()
    {
        text.color = GetTextColor;
    }

}
