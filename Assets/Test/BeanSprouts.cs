using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeanSprouts : TestRange
{
    [SerializeField] List<Transform> transforms;

    protected override async void Create(int idx)
    {
        if (idx == 5)
        {
            await UniTask.Delay(1000);
            CreateRange(10);
            CreateRange(11);
            CreateRange(12);
            CreateRange(13);
        }
        else
        {
            int st = idx;
            int ed = 9 - idx;
            CreateRange(st);
            CreateRange(ed);
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