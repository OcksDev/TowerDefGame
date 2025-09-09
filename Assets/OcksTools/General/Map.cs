using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
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

    int jj = 0;

    public OXPath GetNextPath()
    {
        var a = Pathways[jj];
        jj++;
        jj %= Pathways.Count;
        return a;
    }
    public void Update()
    {
        for(int i =0; i < Nodes.Count;i++)
        {
            if (i <= Poses.Count) Poses.Add(Nodes[i].Node.position);
            else Poses[i] = Nodes[i].Node.position;
        }
    }

    public List<OXPath> Pathways = new List<OXPath>();
    private HashSet<string> already_traversed = new HashSet<string>();
    public OXPath ShortestPath = null;
    [Button]
    public void BakePaths()
    {
        Pathways.Clear();
        already_traversed.Clear();
        Debug.Log("Starting Bake...");
        Recurse("", StartNode, new List<MapNode>());
        foreach(var a in Pathways)
        {

            string s = "";
            foreach(var b in a.Positions)
            {
                s += $"\n{b}";
            }
            Debug.Log("Pathway Found: " + a.total_dist+ s);
        }
    }

    private void Recurse(string hash, MapNode self, List<MapNode> my_path)
    {
        //if (already_traversed.Contains(hash)) return;
        if (self.NextNodes.Count == 0)
        {
            //already_traversed.Add(hash);
            my_path.Add(self);
            var nw = new OXPath();
            nw.AutoEndConnect = false;
            int x = 0;
            foreach(var node in my_path)
            {
                x++;
                if (x <= 1) continue;
                nw.Positions.Add(node.Node.position);
            }
            nw.CalculateStats();
            if (ShortestPath == null || nw.total_dist <= ShortestPath.total_dist) ShortestPath = nw;
            Pathways.Add(nw);
        }
        else
        {
            foreach (var a in self.NextNodes)
            {
                var dd = new List<MapNode>(my_path);
                dd.Add(self);
                Recurse($"{hash}{a}", Nodes[a], dd);
            }
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
