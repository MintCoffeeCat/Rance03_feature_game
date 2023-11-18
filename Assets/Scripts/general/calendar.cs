using UnityEngine;

/// <summary>
/// Day - 存储一天的时间，是否经过完整一天
/// </summary>
[System.Serializable]
public class Day
{
    [SerializeField]
    private int hour;
    public bool Passed{get;private set;}

    /// <summary>
    /// Day构造器，创建新的一天，指定起始时间
    /// </summary>
    /// <param name="startHour">当天的起始时间</param>
    public Day(int startHour)
    {
        this.hour = startHour;
        this.Passed = false;
    }

    /// <summary>
    /// 让这一天经过指定的小时数量，并设定这一天是否过完
    /// </summary>
    /// <param name="hour">经过的小时数量</param>
    /// <param name="maxHour">一天最多的小时数量</param>
    /// <returns>如果过完了这一天，返回超过一天的多少小时</returns>
    public int PassHour(int hour, int maxHour)
    {
        int exeedHour = 0;
        this.hour += hour;
        if(this.hour >= maxHour)
        {
            exeedHour = this.hour - maxHour;
            this.Passed = true;
        }
        return exeedHour;
    }
    public override string ToString()
    {
        return this.hour.ToString();
    }
}

/// <summary>
/// Calendar - 控制日期和时间的组件
/// </summary>
[System.Serializable]
public class Calendar
{
    [SerializeField]
    [DisplayOnly]
    private int hourPerDay;
    [SerializeField]
    private int passedDays;
    [SerializeField]
    private Day today;

    /// <summary>
    /// Calendar构造器，定义起始日期，每天的小时数
    /// </summary>
    /// <param name="hourPerDay">每天的小时数</param>
    /// <param name="firstDayStartHour">日历第一天起始日期</param>
    public Calendar(int hourPerDay, int firstDayStartHour)
    {
        this.hourPerDay = hourPerDay;
        this.passedDays = 0;
        this.today = new Day(firstDayStartHour);
    }
    /// <summary>
    /// 经过指定小时数，
    /// </summary>
    /// <param name="hour">指定的小时数量</param>
    public void PassHour(int hour)
    {
        int exeedHour = this.today.PassHour(hour,this.hourPerDay);
        if(this.today.Passed)
        {
            this.NewDay(exeedHour);
        }
    }
    public override string ToString()
    {
        return $"Day {this.passedDays + 1}, {today}:00";
    }
    private void NewDay(int startHour)
    {
        this.today = new Day(startHour);
        this.passedDays += 1;
    }
}

