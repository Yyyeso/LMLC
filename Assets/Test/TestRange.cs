using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public enum ShapeType
{
    Circle,
    Square,
    Lettuce,
    Kale,
    BeanSprouts,
    Shiitake,
}

public class TestRange : MonoBehaviour
{
    [SerializeField] ShapeType shapeType;
    [SerializeField] protected PlaceMode placeMode;
    Test test;
    GameObject go;
    [SerializeField] protected Vector3 size;

    public ShapeType ShapeType => shapeType;
    protected enum PlaceMode
    {
        Designated,
        Random
    }
    protected float radius;
    protected Vector3 center;
    [SerializeField] protected float interval;
    public async UniTask Play(Test test)
    {
        this.test = test;
        center = test.Center;
        radius = test.OriginRadius - (size.x * 0.5f);
        for (int i = 0; i < count; i++)
        {
            int idx = i;
            Create(idx);
            await UniTask.Delay((int)(interval * 1000));
        }
        await UniTask.Delay((int)(Delay * 1000) - (int)(interval * 1000));
    }

    protected virtual void Create(int idx)
    {
        CreateRange(idx);
    }

    [SerializeField] float FadeDuration;
    [SerializeField] float Delay;
    [SerializeField] float count;

    public async void CreateRange(int idx)
    {
        GameObject p = (test.dict[shapeType].Count <= 0) ? Instantiate(test.shapeList[shapeType]) : test.dict[shapeType].Pop();
        var inner = p.GetComponent<RangeEffect>();

        p.transform.position = GetPos(idx);
        p.transform.localScale = GetSize(idx);
        p.transform.localEulerAngles = GetAngle(idx);
        p.SetActive(true);
        _ = p.GetComponent<SpriteRenderer>().DOFade(0.8f, FadeDuration);
        inner.SetInner(Delay);
        await UniTask.Delay((int)(Delay * 1000));
        await p.GetComponent<SpriteRenderer>().DOFade(0, FadeDuration);
        inner.Release();
        test.dict[shapeType].Push(p);
    }

    protected virtual Vector3 GetSize(int idx)
    {
        return size;
    }
    protected virtual Vector3 GetPos(int idx)
    {
        return Vector3.zero;
    }
    protected virtual Vector3 GetAngle(int idx)
    {
        return Vector3.zero;
    }

    protected Vector2 RandPosInCircle(Vector2 center, float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        float distance = Random.Range(0f, radius);

        float x = Mathf.Cos(angle) * distance;
        float y = Mathf.Sin(angle) * distance;

        return new Vector2(center.x + x, center.y + y);
    }
}