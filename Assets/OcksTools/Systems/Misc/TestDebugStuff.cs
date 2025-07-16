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
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            var a = GameHandler.Instance.SpawnTower("Sniper");
            var d  = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            d.z = 0;
            a.transform.position = d;
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
