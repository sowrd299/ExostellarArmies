using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomPropertyDrawer(typeof(CardUIProperty))]
public class CardUIPropertiesDrawer : PropertyDrawer
{
	// public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	// {
	// 	return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
	// }

	public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
	{
		SerializedProperty element = property.FindPropertyRelative("element");

		EditorGUI.ObjectField(
			new Rect(pos.x, pos.y, EditorGUIUtility.labelWidth, pos.height),
			element, typeof(Element), GUIContent.none
		);

		Rect fieldRect = new Rect(
			pos.x + EditorGUIUtility.labelWidth, pos.y, pos.width - EditorGUIUtility.labelWidth, pos.height);
		// GUIContent fieldLabel = new GUIContent(element.objectReferenceValue.name);

		if (element.objectReferenceValue is ElementText || element.objectReferenceValue is ElementInt)
		{
			SerializedProperty text = property.FindPropertyRelative("text");

			EditorGUI.ObjectField(fieldRect, text, typeof(Text), GUIContent.none);
		}
		else if (element.objectReferenceValue is ElementImage)
		{
			SerializedProperty image = property.FindPropertyRelative("image");

			EditorGUI.ObjectField(fieldRect, image, typeof(SVGImage), GUIContent.none);
		}
	}
}
