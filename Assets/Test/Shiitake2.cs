using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shiitake2 : TestRange
{
    [SerializeField] List<Transform> transforms;

    protected override void Create(int idx)
    {

        if (idx == 3)
        {
            CreateRange(idx);
        }
        else
        {
            int idx2 = 6 - idx;
            CreateRange(idx);
            CreateRange(idx2);
        }
    }

    protected override Vector3 GetPos(int idx)
    {
        return transforms[idx].position;
    }
    protected override Vector3 GetSize(int idx)
    {
        return transforms[idx].lossyScale;
    }
    protected override Vector3 GetAngle(int idx)
    {
        return transforms[idx].eulerAngles;

    }
}