
using UnityEngine;
using TMPro;
using System;

[Serializable]
public abstract class TextDecorator
{
    [SerializeReference]
    public TextDecorator inner;
    public TextDecorator()
    {
        this.inner = null;
        
    }
    public TextDecorator SetInner(TextDecorator inner)
    {
        this.inner = inner;
        return inner;
    }
    public Mesh ApplyInner(TMP_Text textMesh)
    {
        Mesh m;
        if(inner != null)
            m = inner.Render(textMesh);
        else 
            m = textMesh.mesh;
        return m;
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
        Mesh m = ApplyInner(textMesh);
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

class WobbleText: TextDecorator
{
    public Vector2 wobbleDistance;
    public WobbleText(){}
    public WobbleText(TextDecorator inner)
    {
        this.inner = inner;
    }

    public override Mesh Render(TMP_Text textMesh)
    {
        Mesh m = ApplyInner(textMesh);
        Vector3[] vertices = m.vertices;
        for (int i = 0; i < textMesh.textInfo.characterCount; i++)
        {
            TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];
            int index = c.vertexIndex;
            Vector3 offset = this.Wobble(Time.time);
            vertices[index] += offset; 
        }
        m.vertices = vertices;
        return m;
    }

    private Vector2 Wobble(float time)
    {
        return new Vector2(Mathf.Sin(time*wobbleDistance.x),Mathf.Cos(time*wobbleDistance.y));
    }
}
class ShakeText: TextDecorator
{
    [Range(1,20)]
    public float speed=1;
    public int waveNum=1;
    public Vector2 ShakeDistance;
    public ShakeText(){}
    public ShakeText(TextDecorator inner)
    {
        this.inner = inner;
    }

    public override Mesh Render(TMP_Text textMesh)
    {
        Mesh m = ApplyInner(textMesh);
        Vector3[] vertices = m.vertices;
        float waveDistance = 2*waveNum*Mathf.PI/textMesh.textInfo.characterCount;
        for (int i = 0; i < textMesh.textInfo.characterCount; i++)
        {
            TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];
            if(c.character == ' ')
                continue;
            int index = c.vertexIndex;

            vertices[index] += this.Shake(Time.time,i*waveDistance); 
            vertices[index+1] += this.Shake(Time.time,i*waveDistance); 
            vertices[index+2] += this.Shake(Time.time,i*waveDistance); 
            vertices[index+3] += this.Shake(Time.time,i*waveDistance); 
        }
        m.vertices = vertices;
        return m;
    }

    private Vector3 Shake(float time, float index)
    {
        return new Vector2(this.ShakeDistance.x*Mathf.Sin((time+index)*this.speed), this.ShakeDistance.y*Mathf.Sin((time+index)*this.speed));
    }
}