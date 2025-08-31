using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class banana : MonoBehaviour
{
    public TextMeshProUGUI tit;
    public TextMeshProUGUI dick;
    public TextMeshProUGUI tears_of_childen;
    public TextMeshProUGUI UpG;
    public TextMeshProUGUI SellG;
    public List<Color> cols;

    public void Upg()
    {

        var g = GameHandler.Instance;
        var x = g.SelectingTower.GetCostToUpgrade(g.SelectingTower.Level);
        if (GameHandler.Scrap < x) return; // not enough moolah
        GameHandler.Scrap -= x;
        g.SelectingTower.TotalScrapInvested += x;
        g.SelectingTower.RealUpgrade();
        g.OpenInspectMenu(g.SelectingTower);
    }
    public void Usell()
    {

    }

}
