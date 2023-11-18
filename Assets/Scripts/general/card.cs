using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public enum CardType
{
    Normal,
    None,
}

/// <summary>
/// Card抽象类 - 是地图每一个选择卡片的基类
/// </summary>
[Serializable]
public abstract class Card
{
    public static Dictionary<CardType, Func<Card>> createDict = new()
    {
        {CardType.Normal,() => new NormalCard()}
    };
    public static Dictionary<CardType, Func<Type>> typeDict = new()
    {
        {CardType.Normal,() => typeof(NormalCard)}
    };


    [JsonProperty]
    [SerializeReference]
    protected int length;
    
    [SerializeReference]
    public float x;
    [SerializeReference]
    public float y;
    [SerializeReference]
    public float z;

    [JsonIgnore]
    [field:NonSerialized]
    public CardLine BelongsTo {get; private set;}

    public bool MonoInstanciated{get;set;}

    [JsonProperty]
    protected CardType type;
    protected Card(){}
    /// <summary>
    /// Card的基本构造器，指定此卡片的长度占用一行的几格
    /// </summary>
    /// <param name="length">卡片的长度</param>
    public Card(int length)
    {
        this.length = length;
    }
    public Card(Card another)
    {
        this.length = another.length;
    }

    public int GetLength()
    {
        return this.length;
    }
    public void SetLine(CardLine line)
    {
        this.BelongsTo = line;
    }
    public DungeonMap FindMap()
    {
        CardLine line = this.FindLine();
        DungeonMap mp = line.BelongsTo ?? throw new Exception("No Dungeon found with this Card");
        return mp;
    }
    public CardLine FindLine()
    {
        CardLine line = this.BelongsTo ?? throw new Exception("No Line found with this Card.");
        return line;
    }
    public CardType GetCardType()
    {
        return type;
    }
    public Vector3 GetPosition()
    {
        return new Vector3(x,y,z);
    }
    public void SetPosition(Vector3 pos)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
    }
    public void AddPosition(Vector3 pos)
    {
        this.x += pos.x;
        this.y += pos.y;
        this.z += pos.z;
    }
    /// <summary>
    /// 当此卡片出现在视野内时触发的事件
    /// </summary>
    public abstract void OnView();
    /// <summary>
    /// 当此卡片出现在面前时触发的事件
    /// </summary>
    public abstract void OnFace();
    /// <summary>
    /// 当选择此卡片时触发的事件
    /// </summary>
    public abstract void OnStep();

    /// <summary>
    /// 用于给
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
}

/// <summary>
/// NormalCard - 基本卡片，可以转化为其他类型的卡片，也可以当作一般通路，不触发事件
/// </summary>
[Serializable]
public class NormalCard : Card
{
    public NormalCard()
    {
    }

    /// <summary>
    /// NormalCard的构造器不额外创建参数
    /// </summary>
    /// <param name="length">卡片的长度</param>
    /// <returns></returns>
    public NormalCard(int length):base(length)
    {
        this.type = CardType.Normal;
    }
    public NormalCard(NormalCard another):base(another)
    {
        this.type = CardType.Normal;
    }
    /// <summary>
    /// 当此卡片出现在面前时触发的事件
    /// </summary>
    public override void OnFace()
    {
        throw new System.NotImplementedException();
    }
    /// <summary>
    /// 当选择此卡片时触发的事件
    /// </summary>
    public override void OnStep()
    {
        Debug.Log("Choose Card!");
    }
    /// <summary>
    /// 当此卡片出现在视野内时触发的事件
    /// </summary>
    public override void OnView()
    {
        throw new System.NotImplementedException();
    }
}




