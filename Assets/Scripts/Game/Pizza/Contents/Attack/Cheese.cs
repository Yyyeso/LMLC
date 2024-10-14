using DG.Tweening;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Cheese : PizzaAttack
{
    public override PizzaAttack Setup()
    {
        base.Setup();
        return this;
    }

    protected override async UniTask SetSequence()
    {
        await base.SetSequence();

        _ = PizzaGameData.Instance.DelayPlaySFX(PizzaSFXType.Cheese, 0.35f);
        _ = transform.DOLocalMove(Vector3.zero, 0.8f).SetEase(Ease.InOutQuart).ToUniTask(cancellationToken: token);
        await transform.DOScale(new Vector3(0.9f, 0.9f, 1), 0.65f).SetEase(Ease.InQuint).ToUniTask(cancellationToken: token);
        await transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuart).ToUniTask(cancellationToken: token);
    }

    protected override void OnStart()
    {
        base.OnStart();
        transform.position = Vector3.up * (2 * 6);
        transform.localScale = new Vector3(0.2f, 1, 1);
        var area = PizzaGameData.Instance.AttackArea;
        var go = (type == PizzaIngredient.CheeseL) ? area.CheeseLeft : area.CheeseRight;
        var rend = go.GetComponentInChildren<SpriteRenderer>();
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
        var area = PizzaGameData.Instance.AttackArea;
        var go = (type == PizzaIngredient.CheeseL) ? area.CheeseLeft : area.CheeseRight;

        SpriteRenderer renderer = go.GetComponentInChildren<SpriteRenderer>();

        color.a = 0;
        renderer.color = color;
        await renderer.DOFade(0.7f, fadeTime).ToUniTask(cancellationToken: token);
        await UniTask.Delay((int)(duration * 1000), cancellationToken: token);
        await renderer.DOFade(0, fadeTime).ToUniTask(cancellationToken: token);
    }

    protected override bool IsGameOver()
    {
        bool isGameOver = (type == PizzaIngredient.CheeseL) ? (playerPos.x <= 0) : (playerPos.x >= 0);
        return isGameOver;
    }
}