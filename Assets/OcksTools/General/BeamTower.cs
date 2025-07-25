using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeamTower : Tower
{
    public GameObject HitLineObject;
    public Vector3 SpawnOffset = Vector3.zero;
    public List<Beam> BeamAttack = new List<Beam>(new Beam[2]);
    public List<Enemy> MultiTarget = new List<Enemy>();
    public override void ModDamProfile(DamageProfile a)
    {
        Debug.Log("Ran ModDamProfile");
        a.HowDamageWasDealt = DamageProfile.DamageType.Ranged;
        a.WhatWasTheDamage = DamageProfile.DamageType.Magic;
    }
    public void TargetAquired(int Which)
    {
        Debug.Log("Ran TargetAquired");
        if (BeamAttack[Which] != null) { Destroy(BeamAttack[Which].gameObject); }
        BeamAttack[Which] = Instantiate(HitLineObject, Vector3.zero, Quaternion.identity, Tags.refs["BulletHolder"].transform).GetComponent<Beam>();
        BeamAttack[Which].LineRenderer.SetPosition(0, transform.position + Parts[0].transform.rotation * SpawnOffset);
        BeamAttack[Which].LineRenderer.SetPosition(1, MultiTarget[Which].Object.transform.position);
    }
    public void TargetLost(int Which)
    {
        Debug.Log("Ran TargetLost");
        if (BeamAttack[Which] != null) { Destroy(BeamAttack[Which].gameObject); }
    }
    
    public override void Tick()
    {
        Debug.Log("Ran Tick");
        for (int a = 0; a < BeamAttack.Count; a++)
            {
            try
            {
                BeamAttack[a].LineRenderer.SetPosition(0, transform.position + Parts[0].transform.rotation * SpawnOffset);
                BeamAttack[a].LineRenderer.SetPosition(1, MultiTarget[a].Object.transform.position);
            }
            catch { continue; }
            }
    }
    public override void Attack()
    {
        Debug.Log("Ran Tick");
        var d = GetDamProfile();
        EnemyTarget.Hit(d);
    }
    public override void TargettingCode()
    {
        Debug.Log("Ran TargettingCode");
        //Sets up Multitarget, Multitarget comparison, and Target.
        var w = ReadTarget();
        List<Enemy> old_Multi = new List<Enemy>(MultiTarget);
        MultiTarget = new List<Enemy>(w.ToList());
        List<int> Where = new List<int>();

        for (int g = 0; g < old_Multi.Count; g++) 
        { 
            if (!MultiTarget.Contains(old_Multi[g])) { Where.Add(g); }
        }
        if (Where.Count > 0)
        {
            foreach (int g in Where)
            {
                TargetLost(g);
            }
        }

        Where = new List<int>(); // Resets "Where"

        for (int g = 0; g < MultiTarget.Count; g++) 
        { 
            if (!old_Multi.Contains(MultiTarget[g])) { Where.Add(g); } //Checks Multitarget against its old self, Where logs location of differences.
        }
        if (Where.Count > 0)
        {
            foreach (int g in Where)
            {
                TargetAquired(g);
            }
        }
    }
}
// Set EnemyTarget and EnemyTarget2 
// 