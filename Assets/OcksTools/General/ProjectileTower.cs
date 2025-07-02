using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTower : Tower
{
    public override void Attack()
    {
        EnemyTarget.Kill();
    }
}
