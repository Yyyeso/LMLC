using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kale : TestRange
{
    [SerializeField] List<Transform> transforms;

    protected override async UniTask Create(int idx)
    {
        _ =   CreateRange((3 * idx) + 0);
        _ =   CreateRange((3 * idx) + 1);
        await CreateRange((3 * idx) + 2);
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