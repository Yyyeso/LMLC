using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryTomato : TestRange
{
    protected override Vector3 GetPos(int idx)
    {
        return RandPosInCircle(center, radius);
    }
}