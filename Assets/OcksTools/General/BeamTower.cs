using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamTower : Tower
{
    public GameObject HitLineObject;
    public Vector3 SpawnOffset = Vector3.zero;
    private Hitscan BeamAttack = null;
    int CountTilDamage = 0;
    int DamageCount = 100;
    public override void ModDamProfile(DamageProfile a)
    {
        a.HowDamageWasDealt = DamageProfile.DamageType.Ranged;
        a.WhatWasTheDamage = DamageProfile.DamageType.Magic;
    }
    public override void TargetAquired()
    {
        BeamAttack = Instantiate(HitLineObject, Vector3.zero, Quaternion.identity, Tags.refs["BulletHolder"].transform).GetComponent<Hitscan>();
        BeamAttack.LineRenderer.SetPosition(0, transform.position + Parts[0].transform.rotation * SpawnOffset);
        BeamAttack.LineRenderer.SetPosition(1, EnemyTarget.Object.transform.position);
    }
    public override void Attack()
    {
        BeamAttack.LineRenderer.SetPosition(0, transform.position + Parts[0].transform.rotation * SpawnOffset);
        BeamAttack.LineRenderer.SetPosition(1, EnemyTarget.Object.transform.position);
        if (CountTilDamage==DamageCount) {
            var d = GetDamProfile();
            AttackAnim = StartCoroutine(BackPushAnim());
            EnemyTarget.Hit(d);
        }
        CountTilDamage++; if (CountTilDamage == DamageCount+1) { CountTilDamage = 0; }
    }
    public override void AttackTick()
    {
        RealAttack();
    }
}
