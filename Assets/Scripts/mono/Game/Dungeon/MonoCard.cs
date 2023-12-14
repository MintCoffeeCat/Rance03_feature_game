
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
public enum CardType
{
    Normal,
    None,
}
/// <summary>
/// Sprite需要以图片最左边为中心点，方便排列
/// 需要在Unity中将图片的中心点重新编辑才行，默认是图片中心
/// </summary>
public class MonoCard: MonoBehaviour
{
    public static Dictionary<CardType, Func<MonoCard>> createDict = new()
    {
        // TODO: 改成instanciate
        {CardType.Normal,() => new MonoCard()}
    };
    public static Dictionary<CardType, Func<Type>> typeDict = new()
    {
        {CardType.Normal,() => typeof(MonoCard)}
    };

    //贴图本身的宽高像素
    public static int pictureWidth = 240;
    public static int pictureHeight = 400;
    //游戏窗口中，card宽度与窗口宽度的占比
    public static float cardWidthScaleFromScreen= 0.05f;
    //游戏窗口中，card高度与card宽度的占比
    public static float cardHeightScaleFromWidth = (float)MonoCard.pictureHeight/MonoCard.pictureWidth;
    //鼠标悬浮时的缩放比例
    public static float choosenScale = 1.1f;

    [SerializeField]
    [JsonIgnore]
    [DisplayOnly]
    private bool mouseOn;
    private bool clickable;
    private bool initDone=false;


    public CardType type;
    public int length; 

    // public void SetCard(Card cd)
    // {
    //     this.card = cd;
    //     this.UpdateShape();
    // }
    public void UpdateShape()
    {
        Vector3 scale = this.transform.localScale;
        Vector2 concreateCardSize = this.CardSizeScale();
        if(this.mouseOn)
        {
            concreateCardSize *= MonoCard.choosenScale;
        }
        scale.x = concreateCardSize.x;
        scale.y = concreateCardSize.y;
        this.transform.localScale = scale;
        if(!this.initDone)
            this.initDone = true;
    }
    
    /// <summary>
    /// 在transform的scale默认为 1，1，1 时，获得卡片的实际尺寸，以比例表示
    /// 该函数会考虑到迷宫的卡片间距、卡片的宽度位等信息
    /// </summary>
    /// <returns></returns>
    public Vector2 CardSizeScale(float cardL = -1)
    {
        if(cardL == -1)
            cardL = this.length;
        Vector2 gapScreen = DungeonField.GetGapInScreen();
        float sWidth = MonoGameSingleton.GetScreenHelper().ScaledScreenWidth(MonoCard.cardWidthScaleFromScreen);
        float sHeight = sWidth * MonoCard.cardHeightScaleFromWidth;
        float bias = MonoGameSingleton.GetScreenHelper().ScreenToWorldDistance(gapScreen.x * (cardL -1),0).x;
        Vector2 oriSize = this.GetOriginalSizeInWorld();
        sWidth = sWidth * cardL;
        //公式推导得到
        float additionItem = bias*pictureWidth/oriSize.x;
        sWidth += additionItem;
        float pWidthScale = sWidth / pictureWidth;
        float pHeightScale = sHeight / pictureHeight;
        return new Vector2(pWidthScale, pHeightScale);
    }
    public Vector2 GetOriginalSizeInWorld()
    {
        Vector3 bound = this.GetComponent<BoxCollider2D>().bounds.size;
        Vector3 scale = this.transform.localScale;
        return new Vector3(bound.x/scale.x, bound.y/scale.y, bound.z/scale.z);
    }
    // public void UpdatePosition()
    // {
    //     Vector3 pos = this.card.GetPosition();
    //     this.transform.position = pos;
    // }
    void MoveForward(float step)
    {
        // this.card.y += step;
    }
    IEnumerator WaitDungeonInitDone(Action func)
    {
        yield return new WaitUntil(()=>DungeonField.IsInitDone());
        func();
    }
    void Awake()
    {
        this.mouseOn = false;
        this.clickable = true;
    }
    void Start()
    {
        StartCoroutine(WaitDungeonInitDone(this.UpdateShape));
    }
    void Update()
    {
        if(!this.initDone)
            return;
        this.UpdateShape();
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.transform.position,0.02f);
    }
    private void OnMouseDown() {
        if(!this.clickable)
            return;
        DungeonMap map = this.FindMap();
        CardLine line = this.FindLine();
        int cardIdx = line.GetCardIdx(this);
        line.UpdateIdx(cardIdx,true);
        // map.ResetAllLineCenter();
        StartCoroutine(map.ResetAllLineCenterAnimated(20));
    }
    public void SetLine(Transform lineTransform)
    {
        this.transform.parent = lineTransform;
    }
    public void AddPosition(Vector3 vec)
    {
        this.transform.position += vec;
    }
    private void OnMouseEnter() {
        this.mouseOn = true;
    }
    private void OnMouseExit() {
        this.mouseOn = false;
    }
    public DungeonMap FindMap()
    {
        GameObject map = this.transform.parent.parent.gameObject;
        return map.GetComponent<DungeonMap>();
    }
    public CardLine FindLine()
    {
         GameObject line = this.transform.parent.gameObject;
         return line.GetComponent<CardLine>();
    }
}