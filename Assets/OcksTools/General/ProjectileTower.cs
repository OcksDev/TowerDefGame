using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTower : Tower
{
    public GameObject NerdToSpawn;
    public Vector3 SpawnOffset = Vector3.zero;
    public override void Attack()
    {
        var d = GetDamProfile();
        Projectile meme;
        if (TowerType == "Crossbow" && Level >= 3)
        {
            meme = Instantiate(NerdToSpawn, transform.position + Parts[0].transform.rotation * SpawnOffset, Parts[0].rotation, Tags.refs["BulletHolder"].transform).GetComponent<Projectile>();
            meme.Init(this, d);
            var dd = GetDamProfile();
            dd.Damage /= 2;
            meme = Instantiate(NerdToSpawn, transform.position + Parts[0].transform.rotation * SpawnOffset, Parts[0].rotation * Quaternion.Euler(0,0,7.5f), Tags.refs["BulletHolder"].transform).GetComponent<Projectile>();
            meme.Init(this, dd);
            meme = Instantiate(NerdToSpawn, transform.position + Parts[0].transform.rotation * SpawnOffset, Parts[0].rotation * Quaternion.Euler(0,0,-7.5f), Tags.refs["BulletHolder"].transform).GetComponent<Projectile>();
            meme.Init(this, dd);
        }
        else
        {
            meme = Instantiate(NerdToSpawn, transform.position + Parts[0].transform.rotation * SpawnOffset, Parts[0].rotation, Tags.refs["BulletHolder"].transform).GetComponent<Projectile>();
            meme.Init(this, d);
        }
        AttackAnim = StartCoroutine(BackPushAnim());
    }

    int state = 2;
    public override IEnumerator BackPushAnim()
    {
        float x = 0;
        while (x < 1)
        {
            x = Mathf.Clamp01(x + Time.deltaTime*Mathf.Max(GetAttackRate(),1));
            state =  Mathf.FloorToInt(Mathf.Clamp(x * 4,0,2));
            Parts[0].localPosition = Parts[0].rotation * new Vector3(0.2f*(1-RandomFunctions.EaseIn(x)), 0, 0);
            RealUpdateRender();
            yield return null;
        }
        state = 2;
        Parts[0].transform.localPosition = Vector3.zero;
    }

    public override void UpdateTowerRender()
    {
        RenderParts[0].sprite = GameHandler.Instance.BaseIMGS[Level];
        RenderParts[1].sprite = TowerIMGS[Level];
        switch (TowerType)
        {
            case "Crossbow":
                RenderParts[2].sprite = OtherImages[(Level*3)+state];
                break;
            case "Rocket":
                if(state > 1)
                {
                    RenderParts[1].sprite = TowerIMGS[Level];
                }
                else
                {
                    RenderParts[1].sprite = OtherImages[Level];
                }
                break;
        }
    }
    public override void ModDamProfile(DamageProfile a)
    {
        a.HowDamageWasDealt = DamageProfile.DamageType.Ranged;
        a.WhatWasTheDamage = DamageProfile.DamageType.Bullet;
        switch (TowerType)
        {
            case "Rocket":
                a.WhatWasTheDamage = DamageProfile.DamageType.Explosion;
                break;
        }
    }
}
