using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    public static string CurrentKing = "";
    public double GetHealthCreditForWave(int x)
    {
        double y = System.Math.Pow(1.15, Mathf.Clamp(x, 0, 15));
        y += Mathf.Clamp(x - 15, 0, 30);

        return x;
    }
    public Dictionary<int, WaveSet> WaveDict = new Dictionary<int, WaveSet>();

    public IEnumerator ParseWave(int wave)
    {
        var Amnt = GetHealthCreditForWave(1);
        var Set = WaveDict[wave];
        var PerAmnt = Amnt / Set.AmountOfSpawnCalls;
        foreach(var a in Set.Nodes)
        {
            switch (a.Type)
            {
                case WaveNode.WaveNodeType.Wait:
                    yield return new WaitForSeconds(float.Parse(a.Data[0]));
                    break;
                case WaveNode.WaveNodeType.Spawn:
                case WaveNode.WaveNodeType.DSpawn:
                    var dd = int.Parse(a.Data[0]);
                    var gg = GetEnemyForValue(PerAmnt / dd);
                    for(int i = 0; i < dd; i++)
                    {
                        EnemyHandler.Instance.SpawnEnemy(gg);
                        yield return new WaitForSeconds(float.Parse(a.Data[1]));
                    }
                    break;
            }
        }
    }


    public EnemyData GetEnemyForValue(double allocation)
    {
        var k = EnemyHandler.Instance.KingDict[CurrentKing];
        EnemyData big = null;
        double h = double.MinValue;
        foreach(var a in k.enemyDatas)
        {
            var x = a.CalcCreditCost(allocation);
            if(x > h)
            {
                h = x;
                big = a;
            }
        }
        if (big == null) throw new System.Exception("No Enemy To Pull!");
        return big;
    }
}

public class WaveSet
{
    public List<WaveNode> Nodes = new List<WaveNode>();
    public int AmountOfSpawnCalls = 0;
    public int AmountOfEnemsToSpawn = 0;
    public void CalculateStats()
    {
        foreach(var a in Nodes)
        {
            switch (a.Type)
            {
                case WaveNode.WaveNodeType.Spawn:
                case WaveNode.WaveNodeType.DSpawn:
                    AmountOfSpawnCalls++;
                    AmountOfEnemsToSpawn += int.Parse(a.Data[0]);
                    break;
            }
        }
    }
}


public class WaveNode
{
    public enum WaveNodeType
    {
        Spawn,
        DSpawn,
        Wait,
        Event,
        Wave,
        End,
    }
    public List<string> Data = new List<string>();
    public WaveNodeType Type = WaveNodeType.Spawn;
}

