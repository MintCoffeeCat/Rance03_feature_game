
using UnityEngine;
public class MonoCard: MonoBehaviour
{
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
    private Card card;

    [SerializeField]
    [DisplayOnly]
    private bool mouseOn;

    private bool clickable;

    public void SetCard(Card cd)
    {
        this.card = cd;
    }
    void updateShape()
    {
        float sWidth = MonoGameSingleton.GetScreenHelper().ScaledScreenWidth(MonoCard.cardWidthScaleFromScreen);
        float sHeight = sWidth * MonoCard.cardHeightScaleFromWidth;

        float pWidthScale = sWidth / pictureWidth;
        float pHeightScale = sHeight / pictureHeight;


        // Vector2 worldSize = MonoGameSingleton.GetAxisHelper().ScreenToWorldDistance(sWidth,sHeight);
        if(this.mouseOn)
        {
            pWidthScale *= MonoCard.choosenScale;
            pHeightScale *= MonoCard.choosenScale;
        }
        Vector3 scale = this.transform.localScale;
        scale.x = pWidthScale;
        scale.y = pHeightScale;
        this.transform.localScale = scale;
    }
    void updatePosition()
    {
        Vector3 pos = this.card.GetPosition();
        this.transform.position = pos;
    }
    void MoveForward(float step)
    {
        this.card.y += step;
    }
    void Awake()
    {
        this.mouseOn = false;
        this.clickable = true;
        this.updateShape();
    }
    void Start()
    {

    }
    void Update()
    {
        this.updateShape();
        this.updatePosition();
    }

    private void OnMouseDown() {
        if(!this.clickable)
            return;
        DungeonMap map = this.card.FindMap();
        CardLine line = this.card.BelongsTo;
        int cardIdx = line.GetCardIdx(this.card);
        line.UpdateIdx(cardIdx,true);
        StartCoroutine(map.ResetAllLineCenterAnimated(20));
    }
    private void OnMouseEnter() {
        this.mouseOn = true;
    }
    private void OnMouseExit() {
        this.mouseOn = false;
    }
}