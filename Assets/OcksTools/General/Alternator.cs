using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Alternator : HitscanTower
{
    public override void Tick()
    {
    }
    int nerd = -1;
    int amnt = -1;
    public override void Attack()
    {
        nerd++;
        var banas = GetTarget(amnt, TargetType).ToList();
        nerd %= banas.Count; // shouldn't ever divide by 0, cuz if so, we have other problems
        var d = GetDamProfile();
        var meme = Instantiate(HitLineObject, Vector3.zero, Quaternion.identity, Tags.refs["BulletHolder"].transform).GetComponent<Hitscan>();
        //AttackAnim = StartCoroutine(BackPushAnim());
        meme.LineRenderer.SetPosition(0, Parts[1+nerd].position + Parts[0].transform.rotation * SpawnOffset);
        meme.LineRenderer.SetPosition(1, banas[nerd].Object.transform.position);
        bool b = false;
        double tg = 0;
        if(Level >= 2)
        {
            tg = d.Damage * 3;
            if(Level >= 3)
            {
                tg *= 2;
            }
            Debug.Log($"Attempt die: {banas[nerd].Health}, against {tg}");
            if(banas[nerd].Health <= tg)
            {
                banas[nerd].Health = 0;
                b = true;
            }
        }


        var a = banas[nerd].Hit(d);
        if(!a && b) banas[nerd].Kill();
        if (Level >= 1 && (a||b))
        {
            if (b && Level >= 3) d.Damage *= 1.5f;
            int berd = nerd;
            for(int i = 0; i < banas.Count-1; i++)
            {
                berd++;
                berd %= banas.Count; // shouldn't ever divide by 0, cuz if so, we have other problems
                meme = Instantiate(HitLineObject, Vector3.zero, Quaternion.identity, Tags.refs["BulletHolder"].transform).GetComponent<Hitscan>();
                //AttackAnim = StartCoroutine(BackPushAnim());
                meme.LineRenderer.SetPosition(0, Parts[1 + berd].position + Parts[0].transform.rotation * SpawnOffset);
                meme.LineRenderer.SetPosition(1, banas[berd].Object.transform.position);
                banas[berd].Hit(d);
            }
        }

    }
    public override void UpdateRender()
    {
        base.UpdateRender();
        Parts[4].gameObject.SetActive(Level >= 1);
        Parts[5].gameObject.SetActive(Level >= 3);
        RenderParts[2].sprite = OtherImages[Level];
        RenderParts[3].sprite = OtherImages[Level];
        RenderParts[4].sprite = OtherImages[Level];
        Vector3 offnerd = new Vector3(0, -0.020f, 0);
        float dd = 0.55f;
        amnt = 3;
        if (Level >= 1)
        {
            amnt++;
            RenderParts[5].sprite = OtherImages[Level];
            offnerd = new Vector3(0, 0, 0);
            dd = 0.55f - 0.02f;
        }
        if (Level >= 3)
        {
            amnt++;
            RenderParts[6].sprite = OtherImages[Level];
            offnerd = new Vector3(0, -0.020f, 0);
            dd = 0.55f;
        }
        for(int i = 0; i < amnt; i++)
        {
            var d = offnerd;
            if (Level >= 3 && (i == 2 || i == 3)) d += new Vector3(0, 0.010f, 0);
            Parts[1 + i].localPosition = (Quaternion.Euler(0,0,(360f/amnt)*i) * new Vector3(0, dd, 0)) + d;
        }
    }
}
