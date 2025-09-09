using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance;
    public Map Map;
    public List<GameObject> AllMaps = new List<GameObject>();
    public List<Tower> AllTowers = new List<Tower>();
    public List<Card> AllCards = new List<Card>();
    // Start is called before the first frame update
    public Dictionary<string,Tower> AllTowerDict = new Dictionary<string, Tower>();
    public Dictionary<string, Card> AllCardDict = new Dictionary<string, Card>();
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
    public Tower HoveringTower = null;
    public Loadout LocalLoadout = new Loadout();

    public TextAsset DefaultTowerFile;
    public TextAsset DefaultCardFile;

    public static long Scrap = 0;

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
        foreach(var a in AllCards)
        {
            AllCardDict.Add(a.Name, a);
        }
        Tags.refs["UpgradeMenu"].SetActive(false);
        SaveSystem.SaveAllData.Append(SaveLocalLoadout);
        SaveSystem.LoadAllData.Append(LoadLocalLoadout);
    }
    private void Start()
    {
        TowerTargetThreads = new OXThreadPoolA(TowerThreadCount);
        StartCoroutine(BananaSmeg());//dont touch
        var dd = Application.dataPath + "/OcksTools/General/Tower";
        var l = LanguageFileSystem.Instance;

        foreach(var a in AllTowers)
        {
            var x = a.LangFileIndex;
            x.FileName = a.TowerType;
            x.LanguageType = OXLanguageFileIndex.LangType.Tower;
            x.DefaultString = DefaultTowerFile.text;
            l.AddFile(x);
        }
        foreach(var a in AllCards)
        {
            var x = a.LangFileIndex;
            x.FileName = a.Name;
            x.LanguageType = OXLanguageFileIndex.LangType.Card;
            x.DefaultString = DefaultCardFile.text;
            l.AddFile(x);
        }
        //debug start code
        StartGame(NetworkState.Singleplayer);
    }

    public static Dictionary<string, bool> MenuRefs = new Dictionary<string, bool>();

    public static void SetMenuState(string a, bool b)
    {
        if (MenuRefs.ContainsKey(a))
        {
            MenuRefs[a] = b;
        }
        else
        {
            MenuRefs.Add(a, b);
        }
        Tags.refs[a].SetActive(b);
    } 
    
    public static bool GetMenuState(string a)
    {
        if (MenuRefs.ContainsKey(a))
        {
            return MenuRefs[a];
        }
        else
        {
            return false;
        }
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
        LoadoutState = 0;
        if(Map!=null) Destroy(Map.SpawnedScene);
        var winkle = Instantiate(mapgm, Vector3.zero, Quaternion.identity);
        Map = winkle.GetComponent<Map>();
        Map.BakePaths();
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
    public static int LoadoutState = 0;
    public void SetLoadoutDisplays(int state)
    {
        SmegNerd = Tags.refs["LoadoutDisplayHolder"].GetComponent<LoadoutNerds>();
        int x = 0;
        LoadoutState = state;

        if(CurrentState== PlayerState.PlacingTower)CancelPlace();
        else if(CurrentState== PlayerState.PlacingCard)CancelCard();

        foreach (var a in SmegNerd.gg)
        {
            a.gameObject.SetActive(false);
            if(a.DisplayOb != null) Destroy(a.DisplayOb);
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
                    SmegNerd.gg[x].DisplayOb = b.gameObject;
                }
                x++;
            }
        }
        else if (state == 1)
        {
            foreach (var a in LocalLoadout.Cards)
            {
                SmegNerd.gg[x].MyLoadoutIndex = x;
                if (a != "")
                {
                    SmegNerd.gg[x].gameObject.SetActive(true);
                    var b = SpawnDisplayOfCard(a);
                    b.transform.parent = SmegNerd.gg[x].transform;
                    b.transform.position = SmegNerd.gg[x].transform.position;
                    b.transform.localScale = Vector3.one * 0.6f;
                    SmegNerd.gg[x].DisplayOb = b.gameObject;
                }
                x++;
            }
        }

    }

    public GameObject SpawnTower(string nerd)
    {
        return Instantiate(AllTowerDict[nerd].gameObject);
    }
    public GameObject SpawnCard(string nerd)
    {
        return Instantiate(AllCardDict[nerd].gameObject);
    }
    public Tower PlacingTower;
    public Card PlacingCard;
    public void BeginTowerPlace(string nerd)
    {
        if (CurrentState == PlayerState.PlacingTower)
            CancelPlace();
        else if (CurrentState == PlayerState.PlacingCard)
            CancelCard();
        if (CurrentState != PlayerState.None) return;
        var x = AllTowerDict[nerd].GetCostToUpgrade(-1);
        if (Scrap < x) return;
        PlacingTower = SpawnTower(nerd).GetComponent<Tower>();
        PlacingTower.TotalScrapInvested += x;
        var d = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        d.z = 0;
        PlacingTower.transform.position = d;
        PlacingTower.IsPlacing = true;
        PlacingTower.RealUpdateRender();
        CurrentState = PlayerState.PlacingTower;
    }
    
    public void BeginCardPlace(string nerd)
    {
        if (CurrentState == PlayerState.PlacingTower)
            CancelPlace();
        else if (CurrentState == PlayerState.PlacingCard)
            CancelCard();
        if (CurrentState != PlayerState.None) return;

        var x = AllCardDict[nerd].GetCostToUpgrade(-1);
        if (Scrap < x) return;

        PlacingCard = SpawnCard(nerd).GetComponent<Card>();

        var d = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        d.z = 0;
        PlacingCard.transform.position = d;
        PlacingCard.transform.parent = Tags.refs["CardPlace"].transform;
        PlacingCard.transform.localScale = Vector3.one * 0.5f;

        CurrentState = PlayerState.PlacingCard;
    }

    public Tower SpawnDisplayOfTower(string nerd)
    {
        PlacingTower = SpawnTower(nerd).GetComponent<Tower>();
        PlacingTower.IsDisplay = true;
        PlacingTower.DisplaySandwichInit();
        return PlacingTower;
    }
    public Card SpawnDisplayOfCard(string nerd)
    {
        PlacingCard = SpawnCard(nerd).GetComponent<Card>();
        return PlacingCard;
    }

    public void EndPlace()
    {
        Destroy(PlacingTower.gameObject);
        CurrentState = PlayerState.None;
    }
    
    public void CancelPlace()
    {
        EndPlace();
    }
    
    public void EndCard()
    {
        Destroy(PlacingCard.gameObject);
        CurrentState = PlayerState.None;
    }
    public void CancelCard()
    {
        EndCard();
    }

    public static bool AllowSnapping = true;

    private void Update()
    {
        bool allow_place = true;
        HoveringTower = null;
        foreach(var a in AllActiveTowers)
        {
            if (Hover.IsHovering(a.gameObject))
            {
                HoveringTower = a;
                break;
            }
        }
        if (InputManager.IsKeyDown("close_menu"))
        {
            if (CurrentState == PlayerState.PlacingTower)
            {
                CancelPlace();
            }
            else  if (CurrentState == PlayerState.PlacingCard)
            {
                CancelCard();
            }
            else if (GetMenuState("UpgradeMenu"))
            {
                CloseInspectMenu();
            }
        }
        if (CurrentState == PlayerState.PlacingTower)
        {
            if (CurrentGameState == GameState.Game)
            {
                foreach (var a in SmegNerd.gg)
                {
                    if (Hover.IsHovering(a.gameObject))
                    {
                        allow_place = false;
                    }
                }
            }
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
        else if (CurrentState == PlayerState.PlacingCard)
        {
            if (InputManager.IsKeyDown("cancel_place"))
            {
                CancelCard();
                goto next;
            }
            var d = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            d.z = 0;
            PlacingCard.transform.position = d;
            var dd = true; //PlaceTowerConfirm(d, PlacingTower.RenderParts[0].GetComponent<BoxCollider2D>().size);
            //PlacingCard.UpdatePlaceColor(dd);
            if (dd && allow_place && InputManager.IsKeyDown("shoot") && HoveringTower != null)
            {
                HoveringTower.AddCard(PlacingCard);
                EndCard();
            }
        }
        if (CurrentState == PlayerState.None || CurrentState==PlayerState.PlacingTower || CurrentState==PlayerState.PlacingCard)
        {
            if (InputManager.IsKeyDown("cycle_loadout"))
            {
                SetLoadoutDisplays(1 - LoadoutState);
            }
            else
            {
                if(LoadoutState == 0)
                {
                    for (int i = 0; i < 6; i++)
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
                else
                {
                    for (int i = 0; i < 9; i++)
                    {
                        if (InputManager.IsKeyDown($"loadout{i}"))
                        {
                            var s = LocalLoadout.Cards[i];
                            if (s == "") continue;
                            BeginCardPlace(s);
                            break;
                        }
                    }
                }
            }
        }
        if(CurrentState == PlayerState.None)
        {
            if(HoveringTower != null && InputManager.IsKeyDown("shoot"))
            {
                OpenInspectMenu(HoveringTower);
            }
        }
    next:;
    }
    public Tower SelectingTower;
    public int selpage;
    public void OpenInspectMenu(Tower e, int page = 0)
    {
        SelectingTower = e;
        selpage = page;
        var d = Tags.refs["UpgradeMenu"].GetComponent<banana>();
        SetMenuState("UpgradeMenu", true);
        int s = -1;
        bool atmax = false;
        if (page == 0)
        {
            var a = e.GetDescription();
            d.tit.text = LanguageFileSystem.Instance.GetString(e.TowerType, "Name");
            d.dick.text = a;
            s = e.GetCostToUpgrade(e.Level);
            d.tears_of_childen.text = $"- Tier {Converter.NumToRead((1 + e.Level).ToString(), 3)} -";
            d.tears_of_childen.color = d.cols[e.Level];
            atmax = e.Level >= e.MaxLevel;
        }
        else
        {
            Card cd = e.MyCards[page - 1];
            var a = cd.GetDescription();
            d.tit.text = LanguageFileSystem.Instance.GetString(cd.Name, "Name");
            d.dick.text = a;
            s = cd.GetCostToUpgrade(cd.Level);
            d.tears_of_childen.text = $"- Tier {Converter.NumToRead((1 + cd.Level).ToString(), 3)} -";
            d.tears_of_childen.color = d.cols[cd.Level];
            atmax = cd.Level >= cd.MaxLevel;
        }

        if(s > 0 && !atmax)
        {
            d.UpG.text = $"Upgrade ({s})";
            d.UpG.transform.parent.GetComponent<Button>().interactable = true;
        }
        else
        {
            d.UpG.text = $"Upgrade (MAX)";
            d.UpG.transform.parent.GetComponent<Button>().interactable = false;
        }
        d.SellG.text = $"Sell ({e.TotalScrapInvested/2})";
        d.tears_of_childen2.sprite = BaseIMGS[e.Level];
        d.ShowSlots(e);
        d.ColorByPage(page);

        foreach(var ddd in d.aaaaaaa)
        {
            ddd.SetLayoutVertical();
        }

        UpdateThinalongs();
    }

    public void UpdateThinalongs()
    {
        var d = Tags.refs["UpgradeMenu"].GetComponent<banana>();
        d.High.text = SelectingTower.TargetState[0] == 0 ? "Highest" : "Lowest";
        switch (SelectingTower.TargetState[1])
        {
            case 0:
                d.TG.text = "Progress";
                break;
            case 1:
                d.TG.text = "Distance";
                break;
            case 2:
                d.TG.text = "Movement Speed";
                break;
            case 3:
                d.TG.text = "Max Health";
                break;
        }
    }

    public void CloseInspectMenu()
    {
        SetMenuState("UpgradeMenu", false);
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
        PlacingCard,
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
    public List<string> Cards;

    public Loadout()
    {
        Towers = new List<string>(new string[6]);    
        Cards = new List<string>(new string[9]);    
    }


    public string LoadoutToString()
    {
        Dictionary<string,string> d = new Dictionary<string,string>();
        d.Add("T", Converter.ListToString(Towers));
        d.Add("G", Converter.ListToString(Cards));
        return Converter.DictionaryToString(d);
    }
    public void StringToLoadout(string s)
    {
        var d = Converter.StringToDictionary(s);
        Towers = Converter.StringToList(d["T"]);
        Cards = Converter.StringToList(d["G"]);
    }
}
