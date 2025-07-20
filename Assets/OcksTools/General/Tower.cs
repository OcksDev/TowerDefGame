using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class Tower : MonoBehaviour
{
    [HideInInspector]
    public bool IsPlacing = true;
    public string TowerType = "";
    public int DesiredTargetCount = 1;
    public float Range = 5;
    public float AttackRate = 2;
    public double Damage = 5;
    public float ExplosionRange = 0;
    public int Pierce = 0;
    public List<Transform> Parts = new List<Transform>();
    public List<SpriteRenderer> RenderParts = new List<SpriteRenderer>();
    public List<Sprite> TowerIMGS = new List<Sprite>();
    [HideInInspector]
    public Target TargetType;
    [HideInInspector]
    public Enemy EnemyTarget;
    public int MaxLevel = 3; // 0,1,2,3
    private float TimeTillAttack = 0;
    private bool CanAttackTick = true;
    [HideInInspector]
    public List<Tower> RelatedNerds = new List<Tower>();
    [HideInInspector]
    public List<Tower> MyGems = new List<Tower>();
    public List<Sprite> OtherImages = new List<Sprite>();
    [HideInInspector]
    public Vector3 MyPos = Vector3.zero;
    [HideInInspector]
    public Queue<Enemy> TargetHandover = new Queue<Enemy>();
    public Dictionary<string, Buff> Buffs = new Dictionary<string, Buff>();


    // some flags that can be enabled on a per-tower basis. They do nothing by themselves.
    public bool CanAttack = false;
    public bool CanBuffInRange = false;
    public bool CanGenerateMoney = false;
    public int Level = 0;


    private void Start() // debug code
    {
        if(Time.time <= 0.5f)RealPlace();
    }

    protected Enemy old_tg;
    public void FixedUpdate()
    {
        MyPos = transform.position;
        if (IsPlacing) return;
        TargettingCode();
        Tick();
        if (GetCanAttackTick()) AttackTick();
    }

    public virtual void TargettingCode()
    {
        var w = ReadTarget();
        if (w.Count > 0)
        {
            EnemyTarget = w.Dequeue();
        }
        else
        {
            EnemyTarget = null;
        }
        if (EnemyTarget != old_tg)
        {
            if (EnemyTarget != null)
            {
                if (old_tg != null) TargetLost();
                TargetAquired();
            }
            else
            {
                TargetLost();
            }
            old_tg = EnemyTarget;
        }
        //twi towers ideas
    }

    private List<Enemy> mtg = new List<Enemy>();
    private HashSet<Enemy> old_nerds = new HashSet<Enemy>();
    public void TestMultiTarget(int nerds)
    {
        var w = GetTarget(nerds, TargetType);
        mtg = w.ToList();
        var gomnadingle = new HashSet<Enemy>(w);
        foreach (var a in old_nerds)
        {
            if(!gomnadingle.Contains(a)) TargetLost(a);
        }
        foreach (var a in gomnadingle)
        {
            if(!old_nerds.Contains(a)) TargetAquired(a);
        }
        old_nerds = gomnadingle;
    }

    public virtual void TargetAquired(Enemy tg) { }
    public virtual void TargetLost(Enemy tg){ }

    /*private void Update()
    {
        if (InputManager.IsKeyDown("shoot") && Hover.IsHovering(gameObject))
        {
            Upgrade();
        }
    }*/
    public void RealPlace()
    {
        IsPlacing = false;
        GameHandler.Instance.AllActiveTowers.Add(this);
        UpdateAllTowersOfSelf();
        SetStats();
        GameHandler.Instance.NewTowerCreated?.Invoke(this);
        Place();
        MyPos = transform.position;
    }
    
    public void RealRemove()
    {
        GameHandler.Instance.AllActiveTowers.Remove(this);
        UpdateAllTowersOfSelf(false);
        Remove();
    }
    public void RealAttack()
    {
        if (AttackAnim != null) StopCoroutine(AttackAnim);
        if (EnemyTarget.MarkedForDeath)
        {
            TargetLost();
            var a = GetTarget(1, TargetType);
            if (a.Count > 0)
            {
                EnemyTarget = a.Dequeue();
                if (EnemyTarget != null) Attack();
            }
        }
        else
        {
            Attack();
        }
    }

    public virtual void Place()
    {
        RealUpdateRender();
    }
    public virtual void Remove()
    {
        Destroy(gameObject);
    }
    public virtual void Attack()
    {
        Debug.Log("Attacking!");
    }

    public virtual float GetAttackRate()
    {
        return AttackRate;
    }
    
    public virtual double GetDamage()
    {
        return Damage;
    }


    public Coroutine AttackAnim;
    public void SetStats()
    {
        var based = GameHandler.Instance.AllTowerDict[TowerType];
        Damage = based.Damage;
        Range = based.Range;
        AttackRate = based.AttackRate;
        MaxLevel = based.MaxLevel;
        Buffs.Clear();
        TowerSpecificStats();
        foreach (var a in RelatedNerds)
        {
            a.StatMod(this);
        }
        foreach (var a in Buffs)
        {
            ApplyBuff(a.Value);
        }
    }

    public virtual void StatMod(Tower me)
    {
        // apply modifiers to the tower "me" while it is in range
    }
    public void ApplyBuff(Buff b)
    {
        switch (b.Type)
        {
            case "Overload":
                AttackRate *= 10;
                break;
        }
    }
    
    public virtual void TowerSpecificStats()
    {
        switch (TowerType)
        {
            case "Crossbow":
                if(Level >= 1)
                {
                    Damage = 10;
                    AttackRate = 2.5f;
                }
                if(Level >= 2)
                {
                    Damage = 20;
                    AttackRate = 3f;
                    Range += 2;
                }
                if(Level >= 3)
                {
                    Damage = 50;
                    AttackRate = 3.5f;
                    Range += 3;
                }
                break;
        }
    }
    public virtual int GetCostToUpgrade(int level)
    {
        switch (TowerType)
        {
            case "Crossbow":
                switch (Level)
                {
                    default: return 50;
                    case 1: return 150;
                    case 2: return 500;
                }
        }
        return -1;
    }

    public virtual void AttackTick()
    {
        TimeTillAttack = Mathf.Clamp(TimeTillAttack - Time.deltaTime, 0, 10000);
        if(TimeTillAttack == 0 && EnemyTarget != null && EnemyTarget.Health >= 0)
        {
            RealAttack();
            TimeTillAttack = 1 / GetAttackRate();
        }
    }
    public virtual void TargetAquired()
    {

    }
    public virtual void TargetLost()
    {

    }
    public virtual void Tick()
    {
        if (EnemyTarget != null && EnemyTarget.Object != null)
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
        RealUpdateRender();
        SetStats();
    }

    public virtual IEnumerator BackPushAnim()
    {
        float x = 0;
        while (x < 1)
        {
            x = Mathf.Clamp01(x + Time.deltaTime * Mathf.Max(GetAttackRate(), 1));
            Parts[0].localPosition = Parts[0].rotation * new Vector3(0.2f * (1 - RandomFunctions.EaseIn(x)), 0, 0);
            yield return null;
        }
        Parts[0].transform.localPosition = Vector3.zero;

    }
    private Target targetpassover;
    private int CompareBySex(Enemy a, Enemy b)
    {
        if(a==null || b==null) return -1;
        switch (targetpassover)
        {
            case Target.First:
                if (a._TotalMoved > b._TotalMoved) return -1;
                else return 1;
            case Target.Last:
                if (a._TotalMoved < b._TotalMoved) return -1;
                else return 1;
            case Target.HighestHP:
                if (a.Health > b.Health) return -1;
                else if (a.Health < b.Health) return 1;
                else
                {
                    if (a._TotalMoved > b._TotalMoved) return -1;
                    else return 1;
                }
            case Target.LowestHP:
                if (a.Health < b.Health) return -1;
                else if (a.Health > b.Health) return 1;
                else
                {
                    if (a._TotalMoved > b._TotalMoved) return -1;
                    else return 1;
                }
            case Target.Farthest:
                if ((a.mypos - MyPos).sqrMagnitude > (b.mypos - MyPos).sqrMagnitude) return -1;
                else return 1;
            case Target.Closest:
                if ((a.mypos - MyPos).sqrMagnitude > (b.mypos - MyPos).sqrMagnitude) return -1;
                else return 1;
            case Target.Fastest:
                if (a.MovementSpeed > b.MovementSpeed) return -1;
                else if (a.MovementSpeed < b.MovementSpeed) return 1;
                else
                {
                    if (a._TotalMoved > b._TotalMoved) return -1;
                    else return 1;
                }
            case Target.Slowest:
                if (a.MovementSpeed < b.MovementSpeed) return -1;
                else if (a.MovementSpeed > b.MovementSpeed) return 1;
                else
                {
                    if (a._TotalMoved > b._TotalMoved) return -1;
                    else return 1;
                }
        }
        return 0;
    }
    public Queue<Enemy> ReadTarget()
    {
        return TargetHandover;
    }


    public Queue<Enemy> GetTarget(int amnt = 1, Target type = Target.First)
    {
        Queue<Enemy> target = new Queue<Enemy>();

        if (EnemyHandler.Instance.Enemies.Count==0)
        {
            return target;
        }

        if (amnt > 1)
        {
            //multi-get code
            List<Enemy> nerds = new List<Enemy>();
            for (int i = 0; i < EnemyHandler.Instance.Enemies.Count; i++)
            {
                var a = EnemyHandler.Instance.Enemies[i];
                try { if (a == null || a.MarkedForDeath || a.Object == null) continue; }
                catch { }
                if ((a.mypos - MyPos).sqrMagnitude <= Range * Range)
                {
                    nerds.Add(a);
                }
            }

            targetpassover = type;
            nerds.Sort(CompareBySex);
            for (int i = 0; i < nerds.Count && i < amnt; i++)
            {
                target.Enqueue(nerds[i]);
            }
        }
        else
        {
            //optimized single-target code
            Enemy curnerd = null;
            for (int i = 0; i < EnemyHandler.Instance.Enemies.Count; i++)
            {
                var a = EnemyHandler.Instance.Enemies[i];
                try { if (a == null || a.MarkedForDeath || a.Object == null) continue; }
                catch { }
                if ((a.mypos - MyPos).sqrMagnitude <= Range * Range && CompareBySex(a, curnerd) == -1)
                {
                    curnerd = a;
                }
            }
            if(curnerd != null)
            {
                target.Enqueue(curnerd);
            }
        }
            
        return target;
    }
    private bool HasSexPlaced = false;
    public void RealUpdateRender()
    {
        UpdateTowerRender();
        if(!IsPlacing)
        {
            HasSexPlaced = true;
            foreach(var a in RenderParts)
            {
                a.material = GameHandler.Instance.BaseMats[0];
                a.color = GameHandler.Instance.BaseColors[0];
            }
        }
        else
        {
            foreach (var a in RenderParts)
            {
                a.material = GameHandler.Instance.BaseMats[1];
            }
        }
    }
    public virtual void UpdateTowerRender()
    {
        RenderParts[0].sprite = GameHandler.Instance.BaseIMGS[Level];
        RenderParts[1].sprite = TowerIMGS[Level];
    }
    public void UpdatePlaceColor(bool good)
    {
        if (good)
        {
            foreach (var a in RenderParts)
            {
                a.color = GameHandler.Instance.BaseColors[1];
            }
        }
        else
        {
            foreach (var a in RenderParts)
            {
                a.color = GameHandler.Instance.BaseColors[2];
            }
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
        Fastest,
        Slowest,
    }

    public virtual void INowExist(Tower nerd)
    {
        if (!RelatedNerds.Contains(nerd)) RelatedNerds.Add(nerd);
    } 
    public virtual void INowDontExist(Tower nerd)
    {
        if (RelatedNerds.Contains(nerd)) RelatedNerds.Remove(nerd);
    } 

    private void UpdateAllTowersOfSelf(bool existing = true)
    {
        var e = GetAllTowersInRange();
        if (existing)
        {
            foreach (var a in e)
            {
                a.INowExist(this);
                if((a.transform.position-transform.position).sqrMagnitude <= a.Range * a.Range) RelatedNerds.Add(a);
            }
        }
        else
        {
            foreach (var a in e)
            {
                a.INowDontExist(this);
            }
            //dont need to clear own relatednerds since tower is getting destroyed
        }
    }
    public List<Tower> GetAllTowersInRange()
    {
        List<Tower> bana = new List<Tower>();
        foreach(var a in GameHandler.Instance.AllActiveTowers)
        {
            if((a.transform.position-transform.position).sqrMagnitude <= (Range*Range) && a != this)
            {
                bana.Add(a);
            }
        }
        return bana;
    }

    public Buff GetBuff(string buff)
    {
        if(Buffs.ContainsKey(buff)) return Buffs[buff];
        return new Buff();
    }
    
    public void AddBuff(Buff buff)
    {
        if (!Buffs.ContainsKey(buff.Type))
        {
            Buffs.Add(buff.Type, buff);
        }
        else if (Buffs[buff.Type].Level < buff.Level)
        {
            Buffs[buff.Type] = buff;
        }

        //if it didnt add the buff then the same buff was already applied at a higher level
    }


    public DamageProfile GetDamProfile()
    {
        var a = new DamageProfile(this, DamageProfile.DamageType.Unknown, DamageProfile.DamageType.Unknown, GetDamage());
        ModDamProfile(a);
        return a;
    }
    public virtual void ModDamProfile(DamageProfile a)
    {
        
    }



}

public class Buff
{
    public string Type = "";
    public int Level = 0;
    public int Stack = 0;
    public Tower SourceTower;
    public Buff(string ty, Tower tower)
    {
        Type = ty;
        SourceTower = tower;
        Stack = 1;
        Level = tower.Level;
    }
    public Buff()
    {
    }
}