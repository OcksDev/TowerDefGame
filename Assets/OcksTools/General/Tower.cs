using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public string TowerType = "";
    public float Range = 5;
    public int Level = 0;
    public List<Transform> Parts = new List<Transform>();
    public List<SpriteRenderer> RenderParts = new List<SpriteRenderer>();
    public List<Sprite> TowerIMGS = new List<Sprite>();
    public Target TargetType;
    public Enemy EnemyTarget;
    public int MaxLevel = 15;
    public List<Sprite> BaseIMGS = new List<Sprite>();
    private void FixedUpdate()
    {
        EnemyTarget = GetTarget();
        if(EnemyTarget != null)
        {
            Parts[0].rotation = RandomFunctions.PointAtPoint2D(Parts[0].position, EnemyTarget.Object.position, 0);
        }
    }
    private void Start() // DEBUG CODE DELETE LATER
    {
        Place();
    }
    public void Place()
    {
        UpdateRender();
    }

    public void Upgrade()
    {
        Level = Mathf.Clamp(Level + 1, 0, MaxLevel);
        UpdateRender();
    }

    public Enemy GetTarget()
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
    public void UpdateRender()
    {
        switch (TowerType)
        {
            default:
                if (Level < 5)
                {
                    RenderParts[0].sprite = BaseIMGS[0];
                }
                else if (Level < 10)
                {

                    RenderParts[0].sprite = BaseIMGS[1];
                }
                else if(Level < 15)
                {
                    RenderParts[0].sprite = BaseIMGS[2];
                }
                else
                {
                    RenderParts[0].sprite = BaseIMGS[3];
                }
                break;
        }
        switch (TowerType)
        {
            default:
                if (Level < 5)
                {
                    RenderParts[1].sprite = TowerIMGS[0];
                }
                else if (Level < 10)
                {

                    RenderParts[1].sprite = TowerIMGS[1];
                }
                else if(Level < 15)
                {
                    RenderParts[1].sprite = TowerIMGS[2];
                }
                else
                {
                    RenderParts[1].sprite = TowerIMGS[3];
                }
                break;
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

}
