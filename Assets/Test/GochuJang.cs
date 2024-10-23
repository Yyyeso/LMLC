using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GochuJang : TestRange
{
    protected override Vector3 GetPos(int idx)
    {
        return RandPosInCircle(center, radius);
    }

    protected override Vector3 GetSize(int idx)
    {
        float rand = Random.Range(1.0f, 2.5f);
        return Vector3.one * rand;
    }
}
