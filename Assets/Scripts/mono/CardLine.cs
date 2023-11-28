using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
[JsonObject]
public class CardLine : MonoBehaviour, IEnumerable<MonoCard>
{
    [JsonProperty]
    [SerializeField]
    private List<MonoCard> line;
    [JsonProperty]
    [SerializeField]
    public int capability;
    [DisplayOnly]
    public int nowContain;
    public int centerIdx;

    // [JsonIgnore]
    // [field:NonSerialized]

    public void AppendCard(MonoCard card)
    {
        if(this.nowContain + card.length > this.capability)
        {
            throw new Exception("每一行的卡片的总占用长度应与行内定义的capability一致");
        }
        this.line.Add(card);
        card.SetLine(this.transform);
        this.nowContain += card.length;
    }
    /// <summary>
    /// 设置当前行所属的迷宫
    /// </summary>
    /// <param name="map">所属迷宫</param>
    public void SetDungeon(Transform map)
    {
        this.transform.parent = map;
    }
     public DungeonMap FindMap()
    {
        GameObject map = this.transform.parent.gameObject;
        return map.GetComponent<DungeonMap>();
    }
    /// <summary>
    /// 获取某个卡牌c在此行中的索引
    /// </summary>
    /// <param name="c">想要查找索引的卡牌</param>
    /// <returns>卡牌的索引，若卡牌并未在此行中，返回-1</returns>
    public int GetCardIdx(MonoCard c)
    {
        int i=0;
        foreach(MonoCard cd in this.line)
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
            this.FindMap().BroadCastCenterCardIdx(idx);
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
    /// <summary>
    /// 获取到中心卡片的世界坐标
    /// </summary>
    /// <returns>中心卡片的坐标</returns>
    public Vector3 GetCenterCardPosition()
    {
        return this.line[this.centerIdx].transform.position;
    }
    public MonoCard GetCenterCard()
    {
        return this.line[this.centerIdx];
    }
    /// <summary>
    /// 查找中心卡牌之前有多少特宽位
    /// </summary>
    /// <returns>返回额外宽度，包含中心卡牌自身的额外宽度数量</returns>
    public int FindWideCountBeforeCenter()
    {
        int count = 0;
        for(int i=0;i<=this.centerIdx;i++)
        {
            count += this.line[i].length -1;
        }
        return count;
    }
    /// <summary>
    /// 将每行的卡片设置好与迷宫和行的层级关系
    /// </summary>
    public void SetCardParent()
    {
        foreach(MonoCard c in line)
        {
            c.SetLine(this.transform);
        }
    }
    /// <summary>
    /// 将实例化的卡片按照卡片间距排列成一行
    /// </summary>
    /// <param name="startPos">卡片的初始位置</param>
    /// <param name="gap">卡片间的X、Y间距</param>
    /// <param name="lineNo">行号，决定了Z轴位置</param>
    /// <param name="layerDistanceZ">卡片间的Z轴间距</param>
    public void ArrangeCardsInLine(Vector2 startPos, int lineNo, float layerDistanceZ)
    {
        ScreenHelper SH = MonoGameSingleton.GetScreenHelper();
        int j=0;
        Vector3 cardSizeBiasLeft = new(0,0,0);
        foreach(MonoCard c in line)
        {
            Vector3 concreteCardSize = c.CardSizeScale()*c.GetOriginalSizeInWorld();
            concreteCardSize.y = 0;
            concreteCardSize.z = 0;
            Vector2 dis = DungeonField.GetGapInWorld();
            Vector3 worldStartPos = new(startPos.x, startPos.y, SH.GetSurfaceZ());
            Vector3 gapBias = new(j*dis.x,lineNo*dis.y,lineNo*layerDistanceZ);
            if(j==0)
            {
                worldStartPos += cardSizeBiasLeft + gapBias;
                cardSizeBiasLeft += concreteCardSize;
            }
            else
            {
                worldStartPos += cardSizeBiasLeft + gapBias;
                cardSizeBiasLeft += concreteCardSize;
            }
            c.transform.position = worldStartPos;
            j++;
        }
    }
    /// <summary>
    /// 将当前的中心卡片移动到迷宫的中心位置, 其余卡片也跟随一起移动相同距离
    /// </summary>
    public void MoveCenterCardToDungeonCenter()
    {
        //TODO: 计算距离再仔细考虑一下widerCount的情况
        float distanceToMove = this.GetMoveDistanceFromCenter();
        this.MoveCardWithDistance(distanceToMove);
    }
    public float GetMoveDistanceFromCenter()
    {
        Vector3 centerCardPos = this.GetCenterCardPosition();
        MonoCard c = this.line[this.centerIdx];
        Vector2 cardSize = c.CardSizeScale()*c.GetOriginalSizeInWorld();
        float distanceToMove = centerCardPos.x + cardSize.x/2 - this.FindMap().center.x;
        return distanceToMove;
    }
    /// <summary>
    /// 将当前行的所有卡片移动特定距离
    /// </summary>
    /// <param name="distanceToMove">要移动的距离</param>
    public void MoveCardWithDistance(float distanceToMove)
    {
        foreach(MonoCard card in this.line)
        {
            card.AddPosition(new Vector3(-1*distanceToMove,0,0));
        }
    }


    public IEnumerator<MonoCard> GetEnumerator()
    {
        
        return line.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
