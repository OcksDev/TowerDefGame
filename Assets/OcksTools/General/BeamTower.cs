using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamTower : Tower
{
    public GameObject HitLineObject;
    public Vector3 SpawnOffset = Vector3.zero;
    private Beam BeamAttack = null;
    
    public override void ModDamProfile(DamageProfile a)
    {
        a.HowDamageWasDealt = DamageProfile.DamageType.Ranged;
        a.WhatWasTheDamage = DamageProfile.DamageType.Magic;
    }
    public override void TargetAquired()
    {
        BeamAttack = Instantiate(HitLineObject, Vector3.zero, Quaternion.identity, Tags.refs["BulletHolder"].transform).GetComponent<Beam>();
        BeamAttack.LineRenderer.SetPosition(0, transform.position + Parts[0].transform.rotation * SpawnOffset);
        BeamAttack.LineRenderer.SetPosition(1, EnemyTarget.Object.transform.position);
        //beam never gets destroyed
    }
    public override void Tick()
    {
        if(BeamAttack != null && EnemyTarget != null)
        {
            BeamAttack.LineRenderer.SetPosition(0, transform.position + Parts[0].transform.rotation * SpawnOffset);
            BeamAttack.LineRenderer.SetPosition(1, EnemyTarget.Object.transform.position);
        }
    }
    public override void Attack()
    {
        var d = GetDamProfile();
        EnemyTarget.Hit(d);
    }
}
