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
    public TextMeshProUGUI High;
    public TextMeshProUGUI TG;
    public List<Color> cols;
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
        var x = g.SelectingTower.GetCostToUpgrade(g.SelectingTower.Level);
        if (GameHandler.Scrap < x) return; // not enough moolah
        GameHandler.Scrap -= x;
        g.SelectingTower.TotalScrapInvested += x;
        g.SelectingTower.RealUpgrade();
        g.OpenInspectMenu(g.SelectingTower);
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


}
