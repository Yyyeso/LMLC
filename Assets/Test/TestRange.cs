using Cysharp.Threading.Tasks;
using UnityEngine;

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
    protected enum PlaceMode
    {
        Designated,
        Random
    }

    [SerializeField] ShapeType shapeType;
    [SerializeField] protected PlaceMode placeMode;
    [SerializeField] protected Vector2 size;
    [SerializeField] protected float interval;
    [SerializeField] float FadeDuration;
    [SerializeField] protected float Delay;
    [SerializeField] int count;
    [SerializeField] int damage;
    [SerializeField] bool separateAttack;

    public ShapeType ShapeType => shapeType;

    protected float radius;
    protected Vector2 center;
    Test test;


    public async UniTask Play(Test test)
    {
        this.test = test;
        center = test.Center;
        radius = test.OriginRadius - (GetSize(0).x * 0.5f);
        int c = count - 1;
        for (int i = 0; i < c; i++)
        {
            _ = Create(i);
            await UniTask.Delay((int)(interval * 1000));
        }
        await Create(c);
        if (!separateAttack)
        {
            
        }
    }

    protected virtual async UniTask Create(int idx) => await CreateRange(idx);


    protected async UniTask CreateRange(int idx)
    {
        GameObject p = (test.dict[shapeType].Count <= 0) ? Instantiate(test.shapeList[shapeType]) : test.dict[shapeType].Pop();
        var inner = p.GetComponent<RangeEffect>();

        p.transform.position = GetPos(idx);
        p.transform.localScale = GetSize(idx);
        p.transform.localEulerAngles = GetAngle(idx);
        p.SetActive(true);

        bool attackable = await inner.PlayEffect(Delay, FadeDuration);
        if (attackable)
        {
            test.Player.OnDamage(damage);
        }

        test.dict[shapeType].Push(p);
    }

    protected virtual Vector3 GetSize(int idx) => size;

    protected virtual Vector3 GetPos(int idx) => Vector3.zero;

    protected virtual Vector3 GetAngle(int idx) => Vector3.zero;

    protected Vector2 RandPosInCircle(Vector2 center, float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        float distance = Random.Range(0f, radius);

        float x = Mathf.Cos(angle) * distance;
        float y = Mathf.Sin(angle) * distance;

        return new Vector2(center.x + x, center.y + y);
    }
}