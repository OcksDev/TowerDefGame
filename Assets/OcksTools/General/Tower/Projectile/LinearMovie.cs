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
        var valids = OXCollision.LineCastAll(oldpos-lenshingle, oldpos-transform.position+lenshingle, 0.02f);
        if(valids.Count > 0)
        {
            while(valids.Count > 0)
            {
                var a = valids[0];
                if (!PrevHits.Contains(a))
                {
                    Proj.HitEnemy(a);
                    if (Pierce <= 0)
                    {
                        Destroy(gameObject);
                        return;
                    }
                    else
                    {
                        Pierce--;
                        PrevHits.Add(a);
                        break;
                    }
                }
                valids.RemoveAt(0);
            }
            
        }
    }
}
