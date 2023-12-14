using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextManager))]
public class TextManagerEditor : Editor {
    public SerializedProperty decorators;

    public bool showDecorator = true;
    public int selectedOpt = 0;

    private void OnEnable() {
        this.decorators = serializedObject.FindProperty("decorator");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        // 显示一个开关，用于控制列表是否显示
        this.decorators = serializedObject.FindProperty("decorator");
        this.showDecorator = EditorGUILayout.Foldout(this.showDecorator, "Decorators");
        if(this.showDecorator)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.decorators);
            EditorGUI.indentLevel--;
        }
        serializedObject.ApplyModifiedProperties();
    }
}