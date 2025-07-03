using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "String Variable", menuName = "Variables/String engineCurrentRPM")]
public class StringVariable : ScriptableObject
{
    [SerializeField] string label;    
    [SerializeField] string _value;

    public string Value
    {
        get => _value;
        set
        {
            if (this._value == value) return;
            this._value = value;

            OnValueChanged?.Invoke(this._value);
        }
    }
    public string VariableLabel => label;

    public event UnityAction<string> OnValueChanged;
}
