using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;
public interface IJson
{
    // 将对象序列化为XML并保存到文件
    public void SaveToJson(string filePath)
    {
        string str = JsonConvert.SerializeObject(this);
        File.WriteAllText(filePath,str);
    }
}