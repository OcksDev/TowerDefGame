using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanTower : Tower
{
    public GameObject HitLineObject;
    public Vector3 SpawnOffset = Vector3.zero;
    public override void ModDamProfile(DamageProfile a)
    {
        a.HowDamageWasDealt = DamageProfile.DamageType.Ranged;
        a.WhatWasTheDamage = DamageProfile.DamageType.Bullet;
    }
    public override void Attack()
    {
        var d = GetDamProfile();
        var meme = Instantiate(HitLineObject, Vector3.zero, Quaternion.identity, Tags.refs["BulletHolder"].transform).GetComponent<Hitscan>();
        AttackAnim = StartCoroutine(BackPushAnim());
        meme.LineRenderer.SetPosition(0, transform.position + Parts[0].transform.rotation * SpawnOffset);
        meme.LineRenderer.SetPosition(1, EnemyTarget.Object.transform.position);
        EnemyTarget.Hit(d);
    }
}
