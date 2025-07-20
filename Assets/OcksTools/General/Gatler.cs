using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatler : HitscanTower
{
    float tgtime = 0;
    public override float GetAttackRate()
    {
        var dd = base.GetAttackRate();
        dd *= (2*Mathf.Log(tgtime+1))+1;
        return dd; 
    }
    public override void Tick()
    {
        base.Tick();
        if(EnemyTarget != null)tgtime += Time.deltaTime;
    }
    public override void TargetAquired()
    {
        tgtime = 0;
    }
    public override void TargetLost()
    {
        tgtime = 0;
    }
    int state = 1;
    public override IEnumerator BackPushAnim()
    {
        float x = 0;
        while (x < 1)
        {
            x = Mathf.Clamp01(x + Time.deltaTime * Mathf.Max(GetAttackRate(), 1));
            Parts[0].localPosition = Parts[0].rotation * new Vector3(0.2f * (1 - RandomFunctions.EaseIn(x)), 0, 0);
            state = Mathf.FloorToInt(Mathf.Clamp(x * 1.5f, 0, 1));
            RealUpdateRender();
            yield return null;
        }
        Parts[0].transform.localPosition = Vector3.zero; 
        RealUpdateRender();

    }
    public override void UpdateTowerRender()
    {
        base.UpdateTowerRender();
        if (state == 0)
        {
            RenderParts[1].sprite = OtherImages[Level];
        }
    }

}
