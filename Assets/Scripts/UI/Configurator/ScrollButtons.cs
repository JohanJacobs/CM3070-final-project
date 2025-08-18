using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScrollButtons : MonoBehaviour
{
    [SerializeField] RectTransform contentRect;
    float stepSize;
    float minPos;
    float maxPos;

    [SerializeField] Button leftButton;
    [SerializeField] Button rightButton;

    private void Awake()
    {
        contentRect = GetComponent<ScrollRect>().content;
        var pageCount = contentRect.childCount;

        var contentWidth = 0f;
        for (int i = 0; i < contentRect.childCount; i++) 
        {
            float childWidth = contentRect.GetChild(i).GetComponent<RectTransform>().rect.width;
            contentWidth += childWidth;
        }
        
        stepSize = contentWidth / pageCount;

        //set default positions         
        minPos = 0f;
        maxPos = -stepSize * (pageCount-1);      
    }
        
    private void OnEnable()
    {
        leftButton.onClick.AddListener(LeftButton_OnClick);
        rightButton.onClick.AddListener(RightButton_OnClick);
    }
    private void OnDisable()
    {
        leftButton.onClick.RemoveListener(LeftButton_OnClick);
        rightButton.onClick.RemoveListener(RightButton_OnClick);
    }

    void LeftButton_OnClick()
    {
        var newX = contentRect.anchoredPosition.x + stepSize;
        if (newX > minPos)
            newX = maxPos;

        contentRect.anchoredPosition = new Vector2(newX, contentRect.anchoredPosition.y); 
    }
    void RightButton_OnClick()
    {
        var newX = contentRect.anchoredPosition.x - stepSize;
        if (newX < maxPos)
            newX = minPos;

        contentRect.anchoredPosition = new Vector2(newX, contentRect.anchoredPosition.y);
    }
}
