using System;
using UnityEngine;

[Serializable]
public class ScreenHelper
{
    private Camera camera;
    [Header("世界坐标")]
    [SerializeField]
    [DisplayOnly]
    private float cameraZ;
    [SerializeField]
    private float frontSurfaceZ;
    [SerializeField]
    public float width{get;private set;}
    public float height{get;private set;}
    public ScreenHelper()
    {

    }
    public ScreenHelper(Camera camera)
    {
        this.camera = camera;
        this.cameraZ = this.camera.transform.position.z;
        this.width = Screen.width;
        this.height = Screen.height;
    }

    public Vector3 ScreenToWorld(Vector2 vec)
    {
        Vector3 vec3 = new(vec.x, vec.y, this.frontSurfaceZ - this.cameraZ);
        Vector3 worldVec = this.camera.ScreenToWorldPoint(vec3);
        return worldVec;
    }
    public Vector3 ScreenToWorld(float x ,float y)
    {
        Vector3 vec3 = new(x, y, this.frontSurfaceZ - this.cameraZ);
        Vector3 worldVec = this.camera.ScreenToWorldPoint(vec3);
        return worldVec;
    }
    public Vector2 ScreenToWorldDistance(Vector2 vec)
    {
        Vector3 ori = this.ScreenToWorld(new Vector2(0,0));
        Vector3 dist = this.ScreenToWorld(vec);
        Vector3 dis = ori - dist;
        dis = new Vector3(Math.Abs(dis.x), Math.Abs(dis.y), dis.z);
        return dis; 
    }
    public Vector2 ScreenToWorldDistance(float w, float h)
    {
        return this.ScreenToWorldDistance(new Vector2(w,h));
    }
    public float ScaledScreenWidth(float scale)
    {
        return this.width * scale;
    }
    public float ScaledScreenHeight(float scale)
    {
        return this.height * scale;
    }
    public Vector2 ScaledScreenSize(float scale)
    {
        Vector2 scr = new(this.width, this.height);
        return scr * scale;
    }
    public Vector2 ScaledScreenSize(float scaleW, float scaleH)
    {
        float sW = this.ScaledScreenWidth(scaleW);
        float sH = this.ScaledScreenWidth(scaleH);
        return new Vector2(sW,sH);
    }
    public float GetSurfaceZ()
    {
        return this.frontSurfaceZ;
    }
    public void Update()
    {
        if(Screen.width != this.width)
        {
            this.width = Screen.width;
            this.height = Screen.height;
        }
        this.cameraZ = this.camera.transform.position.z;
    }
}