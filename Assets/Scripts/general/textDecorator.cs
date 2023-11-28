using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public abstract class TextDecorator
{
    protected TextDecorator inner;
    public TextDecorator()
    {
        this.inner = null;
        
    }
    public abstract void Render(TMP_Text mesh, Vector3[] vertices);
}

class RainbowText: TextDecorator
{
    public RainbowText(){}
    public RainbowText(TextDecorator inner)
    {
        this.inner = inner;
    }

    public override void Render(TMP_Text mesh, Vector3[] vertices)
    {
        inner.Render(mesh, vertices);
    }
}