using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeamTower : Tower
{
    public GameObject HitLineObject;
    public Vector3 SpawnOffset = Vector3.zero;
    private List<Beam> BeamAttack = new List<Beam>();
    public List<Enemy> MultiTarget = new List<Enemy>();

    public override void ModDamProfile(DamageProfile a)
    {
        a.HowDamageWasDealt = DamageProfile.DamageType.Ranged;
        a.WhatWasTheDamage = DamageProfile.DamageType.Magic;
    }
    public void TargetAquired(int Which)
    {
        BeamAttack[Which] = Instantiate(HitLineObject, Vector3.zero, Quaternion.identity, Tags.refs["BulletHolder"].transform).GetComponent<Beam>();
        BeamAttack[Which].LineRenderer.SetPosition(0, transform.position + Parts[0].transform.rotation * SpawnOffset);
        BeamAttack[Which].LineRenderer.SetPosition(1, EnemyTarget.Object.transform.position);
    }
    public void TargetLost(int Which)
    {
        if (BeamAttack[Which] != null) { Destroy(BeamAttack[Which].gameObject); }
    }
    
    public override void Tick()
    {
        if(BeamAttack != null && EnemyTarget != null)
        {
            int a = 0;
            foreach (var item in BeamAttack)
            {
                item.LineRenderer.SetPosition(0, transform.position + Parts[0].transform.rotation * SpawnOffset);
                item.LineRenderer.SetPosition(1, MultiTarget[a].Object.transform.position);
            }
        }
    }
    public override void Attack()
    {
        var d = GetDamProfile();
        EnemyTarget.Hit(d);
    }
    public override void TargettingCode()
    {
        Debug.Log("weeeeeeeee");
        var w = GetTarget(2, TargetType);
        List<Enemy> old_Multi = new List<Enemy>(MultiTarget);
        MultiTarget = new List<Enemy>();
        foreach(var g in w) { MultiTarget.Add(g); }

        List<int> Where = new List<int>();
        for (int g = 0; g > MultiTarget.Count; g++) { if (old_Multi.Contains(MultiTarget[g])) { Where.Add(g); } }
        if (Where.Count > 0) 
        { 
            foreach (int g in Where) 
            {
                TargetLost(g);
                TargetAquired(g);
            }
        }
        Where = new List<int>();
        for (int g = 0; g > MultiTarget.Count; g++) { if (MultiTarget.Contains(old_Multi[g])) { Where.Add(g); } }
        if (Where.Count > 0)
        {
            foreach (int g in Where)
            {
                TargetLost(g);
            }
        }
    }
}
// Set EnemyTarget and EnemyTarget2 
// 