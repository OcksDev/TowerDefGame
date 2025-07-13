using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();
    public SpriteRenderer spriteRenderer;
    [HideInInspector]
    public Tower Tower;
    [HideInInspector]
    public DamageProfile Profile;
    [HideInInspector]
    public Enemy Target;
    public float LifeSpan = 5f;
    public GameObject nerd;
    public void Init(Tower e, DamageProfile d)
    {
        Tower = e;
        Profile = d;
        Target = e.EnemyTarget;
        switch (e.TowerType)
        {
            case "Crossbow":
                spriteRenderer.sprite = sprites[e.Level];
                break;
        }
    }
    private void FixedUpdate()
    {
        if(Tower == null)
        {
            Destroy(gameObject);
            return;
        }
        LifeSpan -= Time.deltaTime;
        if(LifeSpan <= 0) Destroy(gameObject);
    }
    public virtual bool HitEnemy(Enemy e)
    {
        switch (Tower.TowerType)
        {
            case "Rocket":
                //explode!

                var dd = Instantiate(nerd, transform.position, Quaternion.identity).GetComponent<Explodie>();
                dd.Size = Tower.ExplosionRange*2;
                dd.transform.localScale = Vector3.one * dd.Size;
                e.Hit(Profile);
                var ding = Physics2D.OverlapCircleAll((Vector2)transform.position, Tower.ExplosionRange);
                foreach(var a in ding)
                {
                    if (a.gameObject == e.Object.gameObject) continue;
                    var t = GameHandler.GetObjectType(a,false);
                    if (t.Type==GameHandler.ObjectTypes.Enemy)
                    {
                        EnemyHandler.Instance.ObjectToEnemy[a.gameObject].Hit(Profile);
                    }
                }
                return true;
            default:
                return e.Hit(Profile);
        }
    }

}
