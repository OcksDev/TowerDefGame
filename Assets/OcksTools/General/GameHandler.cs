using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance;
    public Map Map;
    public List<GameObject> AllMaps = new List<GameObject>();
    public List<Tower> AllTowers = new List<Tower>();
    // Start is called before the first frame update
    public Dictionary<string,Tower> AllTowerDict = new Dictionary<string, Tower>();
    public List<Sprite> BaseIMGS = new List<Sprite>();
    [HideInInspector]
    public List<Tower> AllActiveTowers = new List<Tower>();
    public System.Action<Tower> NewTowerCreated;
    void Awake()
    {
        Instance = this;
        foreach(var a in AllTowers)
        {
            AllTowerDict.Add(a.TowerType, a);
        }
    }
    public void ClearMap()
    {
        while (EnemyHandler.Instance.Enemies.Count > 0)
        {
            EnemyHandler.Instance.Enemies[0].Kill(false);
        }
        for(int i = 0;i < AllActiveTowers.Count;)
        {
            AllActiveTowers[0].RealRemove();
        }
    }
    public void SetMap(int balls)
    {
        var mapgm = AllMaps[balls];
        ClearMap();
        if(Map!=null) Destroy(Map.SpawnedScene);
        var winkle = Instantiate(mapgm, Vector3.zero, Quaternion.identity);
        Map = winkle.GetComponent<Map>();
    }

    public void SpawnEnemyWave()
    {
        //idk yet lol
    }
    public GameObject SpawnTower(string nerd)
    {
        return Instantiate(AllTowerDict[nerd].gameObject);
    }

    public enum ObjectTypes
    {
        Unknown,
        Tower,
        Enemy,
    }
    public static ObjectHolder GetObjectType(Collider2D Nerd, bool DoGetComp = true)
    {
        var type = new ObjectHolder();
        type.Object = Nerd.gameObject;
        switch (Nerd.gameObject.tag)
        {
            default:
                type.Type = ObjectTypes.Unknown;
                break;
            case "Tower":
                type.Type = ObjectTypes.Tower;
                if(DoGetComp)
                type.Tower = Nerd.GetComponent<Tower>();
                break;
            case "Enemy":
                type.Type = ObjectTypes.Enemy;
                break;
        }
        return type;
    }
}

public class ObjectHolder
{
    public GameObject Object;
    public GameHandler.ObjectTypes Type;
    public Tower Tower;
}