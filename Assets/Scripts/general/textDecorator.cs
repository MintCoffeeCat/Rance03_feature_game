
using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

[Serializable]
public class RenderIndexSet
{
    public int startIndex;
    public int endIndex;

    public RenderIndexSet()
    {
        this.startIndex = this.endIndex = 0;
    }
    public RenderIndexSet(int start, int end)
    {
        this.startIndex = start;
        this.endIndex = end;
    }
    public RenderIndexSet(Vector2 vec)
    {
        this.startIndex = (int)vec.x;
        this.endIndex = (int)vec.y;
    }

    public bool Contain(int value)
    {
        return this.startIndex <= value && value <= this.endIndex;
    }
    /// <summary>
    /// 给定的indexSet的左端点是否包含在当前区间内
    /// </summary>
    /// <param name="indexSet"></param>
    /// <returns></returns> <summary>
    public bool ContainLeft(RenderIndexSet indexSet)
    {
        return this.startIndex <= indexSet.startIndex;
    }
    /// <summary>
    /// 给定的indexSet的右端点是否包含在当前区间内
    /// </summary>
    /// <param name="indexSet"></param>
    /// <returns></returns> <summary>
    public bool ContainRight(RenderIndexSet indexSet)
    {
        return indexSet.endIndex <= this.endIndex;
    }
    public bool ContainFull(RenderIndexSet indexSet)
    {
        return this.ContainLeft(indexSet) && this.ContainRight(indexSet);
    }
}
public class RenderIndexList 
{
    public List<RenderIndexSet> originalRenderIndexSets;
    public List<RenderIndexSet> mergedRenderIndexSets;

    public RenderIndexList()
    {
        this.originalRenderIndexSets = new();
        this.mergedRenderIndexSets = new();
    }
    public RenderIndexList(List<RenderIndexSet> list)
    {
        this.originalRenderIndexSets = list;
        this.mergedRenderIndexSets = new();
        this.Merge();
    }
    public RenderIndexList(RenderIndexSet indexSet)
    {
        this.originalRenderIndexSets = new()
        {
            indexSet
        };
        this.mergedRenderIndexSets = new();
        this.Merge();
    }
    public void Add(RenderIndexSet indexSet)
    {
        bool added = false;
        foreach(RenderIndexSet _mergedSet in this.mergedRenderIndexSets)
        {
            //端点全在已有的index集合中，不做任何事情
            if(_mergedSet.ContainFull(indexSet))
                return;
            //只要有一个端点在集合中，进行处理
            if(_mergedSet.ContainLeft(indexSet) || _mergedSet.ContainRight(indexSet))
            {
                this.originalRenderIndexSets.Add(indexSet);
                added = true;
                break;
            }

        }
        //左右两个端点都不在集合中的情况
        if(!added)
            this.originalRenderIndexSets.Add(indexSet);
        this.Merge();
    }
    public void Merge()
    {
        if(this.originalRenderIndexSets.Count == 0)
            return;

        this.mergedRenderIndexSets.Clear();
        this.originalRenderIndexSets.Sort((x,y)=>x.startIndex.CompareTo(y.startIndex));
        int i=0;
        RenderIndexSet mergedSet = new();
        foreach(RenderIndexSet idxSet in this.originalRenderIndexSets)
        {
            if(i==0)
            {
                mergedSet = idxSet;
                i++;
                continue;
            }
            //当前原始区间的左端点包含在合并区间内，就把当前原始区间和合并区间合并
            if (mergedSet.ContainLeft(idxSet))
            {
                mergedSet.endIndex = idxSet.endIndex;
            }
            //否则说明当前原始区间与合并区间不相连，开始新的合并区间
            else
            {
                this.mergedRenderIndexSets.Add(mergedSet);
                mergedSet = idxSet;
            }
        }
        this.mergedRenderIndexSets.Add(mergedSet);
    }
    public bool Contain(int index)
    {
        foreach(RenderIndexSet idxSet in this.mergedRenderIndexSets)
        {
            if(idxSet.Contain(index))
                return true;
        }
        return false;
    }
}


[Serializable]
public abstract class TextDecorator
{
    [SerializeReference]
    public TextDecorator inner;
    public RenderIndexList renderIndexList;
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
            //仅渲染在列表里的
            if(!this.renderIndexList.Contain(i))
                continue;

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