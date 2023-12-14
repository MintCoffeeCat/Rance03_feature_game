using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
[JsonObject]
public class DungeonMap : MonoBehaviour, IJson, IEnumerable<CardLine>
{
    [JsonProperty]
    [SerializeField]
    private List<CardLine> dungeon;

    [DisplayOnly]
    public Vector2 center;

    public void AppendLine(CardLine line)
    {
        this.dungeon.Add(line);
        line.SetDungeon(this.transform);
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
    /// <summary>
    /// 用于在读取json迷宫后，实例化卡片以及绑定依赖关系
    /// </summary>
    public void InitAfterJson()
    {
        int i=0;
        foreach(CardLine line in dungeon)
        {
            line.SetDungeon(this.transform);
            line.SetCardParent();
            line.UpdateIdx();
            i++;
        }
    }

    public void AdjustPositionByCenter(float layerDistanceZ)
    {
        this.AdjustPositionByNewCenter(this.center,layerDistanceZ);
    }
    public void AdjustPositionByNewCenter(Vector2 center, float layerDistanceZ)
    {
        int i=0;
        float distance = 0;
        if(center != this.center)
            this.center = center;
        foreach(CardLine line in dungeon)
        {

            line.ArrangeCardsInLine(center,i,layerDistanceZ);
            if(i==0)
                distance = line.GetMoveDistanceFromCenter();
            line.MoveCardWithDistance(distance);
            i++;
        }
    }

    public IEnumerator ResetAllLineCenterAnimated(int frame)
    {

        float distanceToMove = 0;
        // TODO: 禁用所有卡片点击
        for(int i=1; i<=frame;i++)
        {
            int lineNo = 0;
            foreach(CardLine line in dungeon)
            {
                if(lineNo == 0)
                {
                    distanceToMove = line.GetMoveDistanceFromCenter();
                    distanceToMove /= frame-i+1;
                }
                line.MoveCardWithDistance(distanceToMove);
                lineNo++;
            }
            yield return 0;
        }
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
            GameObject obj = Resources.Load<GameObject>("prefabs/Dungeon/map");
            GameObject instance = UnityEngine.Object.Instantiate(obj, new Vector3(0,0,0), Quaternion.identity);
            res = instance.GetComponent<DungeonMap>();

        }
        else if(this.IsLine(objectType))
        {
            GameObject obj = Resources.Load<GameObject>("prefabs/Dungeon/line");
            GameObject instance = UnityEngine.Object.Instantiate(obj, new Vector3(0,0,0), Quaternion.identity);
            res = instance.GetComponent<CardLine>();
        }
        else if(this.IsCard(objectType))
        {
            GameObject obj = Resources.Load<GameObject>("prefabs/Dungeon/card");
            GameObject instance = UnityEngine.Object.Instantiate(obj, new Vector3(0,0,0), Quaternion.identity);
            res = instance.GetComponent<MonoCard>();
            // int type = (int)jo["type"];
            // CardType readType = CardType.None;
            // if(Enum.IsDefined(typeof(CardType), type))
            // {
            //     readType = (CardType)type;
            // }
            // if(MonoCard.createDict.ContainsKey(readType))
            // {
            //     res = MonoCard.createDict[readType]();
            // } 
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
        return typeof(MonoCard).IsAssignableFrom(objectType);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}