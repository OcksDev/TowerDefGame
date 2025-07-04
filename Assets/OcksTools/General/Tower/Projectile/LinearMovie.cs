using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMovie : MonoBehaviour
{
    public float speed = 1;
    public float Length = 0;
    public float Offset = 0;
    public bool DestroyOnTouch = true;
    public Projectile Proj;
    // Update is called once per frame
    private Vector3 oldpos;
    List<Enemy> PrevHits = new List<Enemy>();
    void Update()
    {
        oldpos = transform.position;
        transform.position += transform.right*-speed*Time.deltaTime;
        var offshingle = transform.rotation * new Vector2(Offset, 0);
        var lenshingle = transform.rotation * new Vector2(Length/2, 0);
        var wankers = Physics2D.LinecastAll((Vector2)(oldpos + offshingle - lenshingle), (Vector2)(transform.position + offshingle + lenshingle));
        List<ObjectHolder> valids = new List<ObjectHolder>();
        foreach(var a in wankers)
        {
            var t = GameHandler.GetObjectType(a.collider, false);
            if (t.Type == GameHandler.ObjectTypes.Enemy)
            {
                if(Proj.Target != null && t.Object == Proj.Target.Object.gameObject && !PrevHits.Contains(Proj.Target))
                {
                    Proj.HitEnemy(Proj.Target);
                    if (DestroyOnTouch)
                    {
                        Destroy(gameObject);
                        return;
                    }
                    else
                    {
                        PrevHits.Add(Proj.Target);
                    }
                }
                else
                {
                    valids.Add(t);
                }
            }
        }
        if(valids.Count > 0)
        {
            foreach(var a in EnemyHandler.Instance.Enemies)
            {
                if(a.Object.gameObject == valids[0].Object && !PrevHits.Contains(a))
                {
                    Proj.HitEnemy(a);
                    if (DestroyOnTouch)
                    {
                        Destroy(gameObject);
                        return;
                    }
                    else
                    {
                        PrevHits.Add(a);
                    }
                }
            }
        }

    }
}
