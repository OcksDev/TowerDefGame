using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    public List<Enemy> Enemies = new List<Enemy>();
    public List<Enemy> Deads = new List<Enemy>();
    public Dictionary<GameObject,Enemy> ObjectToEnemy = new Dictionary<GameObject, Enemy>();
    public static EnemyHandler Instance;

    public GameObject EnemyObj;
    public OXThreadPoolA EnemyThreads;
    public static int ThreadCount = 20;
    private void Awake()
    {
        Instance = this;
        EnemyThreads = new OXThreadPoolA(ThreadCount);
        StartCoroutine(BananaSmeg());
    }
    int bongle = 0;
    public IEnumerator BananaSmeg()
    {
        yield return new WaitUntil(() => { return EnemyThreads.allconfirmed; });
        for(int i = 0; i < ThreadCount; i++)
        {
            EnemyThreads.Add(EnemyThread);
        }
    }

    public void EnemyThread()
    {
        int x = bongle++;
        while (true)
        {
            for (int i = x; i < Enemies.Count; i+=ThreadCount) // potential optimization to save enemies[i] to a var instead of continual lookup
            {
                var d = Enemies[i];
                try
                {
                    if (d == null || d.MarkedForDeath || d.Object == null) continue;
                }
                catch { }
                float difft = g_time-d.my_time;
                if (difft <= 0) continue;
                d.my_time = g_time;
                //some wacky ass movement code
                float maxd = d.GetMovementSpeed() * difft;
                var oldp = d.mypos;
                var diff = (GameHandler.Instance.Map.Poses[d.NodeTarget] - oldp);


                if (diff.sqrMagnitude < maxd * maxd)
                {
                    d.NodeTarget = GameHandler.Instance.Map.GetNextIndex(d.NodeTarget);
                    if (d.NodeTarget == -1)
                    {
                        //DIE DIE DIE
                        Deads.Add(d);
                        d.MarkedForDeath = true;
                        continue;
                    }
                }

                var weewee = diff.normalized * maxd + oldp;
                d._TotalMoved += maxd;
                d.mypos = weewee;
            }
            Thread.Sleep(1);
        }
    }


    public Enemy SpawnEnemy(string type)
    {
        var a = new Enemy(type);
        var x = GameHandler.Instance.Map.GetSpawnIndex();
        switch (type)
        {
            default:
                a.Object = Instantiate(EnemyObj, GameHandler.Instance.Map.Nodes[x].Node.position, Quaternion.identity, transform).transform;
                ObjectToEnemy.Add(a.Object.gameObject, a);
                break;
            case "Phys":break;
        }
        a.Max_Shield = 0;
        switch (type)
        {
            default:
                a.Max_Health = 10;
                break;
        }
        a.Health = a.Max_Health;
        a.Shield = a.Max_Shield;
        a.NodeTarget = x;
        Enemies.Add(a);
        a.mypos = GameHandler.Instance.Map.Nodes[x].Node.position;
        a.my_time = g_time;
        return a;
    }


    public static float g_time = 0;
    private void Update()
    {
        g_time = Time.time;
        var map = GameHandler.Instance.Map;
        for (int i = 0; i < Deads.Count; i++) // potential optimization to save enemies[i] to a var instead of continual lookup
        {
            Deads[i]?.Kill(false);
        }
        Deads.Clear();
        for (int i = 0; i < Enemies.Count; i++) // potential optimization to save enemies[i] to a var instead of continual lookup
        {
            //some wacky ass movement code
            Enemies[i].Object.position = Enemies[i].mypos;
        }
    }

}
[System.Serializable]
public class Enemy
{
    public string EnemyType;
    public Transform Object;
    public int NodeTarget;
    public float Radius = 0.3333f;
    public double Health = 100;
    public double Shield = 100;
    public double Max_Health = 100;
    public double Max_Shield = 100;
    public float MovementSpeed = 1;
    public float _TotalMoved = 0;
    public float my_time = 0;
    public Vector3 mypos;
    public bool MarkedForDeath = false;
    public List<EffectProfile> Effects = new List<EffectProfile>();

    public Enemy(string enemyType)
    {
        EnemyType = enemyType; 
    }


    public float GetMovementSpeed()
    {
        //speed up / down modifiers go here lol
        return MovementSpeed;
    }

    public void Kill(bool real_kill = true)
    {
        MarkedForDeath = true;
        EnemyHandler.Instance.Enemies.Remove(this);
        try
        {
            EnemyHandler.Instance.ObjectToEnemy.Remove(Object.gameObject);
        }
        catch
        {

        }
        if (real_kill)
        {
            //give rewards or do extra code
        }
        else
        {
            //the enemy should just stop existing without extra fanfare
        }
        try
        {
            UnityEngine.Object.Destroy(Object.gameObject);
        }
        catch
        {

        }
    }

