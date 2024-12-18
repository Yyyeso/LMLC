using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MincedMeat : TestRange
{
    [SerializeField] List<Transform> transforms;

    protected override async UniTask Create(int idx)
    {
        for (int i = 0; i < 9; i++)
        {
            _ = CreateRange((10 * idx) + i);
            await UniTask.Delay(100);
        }

        await CreateRange((10 * idx) + 9);
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