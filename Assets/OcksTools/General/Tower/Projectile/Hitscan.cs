using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitscan : MonoBehaviour
{
    public float MaxLife = 0.2f;
    public float WidthMult = 0.2f;
    public LineRenderer LineRenderer;
    float life = 0;
    private void Awake()
    {
        life = MaxLife;
    }
    private void Update()
    {
        LineRenderer.widthMultiplier = ( life/MaxLife ) * WidthMult;
        life-=Time.deltaTime;
        if(life <= 0)
        {
            Destroy(gameObject);
        }
    }
}
