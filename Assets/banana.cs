using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class banana : MonoBehaviour
{
    public TextMeshProUGUI tit;
    public TextMeshProUGUI dick;
    public TextMeshProUGUI tears_of_childen;
    public TextMeshProUGUI UpG;
    public TextMeshProUGUI SellG;
    public TextMeshProUGUI High;
    public TextMeshProUGUI TG;
    public Image tears_of_childen2;
    public List<Color> cols;
    public List<Color> cols_sel;
    public List<Image> UpgradeTabs;
    public List<ContentSizeFitter> aaaaaaa;
    private void Update()
    {
        if (InputManager.IsKeyDown("upgrade"))
        {
            Upg();
        }
        if (InputManager.IsKeyDown("sell"))
        {
            Usell();
        }
        if (InputManager.IsKeyDown("change_priority"))
        {
            Uprior();
        }
        if (InputManager.IsKeyDown("change_measurement"))
        {
            Umeas();
        }
    }
    public void Upg()
    {

        var g = GameHandler.Instance;
        Card card = null;
        int x = -1;
        if(g.selpage > 0)
        {
            card = g.SelectingTower.MyCards[g.selpage-1];
            x = card.GetCostToUpgrade(card.Level);
        }
        else
        {
            x = g.SelectingTower.GetCostToUpgrade(g.SelectingTower.Level);
        }

        if (GameHandler.Scrap < x) return; // not enough moolah
        GameHandler.Scrap -= x;
        g.SelectingTower.TotalScrapInvested += x;
        if(g.selpage > 0)
        {
            card.RealUpgrade();
        }
        else
        {
            g.SelectingTower.RealUpgrade();
        }
        g.OpenInspectMenu(g.SelectingTower, g.selpage);
    }
    public void Usell()
    {
        var g = GameHandler.Instance;
        GameHandler.Scrap += g.SelectingTower.TotalScrapInvested / 2;
        g.SelectingTower.RealRemove();
        g.CloseInspectMenu();
    }

    public void Uprior()
    {
        var g = GameHandler.Instance;
        g.SelectingTower.TargetState[0] = g.SelectingTower.TargetState[0] == 0 ? 1:0;
        g.UpdateThinalongs();
        g.SelectingTower.SetTargettingFromTargetState();
    }
    public void Umeas()
    {
        var g = GameHandler.Instance;
        g.SelectingTower.TargetState[1] = RandomFunctions.Mod(g.SelectingTower.TargetState[1]+1, 4);
        g.UpdateThinalongs();
        g.SelectingTower.SetTargettingFromTargetState();
    }
    List<GameObject> ViewNerds = new List<GameObject>();
    public void ShowSlots(Tower e)
    {
        int gemslots = e.MaxCards +1;
        foreach(var a in ViewNerds)
        {
            Destroy(a);
        }
        foreach(var a in UpgradeTabs)
        {
            a.gameObject.SetActive(false);
        }
        ViewNerds.Clear();
        for(int i = 0; i < gemslots; i++)
        {
            UpgradeTabs[i].gameObject.SetActive(true);
        }
        /*
        var cd = GameHandler.Instance.SpawnCard("empty");
        cd.transform.parent = UpgradeTabs[0].transform;
        cd.transform.position = UpgradeTabs[0].transform.position;
        cd.transform.localScale = Vector3.one * 0.5f;
        ViewNerds.Add(cd);*/
        for (int i = 1; i < gemslots; i++)
        {
            if(i-1 >= e.MyCards.Count)
            {
                var cd2 = GameHandler.Instance.SpawnCard("empty");
                cd2.transform.parent = UpgradeTabs[i].transform;
                cd2.transform.position = UpgradeTabs[i].transform.position;
                cd2.transform.localScale = Vector3.one * 0.5f;
                ViewNerds.Add(cd2);
            }
            else
            {
                var cd2 = GameHandler.Instance.SpawnCard(e.MyCards[i-1].Name);
                cd2.transform.parent = UpgradeTabs[i].transform;
                cd2.transform.position = UpgradeTabs[i].transform.position;
                cd2.transform.localScale = Vector3.one * 0.5f;
                ViewNerds.Add(cd2);
            }
        }
    }
    public void ColorByPage(int x)
    {
        for (int i = 0; i < UpgradeTabs.Count; i++)
        {
            UpgradeTabs[i].color = cols_sel[1];
        }
        UpgradeTabs[x].color = cols_sel[0];
    }
    public void AttemptPageClick(int z)
    {
        var g = GameHandler.Instance;
        var e = g.SelectingTower;
        Debug.Log("Run! " + e);   
        if (z - 1 >= e.MyCards.Count) return; //denied, no card to select lol
        Debug.Log("Passed! " + e);
        g.OpenInspectMenu(e,z);
    }
}
