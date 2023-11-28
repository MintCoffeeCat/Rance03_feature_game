
using UnityEngine;
using TMPro;
using System;

[Serializable]
public abstract class TextDecorator
{
    protected TextDecorator inner;
    public TextDecorator()
    {
        this.inner = null;
        
    }
    public TextDecorator SetInner(TextDecorator inner)
    {
        this.inner = inner;
        return inner;
    }
    public abstract Mesh Render(TMP_Text textMesh);
}

class RainbowText: TextDecorator
{
    
    public Gradient rainbow;
    public RainbowText(){}
    public RainbowText(TextDecorator inner)
    {
        this.inner = inner;
    }

    public override Mesh Render(TMP_Text textMesh)
    {
        Mesh m = inner.Render(textMesh);
        Vector3[] vertices = m.vertices;
        Color[] colors = m.colors;
        for (int i = 0; i < textMesh.textInfo.characterCount; i++)
        {
            TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];
            int index = c.vertexIndex;

            colors[index] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index].x*0.001f, 1f));
            colors[index + 1] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 1].x*0.001f, 1f));
            colors[index + 2] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 2].x*0.001f, 1f));
            colors[index + 3] = rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[index + 3].x*0.001f, 1f));
        }
        m.colors = colors;
        return m;
    }
}

class ShakeText: TextDecorator
{
    
    public float yShake;
    public float xShake;
    public ShakeText(){}
    public ShakeText(TextDecorator inner)
    {
        this.inner = inner;
    }

    public override Mesh Render(TMP_Text textMesh)
    {
        Mesh m = inner.Render(textMesh);
        Vector3[] vertices = m.vertices;
        Color[] colors = m.colors;
        for (int i = 0; i < textMesh.textInfo.characterCount; i++)
        {
            TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];
            int index = c.vertexIndex;
            // TODO: 文字摇晃的效果

        }
        m.vertices = vertices;
        return m;
    }
}