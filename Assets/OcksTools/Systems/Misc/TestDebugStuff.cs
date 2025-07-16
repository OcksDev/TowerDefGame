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
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            var a = GameHandler.Instance.SpawnTower("Sniper");
            var d  = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            d.z = 0;
            a.transform.position = d;
        }
    }
}
