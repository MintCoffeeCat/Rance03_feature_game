using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextManager))]
public class TextManagerEditor : Editor {
    public SerializedProperty decorators;

    public bool showDecorator = true;
    public int selectedOpt = 0;

    public static Dictionary<string, Func<TextDecorator>> DECO = new(){
        {"RainbowText", ()=> new RainbowText()},
        {"ShakeText",()=>new ShakeText()}
    };

    private void OnEnable() {
        this.decorators = serializedObject.FindProperty("decorator");
    }

    public override void OnInspectorGUI() {
        // base.OnInspectorGUI();

        serializedObject.Update();
        // if(this.prop == null)
        //     return;
        
        // string[] testOpt = {"one", "two", "three"};

        string[] options = new List<string>(TextManagerEditor.DECO.Keys).ToArray();
        EditorGUILayout.BeginHorizontal();

        this.selectedOpt = EditorGUILayout.Popup("Next Decorator", this.selectedOpt, options);
        if(GUILayout.Button("Add"))
        {
            this.decorators.arraySize++;
            this.decorators.GetArrayElementAtIndex(this.decorators.arraySize - 1).managedReferenceValue = TextManagerEditor.DECO[options[selectedOpt]]();
        }
        if(GUILayout.Button("Pop") && this.decorators.arraySize > 0)
        {
            this.decorators.arraySize--;
        }
        EditorGUILayout.EndHorizontal();
        // 显示一个开关，用于控制列表是否显示
        this.showDecorator = EditorGUILayout.Foldout(this.showDecorator, "Decorators");
        if(this.showDecorator)
        {
            EditorGUI.indentLevel++;
            DrawDecorators();
            EditorGUI.indentLevel--;
        }
        serializedObject.ApplyModifiedProperties();
    }

    public void DrawDecorators()
    {
        EditorGUILayout.LabelField("List Length: " + this.decorators.arraySize, EditorStyles.boldLabel);

        // 绘制列表元素
        for (int i = 0; i <this.decorators.arraySize; i++)
        {
            SerializedProperty element = this.decorators.GetArrayElementAtIndex(i);
            string classname = element.managedReferenceFullTypename;
            string removedStr = "Assembly-CSharp ";
            classname = classname.Replace(removedStr,"");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(
                element, 
                new GUIContent(classname), true);
            EditorGUILayout.EndHorizontal();
        }
    }

}