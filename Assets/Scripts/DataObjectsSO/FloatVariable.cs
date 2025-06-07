using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Float Variable", menuName = "Variables/Float variable")]
public class FloatVariable : ScriptableObject
{
    [SerializeField] string label;
    [SerializeField] float initialValue;
    [SerializeField] float value;

    public float Value
    {
        get => value;
        set
        {
            if (this.value == value) return;
            this.value = value;
            OnValueChanged?.Invoke(value);
        }
    }

    public event UnityAction<float> OnValueChanged;
}
