using DG.Tweening;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class TomatoPaste : PizzaAttack
{
    public override PizzaAttack Setup()
    {
        base.Setup();
        attackDelay = 7;
        return this;
    }

    protected override async UniTask SetSequence()
    {
        await base.SetSequence();

        _ = PizzaGameData.Instance.DelayPlaySFX(PizzaSFXType.Paste, 0.35f);
        _ = transform.DOMove(Vector3.zero, 0.8f).SetEase(Ease.InOutQuart).ToUniTask(cancellationToken: token);
        await transform.DOScale(new Vector3(0.8f, 0.6f, 1), 0.65f).SetEase(Ease.InQuint).ToUniTask(cancellationToken: token);
        await transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuart).ToUniTask(cancellationToken: token);
    }

    protected override void OnStart()
    {
        base.OnStart();
        transform.position = Vector3.up * (2 * 6);
        transform.localScale = new Vector3(0.2f, 1, 1);
        var rend = PizzaGameData.Instance.AttackArea.TomatoPaste.GetComponentInChildren<SpriteRenderer>();
        Color color = rend.color;
        color.a = 0;
        rend.color = color;
    }

    protected override void OnComplete()
    {
        base.OnComplete();
    }

    protected override async UniTask ShowArea(float duration, float fadeTime, Color color)
    {
        SpriteRenderer renderer = PizzaGameData.Instance.AttackArea.TomatoPaste.GetComponentInChildren<SpriteRenderer>();
        color.a = 0;
        renderer.color = color;
        await renderer.DOFade(0.7f, fadeTime).ToUniTask(cancellationToken: token);
        await UniTask.Delay((int)(duration * 1000), cancellationToken: token);
        await renderer.DOFade(0, fadeTime).ToUniTask(cancellationToken: token);
    }

    protected override bool IsGameOver()
    {
        float safeZone = (0.84f * 3);
        float distance = Vector2.Distance(Vector2.zero, playerPos);
        return (distance <= safeZone);
    }
}