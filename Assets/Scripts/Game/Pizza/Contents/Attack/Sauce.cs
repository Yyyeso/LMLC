using DG.Tweening;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Sauce : PizzaAttack
{
    SpriteRenderer[] sprs;


    public override PizzaAttack Setup()
    {
        base.Setup();

        var area = PizzaGameData.Instance.AttackArea;
        var go = (type == PizzaIngredient.SauceWhite) ? area.SauceWhite : area.SauceBrown;
        sprs = go.GetComponentsInChildren<SpriteRenderer>();

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
        Color color = sprs[0].color;
        color.a = 0;
        for (int i = 0; i < sprs.Length; i++)
        {
            sprs[i].color = color;
        }
    }

    protected override async UniTask ShowArea(float duration, float fadeTime, Color color)
    {
        color.a = 0;
        foreach (SpriteRenderer renderer in sprs)
        {
            renderer.color = color;
        }

        for (int i = 0; i < sprs.Length; i++)
        {
            _ = sprs[i].DOFade(0.7f, fadeTime).ToUniTask(cancellationToken: token);
        }
        await UniTask.Delay((int)(fadeTime * 1000), cancellationToken: token);

        await UniTask.Delay((int)(duration * 1000), cancellationToken: token);

        for (int i = 0; i < sprs.Length; i++)
        {
            _ = sprs[i].DOFade(0, fadeTime).ToUniTask(cancellationToken: token);
        }
        await UniTask.Delay((int)(fadeTime * 1000), cancellationToken: token);
    }

    protected override void OnComplete()
    {
        base.OnComplete();
    }

    protected override bool IsGameOver()
    {
        bool isGameOver = false;
        float safeZone = 0.034f * 6; // 0.035
        float dist;
        int c = (int)(sprs.Length * 0.5f);
        PizzaGameData data = PizzaGameData.Instance;
        for (int i = 0; i < sprs.Length; i++)
        {
            var targetPos = sprs[i].transform.position;
            float force = Vector2.Distance(data.Stage.position, targetPos);
            float degree = data.GetDegree(data.Stage.position, targetPos) - stageDegree;
            targetPos = data.GetAnglePos(force, degree);

            float a = (i / c == 0) ? Mathf.Max(targetPos.x, playerPos.x) : Mathf.Max(targetPos.y, playerPos.y);
            float b = (i / c == 0) ? Mathf.Min(targetPos.x, playerPos.x) : Mathf.Min(targetPos.y, playerPos.y);
            dist = a - b;

            if (dist <= safeZone)
            {
                isGameOver = true;
                break;
            }
        }
        return isGameOver;
    }
}