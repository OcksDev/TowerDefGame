using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTower : Tower
{
    public override void Attack()
    {
        EnemyTarget.Hit(GetDamProfile());
        AttackAnim = StartCoroutine(BackPushAnim());
    }

    int state = 2;
    public IEnumerator BackPushAnim()
    {
        float x = 0;
        while (x < 1)
        {
            x = Mathf.Clamp01(x + Time.deltaTime*AttackRate);
            state =  Mathf.FloorToInt(Mathf.Clamp(x * 4,0,2));
            Parts[0].localPosition = Parts[0].rotation * new Vector3(0.2f*(1-RandomFunctions.EaseIn(x)), 0, 0);
            UpdateRender();
            yield return null;
        }
        state = 2;
        Parts[0].transform.localPosition = Vector3.zero;
    }

    public override void UpdateRender()
    {
        RenderParts[0].sprite = GameHandler.Instance.BaseIMGS[Level];
        RenderParts[1].sprite = TowerIMGS[Level];
        switch (TowerType)
        {
            case "Crossbow":
                RenderParts[2].sprite = OtherImages[(Level*3)+state];
                break;
        }
    }
    public override void ModDamProfile(DamageProfile a)
    {
        a.HowDamageWasDealt = DamageProfile.DamageType.Ranged;
    }
}
