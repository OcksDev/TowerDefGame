using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPhys : MonoBehaviour
{
    public List<GameObject> waaaa = new List<GameObject>();
    public Transform follower;
    public Transform follower2;
    public Transform follower3;

    private void Start()
    {
        foreach(var a in waaaa)
        {
            var b= EnemyHandler.Instance.SpawnEnemy("Phys");
            b.MovementSpeed = 0;
            b.Object = a.transform;
            b.DataRef.Radius = 0.5f;
            EnemyHandler.Instance.ObjectToEnemy.Add(a, b);
        }
    }
    public int cursel = 0;
    public float Radius = 1;
    private void Update()
    {
        foreach (var a in waaaa) a.GetComponent<SpriteRenderer>().color = new Color32(255, 150, 150,255);
        var dd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dd.z = 0;
        follower.position = dd;
        follower2.position = Vector3.down * 10000;
        follower3.position = Vector3.down * 10000;
        Radius = Mathf.Clamp(Radius+ Input.GetAxis("Mouse ScrollWheel"), 0, 4);
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            cursel++;
            if(cursel >= 3) cursel = 0;
        }
        switch (cursel)
        {
            case 0:
                follower.localScale = Vector3.one * Radius * 2;
                foreach (var a in EnemyHandler.Instance.Enemies) if(OXCollision.CircleCastSingle(dd, Radius, a))a.Object.GetComponent<SpriteRenderer>().color = new Color32(150, 255, 150, 255);
                break;
            case 1:
                follower.localScale = Vector3.one * Radius * 2;
                follower2.localScale = Vector3.one * Radius * 2;
                follower2.position = dd+(Vector3.up*2);
                follower3.localScale = new Vector3(Radius*2,2,1);
                follower3.position = dd+(Vector3.up);
                foreach (var a in EnemyHandler.Instance.Enemies) if(OXCollision.LineCastSingle(dd, Vector3.up*2, Radius, a))a.Object.GetComponent<SpriteRenderer>().color = new Color32(150, 255, 150, 255);
                break;
            case 2:
                var scale = new Vector3(2, Radius*2, 1);
                follower3.localScale = scale;
                follower3.position = dd;
                follower.position = Vector3.down * 10000;
                foreach (var a in EnemyHandler.Instance.Enemies) if (OXCollision.SquareCastSingle(dd, scale, a)) a.Object.GetComponent<SpriteRenderer>().color = new Color32(150, 255, 150, 255);

                break;
        }
    }
}
