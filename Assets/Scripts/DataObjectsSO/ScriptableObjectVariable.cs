using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using vc;

[CreateAssetMenu(fileName = "Scriptable Variable", menuName = "Variables/ScriptableVariableSO")]
public class ScriptableObjectVariable : ScriptableObject
{
    [SerializeField] string label;
    [SerializeField] ScriptableObjectBase _value;

    public ScriptableObjectBase Value
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

    public event UnityAction<ScriptableObjectBase> OnValueChanged;
}
