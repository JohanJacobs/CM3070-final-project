using UnityEngine;
using UnityEngine.Events;
using vc.VehicleComponent;

[CreateAssetMenu(fileName = "Transmission Type Variable", menuName = "Variables/Transmission Type VariableSO")]
public class TransmissionTypeVariable : ScriptableObject
{
    [SerializeField] string label;
    [SerializeField] TransmissionComponent.TransmissionType _value;

    public TransmissionComponent.TransmissionType Value
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

    public event UnityAction<TransmissionComponent.TransmissionType> OnValueChanged;
}
