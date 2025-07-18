using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class LightningTower : ProjectileTower
{
    public override void Attack()
    {
        var d = GetDamProfile();

        var dd = Instantiate(NerdToSpawn, EnemyTarget.Object.position, Quaternion.identity).GetComponent<Explodie>();
        dd.Size = ExplosionRange * 2;
        dd.transform.localScale = Vector3.one * dd.Size;
        var ding = OXCollision.CircleCastAll(EnemyTarget.Object.position, ExplosionRange);
        foreach (var a in ding)
        {
            a.Hit(d);
        }
    }
    public override void Tick()
    {
    }
    public override void UpdateRender()
    {
        RenderParts[0].sprite = GameHandler.Instance.BaseIMGS[Level];
        RenderParts[1].sprite = TowerIMGS[Level];
       
    }
    public override void ModDamProfile(DamageProfile a)
    {
        a.HowDamageWasDealt = DamageProfile.DamageType.Ranged;
        a.WhatWasTheDamage = DamageProfile.DamageType.Lightning;
    }
}
