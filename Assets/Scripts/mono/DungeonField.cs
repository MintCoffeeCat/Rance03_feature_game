using System;
using System.Collections.Generic;
using UnityEngine;


class DungeonField : Singleton<DungeonField>
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
    public Vector2 gap;

    [Header("Card数据")]
    [SerializeField]
    [DisplayOnly]
    private DungeonMap map;

    private bool initDone = false;

    private void initField()
    {
        ScreenHelper SH = MonoGameSingleton.GetScreenHelper();

        float leftSpace = SH.width * this.leftSpaceScale;
        float rightSpace = SH.width * this.rightSpaceScale;
        float upperSpace = SH.height * this.upperSpaceScale;
        float bottomSpace = SH.height * this.bottomSpaceScale;

        float dungeonWidth = SH.width - leftSpace - rightSpace;
        float dungeonHeight = SH.height - upperSpace - bottomSpace;

        Vector2 newDungeonSize = new(dungeonWidth, dungeonHeight);
        this.dungeonSize = SH.ScreenToWorldDistance(newDungeonSize);
        this.gap = SH.ScaledScreenSize(this.layerDistanceX, this.layerDistanceY);
        
        // Vector2 center = new(SH.width / 2,SH.height / 2);
        // this.map.center = SH.ScreenToWorld(center);

        this.initDone = true;
    }
    private void UpdateDungeonFieldPositionAndSize()
    {
        ScreenHelper SH = MonoGameSingleton.GetScreenHelper();

        float leftSpace = SH.width * this.leftSpaceScale;
        float rightSpace = SH.width * this.rightSpaceScale;
        float upperSpace = SH.height * this.upperSpaceScale;
        float bottomSpace = SH.height * this.bottomSpaceScale;

        float dungeonWidth = SH.width - leftSpace - rightSpace;
        float dungeonHeight = SH.height - upperSpace - bottomSpace;
        Vector2 newDungeonSize = new(dungeonWidth, dungeonHeight);
        newDungeonSize = SH.ScreenToWorldDistance(newDungeonSize);

        Vector2 newGap = SH.ScaledScreenSize(this.layerDistanceX, this.layerDistanceY);
        Vector2 center = new(SH.width / 2,SH.height / 2);
        center = SH.ScreenToWorld(center);

        bool changeDungeonSize = this.dungeonSize != newDungeonSize;
        bool changeGap = this.gap != newGap;
        bool changeCenter = this.map.center != center;
        if(changeGap || changeCenter || changeDungeonSize)
        {
            if(changeDungeonSize)
                this.dungeonSize = newDungeonSize;
            if(changeGap)
                this.gap = newGap;
            //TODO: 注意在卡片移动动画的同时发生这个函数会导致问题
            this.map.AdjustPositionByNewCenter(center,this.layerDistanceZ);
        }
    }
    private void OnDrawGizmos() {
        if(this.map == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.map.center,0.05f);
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

    public static Vector2 GetGapInScreen()
    {
        return ((DungeonField)DungeonField.instance).gap;
    }
    public static Vector2 GetGapInWorld()
    {
        ScreenHelper SH = MonoGameSingleton.GetScreenHelper();
        Vector2 gap = ((DungeonField)DungeonField.instance).gap;
        return SH.ScreenToWorldDistance(gap);
    }
    public static bool IsInitDone()
    {
        return ((DungeonField)DungeonField.instance).initDone;
    }
    public new void Awake()
    {
        base.Awake();
        //这里要保证此脚本的Awake晚于MonoGameSingleton执行
        //通过在编辑器中选中任意脚本，点击右上角Execution Order进行优先级设置
        this.initField();
        this.map = DungeonMap.CreateFromJson("map1.json");
        this.map.InitAfterJson();
    }
    void Start()
    {
        // this.map.AdjustPositionByNewCenter(center,this.layerDistanceZ);
        this.map.AdjustPositionByCenter(this.layerDistanceZ);
    }
    private void Update()
    {
        if(!this.initDone)
            return;
        this.UpdateDungeonFieldPositionAndSize();
        this.drawDungeonField(Color.red);
    }
}