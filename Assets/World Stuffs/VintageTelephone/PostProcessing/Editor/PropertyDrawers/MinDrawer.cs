using UnityEngine;
using UnityEditor;

namespace VintageTelephone.PostProcessing.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(UnityEngine.PostProcessing.MinAttribute))]
    public class MinDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var minAttribute = (UnityEngine.PostProcessing.MinAttribute)attribute;

            if (property.propertyType == SerializedPropertyType.Float)
            {
                property.floatValue = Mathf.Max(minAttribute.min, EditorGUI.FloatField(position, label, property.floatValue));
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = Mathf.Max((int)minAttribute.min, EditorGUI.IntField(position, label, property.intValue));
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use Min with float or int.");
            }
        }
    }
}
