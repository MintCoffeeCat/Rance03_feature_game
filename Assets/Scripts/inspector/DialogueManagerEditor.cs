using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueManager))]
public class DialogueManagerEditor : Editor {
    public SerializedProperty dialogue;
    public SerializedProperty currentUnit;
    public SerializedProperty units;
    public bool showDialogue = true;
    public int selectedOpt = 0;

    private void OnEnable() {
        this.dialogue = serializedObject.FindProperty("dialogue");
        this.currentUnit = this.dialogue.FindPropertyRelative("currentUnit");
        this.units = this.dialogue.FindPropertyRelative("units");
    }
    public override void OnInspectorGUI() {
        serializedObject.Update();

        this.showDialogue = EditorGUILayout.Foldout(this.showDialogue, "Dialogue");
        // if(this.showDialogue)
        // {
        //     this.DrawTalkUnits();
        // }
    }

    public void DrawTalkUnits()
    {
        EditorGUI.indentLevel++;
        for(int i=0;i<this.units.arraySize;i++)
        {
            SerializedProperty unit = this.units.GetArrayElementAtIndex(i);
            SerializedProperty decorator = unit.FindPropertyRelative("decorator");
            SerializedProperty content = unit.FindPropertyRelative("content");
            SerializedProperty speaker = unit.FindPropertyRelative("speaker");
            EditorGUILayout.PropertyField(speaker);
            EditorGUILayout.PropertyField(content);
            this.DrawDecorators(decorator);
        }
        EditorGUI.indentLevel--;
    }
    public void DrawDecorators(SerializedProperty decorator)
    {
        SerializedProperty current = decorator;
        while(current.managedReferenceValue != null)
        {
            string classname = current.managedReferenceFullTypename;
            string removedStr = "Assembly-CSharp ";
            classname = classname.Replace(removedStr,"");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(
                current, 
                new GUIContent(classname), true);
            EditorGUILayout.EndHorizontal();
            current = current.FindPropertyRelative("inner");
        }
    }
}