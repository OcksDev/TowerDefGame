using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transformer : StationaryTower
{
    public override void Place()
    {
        base.Place();
        GameHandler.Instance.NewTowerCreated += Dingle;
    }
    public override void Remove()
    {
        GameHandler.Instance.NewTowerCreated -= Dingle;
        base.Remove();
    }
    public void Dingle(Tower nerd)
    {
        if (Time.time <= 5f) return;
        if (nerd.TowerType == "Crossbow" || nerd.TowerType == TowerType) return;
        var ppos = nerd.transform.position;
        if ((ppos - transform.position).sqrMagnitude > Range * Range) return;
        nerd.RealRemove();
        var meme = GameHandler.Instance.SpawnTower("Crossbow").GetComponent<Tower>();
        meme.transform.position = ppos;
        meme.Level = Level;
    }

    /*
     * 
    */
}
