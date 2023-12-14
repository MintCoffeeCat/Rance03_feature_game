
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class TalkUnit
{
    [SerializeReference]
    public TextDecorator decorator;
    public string content;
    public string speaker;
}

[Serializable]
class Dialogue
{
    [SerializeReference]
    public List<TalkUnit> units;
    public int currentUnit;

    public Dialogue()
    {
        this.units = new();
        this.currentUnit = 0;
    }

    public TalkUnit Next()
    {
        if(currentUnit > units.Count-1)
            return null;
        

        TalkUnit u = this.units[currentUnit];
        currentUnit+=1;
        return u;
    }
}