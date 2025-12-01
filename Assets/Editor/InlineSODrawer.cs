using UnityEditor;
using UnityEngine;

/// <summary>
/// Makes any ScriptableObject referenced in a field expand inline in the inspector.
/// </summary>
[CustomPropertyDrawer(typeof(ScriptableObject), true)]
public class InlineSODrawer : PropertyDrawer
{
    private bool foldout;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;

        if (!foldout || property.objectReferenceValue == null)
            return height;

        SerializedObject so = new SerializedObject(property.objectReferenceValue);
        SerializedProperty p = so.GetIterator();

        p.NextVisible(true);
        while (p.NextVisible(false))
        {
            if (p.name == "m_Script") continue;
            height += EditorGUI.GetPropertyHeight(p, true) + EditorGUIUtility.standardVerticalSpacing;
        }

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Foldout
        foldout = EditorGUI.Foldout(
            new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight),
            foldout, label, true);

        // Object field
        Rect objFieldRect = new Rect(
            position.x + EditorGUIUtility.labelWidth,
            position.y,
            position.width - EditorGUIUtility.labelWidth,
            EditorGUIUtility.singleLineHeight);

        EditorGUI.PropertyField(objFieldRect, property, GUIContent.none);

        // Draw inline contents
        if (foldout && property.objectReferenceValue != null)
        {
            EditorGUI.indentLevel++;

            SerializedObject so = new SerializedObject(property.objectReferenceValue);
            SerializedProperty p = so.GetIterator();

            float y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            p.NextVisible(true);
            while (p.NextVisible(false))
            {
                if (p.name == "m_Script") continue;

                float h = EditorGUI.GetPropertyHeight(p, true);
                Rect r = new Rect(position.x, y, position.width, h);

                EditorGUI.PropertyField(r, p, true);
                y += h + EditorGUIUtility.standardVerticalSpacing;
            }

            so.ApplyModifiedProperties();
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }
}
