using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rigid;
    public float move_speed = 2;
    public float decay = 0.8f;
    private Vector3 move = new Vector3(0, 0, 0);
    public GameObject Grapp;
    private void Start()
    {
        rigid= GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        move *= decay;
        Vector3 dir = new Vector3(0, 0, 0);
        if (InputManager.IsKey("move_forward")) dir += Vector3.up;
        if (InputManager.IsKey("move_back")) dir += Vector3.down;
        if (InputManager.IsKey("move_right")) dir += Vector3.right;
        if (InputManager.IsKey("move_left")) dir += Vector3.left;
        if(dir.magnitude > 0.5f)
        {
            dir.Normalize();
            move += dir;
        }
        Vector3 bgalls = move * Time.deltaTime * move_speed * 20;
        rigid.velocity += new Vector2(bgalls.x, bgalls.y);
        if (CameraLol.Instance != null)
        {
            CameraLol.Instance.targetpos = transform.position;
        }

    }
    private void Update()
    {
        if (InputManager.IsKeyDown("aim"))
        {
            Grapple();
        }
    }
    public void Grapple()
    {
        var pp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pp.z = 0;

        Tower smegma = null;
        float dist = 10f;
        foreach (var a in GameHandler.Instance.AllActiveTowers)
        {
            var zinkle = a.transform.position;
            zinkle.z = 0;
            if ((zinkle-pp).sqrMagnitude <= dist)
            {
                smegma = a;
                dist = (zinkle - pp).sqrMagnitude;
            }
        }
        if(smegma != null)
        {
            ShouldMoveByGrap = false;
            nerdl = Instantiate(Grapp,transform.position, RandomFunctions.PointAtPoint2D(transform.position, smegma.transform.position, 180)).GetComponent<Grapp>();
            nerdl.controller = this;
            StartCoroutine(WaitForNoGrap());
        }
    }
    private Grapp nerdl;
    public bool ShouldMoveByGrap = false;
    public IEnumerator WaitForNoGrap()
    {
        yield return new WaitUntil(() => { return !InputManager.IsKey("aim"); });
        Destroy(nerdl.gameObject);
        ShouldMoveByGrap = false;
        // stop grap
    }
}
