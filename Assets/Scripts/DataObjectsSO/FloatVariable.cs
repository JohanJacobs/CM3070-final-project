using UnityEngine;
using UnityEngine.Events;

/*
    Scriptable object variables allows a primitive type 
    to be modified and notify observers of the type when the
    value has changed. Additionaly it decouples systems 
    that use the variable
 */
[CreateAssetMenu(fileName = "Float Variable", menuName = "Variables/FloatVariableSO")]
public class FloatVariable : ScriptableObject
{
    [SerializeField] string label;
    [SerializeField] float _value;

    public float Value
    {
        // Return the current value that this variable is storing 
        get => this._value;
        set
        {
            if (this._value == value) return;
            this._value = value;

            // notify listeners that this variable has changed.
            OnValueChanged?.Invoke(this._value);
        }
    }

    // Return the current Label for this variable.
    public string VariableLabel=> label;

    // Observer pattern: this event can be subscribed to so that all 
    // listeners are notified when this variable changes.
    public event UnityAction<float> OnValueChanged;
}
