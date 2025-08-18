using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBarDefaultPosition : MonoBehaviour
{
    [SerializeField] float defaultPosition;
    private void Start()
    {
        var s = GetComponent<ScrollRect>();        
        var newPos = 1f / s.content.childCount;
        defaultPosition = newPos / 2f;
        s.horizontalNormalizedPosition = defaultPosition;
    }
}
