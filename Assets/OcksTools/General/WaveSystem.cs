using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    public static string CurrentKing = "";
    public TextAsset WaveFile;
    public static WaveSystem Instance;
    public double GetHealthCreditForWave(int x)
    {
        double y = System.Math.Pow(1.15, Mathf.Clamp(x, 0, 15));
        y += Mathf.Clamp(x - 15, 0, 30);

        return x * 100;
    }

    public Dictionary<WaveNode.WaveNodeType, List<string>> bananas = new Dictionary<WaveNode.WaveNodeType, List<string>>();
    private void Awake()
    {
        Instance = this;

        bananas = new Dictionary<WaveNode.WaveNodeType, List<string>>()
        {
            {WaveNode.WaveNodeType.Wave,new List<string>(){ "" } },
            {WaveNode.WaveNodeType.End,new List<string>(){ "" } },
            {WaveNode.WaveNodeType.Spawn,new List<string>(){ "aa", "0.5" } },
            {WaveNode.WaveNodeType.DSpawn,new List<string>(){ "aa", "0.5" } },
            {WaveNode.WaveNodeType.Wait,new List<string>(){ "" } },
        };

        var e = Converter.StringToList(WaveFile.text, System.Environment.NewLine);
        WaveSet w = null;
        WaveNode n = null;
        foreach(var a in e)
        {
            if (a.Length <= 2)
            {
                continue;
            }
            var b = Converter.StringToList(a, " ");
            if (b.Count <= 0) continue;

            Enum.TryParse(b[0], out WaveNode.WaveNodeType myStatus);

            switch (myStatus)
            {
                case WaveNode.WaveNodeType.Wave:
                    w = new WaveSet(int.Parse(b[1]));
                    break;
                case WaveNode.WaveNodeType.Spawn:
                    b.RemoveAt(0);
                    n = new WaveNode();
                    n.Type = WaveNode.WaveNodeType.Spawn;
                    n.SetData(b);
                    w.Nodes.Add(n);
                    break;
                case WaveNode.WaveNodeType.DSpawn:
                    b.RemoveAt(0);
                    n = new WaveNode();
                    n.Type = WaveNode.WaveNodeType.DSpawn;
                    n.SetData(b);
                    w.Nodes.Add(n);
                    break;
                case WaveNode.WaveNodeType.Wait:
                    b.RemoveAt(0);
                    n = new WaveNode();
                    n.Type = WaveNode.WaveNodeType.Wait;
                    n.SetData(b);
                    w.Nodes.Add(n);
                    break;
                case WaveNode.WaveNodeType.End:
                    w.CalculateStats();
                    WaveDict.Add(w.WaveNum, w);
                    break;
            }
        }
    }

    public Dictionary<int, WaveSet> WaveDict = new Dictionary<int, WaveSet>();


    public void StartWave(int wave)
    {
        StartCoroutine(ParseWave(wave));
    }

    public IEnumerator ParseWave(int wave)
    {
        var Amnt = GetHealthCreditForWave(wave);
        var Set = WaveDict[wave];
        var PerAmnt = Amnt / Set.AmountOfSpawnCalls;
        foreach (var a in Set.Nodes)
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
                    if(a.Type == WaveNode.WaveNodeType.Spawn)
                    {
                        for (int i = 0; i < dd; i++)
                        {
                            EnemyHandler.Instance.SpawnEnemy(gg);
                            yield return new WaitForSeconds(float.Parse(a.Data[1]));
                        }
                    }
                    else
                    {
                        StartCoroutine(DifferedSpawn(dd, gg, a));
                    }
                    break;
            }
        }
    }
    public IEnumerator DifferedSpawn(int dd, EnemyData gg, WaveNode a)
    {
        for (int i = 0; i < dd; i++)
        {
            EnemyHandler.Instance.SpawnEnemy(gg);
            yield return new WaitForSeconds(float.Parse(a.Data[1]));
        }
    }

    public EnemyData GetEnemyForValue(double allocation, string? tt = null)
    {
        if (tt == null) tt = CurrentKing;
        var k = EnemyHandler.Instance.KingDict[tt];
        EnemyData big = null;
        double h = double.MinValue;
        foreach(var a in k.enemyDatas)
        {
            var x = a.CalcCreditCost(allocation);
            if(x > h && x <= allocation)
            {
                h = x;
                big = a;
            }
        }
        if (big == null)
        {
            if(tt == "Blank")
            {
                throw new System.Exception("No Enemy To Pull!");
            }
            else
            {
                return GetEnemyForValue(allocation, "Blank");
            }
        }
        return big;
    }
}

public class WaveSet
{
    public List<WaveNode> Nodes = new List<WaveNode>();
    public int AmountOfSpawnCalls = 0;
    public int AmountOfEnemsToSpawn = 0;
    public int WaveNum = -1;
    public void CalculateStats()
    {
        AmountOfSpawnCalls = 0;
        AmountOfEnemsToSpawn = 0;
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
    public WaveSet(int wave)
    {
        WaveNum = wave;
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

    public void SetData(List<string> d)
    {
        Data = new List<string>(WaveSystem.Instance.bananas[Type]);
        int x = -1;
        foreach(var a in d)
        {
            x++;
            if (a == "" || a == " ") continue;
            Data[x] = a;
        }
    }
    public void DebugPrint()
    {
        string a = Type.ToString() + $" [{Converter.ListToString(Data)}]";
        Debug.Log(a);
    }
}

