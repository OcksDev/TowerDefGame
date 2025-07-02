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

}
