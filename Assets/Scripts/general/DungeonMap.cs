using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEngine;

[Serializable]
[JsonObject]
public class DungeonMap : IJson, IEnumerable<CardLine>
{
    [JsonProperty]
    [SerializeField]
    private List<CardLine> dungeon;

    [DisplayOnly]
    public Vector2 center;

    public DungeonMap(){}
    public DungeonMap(List<CardLine> map)
    {
        this.dungeon = map;
    }

    public static DungeonMap CreateFromJson(string file)
    {
        if(!File.Exists(file))
        {
            throw new Exception($"The map Json file {file} is not exist");            
        }
        string jsonData = File.ReadAllText(file);
        DungeonMap map = JsonConvert.DeserializeObject<DungeonMap>(jsonData,new DungeonMapConverter());
        return map;
    }
    public void InitAfterJson(Vector2 gap, float layerDistanceZ)
    {
        int i=0;
        foreach(CardLine line in dungeon)
        {
            line.SetDungeon(this);
            line.UpdateIdx();
            line.InstanciateCard();
            line.ArrangeCardsInLine(this.center,gap,i,layerDistanceZ);
            line.MoveCenterCardToDungeonCenter();
            i++;
        }
    }
    public void ReAdjustPosition(Vector2 center, Vector2 gap, float layerDistanceZ)
    {
        int i=0;
        if(center != this.center)
            this.center = center;
        foreach(CardLine line in dungeon)
        {
            line.ArrangeCardsInLine(center,gap,i,layerDistanceZ);
            line.MoveCenterCardToDungeonCenter();
            i++;
        }
    }
    public void ResetAllLineCenter()
    {

        foreach(CardLine line in dungeon)
        {
            line.MoveCenterCardToDungeonCenter();
        }
    }
    public IEnumerator ResetAllLineCenterAnimated(int frame)
    {

        // TODO: 禁用所有卡片点击
        for(int i=1; i<=frame;i++)
        {
            foreach(CardLine line in dungeon)
            {
                Vector2 cardCenterPos = line.GetCenterCardPosition();
                Vector3 dungeonCenterPos = this.center;
                float distanceToMove = (cardCenterPos.x - dungeonCenterPos.x)/(frame-i+1);
                line.MoveCardWithDistance(distanceToMove);
            }
            yield return 0;
        }
        this.ResetAllLineCenter();

        //TODO: 启用所有卡片点击
    }

    public void BroadCastCenterCardIdx(int idx)
    {
        foreach(CardLine line in this.dungeon)
        {
            line.UpdateIdx(idx);
        }
    }
    /// <summary>
    /// 给定一个地牢尺寸，根据当前设定的地牢中心点，告知地牢的四个角落坐标
    /// </summary>
    /// <param name="dungeonSize">地牢的中心点</param>
    /// <returns>四个角落的三维坐标点，顺序为 [左上，左下，右上，右下] </returns>
    public List<Vector3> GetFourCorner(Vector2 dungeonSize)
    {
        ScreenHelper SH = MonoGameSingleton.GetScreenHelper();
        Vector2 halfSize = dungeonSize/2;
        List<Vector3> res = new()
        {
            new Vector3(this.center.x - halfSize.x, this.center.y - halfSize.y, SH.GetSurfaceZ()),
            new Vector3(this.center.x - halfSize.x, this.center.y + halfSize.y, SH.GetSurfaceZ()),
            new Vector3(this.center.x + halfSize.x, this.center.y - halfSize.y, SH.GetSurfaceZ()),
            new Vector3(this.center.x + halfSize.x, this.center.y + halfSize.y, SH.GetSurfaceZ())
        };
        return res;
    }
    public IEnumerator<CardLine> GetEnumerator()
    {
        return this.dungeon.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class DungeonMapConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {

        return this.IsCard(objectType) || this.IsLine(objectType) || this.IsDungeon(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jo = JObject.Load(reader);
        object res = null;
        if(this.IsDungeon(objectType))
        {
            res = new DungeonMap();
        }else if(this.IsLine(objectType))
        {
            res = new CardLine();
        }else if(this.IsCard(objectType))
        {
            int type = (int)jo["type"];
            CardType readType = CardType.None;
            if(Enum.IsDefined(typeof(CardType), type))
            {
                readType = (CardType)type;
            }
            if(Card.createDict.ContainsKey(readType))
            {
                res = Card.createDict[readType]();
            } 
        }else{
            throw new Exception($"Can not Convert {objectType}");
        }
        serializer.Populate(jo.CreateReader(), res);
        return res;
    }
    protected bool IsDungeon(Type objectType)
    {
        return typeof(DungeonMap).IsAssignableFrom(objectType);
    }
    protected bool IsLine(Type objectType)
    {
        return typeof(CardLine).IsAssignableFrom(objectType);
    }
    protected bool IsCard(Type objectType)
    {
        return typeof(Card).IsAssignableFrom(objectType);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}