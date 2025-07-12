using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    public float WidthMult = 0.2f;
    public LineRenderer LineRenderer;
    float Osc = 0;

    private void Update()
    {
        LineRenderer.widthMultiplier = Osc * WidthMult;
        Osc++; if (Osc == 5) { Osc = 3; }
    }
}

