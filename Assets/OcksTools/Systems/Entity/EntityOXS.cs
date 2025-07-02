using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityOXS : MonoBehaviour
{
    public EntityType Type = EntityType.Enemy;
    public double Health = 100;
    public double Shield = 100;
    public double Max_Health = 100;
    public double Max_Shield = 100; 
    public List<EffectProfile> Effects = new List<EffectProfile>();
    public void Hit(DamageProfile hit)
    {
        var dmg = hit.Damage;
        foreach (var effect in hit.Effects)
        {
            AddEffect(effect);
        }
        Shield -= dmg;
        if (Shield < 0)
        {
            Health += Shield;
        }
        Shield = System.Math.Clamp(Shield, 0, Max_Shield);
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

    public void Kill()
    {
        Destroy(gameObject);
    }
    private void Update()
    {
        Health = System.Math.Clamp(Health, 0, Max_Health);
        Shield = System.Math.Clamp(Shield, 0, Max_Shield);
        for(int i = 0; i < Effects.Count; i++)
        {
            Effects[i].TimeRemaining -= Time.deltaTime;
            if (Effects[i].TimeRemaining <= 0)
            {
                Effects.RemoveAt(i);
                i--;
            }
        }
        if (Health <= 0)
        {
            Kill();
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

    public enum EntityType
    {
        Enemy,
        Player,
        NPC,
        World,
    }

}

