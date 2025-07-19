using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overloader : StationaryTower
{
    public Tower BuffNerd;
    public void LinkTower(Tower Nerd)
    {
        var d = Nerd.GetBuff("Overload");
        if (d.Stack <= 0)
        {
            BuffNerd = Nerd;
            Nerd.SetStats();
        }
    }
    public override void StatMod(Tower me)
    {
        if(me==BuffNerd)me.AddBuff(new Buff("Overload", this));
    }
    public override void Tick()
    {
        if(BuffNerd != null)
        {
            Parts[0].rotation = RandomFunctions.PointAtPoint2D(Parts[0].position, BuffNerd.transform.position, 0);
        }
    }
}
