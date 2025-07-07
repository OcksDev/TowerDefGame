using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapp : MonoBehaviour
{
    public float spd = 1.0f;
    public float falloff = 1.0f;
    public PlayerController controller;
    // Update is called once per frame
    private void FixedUpdate()
    {
        if(dongdong) spd *= falloff;
    }

    bool dongdong = true;

    void Update()
    {
        if (!dongdong) return;
        var dingding = transform.right * spd * Time.deltaTime;
        var wink = Physics2D.LinecastAll(transform.position, transform.position+dingding);
        bool d = true;
        foreach (var a in wink)
        {
            var tt = GameHandler.GetObjectType(a.collider);
            if(tt.Type == GameHandler.ObjectTypes.Tower)
            {
                transform.position = a.point;
                d = false;
                dongdong=true;
                controller.ShouldMoveByGrap = true;
                break;
            }
        }

        if (d)
        {
            transform.position += dingding;
        }
    }
}
