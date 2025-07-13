using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeamTower : Tower
{
    public GameObject HitLineObject;
    public Vector3 SpawnOffset = Vector3.zero;
    private List<Beam> BeamAttack = new List<Beam>(new Beam[2]);
    public List<Enemy> MultiTarget = new List<Enemy>();
    public override void ModDamProfile(DamageProfile a)
    {
        a.HowDamageWasDealt = DamageProfile.DamageType.Ranged;
        a.WhatWasTheDamage = DamageProfile.DamageType.Magic;
    }
    public void TargetAquired(int Which)
    {
        Debug.Log("I AM BANANA");
        BeamAttack[Which] = Instantiate(HitLineObject, Vector3.zero, Quaternion.identity, Tags.refs["BulletHolder"].transform).GetComponent<Beam>();
        BeamAttack[Which].LineRenderer.SetPosition(0, transform.position + Parts[0].transform.rotation * SpawnOffset);
        BeamAttack[Which].LineRenderer.SetPosition(1, MultiTarget[Which].Object.transform.position);
    }
    public void TargetLost(int Which)
    {
        Debug.Log("I AM NOTTTTTTT BANANA");
        if (BeamAttack[Which] != null) { Destroy(BeamAttack[Which].gameObject); }
    }
    
    public override void Tick()
    {
        int a = 0;
        foreach (var item in BeamAttack)
        {
            if (item == null) continue;
            if (MultiTarget.Count < a-1 || MultiTarget[a] == null) continue;
            item.LineRenderer.SetPosition(0, transform.position + Parts[0].transform.rotation * SpawnOffset);
            item.LineRenderer.SetPosition(1, MultiTarget[a].Object.transform.position);
        }
    }
    public override void Attack()
    {
        var d = GetDamProfile();
        EnemyTarget.Hit(d);
    }
    public override void TargettingCode()
    {
        var w = GetTarget(2, TargetType);
        List<Enemy> old_Multi = new List<Enemy>(MultiTarget);
        MultiTarget = new List<Enemy>();
        foreach(var g in w) { MultiTarget.Add(g); Debug.Log("cumin");  }

        List<int> Where = new List<int>();
        Debug.Log("bbbbb");
        for (int g = 0; g < MultiTarget.Count; g++) { if (old_Multi.Contains(MultiTarget[g])) { Where.Add(g); Debug.Log("gamin"); } } //on it
        if (Where.Count > 0)
        {
            Debug.Log("a");

            foreach (int g in Where)
            {
                Debug.Log("b");
                TargetLost(g);
                TargetAquired(g);
            }
        }
        Where = new List<int>();
        for (int g = 0; g < old_Multi.Count; g++) { if (MultiTarget.Contains(old_Multi[g])) { Where.Add(g); } }
        if (Where.Count > 0)
        {
            Debug.Log("c");
            foreach (int g in Where)
            {
                Debug.Log("d");
                //TargetLost(g);
            }
        }
    }
}
// Set EnemyTarget and EnemyTarget2 
// 