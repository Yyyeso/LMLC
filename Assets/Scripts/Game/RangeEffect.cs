using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class RangeEffect : MonoBehaviour
{
    [SerializeField] Transform inner;
    [SerializeField] TriggerCheck triggerCheck;
    int damage = 50;

    public async void SetInner(float dur)
    {
        _ = inner.DOScale(Vector3.one, dur).SetEase(Ease.Linear);
        await UniTask.Delay((int)(dur * 1000));
        _ = inner.GetComponent<SpriteRenderer>().DOFade(0, 0.15f);
        var atkTarget = triggerCheck.DDDDD();
        if (atkTarget != null)
        {
            atkTarget.OnDamage(damage);
        }
    }

    public void Release()
    {
        Color c = inner.GetComponent<SpriteRenderer>().color;
        c.a = 0.8f;
        inner.localScale = Vector3.zero;
        inner.GetComponent<SpriteRenderer>().color = c;
    }
}