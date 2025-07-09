using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapp : MonoBehaviour
{
    public float spd = 1.0f;
    public float falloff = 1.0f;
    public PlayerController controller;
    public LineRenderer lineRenderer;
    public float Wiggle = 1;
    public float Waggle = 1;
    public float Woggle = 1;
    public float Wuggle = 1;
    // Update is called once per frame
    private void FixedUpdate()
    {
        if(dongdong) spd *= falloff;
        Wiggle = Mathf.Max(Wiggle - Time.deltaTime*3, 0);
        Waggle = Mathf.Max(Waggle - Time.deltaTime/2, 0);
    }

    bool dongdong = true;

    void Update()
    {
        if (!dongdong)
        {
            LineRenderSex();
        }
        else
        {
            var dingding = transform.right * spd * Time.deltaTime;
            var wink = Physics2D.LinecastAll(transform.position, transform.position + dingding);
            bool d = true;
            foreach (var a in wink)
            {
                var tt = GameHandler.GetObjectType(a.collider);
                if (tt.Type == GameHandler.ObjectTypes.Tower)
                {
                    transform.position = a.point;
                    d = false;
                    dongdong = false;
                    controller.ShouldMoveByGrap = true;
                    Wiggle += 0.25f;
                    break;
                }
            }

            if (d)
            {
                transform.position += dingding;
            }
            LineRenderSex();
        }
        
    }
    private float cut_wangle = 1;



    public void LineRenderSex()
    {
        if (!dongdong)
        {
            Wiggle = Mathf.Max(Wiggle - Time.deltaTime * 4, 0);
            Waggle = Mathf.Min(Waggle + Time.deltaTime * 40, 100);
        }
        int m = lineRenderer.positionCount;
        cut_wangle += Time.deltaTime * Waggle;
        var based = Quaternion.Inverse(transform.rotation) * (controller.transform.position - transform.position);
        var tg = based/transform.localScale.x;
        var right = Quaternion.Euler(0,0,90)*based.normalized;
        for (int i = 0; i< m; i++)
        {
            float perc = i / (float)(m);
            Debug.Log($"ISTF {i}, {perc}");
            var offdingle = right * Mathf.Sin(Mathf.PI * (perc+ cut_wangle) * Woggle);
            offdingle *= Mathf.Sin(Mathf.PI * perc) * Wiggle * Wuggle;
            lineRenderer.SetPosition(i, Vector3.Lerp(Vector3.zero, tg, perc) + offdingle);
        }

    }
}
