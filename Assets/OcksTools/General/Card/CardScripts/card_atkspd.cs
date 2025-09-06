using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class card_atkspd : Card
{
    public override void CreateHooks()
    {
        AddHook(Tower.StatHook, "atkspd", () => { Tower.AttackRate *= 2; });
        Tower.SetStats();
    }
}
