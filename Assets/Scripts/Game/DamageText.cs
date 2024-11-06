using TMPro;
using DG.Tweening;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public class DamageText : MonoBehaviour
{
    [SerializeField] RectTransform rect;
    [SerializeField] TMP_Text txtDamage;
    [SerializeField] float durationScale;
    [SerializeField] float delay;
    [SerializeField] Ease easePop = Ease.OutBack;
    [SerializeField] Ease easePush = Ease.OutQuad;

    CancellationTokenSource cancel;
    CancellationTokenSource linked;
    int damage = -1;

    void Awake() => SetPos();

    public void SetPos()
    {
        rect.anchoredPosition = Vector3.zero;
        rect.localScale = Vector3.zero;
    }

    public void SetDamage(int damage)
    {
        if (damage != this.damage)
        {
            this.damage = damage;
            txtDamage.text = damage.ToString();
        }
        PlayEffect();
    }

    async void PlayEffect()
    {
        CancelEffect();
        try
        {
            await rect.DOScale(Vector3.one, durationScale).SetEase(easePop).ToUniTask(cancellationToken: linked.Token);
            await UniTask.Delay((int)(delay * 1000), cancellationToken: linked.Token);
            await rect.DOScale(Vector3.zero, durationScale).SetEase(easePush).ToUniTask(cancellationToken: linked.Token);
            PoolManager.Instance.Push(gameObject);
        }
        catch (System.Exception)
        {
            SetPos();
        }
    }

    void CancelEffect()
    {
        if (cancel != null)
        {
            cancel.Cancel();
            cancel.Dispose();
            linked.Dispose();
        }
        cancel = new();
        linked = CancellationTokenSource.CreateLinkedTokenSource(cancel.Token, this.GetCancellationTokenOnDestroy());
    }
}