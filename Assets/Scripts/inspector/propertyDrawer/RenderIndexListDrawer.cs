using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RenderIndexList))]
public class RenderIndexListDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty mergedList = property.FindPropertyRelative("mergedRenderIndexSets");
        return mergedList.isExpanded? 
                (EditorGUIUtility.singleLineHeight+EditorGUIUtility.standardVerticalSpacing)*(mergedList.arraySize+1):
                EditorGUIUtility.singleLineHeight+EditorGUIUtility.standardVerticalSpacing;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty lst = property.FindPropertyRelative("mergedRenderIndexSets");
        EditorGUI.BeginProperty(position, label, property);
        lst.isExpanded = EditorGUI.Foldout(position,lst.isExpanded,new GUIContent("Index Lists"));
        if(lst.isExpanded)
        {
            for(int i=0;i<lst.arraySize;i++)
            {
                SerializedProperty indexSet = lst.GetArrayElementAtIndex(i);
                EditorGUI.PropertyField(position,indexSet,new GUIContent($"{i}"));
                position.y += EditorGUIUtility.singleLineHeight+EditorGUIUtility.standardVerticalSpacing;
            }
        }
        EditorGUI.EndProperty();
    }

}


[CustomPropertyDrawer(typeof(RenderIndexSet))]
public class RenderIndexSetDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight+EditorGUIUtility.standardVerticalSpacing;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty startIdx = property.FindPropertyRelative("startIndex");
        SerializedProperty endIdx = property.FindPropertyRelative("endIndex");
        EditorGUI.BeginProperty(position, label, property);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(startIdx,new GUIContent("Start"));
        EditorGUILayout.PropertyField(endIdx,new GUIContent("End"));
        EditorGUILayout.EndHorizontal();
        EditorGUI.EndProperty();
    }
}