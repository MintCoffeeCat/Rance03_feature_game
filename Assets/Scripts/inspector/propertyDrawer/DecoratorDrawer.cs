using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

// IngredientDrawerUIE
[CustomPropertyDrawer(typeof(TextDecorator))]
public class TextDecoratorDrawer : PropertyDrawer
{
    public int selectedOpt = 0;
    public string GetDerivedClassName(SerializedProperty property)
    {
        string classname = property.managedReferenceFullTypename;
        string removedStr = "Assembly-CSharp ";
        classname = classname.Replace(removedStr,"");
        return classname;
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int decoratorNum = 0;
        int spacingNum = 0;
        float totalHeight = 0;
        SerializedProperty current = property;
        while(current.managedReferenceValue != null)
        {
            SerializedProperty indexList = property.FindPropertyRelative("renderIndexList");
            if(indexList.managedReferenceValue != null)
            {
                //SerializedProperty sets = indexList.FindPropertyRelative("mergedRenderIndexSets");
                totalHeight += EditorGUI.GetPropertyHeight(indexList);
                // indexSetNum += sets.arraySize;
            }
            decoratorNum +=1;
            current = current.FindPropertyRelative("inner");
        }
        spacingNum = decoratorNum*2;
        totalHeight +=  EditorGUIUtility.singleLineHeight * (decoratorNum+1) + 
                        EditorGUIUtility.standardVerticalSpacing*spacingNum;
        return totalHeight;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        List<bool> res = new()
        {
            false,
            false
        };
        if(property.managedReferenceValue == null)
        {
            res = this.DrawButtons(position,property);
            this.SolveButton(res,property);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            return;
        }

        if((property.managedReferenceValue as TextDecorator).IsRoot())
        {
            res = this.DrawButtons(position,property);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        this.DrawSpecificUI(position,property);
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        this.DrawIndexListUI(position,property.FindPropertyRelative("renderIndexList"));
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        SerializedProperty inner = property.FindPropertyRelative("inner");
        if(inner.managedReferenceValue != null)
        {
            EditorGUI.PropertyField(new Rect(
                position.x, 
                position.y, 120, 
                EditorGUIUtility.singleLineHeight
                ), inner, GUIContent.none);
        }
        this.SolveButton(res,property);
        EditorGUI.EndProperty();
    }
    public List<bool> DrawButtons(Rect position,SerializedProperty decorator)
    {
        List<bool> res = new();
        float popUpWidth = 160;
        float buttonWidth = 60;
        float space = 10;
        //获取构造器字典的键
        var deco = TextDecoratorDrawer.DECO();
        string[] options = new List<string>(deco.Keys).ToArray();

        //创建一个下拉菜单，菜单内容为options,返回选中的index
        Rect rectForButton = new(position);
        EditorGUI.LabelField(
            new Rect(
                rectForButton.x,
                rectForButton.y,
                EditorGUIUtility.labelWidth,
                EditorGUIUtility.singleLineHeight
            ),
            new GUIContent("Next Decorator")
        );
        rectForButton.x+=EditorGUIUtility.labelWidth;
        this.selectedOpt = EditorGUI.Popup(
            new Rect(rectForButton.x,rectForButton.y,popUpWidth,EditorGUIUtility.singleLineHeight),
            this.selectedOpt, 
            options
        );
        rectForButton.x+=popUpWidth+space;

        //创建一个添加按钮，将刚刚选中的类型实例化，并添加到list中去
        res.Add(GUI.Button(
            new Rect(rectForButton.x,rectForButton.y,buttonWidth,EditorGUIUtility.singleLineHeight),
            "Add"
        ));
        rectForButton.x+=buttonWidth+space;
        // if(buttonClick)
        // {
        //     TextDecorator newDeco = deco[options[selectedOpt]]();
        //     TextDecorator oriDeco = decorator.managedReferenceValue as TextDecorator;
        //     if(oriDeco == null)
        //     {
        //         decorator.managedReferenceValue = newDeco;
        //     }
        //     else 
        //     {
        //         newDeco.SetInner(oriDeco);
        //         newDeco.SetRoot(true);
        //         decorator.managedReferenceValue = newDeco;
        //     }
        // }
        //创建一个按钮，将list最末尾的一个元素去掉
        res.Add(GUI.Button(
            new Rect(rectForButton.x,rectForButton.y,buttonWidth,EditorGUIUtility.singleLineHeight),
            "Pop"
        ));
        // if(buttonClick && decorator.managedReferenceValue != null)
        // {
        //     decorator.managedReferenceValue = (decorator.managedReferenceValue as TextDecorator).inner;
        //     if(decorator.managedReferenceValue != null)
        //         (decorator.managedReferenceValue as TextDecorator).SetRoot(true);
        // }
        return res;
    }
    public void SolveButton(List<bool> buttonClick, SerializedProperty decorator)
    {
        var deco = TextDecoratorDrawer.DECO();
        string[] options = new List<string>(deco.Keys).ToArray();

        //0 for Add button
        if(buttonClick[0])
        {
            TextDecorator newDeco = deco[options[selectedOpt]]();
            TextDecorator oriDeco = decorator.managedReferenceValue as TextDecorator;
            if(oriDeco == null)
            {
                decorator.managedReferenceValue = newDeco;
            }
            else 
            {
                newDeco.SetInner(oriDeco);
                newDeco.SetRoot(true);
                decorator.managedReferenceValue = newDeco;
            }
        }
        //1 for pop button
        else if(buttonClick[1] && decorator.managedReferenceValue != null)
        {
           decorator.managedReferenceValue = (decorator.managedReferenceValue as TextDecorator).inner;
            if(decorator.managedReferenceValue != null)
                (decorator.managedReferenceValue as TextDecorator).SetRoot(true);
        }
    }

    public virtual void DrawSpecificUI(Rect position,SerializedProperty decorator)
    {
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Empty"));
    }
    public virtual void DrawIndexListUI(Rect position,SerializedProperty renderIndexList)
    {
        EditorGUI.PropertyField(new Rect(
                position.x, 
                position.y, 160, 
                EditorGUI.GetPropertyHeight(renderIndexList)
                ), renderIndexList, GUIContent.none);
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
}

[CustomPropertyDrawer(typeof(RainbowText))]
public class RainbowTextDrawer :  TextDecoratorDrawer
{
    public new int selectedOpt = 0;
    public override void DrawSpecificUI(Rect position,SerializedProperty decorator)
    {
        Rect afterFixPosition = EditorGUI.PrefixLabel(
            position, 
            GUIUtility.GetControlID(FocusType.Passive), 
            new GUIContent(this.GetDerivedClassName(decorator))
        );
        var rainbowRect = new Rect(afterFixPosition.x, afterFixPosition.y, 160, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(rainbowRect, decorator.FindPropertyRelative("rainbow"), GUIContent.none);
    }

}

[CustomPropertyDrawer(typeof(WobbleText))]
public class WobbleTextDrawer :  TextDecoratorDrawer
{
    public new int selectedOpt = 1;
    public override void DrawSpecificUI(Rect position,SerializedProperty decorator)
    {
        Rect afterFixPosition = EditorGUI.PrefixLabel(
            position, 
            GUIUtility.GetControlID(FocusType.Passive), 
            new GUIContent(this.GetDerivedClassName(decorator))
        );
        var wobbleRect = new Rect(afterFixPosition.x, afterFixPosition.y, 160, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(wobbleRect, decorator.FindPropertyRelative("wobbleDistance"), GUIContent.none);
    }

}

[CustomPropertyDrawer(typeof(ShakeText))]
public class ShakeTextDrawer :  TextDecoratorDrawer
{
    public new int selectedOpt = 2;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + 2 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
    }
    public override void DrawSpecificUI(Rect position,SerializedProperty decorator)
    {
        Rect afterFixPosition = EditorGUI.PrefixLabel(
            position, 
            GUIUtility.GetControlID(FocusType.Passive), 
            new GUIContent(this.GetDerivedClassName(decorator))
        );
        var shakeRect = new Rect(afterFixPosition.x, afterFixPosition.y, 160, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(shakeRect, decorator.FindPropertyRelative("speed"), new GUIContent("speed"));
        shakeRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(shakeRect, decorator.FindPropertyRelative("wave"), new GUIContent("wave"));
        shakeRect.y +=EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(shakeRect, decorator.FindPropertyRelative("ShakeDistance"), new GUIContent("ShakeDistance"));
        shakeRect.y +=EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    }

}