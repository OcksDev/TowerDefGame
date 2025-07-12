using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    public float WidthMult = 0.1f;
    public LineRenderer LineRenderer;
    float Osc = 0;

    private void FixedUpdate() //changed to be fixed update so its not tied to framerate
    {
        LineRenderer.widthMultiplier = Osc * WidthMult;
        Osc++; if (Osc >= 10) { Osc = 6; }
    }
}

