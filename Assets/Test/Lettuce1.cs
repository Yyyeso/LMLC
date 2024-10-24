using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class Lettuce1 : TestRange
{
    [SerializeField] List<Transform> transforms;


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