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
        LifeSpan -= Time.deltaTime;
        if(LifeSpan <= 0) Destroy(gameObject);
    }
    public virtual void HitEnemy(Enemy e)
    {
        e.Hit(Profile);
    }

}
