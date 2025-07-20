using TMPro;
using UnityEngine;

public class UIDisplayRPM : MonoBehaviour
{
    [SerializeField] FloatVariable currentRPM;
    [SerializeField] FloatVariable maxRPM;
    [SerializeField] string preFix;
    [SerializeField] string postFix;
    [SerializeField] TextMeshProUGUI textDisplay;

    public void Start()
    {
    }

    private void OnEnable()
    {
        currentRPM.OnValueChanged += Variable_OnValueChanged;        
    }

    private void OnDisable()
    {
        currentRPM.OnValueChanged -= Variable_OnValueChanged;
    }

    void Variable_OnValueChanged(float newValue)
    {
        textDisplay.text =preFix + (Mathf.RoundToInt(newValue/10f)*10f).ToString("f0") + postFix;
        //TODO: Redline start should be a variable
        if (newValue > (maxRPM.Value-500f))
            textDisplay.color = Color.red;
        else
            textDisplay.color = Color.white;

    }
}
