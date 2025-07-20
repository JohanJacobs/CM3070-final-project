using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

// Unity inspector
[CustomPropertyDrawer(typeof(BoolVariable))]
public class BoolVariableInspector : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();
        var objectField = new ObjectField(property.displayName)
        {
            objectType = typeof(BoolVariable)
        };

        objectField.BindProperty(property);

        var valueLabel = new Label();
        valueLabel.style.paddingLeft = 20;

        container.Add(objectField);
        container.Add(valueLabel);

        objectField.RegisterValueChangedCallback(
            (EventCallback<ChangeEvent<UnityEngine.Object>>)(e =>
            {
                var variable = e.newValue as BoolVariable;
                if (variable != null)
                {
                    valueLabel.text = $"Current Value: {variable.Value}";
                    variable.OnValueChanged += (bool newValue) => valueLabel.text = $"Current Value: {newValue}";
                }
                else
                {
                    valueLabel.text = string.Empty;
                }
            })
        );

        var currentVariable = property.objectReferenceValue as BoolVariable;

        if (currentVariable != null)
        {
            valueLabel.text = $"Current Value: {currentVariable.Value}";
            currentVariable.OnValueChanged += newValue => valueLabel.text = $"Current Value: {currentVariable.Value}";
        }
        return container;
    }
}