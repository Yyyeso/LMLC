using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tongs : TestRange
{
    [SerializeField] List<Transform> transforms;

    protected override async UniTask Create(int idx)
    {
        _ = CreateRange(2 * idx);
        await CreateRange(2 * idx + 1);
    }

    protected override Vector3 GetPos(int idx)
    {
        return transforms[idx].position;
    }
    protected override Vector3 GetSize(int idx)
    {
        return transforms[idx].lossyScale;
    }
}