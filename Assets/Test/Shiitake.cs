using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shiitake : TestRange
{
    [SerializeField] List<int> ints = new() { 2, 6, 12, 17 };
    [SerializeField] List<Transform> transforms;

    protected override void Create(int idx)
    {
        int st = (idx == 0) ? 0 : ints[idx - 1];
        for (int i = st; i < ints[idx]; i++) 
        {
            CreateRange(i);
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