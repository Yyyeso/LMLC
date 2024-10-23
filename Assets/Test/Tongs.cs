using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tongs : TestRange
{
    [SerializeField] List<Transform> transforms;

    protected override void Create(int idx)
    {
        CreateRange(idx);
        CreateRange((2* idx)+ idx);
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