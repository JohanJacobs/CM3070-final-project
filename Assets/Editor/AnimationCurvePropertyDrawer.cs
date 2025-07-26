using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AnimationCurve))]
public class AnimationCurvePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position,label,property);
        // draw the label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive),label);

        // draw the content        
        EditorGUI.PropertyField(new Rect(position.x,position.y,position.width,position.height + 200f), property, GUIContent.none);
        EditorGUI.EndProperty();
    }
}
