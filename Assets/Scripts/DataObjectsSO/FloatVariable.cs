using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Float Variable", menuName = "Variables/FloatVariableSO")]
public class FloatVariable : ScriptableObject
{
    [SerializeField] string label;
    [SerializeField] float _value;

    public float Value
    {
        get => this._value;
        set
        {
            if (this._value == value) return;
            this._value = value;

            OnValueChanged?.Invoke(this._value);
        }
    }
    public string VariableLabel=> label;

    public event UnityAction<float> OnValueChanged;
}
