using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class RangeEffect : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] rend;
    [SerializeField] Transform inner;
    [SerializeField] TriggerCheck triggerCheck;
    int damage = 50;
    PolygonCollider2D coll;
    public void Setup(Sprite spr, Vector3 size)
    {
        rend[0].sprite  = rend[1].sprite = spr;
        transform.localScale = size;
        inner.transform.localScale = Vector3.zero;
        coll = gameObject.AddComponent<PolygonCollider2D>();
    }

    void TriggerCheck()
    {
        
    }

    public async void SetInner(float dur)
    {
        _ = inner.DOScale(Vector3.one, dur).SetEase(Ease.Linear);
        await UniTask.Delay((int)(dur * 1000));
        _ = inner.GetComponent<SpriteRenderer>().DOFade(0, 0.15f);
        //var atkTarget = triggerCheck.DDDDD();
        //if (atkTarget != null)
        //{
        //    atkTarget.OnDamage(damage);
    }

    public void Release()
    {
        Color c = inner.GetComponent<SpriteRenderer>().color;
        c.a = 0.8f;
        inner.localScale = Vector3.zero;
        inner.GetComponent<SpriteRenderer>().color = c;
    }
}