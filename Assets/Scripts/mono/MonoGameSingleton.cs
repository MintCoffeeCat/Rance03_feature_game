using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoGameSingleton : Singleton<MonoGameSingleton>
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    public Calendar calendar;

    [SerializeField]
    public ScreenHelper screenHelper;
    

    protected new void Awake()
    {
        base.Awake();
        this.calendar = new Calendar(30,7);
        this.screenHelper = new ScreenHelper(this.mainCamera);
    }
    public static ScreenHelper GetScreenHelper()
    {
        return ((MonoGameSingleton)MonoGameSingleton.instance).screenHelper;
    }
    public static Calendar GetCalendar()
    {
        return ((MonoGameSingleton)MonoGameSingleton.instance).calendar;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.screenHelper.Update();
        this.screenHelper.DrawScreenArea(Color.green);
    }
}
