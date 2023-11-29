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
    /// <summary>
    /// 动态获取一个基本类型下的所有子类
    /// </summary>
    /// <param name="baseType">基本类型</param>
    /// <returns>返回一个可迭代的子类列表</returns>
    private static IEnumerable GetClasses(Type baseType)
    {
        return Assembly.GetAssembly(baseType).GetTypes().Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t));
    }

    /// <summary>
    /// 获取子类构造Map
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, Func<TextDecorator>> DECO()
    {
        Dictionary<string, Func<TextDecorator>> deco = new();
        foreach(Type tp in GetClasses(typeof(TextDecorator)))
        {
            deco.Add(tp.Name,()=>Activator.CreateInstance(tp) as TextDecorator);
        }
        return deco;
    }
    public override void OnInspectorGUI() {
        serializedObject.Update();
        //获取构造器字典的键
        var deco = TextManagerEditor.DECO();
        string[] options = new List<string>(deco.Keys).ToArray();
        //横向布局启动！
        EditorGUILayout.BeginHorizontal();
        //创建一个下拉菜单，菜单内容为options,返回选中的index
        this.selectedOpt = EditorGUILayout.Popup("Next Decorator", this.selectedOpt, options);
        //创建一个添加按钮，将刚刚选中的类型实例化，并添加到list中去
        if(GUILayout.Button("Add"))
        {
            TextDecorator newDeco = deco[options[selectedOpt]]();
            TextDecorator oriDeco = this.decorators.managedReferenceValue as TextDecorator;
            if(oriDeco == null)
            {
                this.decorators.managedReferenceValue = newDeco;
            }
            else 
            {
                newDeco.SetInner(oriDeco);
                this.decorators.managedReferenceValue = newDeco;
            }
        }
        //创建一个按钮，将list最末尾的一个元素去掉
        if(GUILayout.Button("Pop") && this.decorators.managedReferenceValue != null)
        {
            this.decorators.managedReferenceValue = (this.decorators.managedReferenceValue as TextDecorator).inner;//
        }
        //横向布局结束
        EditorGUILayout.EndHorizontal();
        // 显示一个开关，用于控制列表是否显示
        this.showDecorator = EditorGUILayout.Foldout(this.showDecorator, "Decorators");
        if(this.showDecorator)
        {
            //绘制list的每一个元素
            EditorGUI.indentLevel++;
            DrawDecorators();
            EditorGUI.indentLevel--;
        }
        serializedObject.ApplyModifiedProperties();
    }

    public void DrawDecorators()
    {
        SerializedProperty current = this.decorators;
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