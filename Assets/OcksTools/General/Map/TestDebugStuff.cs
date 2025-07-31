using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDebugStuff : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPeriod))
        {
            EnemyHandler.Instance.SpawnEnemy("Nerd");
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            StartCoroutine(Spawn(5));
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            StartCoroutine(Spawn(15));
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            var a = GameHandler.Instance.SpawnDisplayOfTower("Crossbow");
            a.transform.position = Tags.refs["Canvas_Bottom"].transform.position;
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            string tg = "Crossbow";
            var g = GameHandler.Instance;
            if (g.CurrentState == GameHandler.PlayerState.PlacingTower)
            {
                tg = g.AllTowers[(g.AllTowers.IndexOf(g.AllTowerDict[g.PlacingTower.TowerType]) + 1) % g.AllTowers.Count].TowerType;
                Destroy(g.PlacingTower.gameObject);
            }
            g.CurrentState = GameHandler.PlayerState.None;
            g.BeginTowerPlace(tg);
        }
    }
    public IEnumerator Spawn(int x)
    {
        for(int i = 0; i < x; i++)
        {
            yield return new WaitForSeconds(0.1f);
            EnemyHandler.Instance.SpawnEnemy("Nerd");
        }
    }

}
