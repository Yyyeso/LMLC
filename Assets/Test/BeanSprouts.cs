using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeanSprouts : TestRange
{
    [SerializeField] List<Transform> transforms;

    protected override async UniTask Create(int idx)
    {
        if (idx == 5)
        {
            await UniTask.Delay(700);
            _ = CreateRange(10);
            _ = CreateRange(11);
            _ = CreateRange(12);
            await CreateRange(13);
        }
        else
        {
            int st = idx;
            int ed = 9 - idx;
            _ = CreateRange(st);
            await CreateRange(ed);
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