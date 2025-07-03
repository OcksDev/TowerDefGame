using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public string TowerType = "";
    public float Range = 5;
    public float AttackRate = 2;
    public double Damage = 5;
    public List<Transform> Parts = new List<Transform>();
    public List<SpriteRenderer> RenderParts = new List<SpriteRenderer>();
    public List<Sprite> TowerIMGS = new List<Sprite>();
    [HideInInspector]
    public Target TargetType;
    [HideInInspector]
    public Enemy EnemyTarget;
    public int MaxLevel = 15;
    private float TimeTillAttack = 0;
    private bool CanAttackTick = false;
    [HideInInspector]
    public List<Tower> RelatedNerds = new List<Tower>();

    // some flags that can be enabled on a per-tower basis. They do nothing by themselves.
    public bool CanAttack = false;
    public bool CanBuffInRange = false;
    public bool CanGenerateMoney = false;
    public int Level = 0;


    private void Start()
    {
        RealPlace();
    }


    public void FixedUpdate()
    {
        EnemyTarget = GetTarget();
        if (!GetCanAttackTick()) AttackTick();
        Tick();
    }

    public void RealPlace()
    {
        GameHandler.Instance.AllActiveTowers.Add(this);
        UpdateAllTowersOfSelf();
        SetStats();
        Place();
    }
    
    public void RealRemove()
    {
        GameHandler.Instance.AllActiveTowers.Remove(this);
        UpdateAllTowersOfSelf(false);
        Remove();
    }

    public virtual void Place()
    {
        UpdateRender();
    }
    public virtual void Remove()
    {
        Destroy(gameObject);
    }
    public virtual void Attack()
    {
        Debug.Log("Attacking!");
    }


    public void SetStats()
    {
        var based = GameHandler.Instance.AllTowerDict[TowerType];
        Damage = based.Damage;
        Range = based.Range;
        AttackRate = based.AttackRate;
        MaxLevel = based.MaxLevel;
        foreach(var a in RelatedNerds)
        {
            a.StatMod(this);
        }
    }

    public virtual void StatMod(Tower me)
    {
        // apply modifiers to the tower "me" while it is in range
    }

    public virtual void AttackTick()
    {
        TimeTillAttack = Mathf.Clamp(TimeTillAttack - Time.deltaTime, 0, 10000);
        if(TimeTillAttack == 0 && EnemyTarget != null && EnemyTarget.Health >= 0)
        {
            Attack();
            TimeTillAttack = 1 / AttackRate;
        }
    }
    public virtual void Tick()
    {
        if (EnemyTarget != null)
        {
            Parts[0].rotation = RandomFunctions.PointAtPoint2D(Parts[0].position, EnemyTarget.Object.position, 0);
        }
    }

    public bool GetCanAttackTick()
    {
        //allows for stuns or other such mechanics to be universally applied to towers
        return CanAttackTick;
    }

    public virtual void Upgrade()
    {
        Level = Mathf.Clamp(Level + 1, 0, MaxLevel);
        UpdateRender();
    }

    public virtual Enemy GetTarget()
    {
        Enemy target = null;
        float td = float.MinValue;
        double hp = float.MinValue;
        switch (TargetType)
        {
            case Target.First:
                foreach (var a in EnemyHandler.Instance.Enemies)
                {
                    if (a._TotalMoved > td && (a.Object.position - transform.position).sqrMagnitude <= Range*Range)
                    {
                        target = a;
                        td = a._TotalMoved;
                    }
                }
                break;
            case Target.Last:
                td = float.MaxValue;
                foreach (var a in EnemyHandler.Instance.Enemies)
                {
                    if (a._TotalMoved < td && (a.Object.position - transform.position).sqrMagnitude <= Range*Range)
                    {
                        target = a;
                        td = a._TotalMoved;
                    }
                }
                break;
            case Target.HighestHP:
                foreach (var a in EnemyHandler.Instance.Enemies)
                {
                    if (a._TotalMoved > td && a.Health > hp && (a.Object.position - transform.position).sqrMagnitude <= Range * Range)
                    {
                        target = a;
                        td = a._TotalMoved;
                        hp = a.Health;
                    }
                }
                break;
            case Target.LowestHP:
                hp = double.MaxValue;
                foreach (var a in EnemyHandler.Instance.Enemies)
                {
                    if (a._TotalMoved > td && a.Health < hp && (a.Object.position - transform.position).sqrMagnitude <= Range * Range)
                    {
                        target = a;
                        td = a._TotalMoved;
                        hp = a.Health;
                    }
                }
                break;
            case Target.Farthest:
                foreach (var a in EnemyHandler.Instance.Enemies)
                {
                    if ((a.Object.position - transform.position).sqrMagnitude <= Range * Range && (a.Object.position - transform.position).sqrMagnitude > td)
                    {
                        target = a;
                        td = (a.Object.position - transform.position).sqrMagnitude;
                    }
                }
                break;
            case Target.Closest:
                td = float.MaxValue;
                foreach (var a in EnemyHandler.Instance.Enemies)
                {
                    if ((a.Object.position - transform.position).sqrMagnitude <= Range * Range && (a.Object.position - transform.position).sqrMagnitude < td)
                    {
                        target = a;
                        td = (a.Object.position - transform.position).sqrMagnitude;
                    }
                }
                break;
        }

        return target;
    }
    public virtual void UpdateRender()
    {
        if (Level < 5)
        {
            RenderParts[0].sprite = GameHandler.Instance.BaseIMGS[0];
            RenderParts[1].sprite = TowerIMGS[0];
        }
        else if (Level < 10)
        {

            RenderParts[0].sprite = GameHandler.Instance.BaseIMGS[1];
            RenderParts[1].sprite = TowerIMGS[1];
        }
        else if (Level < 15)
        {
            RenderParts[0].sprite = GameHandler.Instance.BaseIMGS[2];
            RenderParts[1].sprite = TowerIMGS[2];
        }
        else
        {
            RenderParts[0].sprite = GameHandler.Instance.BaseIMGS[3];
            RenderParts[1].sprite = TowerIMGS[3];
        }
    }
    public enum Target
    {
        First,
        Last,
        HighestHP,
        LowestHP,
        Farthest,
        Closest,
    }

    private void UpdateAllTowersOfSelf(bool existing = true)
    {
        var e = GetAllTowersInRange();
        if (existing)
        {
            foreach (var a in e)
            {
                if (!a.RelatedNerds.Contains(this)) a.RelatedNerds.Add(this);
            }
        }
        else
        {
            foreach (var a in e)
            {
                if (a.RelatedNerds.Contains(this)) a.RelatedNerds.Remove(this);
            }
        }
    }
    public List<Tower> GetAllTowersInRange()
    {
        List<Tower> bana = new List<Tower>();
        foreach(var a in GameHandler.Instance.AllActiveTowers)
        {
            if((a.transform.position-transform.position).sqrMagnitude <= Range*Range) bana.Add(a);
        }
        return bana;
    }
}
