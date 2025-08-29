
using TMPro;
using UnityEngine;

public class UIEnableDisableVariableChanger : MonoBehaviour
{
    [SerializeField] BoolVariable variable;

    [SerializeField] TextMeshProUGUI label;

    [SerializeField] TMPro.TMP_Dropdown dropdown;

    string enableText = "Enabled";
    string disableText = "Disabled";
    private void Awake()
    {
        label.text = variable.VariableLabel;

        dropdown.options.Clear();
        dropdown.options.Add(new TMP_Dropdown.OptionData(enableText));
        dropdown.options.Add(new TMP_Dropdown.OptionData(disableText));
        
    }

    int enableSelectedOptionValue = 0;
    int disableSelectedOptionValue = 1;
    private void Start()
    {
        dropdown.SetValueWithoutNotify(variable.Value ? enableSelectedOptionValue : disableSelectedOptionValue);
    }
    private void OnEnable()
    {
        dropdown.onValueChanged.AddListener(TMPDropdown_OnValueChanged);
    }

    private void OnDisable()
    {
        dropdown.onValueChanged.RemoveListener(TMPDropdown_OnValueChanged);
    }


    void TMPDropdown_OnValueChanged(int option)
    {
        if (dropdown.options[option].text == enableText)
        {
            variable.Value = true;
            Debug.Log($"Setting {variable.VariableLabel} to true");
        }
        if (dropdown.options[option].text == disableText)
        {
            variable.Value = false;
            Debug.Log($"Setting {variable.VariableLabel} to false");
        }
    }
}
