using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSex : MonoBehaviour
{
    public LineRenderer lr;
    public Transform aa;
    public float Radious = 1;
    // Start is called before the first frame update

    float cur = 0;
    Coroutine dd;
    void Start()
    {
        dd = StartCoroutine(OXLerp.Linear((x) => { cur = RandomFunctions.EaseIn(x); SetFromRadiuiusss(cur * Radious); }, 0.25f));
    }

    public void UpdateToNewRad(float rad)
    {
        if (dd != null) StopCoroutine(dd);
        var d = Radious * cur;
        Radious = rad;
        dd = StartCoroutine(OXLerp.Linear((x) => { cur = RandomFunctions.EaseIn(x); SetFromRadiuiusss(Mathf.Lerp(d,Radious,cur)); }, 0.25f));
    }


    public void SetKill()
    {
        StartCoroutine(KillTim());
    }

    public IEnumerator KillTim()
    {
        if(dd != null) StopCoroutine(dd);
        yield return StartCoroutine(OXLerp.Linear((x) => { SetFromRadiuiusss(RandomFunctions.EaseIn(1-x) * Radious * cur); }, 0.15f*cur));
        Destroy(gameObject);
    }

    public void SetFromRadiuiusss(float rad)
    {
        int amnt = 100;
        float inc = 360f / amnt;
        lr.positionCount = amnt;
        for (int i = 0; i < amnt; i++)
        {
            lr.SetPosition(i, Quaternion.Euler(0, 0, inc * i) * Vector3.up * rad);
        }
        aa.localScale = Vector3.one * ((rad*2 * 1.011f) - 0.2f);
    }

}
