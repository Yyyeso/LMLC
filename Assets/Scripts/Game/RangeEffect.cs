using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class RangeEffect : MonoBehaviour
{
    [SerializeField] SpriteRenderer rendOuter;
    [SerializeField] SpriteRenderer rendInner;
    [SerializeField] Transform inner;

    bool onTriggerEnter;

    public bool TriggerCheck => onTriggerEnter;

    public async UniTask<bool> PlayEffect(float innerDur, float fadeDur)
    {
        _ =   rendOuter.DOFade(0.8f, fadeDur);
        await inner.DOScale(Vector3.one, innerDur).SetEase(Ease.Linear);
        bool attackable = onTriggerEnter;
        _ =   rendInner.DOFade(0, fadeDur);
        await rendOuter.DOFade(0, fadeDur);
        Release();
        return attackable;
    }

    public async void SetInner(float dur)
    {
        _ = inner.DOScale(Vector3.one, dur).SetEase(Ease.Linear);
        await UniTask.Delay((int)(dur * 1000));
        _ = rendInner.DOFade(0, 0.15f);
    }

    public void Release()
    {
        gameObject.SetActive(false);
        Color c = rendInner.color;
        c.a = 0.8f;
        rendInner.color = c;
        inner.localScale = Vector3.zero;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Const.Player))
        {
            onTriggerEnter = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Const.Player))
        {
            onTriggerEnter = false;
        }
    }

}