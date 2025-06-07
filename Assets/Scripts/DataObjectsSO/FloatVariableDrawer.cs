using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

// Unity inspector
[CustomPropertyDrawer(typeof(FloatVariable))]
public class FloatvariableInspector : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();
        var objectField = new ObjectField(property.displayName)
        {
            objectType = typeof(FloatVariable)
        };

        objectField.BindProperty(property);

        var valueLabel = new Label();
        valueLabel.style.paddingLeft = 20;

        container.Add(objectField);
        container.Add(valueLabel);

        objectField.RegisterValueChangedCallback(
            e =>
            {
                var variable = e.newValue as FloatVariable;
                if (variable != null)
                {
                    valueLabel.text = $"Current Value: {variable.Value}";
                    variable.OnValueChanged += newValue => valueLabel.text = $"Current Value: {newValue}";
                }
                else
                {
                    valueLabel.text = string.Empty;
                }
            }
        );

        var currentVariable = property.objectReferenceValue as FloatVariable;

        if (currentVariable != null)
        {
            valueLabel.text = $"Current Value: {currentVariable.Value}";
            currentVariable.OnValueChanged += newValue => valueLabel.text = $"Current Value: {currentVariable.Value}";
        }
        return container;
    }
}