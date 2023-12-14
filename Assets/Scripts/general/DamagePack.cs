using System;

[Serializable]
public class DamagePack
{
    public int damage;
    public int hitChance;
    public int criticalChance;

    public DamagePack(int damage, int hitChance, int criticalChance)
    {
        this.damage = damage;
        this.hitChance = hitChance;
        this.criticalChance = criticalChance;
    }
    public DamagePack()
    {
        this.damage = 0;
        this.hitChance = 100;
        this.criticalChance = 0;
    }

    public static DamagePack operator +(DamagePack one, DamagePack another)
    {
        DamagePack dmg = new()
        {
            damage = one.damage + another.damage,
            hitChance = one.hitChance + another.hitChance,
            criticalChance = one.criticalChance + another.criticalChance
        };
        return dmg;
    }
}