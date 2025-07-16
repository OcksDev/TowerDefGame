using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nothing : StationaryTower
{
    public override void UpdateRender()
    {
        RenderParts[0].sprite = GameHandler.Instance.BaseIMGS[Level];
    }
}
