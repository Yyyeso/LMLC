using DG.Tweening;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Corn : PizzaAttack
{
    [SerializeField] private GameObject obj;

    Transform[] tr;
    int count = 16;
    SpriteRenderer[] sprs;
    Vector3[] goalStart;
    Vector3[] goalEnd;
    Vector3[] defaultPos;


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

        attackDelay = 3f;
        goalStart = new Vector3[count];
        goalEnd = new Vector3[count];
        defaultPos = new Vector3[count];
        sprs = PizzaGameData.Instance.AttackArea.Corn.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < count; i++)
        {
            int idx = i;
            float force = (idx < 10) ? 0.35f : 0.15f;
            float angle = (idx < 10) ? (idx * 360 * 0.1f) : (idx * 360 / 6);

            defaultPos[i] = PizzaGameData.Instance.GetAnglePos(force, angle);
        }
        base.Setup();
        return this;
    }

    void SetGoal()
    {
        for (int i = 0; i < count; i++)
        {
            float randX = Random.Range(-0.06f, 0.06f);
            float randY = Random.Range(-0.06f, 0.06f);
            goalStart[i] = new(randX, randY, 0);

            randX = Random.Range(-0.03f, 0.03f);
            randY = Random.Range(-0.03f, 0.03f);
            goalEnd[i] = defaultPos[i] + new Vector3(randX, randY, 0);
        }
    }

    protected override async UniTask SetSequence()
    {
        SetGoal();

        _ = ShowArea(1.9f, 0.3f, PizzaGameData.Instance.GetColor("#FF4B00"));
        SetTimer();
        await UniTask.Delay((int)(attackDelay * 1000 * 0.5f));
        OnStart();
        for (int i = 0; i < count; i++)
        {
            int rand = Random.Range(0, 36) * 10;
            _ = tr[i].DOLocalMove(goalStart[i], attackDelay * 0.5f).SetEase(Ease.InOutQuart).ToUniTask(cancellationToken: token);
            _ = tr[i].DORotateQuaternion(Quaternion.Euler(Vector3.forward * rand), 0.3f).SetEase(Ease.InOutQuart).ToUniTask(cancellationToken: token);
        }
        await UniTask.Delay((int)(attackDelay * 1000 * 0.5f), cancellationToken: token);

        _ = PizzaGameData.Instance.PlaySFX(PizzaSFXType.Corn);
        PizzaGameData.Instance.Player.Fallen();
        for (int i = 0; i < count; i++)
        {
            _ = tr[i].DOLocalMove(goalEnd[i], 0.8f).SetEase(Ease.InOutQuart).ToUniTask(cancellationToken: token);
        }
        await UniTask.Delay((int)(0.8f * 1000), cancellationToken: token);
    }

    protected override void OnStart()
    {
        base.OnStart();
        foreach (Transform transform in tr)
        {
            transform.position = Vector3.up * (2 * 6);
            transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
        Color color = sprs[0].color;
        color.a = 0;
        for (int i = 0; i < sprs.Length; i++)
        {
            sprs[i].color = color;
        }
    }

    protected override async UniTask ShowArea(float duration, float fadeTime, Color color)
    {
        int loopCount = 3;
        color.a = 0;
        foreach (SpriteRenderer renderer in sprs)
        {
            var tr = renderer.transform;
            renderer.color = color;
            tr.localPosition = Vector3.up * 0.2f;
            _ = tr.DOLocalMoveY(0.35f, attackDelay / loopCount).SetLoops(loopCount).ToUniTask(cancellationToken: token);
        }

        async UniTask Fade(float value)
        {
            for (int i = 0; i < sprs.Length; i++)
            {
                _ = sprs[i].DOFade(value, fadeTime).ToUniTask(cancellationToken: token);
            }
            await UniTask.Delay((int)(fadeTime * 1000), cancellationToken: token);
        }

        await Fade(0.7f);
        await UniTask.Delay((int)(duration * 1000), cancellationToken: token);
        await Fade(0);
    }

    protected override void OnComplete()
    {
        base.OnComplete();
    }
}
