using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunnyLerpOnPlayer : MonoBehaviour
{
    private Vector3 OldPos;
    public Vector3 LastPos;
    public Vector3 TargetPos;
    public List<float> Times = new List<float>();
    // Start is called before the first frame update
    void Start()
    {
        Times.Add(0.5f);
        LastPos = transform.position;
        TargetPos = transform.position;
    }

    // Update is called once per frame
    float x = 0;
    float tx = 0;
    public float avg;

    void LateUpdate()
    {
        if (transform.parent.position != TargetPos)
        {
            LastPos = OldPos;
            TargetPos = transform.parent.position;
            Times.Add(tx);
            if (Times.Count > 20) Times.RemoveAt(0);
            x = 0;
            tx = 0;
        }
        avg = 0;
        foreach(var a in Times)
        {
            avg += a;
        }
        avg /= Times.Count;

        if(tx <= avg + 0.05f) tx += Time.deltaTime;
        x += Time.deltaTime * (1/avg);

        transform.position = Vector3.LerpUnclamped(LastPos,TargetPos,x);
        OldPos = transform.position;
    }
}
