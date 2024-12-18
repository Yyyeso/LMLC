using DG.Tweening;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Mushroom : PizzaAttack
{
    [SerializeField] private GameObject obj;

    Transform[] tr;
    int count = 12;


    public override PizzaAttack Setup()
    {
        if (tr == null)
        {
            tr = new Transform[count];
            for (int i = 0; i < count; i++)
            {
                GameObject go = Instantiate(obj, transform);
                tr[i] = go.transform;
                go.SetActive(true);
            }
        }
        base.Setup();
        return this;
    }

    protected override async UniTask SetSequence()
    {
        float delay = 0.035f;
        float force = 0.395f;
        float offset;
        void AddTween(int idx)
        {
            float angle = (idx * 360 / count);
            float lotation = -90 + angle;

            Vector3 goal = PizzaGameData.Instance.GetAnglePos(force, angle);

            offset = delay * idx;
            _ = tr[idx].DOLocalMove(goal, 0.4f + offset).SetEase(Ease.InOutQuart).ToUniTask(cancellationToken: token);
            _ = tr[idx].DOLocalRotateQuaternion(Quaternion.Euler(Vector3.forward * lotation), 0.4f + offset).SetEase(Ease.InOutQuart).ToUniTask(cancellationToken: token);
        }

        await base.SetSequence();

        for (int i = 0; i < tr.Length; i++)
        {
            AddTween(i);
        }
        offset = delay * tr.Length;

        _ = PizzaGameData.Instance.DelayPlaySFX(PizzaSFXType.Ingredient, 0.2f, 0.8f);
        _ = PizzaGameData.Instance.DelayPlaySFX(PizzaSFXType.Ingredient, 0.3f, 0.8f);
        _ = PizzaGameData.Instance.DelayPlaySFX(PizzaSFXType.Ingredient, 0.4f);
        await UniTask.Delay((int)((0.4f + offset) * 1000), cancellationToken: token);
    }
    protected override void OnStart()
    {
        base.OnStart();
        foreach (Transform transform in tr)
        {
            transform.position = Vector3.up * (2 * 6);
        }
        var rend = PizzaGameData.Instance.AttackArea.Mushroom.GetComponentInChildren<SpriteRenderer>();
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
        SpriteRenderer renderer = PizzaGameData.Instance.AttackArea.Mushroom.GetComponentInChildren<SpriteRenderer>();

        color.a = 0;
        renderer.color = color;
        await renderer.DOFade(0.7f, fadeTime).ToUniTask(cancellationToken: token);
        await UniTask.Delay((int)(duration * 1000), cancellationToken: token);
        await renderer.DOFade(0, fadeTime).ToUniTask(cancellationToken: token);
    }

    protected override bool IsGameOver()
    {
        float safeZone = (0.6f * 3);
        float distance = Vector2.Distance(Vector2.zero, playerPos);
        return (distance >= safeZone);
    }
}