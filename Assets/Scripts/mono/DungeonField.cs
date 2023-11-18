using System;
using System.Collections.Generic;
using UnityEngine;


class DungeonField : MonoBehaviour
{
    [SerializeField]
    private float leftSpaceScale;
    [SerializeField]
    private float rightSpaceScale;
    [SerializeField]
    private float upperSpaceScale;
    [SerializeField]
    private float bottomSpaceScale;

    [SerializeField]
    [Range(0,1)]
    private float layerDistanceX;
    [SerializeField]
    [Range(0,1)]
    private float layerDistanceY;
    [SerializeField]
    private float layerDistanceZ;

    [SerializeField]
    private Boolean showBorder;

    [Header("世界坐标")]
    [SerializeField]
    [DisplayOnly]
    private Vector2 dungeonSize;

    [SerializeField]
    [DisplayOnly]
    private Vector2 gap;

    [Header("Card数据")]
    [SerializeField]
    [DisplayOnly]
    private DungeonMap map;

    private void UpdateDungeonFieldPositionAndSize()
    {
        ScreenHelper SH = MonoGameSingleton.GetScreenHelper();

        float leftSpace = SH.width * this.leftSpaceScale;
        float rightSpace = SH.width * this.rightSpaceScale;
        float upperSpace = SH.height * this.upperSpaceScale;
        float bottomSpace = SH.height * this.bottomSpaceScale;

        float dungeonWidth = SH.width - leftSpace - rightSpace;
        float dungeonHeight = SH.height - upperSpace - bottomSpace;
        this.dungeonSize = new Vector2(dungeonWidth, dungeonHeight);
        this.dungeonSize = SH.ScreenToWorldDistance(this.dungeonSize);

        Vector2 newGap = SH.ScaledScreenSize(this.layerDistanceX, this.layerDistanceY);
        Vector2 center = new(SH.width / 2,SH.height / 2);
        center = SH.ScreenToWorld(center);

        bool changeGap = this.gap != newGap;
        bool changeCenter = this.map.center != center;
        if(changeGap || changeCenter)
        {
            if(changeGap)
                this.gap = newGap;
            if(changeCenter)
                this.map.center = center;
            //TODO: 注意在卡片移动动画的同时发生这个函数会导致问题
            this.map.ReAdjustPosition(center,this.gap,this.layerDistanceZ);
        }
    }
    private void drawDungeonField(Color clr)
    {
        if (!this.showBorder)
        {
            return;
        }
        List<Vector3> corner = this.map.GetFourCorner(this.dungeonSize);
        Debug.DrawLine(corner[0], corner[1],clr);
        Debug.DrawLine(corner[0], corner[2], clr);
        Debug.DrawLine(corner[3], corner[1], clr);
        Debug.DrawLine(corner[3], corner[2], clr);
    }

    private void LoadDungeon(string file)
    {
        ScreenHelper SH = MonoGameSingleton.GetScreenHelper();
        Vector2 gap = MonoGameSingleton.GetScreenHelper().ScaledScreenSize(this.layerDistanceX, this.layerDistanceY);
        this.map = DungeonMap.CreateFromJson(file);
        this.map.InitAfterJson(gap,this.layerDistanceZ);
    }
    void Awake()
    {

    }
    void Start()
    {
        this.UpdateDungeonFieldPositionAndSize();
        this.LoadDungeon("map1.json");
    }
    private void Update()
    {
        this.UpdateDungeonFieldPositionAndSize();
        this.drawDungeonField(Color.red);
    }
}