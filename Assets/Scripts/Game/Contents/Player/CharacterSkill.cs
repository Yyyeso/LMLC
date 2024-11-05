using DG.Tweening;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public static class CharacterSkill
{
    public const float dashCoolDown = 10;
    const float dashSizeX = 0.6f;

    public static async UniTask Dash(Transform tr, float dashDist, Vector3 dashDir, float dashDuration, Ease dashEase, CancellationToken cancelToken)
    {
        Vector3 dest = tr.position + dashDist * dashSizeX * dashDir.normalized;
        await tr.DOMove(dest, dashDuration).SetEase(dashEase).ToUniTask(cancellationToken: cancelToken);
    }
}