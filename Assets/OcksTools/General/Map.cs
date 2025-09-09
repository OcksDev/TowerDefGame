using com.cyborgAssets.inspectorButtonPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [ProButton]
    public void TestButton()
    {
        Debug.Log("AAA");
    }
    public MapNode StartNode;
    public List<MapNode> Nodes = new List<MapNode>();
    public List<Vector3> Poses = new List<Vector3>();
    public GameObject Prefab;
    public GameObject SpawnedScene;


    public int GetNextIndex(int curindex)
    {
        if (Nodes[curindex].NextNodes.Count == 0) curindex++;
        else
        {
            int x = curindex;
            curindex = Nodes[curindex].NextNodes[Nodes[curindex].index];
            Nodes[x].index = RandomFunctions.Mod(Nodes[x].index + 1, Nodes[x].NextNodes.Count);
        }


        if (curindex >= Nodes.Count || curindex == -1) return -1;
        return curindex;
    }
    public int GetSpawnIndex()
    {
        if (StartNode.NextNodes.Count == 0) return 0;
        else
        {
            int curindex = 0;
            curindex = StartNode.NextNodes[StartNode.index];
            StartNode.index = RandomFunctions.Mod(StartNode.index + 1, StartNode.NextNodes.Count);
            return curindex;
        }


    }
    public void Update()
    {
        for(int i =0; i < Nodes.Count;i++)
        {
            if (i <= Poses.Count) Poses.Add(Nodes[i].Node.position);
            else Poses[i] = Nodes[i].Node.position;
        }
    }
}
[System.Serializable]
public class MapNode
{
    public Transform Node;
    public List<int> NextNodes = new List<int>();
    [HideInInspector]
    public int index = 0;
}
