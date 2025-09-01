using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public string Name;
    public Sprite Image;
    public int MaxLevel;
    [HideInInspector]
    public Tower Tower;
    [HideInInspector]
    public int Level;

    public OXLanguageFileIndex LangFileIndex;

    public void Initialize(Tower owner)
    {
        Tower = owner;
        CreateHooks();
    }
    public virtual void CreateHooks()
    {
        //gems override this to have their own functionality
        AddHook(Tower.AttackHook, "example", () => { Debug.Log("Gem saw the attack!"); });
    }
    public void AddHook(OXEvent e,string name, System.Action Method)
    {
        e.Append($"{Name}_{name}", Method);
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
                switch (level)
                {
                    default: return -1;
                    case -1: return 50;
                    case 0:  return 150;
                    case 1:  return 500;
                    case 2:  return 1000;
                }
        }
        return -1;
    }
}

