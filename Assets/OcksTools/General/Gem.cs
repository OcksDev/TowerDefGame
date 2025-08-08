using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public string Name;
    public Sprite Image;
    public int MaxLevel;
    [HideInInspector]
    public Tower Tower;
    [HideInInspector]
    public int Level;

    public void Initialize(Tower owner)
    {
        Tower = owner;
        CreateHooks();
    }
    public virtual void CreateHooks()
    {
        //gems override this to have their own functionality
    }

    public void RealUpgrade()
    {
        Upgrade();
    }
    
    public virtual void Upgrade()
    {
        Level = Mathf.Clamp(Level + 1, 0, MaxLevel);
    }

    public virtual int GetCostToUpgrade(int level)
    {
        switch (Name)
        {
            case "Smeg":
                switch (Level)
                {
                    default: return 50;
                    case 1: return 150;
                    case 2: return 500;
                }
        }
        return -1;
    }
}