    public bool Hit(DamageProfile hit)
    {
        var dmg = hit.CalcDamage();
        foreach (var effect in hit.Effects)
        {
            AddEffect(effect);
        }
        Shield -= dmg;
        if (Shield < 0)
        {
            Health = System.Math.Clamp(Health + Shield, 0, Max_Health);
            if(Health <= 0)
            {
                Kill();
                return true;
            }
        }
        Shield = System.Math.Clamp(Shield, 0, Max_Shield);
        return false;
    }

    public void Heal(double amount)
    {
        var oldh = Health;
        Health = System.Math.Clamp(Health + amount, 0, Max_Health);
        var change = amount - (Health - oldh);
        var olds = Shield;
        Shield = System.Math.Clamp(Shield + change, 0, Max_Shield);
        var change2 = change - (Shield - olds);

        // Amount Healed: RandomFunctions.Instance.NumToRead(((System.Numerics.BigInteger)System.Math.Round(amount - change2)).ToString())

        if (Health != oldh || Shield != olds)
        {
            //runs if heal was successful
        }
    }



    public EffectProfile GetEffect(string name)
    {
        foreach (var ef in Effects)
        {
            if (name == ef.Name)
            {
                return ef;
            }
        }
        return null;
    }
    public EffectProfile GetEffect(EffectProfile eff)
    {
        foreach (var ef in Effects)
        {
            if (eff.Name == ef.Name)
            {
                return ef;
            }
        }
        return null;
    }


    public void AddEffect(EffectProfile eff)
    {
        eff.TimeRemaining = eff.Duration;
        EffectProfile s = GetEffect(eff.Name);
        if (s != null)
        {
            switch (eff.CombineMethod)
            {
                default:
                    //replace existing effect with new
                    Effects.Remove(s);
                    Effects.Add(eff);
                    break;
                case 1:
                    //apply effect as new
                    Effects.Add(eff);
                    break;
                case 2:
                    //increase stack count
                    s.Stack += eff.Stack;
                    break;
                case 3:
                    //increase stack count, up to maximum value
                    s.Stack += eff.Stack;
                    if (s.Stack > s.MaxStack) s.Stack = s.MaxStack;
                    break;
                case 4:
                    //increase stack count, up to maximum value, refresh duration
                    s.Stack += eff.Stack;
                    s.TimeRemaining = eff.Duration;
                    if (s.Stack > s.MaxStack) s.Stack = s.MaxStack;
                    break;
                case 5:
                    //add old time remaining with new time (2s + 5s = 7s)
                    s.TimeRemaining += eff.Duration;
                    break;
                case 6:
                    //add old time remaining with new time (2s + 5s = 7s), also increase stack count
                    s.TimeRemaining += eff.Duration;
                    s.Stack += eff.Stack;
                    break;
                case 7:
                    //increase stack count, refresh time remaining
                    s.Stack += eff.Stack;
                    s.TimeRemaining = eff.Duration;
                    break;
            }
        }
        else
        {
            Effects.Add(eff);
        }


    }

}

public class DamageProfile
{
    public Tower SourceObject;
    public DamageType HowDamageWasDealt = DamageType.Unknown;
    public DamageType WhatWasTheDamage = DamageType.Unknown;
    public double Damage;
    public List<EffectProfile> Effects = new List<EffectProfile>();
    public Dictionary<string, int> Procs = new Dictionary<string, int>();
    public DamageProfile(Tower OB, DamageType How, DamageType What, double damage)
    {
        SourceObject = OB;
        HowDamageWasDealt = How;
        WhatWasTheDamage = What;
        Damage = damage;
    }
    public DamageProfile(DamageProfile pp)
    {
        SourceObject = pp.SourceObject;
        HowDamageWasDealt = pp.HowDamageWasDealt;
        Damage = pp.Damage;
        Procs = new Dictionary<string, int>(pp.Procs);
        Effects = new List<EffectProfile>(pp.Effects);
    }
    public double CalcDamage()
    {
        var x = Damage;

        //do some damage calculaations

        return x;
    }
    public enum DamageType
    {
        Unknown,
        Melee,
        Ranged,
        Trap,
        World,
        Explosion,
        Bullet,
        Magic,
        Fire,
        Ice,
        Lightning,
        Air,
    }

}

public class EffectProfile
{
    //data you pass in
    public string Name;
    public float Duration;
    public int CombineMethod;
    //other data
    public int Stack = 1;
    public float TimeRemaining;
    //non-transferable data
    public int MaxStack;
    public EffectProfile(string type, float time, int add_method, int stacks = 1)
    {
        Name = type;
        Duration = time;
        CombineMethod = add_method;
        Stack = stacks;
        SetData();
    }
    public EffectProfile()
    {
        SetData();
    }

    public void SetData()
    {
        MaxStack = 0;
        switch (Name)
        {
            //some example effects
            case "Boner Juice":
                MaxStack = 6;
                break;
        }
    }
    public EffectProfile(EffectProfile pp)
    {
        Name = pp.Name;
        Duration = pp.Duration;
        CombineMethod = pp.CombineMethod;
        Stack = pp.Stack;
        TimeRemaining = pp.TimeRemaining;
        SetData();
    }

}

public class ret_cum_shenan
{
    public bool hasthing;
    public EffectProfile susser;
}