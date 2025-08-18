using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AnimationCurve))]
public class AnimationCurvePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        
        EditorGUI.BeginProperty(new Rect(position.x, position.y, position.width,  position.height), label, property);
        // draw the label
        position = EditorGUI.PrefixLabel(new Rect(position.x, position.y, position.width, position.height), GUIUtility.GetControlID(FocusType.Passive),label);

        // draw the content        
        EditorGUI.PropertyField(new Rect(position.x,position.y,position.width, position.height), property, GUIContent.none);
        EditorGUI.EndProperty();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 100f;
    }
}
