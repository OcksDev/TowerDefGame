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

    public void Initialize(Tower owner)
    {
        Tower = owner;
        CreateHooks();
    }
    public virtual void CreateHooks()
    {
        //gems override this to have their own functionality
    }
}

