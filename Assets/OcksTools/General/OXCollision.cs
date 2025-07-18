using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OXCollision : MonoBehaviour
{
    public static List<Enemy> CircleCastAll(Vector3 startpos, float radius)
    {
        List<Enemy> hits = new List<Enemy>();
        foreach(var a in EnemyHandler.Instance.Enemies)
        {
            if(CircleCastSingle(startpos,radius,a)) hits.Add(a);
        }
        return hits;
    }
    public static bool CircleCastSingle(Vector3 startpos, float radius, Enemy enem)
    {
        var dir = (startpos - enem.Object.position);
        var x = radius + enem.Radius;
        return dir.sqrMagnitude <= x * x;
    }
    public static List<Enemy> LineCastAll(Vector3 startpos, Vector3 line_relative, float radius)
    {
        List<Enemy> hits = new List<Enemy>();
        foreach (var a in EnemyHandler.Instance.Enemies)
        {
            if (LineCastSingle(startpos, line_relative, radius, a)) hits.Add(a);
        }
        return hits;
    }
    public static bool LineCastSingle(Vector3 startpos, Vector3 line_relative, float radius, Enemy enem)
    {
        var dir = (enem.Object.position - startpos);
        var dot = Mathf.Clamp01(Vector3.Dot(line_relative.normalized, dir.normalized) * (dir.magnitude/line_relative.magnitude));
        return CircleCastSingle(startpos + (dot * line_relative), radius, enem);
    }


    public static List<Enemy> SquareCastAll(Vector3 startpos, Vector2 box_size, Enemy enem)
    {
        List<Enemy> hits = new List<Enemy>();
        foreach (var a in EnemyHandler.Instance.Enemies)
        {
            if (SquareCastSingle(startpos, box_size, a)) hits.Add(a);
        }
        return hits;
    }

    public static List<Enemy> SquareCastAll(Vector3 startpos, Vector2 box_size, Quaternion rotation, Enemy enem)
    {
        List<Enemy> hits = new List<Enemy>();
        foreach (var a in EnemyHandler.Instance.Enemies)
        {
            if (SquareCastSingle(startpos, box_size, rotation, a)) hits.Add(a);
        }
        return hits;
    }
    private static bool LineCastSingleSQ(Vector3 startpos, Vector3 line_relative, float radius, Enemy enem, Vector3 v)
    {
        var dir = (v - startpos);
        var dot = Mathf.Clamp(Vector3.Dot(line_relative.normalized, dir.normalized) * (dir.magnitude / line_relative.magnitude), -1,1);
        return CircleCastSingleSQ(startpos + (dot * line_relative), radius, enem,v);
    }

    private static bool CircleCastSingleSQ(Vector3 startpos, float radius, Enemy enem, Vector3 v)
    {
        var dir = (startpos - v);
        var x = radius + enem.Radius;
        return dir.sqrMagnitude <= x * x;
    }
    public static bool SquareCastSingle(Vector3 startpos, Vector2 box_size, Enemy enem)
    {
        return SquareCastSingle(startpos, box_size, Quaternion.identity, enem);
    }
    public static bool SquareCastSingle(Vector3 startpos, Vector2 box_size, Quaternion rotation, Enemy enem)
    {
        var v = enem.Object.position - startpos;
        v = Quaternion.Inverse(rotation) * v;
        if (v.x < 0) v.x *= -1;
        if (v.y < 0) v.y *= -1;
        var new_global = v + startpos;

        if (v.x < box_size.x / 2 && v.y < box_size.y / 2) return true;

        var a = startpos + (Vector3.right * box_size.x / 2);
        bool one = LineCastSingleSQ(a, Vector3.up * box_size.y / 2, 0, enem,new_global);
        a = startpos + (Vector3.up * box_size.y / 2);
        bool two = LineCastSingleSQ(a, Vector3.right * box_size.x / 2, 0, enem, new_global);
  
        return one || two;
    }

}
