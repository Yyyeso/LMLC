using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shiitake : TestRange
{
    [SerializeField] List<Transform> transforms;

    protected override async UniTask Create(int idx)
    {
        if (idx == 3)
        {
            await CreateRange(idx);
        }
        else
        {
            int idx2 = 6 - idx;
            _ = CreateRange(idx);
            await CreateRange(idx2);
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