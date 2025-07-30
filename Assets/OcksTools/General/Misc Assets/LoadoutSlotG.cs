using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutSlotG : MonoBehaviour
{
    public int MyLoadoutIndex;
    public void Click()
    {
        GameHandler.Instance.BeginTowerPlace(GameHandler.Instance.LocalLoadout.Towers[MyLoadoutIndex]);
    }
}
