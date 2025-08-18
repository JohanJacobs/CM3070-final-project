using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIVariableChanger : MonoBehaviour
{
    [SerializeField] FloatVariable variable;
    [SerializeField] float minValue;
    [SerializeField] float maxValue;
    [SerializeField] float changeValue;

    TextMeshProUGUI label;
    Button lessButton;
    Button moreButton;
    TextMeshProUGUI valueLabel;

    private void Awake()
    {
        label = transform.Find("Label").GetComponent<TextMeshProUGUI>();
        lessButton = transform.Find("LessButton").GetComponent<Button>();
        moreButton = transform.Find("MoreButton").GetComponent<Button>();
        valueLabel = transform.Find("ValueLabel").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        label.text = variable.VariableLabel;
        Variable_OnValueChanged(variable.Value);
    }
    private void OnEnable()
    {
        variable.OnValueChanged += Variable_OnValueChanged;
        moreButton.onClick.AddListener(MoreButton_OnPressed);
        lessButton.onClick.AddListener(LessButton_OnPressed);
    }
    private void OnDisable()
    {
        variable.OnValueChanged -= Variable_OnValueChanged;
        moreButton.onClick.RemoveListener(MoreButton_OnPressed);
        lessButton.onClick.RemoveListener(LessButton_OnPressed);
    }
    private void MoreButton_OnPressed()
    {
        UpdateVariable(changeValue);
    }
    private void LessButton_OnPressed()
    {
        UpdateVariable(-changeValue);
    }
    
    private void UpdateVariable(float changeAmount)
    {
        variable.Value = Mathf.Clamp(variable.Value + changeAmount, minValue, maxValue);
    }
    private void Variable_OnValueChanged(float value)
    {
        valueLabel.text = value.ToString("F2");
    }
}
