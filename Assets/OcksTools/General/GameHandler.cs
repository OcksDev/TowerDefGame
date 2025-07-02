using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance;
    public Map Map;
    public List<Map> AllMaps = new List<Map>();
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }
    public void ClearMap()
    {
        while (EnemyHandler.Instance.Enemies.Count > 0)
        {
            EnemyHandler.Instance.Enemies[0].Kill(false);
        }
    }
    public void SetMap(Map map)
    {
        ClearMap();
        if(Map!=null) Destroy(Map.SpawnedScene);
        Map = map;
        Instantiate(Map.Prefab, Vector3.zero, Quaternion.identity);
    }

    public void SpawnEnemyWave()
    {
        //idk yet lol
    }

}
