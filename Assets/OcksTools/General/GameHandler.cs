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
    public List<Gem> AllGems = new List<Gem>();
    // Start is called before the first frame update
    public Dictionary<string,Tower> AllTowerDict = new Dictionary<string, Tower>();
    public Dictionary<string, Gem> AllGemDict = new Dictionary<string, Gem>();
    public List<Sprite> BaseIMGS = new List<Sprite>();
    public List<Material> BaseMats = new List<Material>();
    public List<Color> BaseColors = new List<Color>();
    [HideInInspector]
    public List<Tower> AllActiveTowers = new List<Tower>();
    public System.Action<Tower> NewTowerCreated;
    public PlayerState CurrentState= PlayerState.None;
    public GameState CurrentGameState = GameState.MainMenu;
    public NetworkState CurrentMultiplayerState = NetworkState.Singleplayer;
    public OXThreadPoolA TowerTargetThreads;
    public static int TowerThreadCount = 20;

    public Loadout LocalLoadout = new Loadout();

    void Awake()
    {
#if !UNITY_EDITOR
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
#endif
        Instance = this;
        foreach(var a in AllTowers)
        {
            AllTowerDict.Add(a.TowerType, a);
        }
        foreach(var a in AllGems)
        {
            AllGemDict.Add(a.Name, a);
        }
        SaveSystem.SaveAllData.Append(SaveLocalLoadout);
        SaveSystem.LoadAllData.Append(LoadLocalLoadout);
    }
    private void Start()
    {
        TowerTargetThreads = new OXThreadPoolA(TowerThreadCount);
        StartCoroutine(BananaSmeg());//dont touch



        //debug start code
        StartGame(NetworkState.Singleplayer);
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
        SpawnLoadoutDisplays();
        if(Map!=null) Destroy(Map.SpawnedScene);
        var winkle = Instantiate(mapgm, Vector3.zero, Quaternion.identity);
        Map = winkle.GetComponent<Map>();
        CurrentGameState = GameState.Game;
    }
    public GameObject Player;
    public void StartGame(NetworkState state = NetworkState.Singleplayer)
    {
        if(PlayerController.Instance != null)
        {
            Destroy(PlayerController.Instance.gameObject);
            Console.Log("A");
        }
        WaveSystem.CurrentKing = "Blank";
        CurrentMultiplayerState = state;
        if(state == NetworkState.Singleplayer)
        {
            Instantiate(Player);
        }
    }
    public static List<PlayerController> Players = new List<PlayerController>();
    public static PlayerController GetPlayer(ulong i)
    {
        foreach(var a in Players)
        {
            if(a.ID==i) return a;
        }
        return null;
    }

    public void HostGame()
    {
        if(PlayerController.Instance != null)
        {
            Destroy(PlayerController.Instance.gameObject);
            Console.Log("A");
        }
        StartGame(NetworkState.Multiplayer);
        RelayMoment.Instance.GetComponent<PickThingymabob>().MakeGame();
    }
    private LoadoutNerds SmegNerd = null;
    public void SpawnLoadoutDisplays()
    {
        SetLoadoutDisplays(0);
    }

    public void SetLoadoutDisplays(int state)
    {
        SmegNerd = Tags.refs["LoadoutDisplayHolder"].GetComponent<LoadoutNerds>();
        int x = 0;

        foreach (var a in SmegNerd.gg)
        {
            a.gameObject.SetActive(false);
        }
        if (state == 0)
        {
            foreach (var a in LocalLoadout.Towers)
            {
                SmegNerd.gg[x].MyLoadoutIndex = x;
                if (a != "" && a != " " && a != null)
                {
                    SmegNerd.gg[x].gameObject.SetActive(true);
                    var b = SpawnDisplayOfTower(a);
                    b.transform.parent = SmegNerd.gg[x].transform;
                    b.transform.position = SmegNerd.gg[x].transform.position + b.UIDisplayOffset;
                    b.transform.localScale *= 0.75f * b.UIDisplayScaleMult;
                }
                x++;
            }
        }
        if (state == 1)
        {
            foreach (var a in LocalLoadout.Towers)
            {
                SmegNerd.gg[x].MyLoadoutIndex = x;
                if (a != "")
                {
                    SmegNerd.gg[x].gameObject.SetActive(true);
                    /*var b = SpawnDisplayOfCard(a);
                    b.transform.parent = SmegNerd.gg[x].transform;
                    b.transform.position = SmegNerd.gg[x].transform.position + b.UIDisplayOffset;*/
                }
                x++;
            }
        }

    }

    public GameObject SpawnTower(string nerd)
    {
        return Instantiate(AllTowerDict[nerd].gameObject);
    }
    public Tower PlacingTower;
    public void BeginTowerPlace(string nerd)
    {
        if (CurrentState == PlayerState.PlacingTower)
            CancelPlace();
        if (CurrentState != PlayerState.None) return;
        PlacingTower = SpawnTower(nerd).GetComponent<Tower>();
        var d = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        d.z = 0;
        PlacingTower.transform.position = d;
        PlacingTower.IsPlacing = true;
        PlacingTower.RealUpdateRender();
        CurrentState = PlayerState.PlacingTower;
    }

    public Tower SpawnDisplayOfTower(string nerd)
    {
        PlacingTower = SpawnTower(nerd).GetComponent<Tower>();
        PlacingTower.IsDisplay = true;
        PlacingTower.DisplaySandwichInit();
        return PlacingTower;
    }

    public void CancelPlace()
    {
        Destroy(PlacingTower.gameObject);
        CurrentState = PlayerState.None;
    }

    public static bool AllowSnapping = true;

    private void Update()
    {
        bool allow_place = true;
        if (CurrentGameState == GameState.Game)
        {
            foreach(var a in SmegNerd.gg)
            {
                if (Hover.IsHovering(a.gameObject))
                {
                    allow_place = false;
                }
            }
        }
        if (CurrentState == PlayerState.PlacingTower)
        {
            if (InputManager.IsKeyDown("cancel_place"))
            {
                CancelPlace();
                goto next;
            }
            var d = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            d.z = 0;

            if (AllowSnapping)
            {
                Tower t = null;
                float x = float.MaxValue;
                foreach(var a in AllActiveTowers)
                {
                    if((a.transform.position-d).sqrMagnitude < x)
                    {
                        x = (a.transform.position - d).sqrMagnitude;
                        t = a;
                    }
                }
                if (t != null)
                {
                    var init = (t.transform.position - d);
                    int x_f = -1;
                    int y_f = -1;
                    float x_o = 0;
                    float y_o = 0;
                    if(init.sqrMagnitude <= 4)
                    {
                        float outer = 0.75f;
                        if(init.x < 0)
                        {
                            x_f *= -1;
                            init.x *= -1;
                        }
                        if(init.y < 0)
                        {
                            y_f *= -1;
                            init.y *= -1;
                        }

                        if(init.x < outer + 0.5f && init.y < outer + 0.5f)
                        {
                            if(init.x > 0.5f)
                            {
                                x_o = 1f;
                            }
                            else
                            {
                                x_o = 0;
                            }
                            if(init.y > 0.5f)
                            {
                                y_o = 1f;
                            }
                            else
                            {
                                y_o = 0;
                            }
                            d = new Vector3(x_o * x_f, y_o * y_f,0) + t.transform.position;
                        }
                    }
                }
            }

            PlacingTower.transform.position = d;
            var dd = PlaceTowerConfirm(d, PlacingTower.RenderParts[0].GetComponent<BoxCollider2D>().size);
            PlacingTower.UpdatePlaceColor(dd);
            if (dd && allow_place && InputManager.IsKeyDown("shoot"))
            {
                PlacingTower.RealPlace();
                CurrentState = PlayerState.None;
            }
        }
        if(CurrentState == PlayerState.None || CurrentState==PlayerState.PlacingTower)
        {
            for (int i = 0; i < 9; i++)
            {
                if (InputManager.IsKeyDown($"loadout{i}"))
                {
                    var s = LocalLoadout.Towers[i];
                    if (s == "") continue;
                    BeginTowerPlace(s);
                    break;
                }
            }
        }
    next:;
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
    
    public enum GameState
    {
        MainMenu,
        Game,
    }
    
    public enum NetworkState
    {
        Singleplayer,
        Multiplayer,
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
        else
        {
            //default loadout setting
            LocalLoadout.Towers = new List<string> { "Crossbow", "Sniper", "Rocket", "", "", "" };
        }
    }

}

public class ObjectHolder
{
    public GameObject Object;
    public GameHandler.ObjectTypes Type;
    public Tower Tower;
}

[System.Serializable]
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