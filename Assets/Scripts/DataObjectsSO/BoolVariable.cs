using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Bool Variable", menuName = "Variables/BoolVariableSO")]
public class BoolVariable : ScriptableObject
{
    [SerializeField] string label;
    [SerializeField] bool _value;

    public bool Value
    {
        get => this._value;
        set
        {
            if (this._value == value) return;
            this._value = value;

            OnValueChanged?.Invoke(this._value);
        }
    }
    public string VariableLabel => label;

    public event UnityAction<bool> OnValueChanged;
}
