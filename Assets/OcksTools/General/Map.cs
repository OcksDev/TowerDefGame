using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class Map : MonoBehaviour
{
    public MapNode StartNode;
    public List<MapNode> Nodes = new List<MapNode>();
    public List<Vector3> Poses = new List<Vector3>();
    public GameObject SpawnedScene;
    public GameObject HitBoxHolder;
    public GameObject VisHolder;
    public Sprite VisSpriteDefault;
    public Sprite VisCornerSpriteDefault;
    public GameObject HitBoxPrefab;
    public float PathWidth = 1f;
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
    public OXPath ShortestPath = null;
    [Button]
    public void BakePaths()
    {
        Pathways.Clear();
        Debug.Log("Starting Bake Paths...");
        Recurse(StartNode, new List<MapNode>());
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

    private void Recurse(MapNode self, List<MapNode> my_path)
    {
        //if (already_traversed.Contains(hash)) return;
        if (self.NextNodes.Count == 0)
        {
            //already_traversed.Add(hash);
            my_path.Add(self);
            var nw = new OXPath();
            nw.AutoEndConnect = false;
            int x = 0;
            foreach (var node in my_path)
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
                Recurse(Nodes[a], dd);
            }
        }
    }
    [Button]
    public void ClearVis()
    {
        var c = VisHolder.GetComponentsInChildren<Transform>();
        foreach (var b in c)
        {
            if (b != VisHolder.transform) DestroyImmediate(b.gameObject);
        }
    }

    [Button]
    public void BakeVis()
    {
        Traversed.Clear();

        ClearVis();

        Debug.Log("Starting Bake Vis...");
        foreach (var a in Nodes)
        {
            var b = Instantiate(HitBoxPrefab, a.Node.position, Quaternion.identity, VisHolder.transform);
            var x = b.AddComponent<SpriteRenderer>();
            x.transform.localScale = Vector3.one* PathWidth;
            x.sprite = VisCornerSpriteDefault;
            x.sortingOrder = -5;
        }

        RecurseVis(StartNode, -1);

    }
    private void RecurseVis(MapNode self, int selfindex)
    {
        foreach(var a in self.NextNodes)
        {
            var vec = new Vector2Int(selfindex, a);
            if (Traversed.Contains(vec)) continue;//collider already made
            var next = Nodes[a];
            if (self == StartNode) goto banana;
            Traversed.Add(vec);
            var b = Instantiate(HitBoxPrefab, Vector3.Lerp(self.Node.position, next.Node.position, 0.5f), RandomFunctions.PointAtPoint2D(self.Node.position, next.Node.position, 0), VisHolder.transform);
            var x = b.AddComponent<SpriteRenderer>();
            x.transform.localScale = new Vector2(RandomFunctions.Dist(self.Node.position, next.Node.position), PathWidth);
            x.sprite = VisSpriteDefault;
            x.sortingOrder = -5;
        banana:
            RecurseVis(next, a);
        }
    }
    [Button]
    public void ClearColliders()
    {
        var c = HitBoxHolder.GetComponentsInChildren<Transform>();
        foreach (var b in c)
        {
            if (b != HitBoxHolder.transform) DestroyImmediate(b.gameObject);
        }
    }

    [Button]
    public void BakeColliders()
    {
        Traversed.Clear();

        ClearColliders();

        Debug.Log("Starting Bake Colliders...");
        foreach (var a in Nodes)
        {
            var b = Instantiate(HitBoxPrefab, a.Node.position, Quaternion.identity, HitBoxHolder.transform);
            var x = b.AddComponent<CircleCollider2D>();
            x.radius = PathWidth / 2;
            x.isTrigger = true;
        }

        RecurseCollider(StartNode, -1);

    }
    private HashSet<Vector2Int> Traversed = new HashSet<Vector2Int>();
    private void RecurseCollider(MapNode self, int selfindex)
    {
        foreach (var a in self.NextNodes)
        {
            var vec = new Vector2Int(selfindex, a);
            if (Traversed.Contains(vec)) continue;//collider already made
            var next = Nodes[a];
            if (self == StartNode) goto banana;
            Traversed.Add(vec);
            var b = Instantiate(HitBoxPrefab, Vector3.Lerp(self.Node.position, next.Node.position, 0.5f), RandomFunctions.PointAtPoint2D(self.Node.position, next.Node.position, 0), HitBoxHolder.transform);
            var x = b.AddComponent<BoxCollider2D>();
            x.size = new Vector2(RandomFunctions.Dist(self.Node.position, next.Node.position), PathWidth);
            x.isTrigger = true;
        banana:
            RecurseCollider(next, a);
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
