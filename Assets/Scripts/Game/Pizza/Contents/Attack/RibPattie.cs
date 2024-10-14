using DG.Tweening;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class RibPattie : PizzaAttack
{
    [SerializeField] private GameObject obj;

    Transform[] tr;
    int count = 5;
    Vector3[] goal;
    float[] angle;
    public SpriteRenderer[] sprs;


    public override PizzaAttack Setup()
    {
        goal = new Vector3[count];
        angle = new float[count];
        sprs = new SpriteRenderer[count];
        var area = PizzaGameData.Instance.AttackArea.RibPattie;
        if (tr == null)
        {
            float force = 0.32f;
            tr = new Transform[count];
            for (int i = 0; i < count; i++) 
            {
                GameObject go = Instantiate(obj, transform);
                tr[i] = go.transform;
                go.SetActive(true);

                angle[i] = i * 360 / count;
                goal[i] = PizzaGameData.Instance.GetAnglePos(force, angle[i] + 90);
                sprs[i] = area[i].GetComponent<SpriteRenderer>();
            }
        }
        base.Setup();
        return this;
    }

    protected override async UniTask SetSequence()
    {
        float delay = 0.035f;
        float offset;
        void AddTween(int idx)
        {
            offset = delay * idx;
            _ = tr[idx].DOLocalMove(goal[idx], 0.4f + offset).SetEase(Ease.InOutQuart).ToUniTask(cancellationToken: token);
            _ = tr[idx].DOLocalRotateQuaternion(Quaternion.Euler(Vector3.forward * angle[idx]), 0.4f + offset).SetEase(Ease.InOutQuart).ToUniTask(cancellationToken: token);
        }

        await base.SetSequence();

        for (int i = 0; i < tr.Length; i++)
        {
            AddTween(i);
        }

        _ = PizzaGameData.Instance.DelayPlaySFX(PizzaSFXType.Ingredient, 0.2f, 0.8f);
        _ = PizzaGameData.Instance.DelayPlaySFX(PizzaSFXType.Ingredient, 0.3f, 0.8f);
        _ = PizzaGameData.Instance.DelayPlaySFX(PizzaSFXType.Ingredient, 0.4f);
    }

    protected override void OnStart()
    {
        base.OnStart();
        foreach (Transform transform in tr)
        {
            transform.position = Vector3.up * (2 * 6);
            transform.GetComponent<Collider2D>().enabled = false;
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
        foreach (Transform transform in tr)
        {
            transform.GetComponent<Collider2D>().enabled = true;
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

    protected override bool IsGameOver()
    {
        bool isGameOver = false;
        float safeZoneX = 0.089f * 6;
        float safeZoneY = 0.031f * 6;
        PizzaGameData data = PizzaGameData.Instance;

        float GetDist(float a, float b) => Mathf.Max(a, b) - Mathf.Min(a, b);
        void SetDist(int idx, out float xDist, out float yDist)
        {
            float force, degree;
            var targetPos = goal[idx] * 6;
            force = Vector2.Distance(data.Stage.position, targetPos);
            degree = data.GetDegree(data.Stage.position, targetPos);
            targetPos = data.GetAnglePos(force, degree);
            //test2[idx].transform.position = targetPos;

            force = Vector2.Distance(playerPos, targetPos);
            degree = data.GetDegree(playerPos, targetPos) - angle[idx];
            var pos = data.GetAnglePos(force, degree) + playerPos;
            xDist = GetDist(pos.x, playerPos.x);
            yDist = GetDist(pos.y, playerPos.y);
            //test2[idx + 5].transform.position = pos;
        }

        for (int i = 0; i < count; i++)
        {
            SetDist(i, out float xDist, out float yDist);
            if (xDist <= safeZoneX && yDist <= safeZoneY)
            {
                isGameOver = true;
                break;
            }
        }

        return isGameOver;
    }
}