using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(ExpressionNameAttribute))]
public class ExpressionNameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        AboutMenuController controller = Object.FindFirstObjectByType<AboutMenuController>();

        if (controller != null)
        {
            List<string> options = controller.GetExpressionNames();

            if (options != null && options.Count > 0)
            {
                int currentIndex = Mathf.Max(0, options.IndexOf(property.stringValue));

                int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, options.ToArray());

                property.stringValue = options[selectedIndex];
                return;
            }
        }

        EditorGUI.PropertyField(position, property, label);
    }
}