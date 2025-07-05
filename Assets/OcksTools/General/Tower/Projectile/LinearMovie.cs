using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMovie : MonoBehaviour
{
    public float speed = 1;
    public float Length = 0;
    public float Offset = 0;
    public int Pierce = 0;
    public Projectile Proj;
    // Update is called once per frame
    private Vector3 oldpos;
    List<Enemy> PrevHits = new List<Enemy>();

    private void Start()
    {
        Pierce = Proj.Tower.Pierce;
    }

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
                if(Proj.Target != null && Proj.Target.Object != null && t.Object != null && t.Object == Proj.Target.Object.gameObject && !PrevHits.Contains(Proj.Target))
                {
                    Proj.HitEnemy(Proj.Target);
                    if (Pierce <= 0)
                    {
                        Destroy(gameObject);
                        return;
                    }
                    else
                    {
                        Pierce--;
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
            for(int i = 0; i < EnemyHandler.Instance.Enemies.Count; i++)
            {
                var a = EnemyHandler.Instance.Enemies[i];
                if (a.Object.gameObject == valids[0].Object && !PrevHits.Contains(a))
                {
                    if (Proj.HitEnemy(a))
                    {
                        i--;
                    }
                    if (Pierce <= 0)
                    {
                        Destroy(gameObject);
                        return;
                    }
                    else
                    {
                        Pierce--;
                        PrevHits.Add(a);
                    }
                }
            }
        }

    }
}
