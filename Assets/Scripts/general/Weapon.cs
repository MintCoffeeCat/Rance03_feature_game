
using System;

[Serializable]
public abstract class Weapon
{
    public string name;
    public int atk;

    public Weapon(string name="Undefined")
    {
        this.name = name;
        this.atk = 0;
    }
    public abstract void RangeEffect();
    public abstract void Attack();
}

public abstract class MeleeWeapon : Weapon
{
    public MeleeWeapon(string name="Undefined Melee") : base(name)
    {

    }
}

public abstract class RangedWeapon : Weapon
{
    public RangedWeapon(string name="Undefined Ranged"): base(name)
    {

    }
}