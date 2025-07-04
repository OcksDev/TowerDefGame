using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public MapNode StartNode;
    public List<MapNode> Nodes = new List<MapNode>();
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

}
[System.Serializable]
public class MapNode
{
    public Transform Node;
    public List<int> NextNodes = new List<int>();
    [HideInInspector]
    public int index = 0;
}
