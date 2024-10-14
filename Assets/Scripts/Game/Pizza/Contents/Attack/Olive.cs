using DG.Tweening;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Olive : PizzaAttack
{
    [SerializeField] private GameObject obj;
    Transform[] tr;
    int count = 5;
    SpriteRenderer[] sprs;
    Vector3[] goal;
    Vector3[] defaultPos;


    public override PizzaAttack Setup()
    {
        if (tr == null)
        {
            tr = new Transform[count];
            for (int i = 0; i < count; i++)
            {
                GameObject clone = Instantiate(obj, transform);
                tr[i] = clone.transform;
                clone.SetActive(true);
            }
        }

        goal = new Vector3[count];
        defaultPos = new Vector3[count];
        sprs = new SpriteRenderer[count];

        float force = 0.21f;
        var go = PizzaGameData.Instance.AttackArea.Olive;
        for (int i = 0; i < count; i++)
        {
            int idx = i;
            float angle = (idx * 360 / count) + 90;

            defaultPos[i] = PizzaGameData.Instance.GetAnglePos(force, angle);
            sprs[i] = go[i].GetComponentInChildren<SpriteRenderer>();
        }
        base.Setup();
        return this;
    }

    void SetGoal(byte count)
    {
        byte[] f = new byte[count];
        ushort[] d = new ushort[count];

        for (int i = 0; i < count; i++)
        {
            f[i] = (byte)Random.Range(0, 211);
            d[i] = (ushort)Random.Range(0, 361);
        }
    }

    void SetGoal()
    {
        PizzaGameData data = PizzaGameData.Instance;
        var go = data.AttackArea.Olive;
        for (int i = 0; i < count; i++)
        {
            byte randomForce = (data.IsMulti) ? data.RandomForce[i] : (byte)Random.Range(0, 211);
            ushort randomDegree = (data.IsMulti) ? data.RandomDegree[i] : (ushort)Random.Range(0, 361);
            Vector3 rand = data.GetAnglePos(randomForce * 0.001f, randomDegree);
            goal[i] = defaultPos[i] + rand;
            go[i].transform.localPosition = goal[i];
        }
    }

    protected override async UniTask SetSequence()
    {
        SetGoal();
        float delay = 0.035f;
        float offset;
        void AddTween(int idx)
        {
            int rand = Random.Range(0, 36) * 10;
            offset = delay * idx;
            _ = tr[idx].DOLocalMove(goal[idx], 0.4f + offset).SetEase(Ease.InOutQuart).ToUniTask(cancellationToken: token);
            _ = tr[idx].DOLocalRotateQuaternion(Quaternion.Euler(Vector3.forward * rand), 0.4f + offset).SetEase(Ease.InOutQuart).ToUniTask(cancellationToken: token);
        }

        await base.SetSequence();


        for (int i = 0; i < count; i++)
        {
            AddTween(i);
        }
        _ = PizzaGameData.Instance.DelayPlaySFX(PizzaSFXType.Ingredient, 0.2f, 0.8f);
        _ = PizzaGameData.Instance.DelayPlaySFX(PizzaSFXType.Ingredient, 0.3f, 0.8f);
        _ = PizzaGameData.Instance.DelayPlaySFX(PizzaSFXType.Ingredient, 0.4f);
        await UniTask.Delay((int)((delay * (count) + 0.4f) * 1000), cancellationToken: token);

        for (int i = 0; i < count; i++)
        {
            int idx = i;
            _ = tr[idx].DOScale(Vector3.one * 0.11f, 0.2f).ToUniTask(cancellationToken: token);
        }
        await UniTask.Delay((int)(0.2f * 1000), cancellationToken: token);
    }
    protected override void OnStart()
    {
        base.OnStart();
        foreach (Transform transform in tr)
        {
            transform.position = Vector3.up * (2 * 6);
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            transform.localScale = Vector3.one * 0.25f;
        }
        Color color = sprs[0].color;
        color.a = 0;
        for (int i = 0; i < sprs.Length; i++)
        {
            sprs[i].color = color;
        }
    }

    protected override void OnComplete()
    {
        base.OnComplete();
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

    protected override bool IsGameOver()
    {
        bool isGameOver = false;
        float min = (0.0867f * 6);
        float max = (0.18755f * 6);
        PizzaGameData data = PizzaGameData.Instance;

        foreach (var pos in goal)
        {
            var targetPos = pos * 6;
            float force = Vector2.Distance(data.Stage.position, targetPos);
            float degree = data.GetDegree(data.Stage.position, targetPos);
            targetPos = data.GetAnglePos(force, degree);

            float distance = Vector2.Distance(targetPos, playerPos);
            if (distance <= max && distance >= min)
            {
                isGameOver = true;
                break;
            }
        }

        return isGameOver;
    }
}