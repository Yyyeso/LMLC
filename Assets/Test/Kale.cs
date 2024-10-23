using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kale : TestRange
{
    [SerializeField] List<Transform> transforms;

    protected override void Create(int idx)
    {
        for (int i = 0; i < 3; i++)
        {
            CreateRange((3 * idx) + i);
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