using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance;
    public Map Map;
    public List<GameObject> AllMaps = new List<GameObject>();
    public List<Tower> AllTowers = new List<Tower>();
    // Start is called before the first frame update
    public Dictionary<string,Tower> AllTowerDict = new Dictionary<string, Tower>();
    public List<Sprite> BaseIMGS = new List<Sprite>();
    public List<Material> BaseMats = new List<Material>();
    public List<Color> BaseColors = new List<Color>();
    [HideInInspector]
    public List<Tower> AllActiveTowers = new List<Tower>();
    public System.Action<Tower> NewTowerCreated;
    public PlayerState CurrentState= PlayerState.None;
    public OXThreadPoolA TowerTargetThreads;
    public static int TowerThreadCount = 20;

    public Loadout LocalLoadout = new Loadout();

    void Awake()
    {
        Instance = this;
        foreach(var a in AllTowers)
        {
            AllTowerDict.Add(a.TowerType, a);
        }
        SaveSystem.SaveAllData.Append(SaveLocalLoadout);
        SaveSystem.LoadAllData.Append(SaveLocalLoadout);
    }
    private void Start()
    {
        TowerTargetThreads = new OXThreadPoolA(TowerThreadCount);
        StartCoroutine(BananaSmeg());
    }
    public IEnumerator BananaSmeg()
    {
        MultiThreaderEnsure.Instance.StartCoroutine(MultiThreaderEnsure.Instance.FixSlackers(TowerTargetThreads));
        yield return new WaitUntil(() => { return TowerTargetThreads.allconfirmed; });
        for (int i = 0; i < TowerThreadCount; i++)
        {
            TowerTargetThreads.Add(Towerrargettr);
        }
    }
    static int bongle = 0;
    public void Towerrargettr()
    {
        int x = bongle++;
        while (true)
        {
            for(int i = x; i < AllActiveTowers.Count; i += TowerThreadCount)
            {
                var t = AllActiveTowers[i];
                if (t == null) continue;

                t.TargetHandover = t.GetTarget(t.DesiredTargetCount,t.TargetType);
            }
            Thread.Sleep(1);
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
    public Tower PlacingTower;
    public void BeginTowerPlace(string nerd)
    {
        if (CurrentState != PlayerState.None) return;
        PlacingTower = SpawnTower(nerd).GetComponent<Tower>();
        var d = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        d.z = 0;
        PlacingTower.transform.position = d;
        PlacingTower.IsPlacing = true;
        PlacingTower.RealUpdateRender();
        CurrentState = PlayerState.PlacingTower;
        PlacingTower.UpdatePlaceColor(PlaceTowerConfirm(d, PlacingTower.RenderParts[0].GetComponent<BoxCollider2D>().size));
    }

    public Tower SpawnDisplayOfTower(string nerd)
    {
        PlacingTower = SpawnTower(nerd).GetComponent<Tower>();
        PlacingTower.IsDisplay = true;
        PlacingTower.DisplaySandwichInit();
        return PlacingTower;
    }


    private void Update()
    {
        if (CurrentState == PlayerState.PlacingTower)
        {
            var d = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            d.z = 0;
            PlacingTower.transform.position = d;
            var dd = PlaceTowerConfirm(d, PlacingTower.RenderParts[0].GetComponent<BoxCollider2D>().size);
            PlacingTower.UpdatePlaceColor(dd);
            if (dd && InputManager.IsKeyDown("shoot"))
            {
                PlacingTower.RealPlace();
                CurrentState = PlayerState.None;
            }
        }
    }
    public bool PlaceTowerConfirm(Vector3 pos, Vector3 size)
    {
        var a = Physics2D.OverlapBoxAll(pos, size,0);
        foreach(var b in a)
        {
            if (b.gameObject == PlacingTower.gameObject) continue;
            var d = GetObjectType(b);
            if (d.Type==ObjectTypes.Tower) return false;
        }
        return true;
    }
    public GameObject TowerUIBase;
    public GameObject TowerUIPart;
    public Transform ConvertTowerToUI(string gam, Transform parent)
    {
        var d = Instantiate(TowerUIBase, parent.transform.position, Quaternion.identity, parent).transform;
        var gamin = Instantiate(AllTowerDict[gam].gameObject).GetComponent<Tower>();
        gamin.IsPlacing = false;
        gamin.RealUpdateRender();
        List<int> orders=  new List<int>();
        foreach (var a in gamin.RenderParts)
        {
            if (!a.gameObject.activeInHierarchy) continue;
            if (!orders.Contains(a.sortingOrder))
            {
                orders.Add(a.sortingOrder);
            }
        }
        orders.Sort();
        foreach(var a in orders)
        {
            foreach (var b in gamin.RenderParts)
            {
                if (!b.gameObject.activeInHierarchy) continue;
                if (a == b.sortingOrder)
                {
                    var dd = b.transform.localPosition;
                    if (b == gamin.RenderParts[0]) { dd = Vector3.zero; }
                    var c = Instantiate(TowerUIPart, d.transform.position + dd, b.transform.rotation,d);
                    c.transform.localScale = (Vector3.one / 17 ) * b.sprite.texture.width;
                    c.GetComponent<Image>().sprite = b.sprite;
                }
            }
        }
        Destroy(gamin.gameObject);
        return d;
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


    public enum PlayerState
    {
        None,
        PlacingTower,
        InspectingTower,
    }

    public void SaveLocalLoadout(string dict)
    {
        SaveSystem.Instance.SetString("Loadout", LocalLoadout.LoadoutToString(), dict);
    }
    public void LoadLocalLoadout(string dict)
    {
        var a = SaveSystem.Instance.GetString("Loadout", "-", dict);
        LocalLoadout = new Loadout();
        if(a != "-") LocalLoadout.StringToLoadout(a);
    }

}

public class ObjectHolder
{
    public GameObject Object;
    public GameHandler.ObjectTypes Type;
    public Tower Tower;
}


public class Loadout
{
    public List<string> Towers;
    public List<string> Gems;

    public Loadout()
    {
        Towers = new List<string>(new string[6]);    
        Gems = new List<string>(new string[9]);    
    }


    public string LoadoutToString()
    {
        Dictionary<string,string> d = new Dictionary<string,string>();
        d.Add("T", Converter.ListToString(Towers));
        d.Add("G", Converter.ListToString(Gems));
        return Converter.DictionaryToString(d);
    }
    public void StringToLoadout(string s)
    {
        var d = Converter.StringToDictionary(s);
        Towers = Converter.StringToList(d["T"]);
        Gems = Converter.StringToList(d["G"]);
    }
}