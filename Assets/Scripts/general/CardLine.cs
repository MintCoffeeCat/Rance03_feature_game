using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
[JsonObject]
public class CardLine : IEnumerable<Card>
{
    [JsonProperty]
    [SerializeField]
    private List<Card> line;
    [JsonProperty]
    [SerializeField]
    private int capability;

    private int centerIdx;

    [JsonIgnore]
    [field:NonSerialized]
    public DungeonMap BelongsTo{get;private set;}
    public CardLine(){}

    public CardLine(List<Card> cards, int capability)
    {
        this.line = new List<Card>();
        this.capability = capability;
        int cost = 0;
        foreach(Card card in cards)
        {
            if(Card.typeDict.ContainsKey(card.GetCardType()))
            {
                this.line.Add(card);
                card.SetLine(this);
            }
            cost += card.GetLength();
        }
        if( cost != this.capability)
        {
            throw new Exception("每一行的卡片的总占用长度应与行内定义的capability一致");
        }
    }
    /// <summary>
    /// 设置当前行所属的迷宫
    /// </summary>
    /// <param name="map">所属迷宫</param>
    public void SetDungeon(DungeonMap map)
    {
        this.BelongsTo = map;
    }
    /// <summary>
    /// 获取某个卡牌c在此行中的索引
    /// </summary>
    /// <param name="c">想要查找索引的卡牌</param>
    /// <returns>卡牌的索引，若卡牌并未在此行中，返回-1</returns>
    public int GetCardIdx(Card c)
    {
        int i=0;
        foreach(Card cd in this.line)
        {
            if(cd == c)
            {
                return i;
            }
            i++;
        }
        return -1;
    }
    /// <summary>
    /// 更新当前行的中心卡牌索引，可选择是否扩散
    /// </summary>
    /// <param name="idx">卡牌索引</param>
    /// <param name="broadCast">是否扩散，若为True，同迷宫下其他行的中心索引也会设置为相同的索引号</param> 
    public void UpdateIdx(int idx = -1, bool broadCast=false)
    {
        if(broadCast)
        {
            this.BelongsTo.BroadCastCenterCardIdx(idx);
        }
        else
        {
            this.UpdateIdx(idx);
        }
    }
    /// <summary>
    /// 更新当前行的中心卡牌索引，无扩散功能
    /// </summary>
    /// <param name="idx"></param>
    private void UpdateIdx(int idx = -1)
    {
        if(idx == -1)
            this.centerIdx = this.line.Count / 2;
        else
        {
            this.centerIdx = idx;
        }
    }
    
    public Vector3 GetCenterCardPosition()
    {
        return this.line[this.centerIdx].GetPosition();
    }
    
    /// <summary>
    /// 将每行的卡片都实例化为游戏对象，并设置好与迷宫和行的层级关系
    /// </summary>
    public void InstanciateCard()
    {
        foreach(Card c in line)
        {
            if(!c.MonoInstanciated)
            {
                GameObject obj = Resources.Load<GameObject>("prefabs/card");
                GameObject instance = UnityEngine.Object.Instantiate(obj, new Vector3(0,0,0), Quaternion.identity);
                ((MonoCard)instance.GetComponent<MonoCard>()).SetCard(c);
                c.SetLine(this);
                c.MonoInstanciated = true;
            }
        }
    }
    /// <summary>
    /// 将实例化的卡片按照卡片间距排列成一行
    /// </summary>
    /// <param name="startPos">卡片的初始位置</param>
    /// <param name="gap">卡片间的X、Y间距</param>
    /// <param name="lineNo">行号，决定了Z轴位置</param>
    /// <param name="layerDistanceZ">卡片间的Z轴间距</param>
    public void ArrangeCardsInLine(Vector2 startPos, Vector2 gap, int lineNo, float layerDistanceZ)
    {
        ScreenHelper SH = MonoGameSingleton.GetScreenHelper();
        int j=0;
        foreach(Card c in line)
        {
            Vector2 dis = SH.ScreenToWorldDistance(gap);
            Vector3 v = new(startPos.x,startPos.y,SH.GetSurfaceZ());
            v += new Vector3(j*dis.x,lineNo*dis.y,lineNo*layerDistanceZ);
            c.SetPosition(v);
            j++;
        }
    }
    /// <summary>
    /// 将当前的中心卡片移动到迷宫的中心位置, 其余卡片也跟随一起移动相同距离
    /// </summary>
    public void MoveCenterCardToDungeonCenter()
    {
        Vector3 centerCardPos = this.GetCenterCardPosition();
        float distanceToMove = centerCardPos.x - this.BelongsTo.center.x;
        this.MoveCardWithDistance(distanceToMove);
    }
    /// <summary>
    /// 将当前行的所有卡片移动特定距离
    /// </summary>
    /// <param name="distanceToMove">要移动的距离</param>
    public void MoveCardWithDistance(float distanceToMove)
    {
        foreach(Card card in this.line)
        {
            card.AddPosition(new Vector3(-1*distanceToMove,0,0));
        }
    }


    public IEnumerator<Card> GetEnumerator()
    {
        
        return line.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
