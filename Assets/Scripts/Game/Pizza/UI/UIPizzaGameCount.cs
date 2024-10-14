using TMPro;
using System;
using DG.Tweening;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public class UIPizzaGameCount : UIPizzaBase
{
    [SerializeField] private TMP_Text txtCount;

    string[] strRound = new string[3] { "1 Round", "2 Round", "3 Round" };
    string[] str = new string[4] { "3", "2", "1", "Go!" };
    PizzaGameData data;
    Action<bool> FreezeAction;


    public UIPizzaGameCount SetFreezeAction(Action<bool> action)
    {
        FreezeAction = action;
        CloseUI();
        return this;
    }
    protected override void Init() => data = PizzaGameData.Instance;

    async UniTask TextAnim(float delay, bool useRotate, CancellationToken token)
    {
        txtCount.transform.localScale = Vector3.zero;
        txtCount.transform.rotation = Quaternion.identity;

        await txtCount.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack).ToUniTask(cancellationToken: token);
        int d = (int)(delay * 1000);
        await UniTask.Delay(d, cancellationToken: token);
        if (useRotate)
        {
            _ = txtCount.transform.DORotate(Vector3.back * 180, 0.3f).SetEase(Ease.OutQuart).ToUniTask(cancellationToken: token);
        }
        await txtCount.transform.DOScale(0, 0.6f).SetEase(Ease.OutQuart).ToUniTask(cancellationToken: token);
        await UniTask.Delay(150, cancellationToken: token);
    }

    public async UniTask SetText(int round, CancellationToken token)
    {
        gameObject.SetActive(true);
        FreezeAction.Invoke(true);
        float delay = 0.5f;
        int count = 0;

        txtCount.text = strRound[round];
        _ = data.PlaySFX(PizzaSFXType.Combo);
        await TextAnim(delay, false, token);
        await UniTask.Delay(100, cancellationToken: token);
        
        txtCount.text = str[count++];
        _ = data.PlaySFX(PizzaSFXType.Waterdrop);
        await TextAnim(delay, true, token);

        txtCount.text = str[count++];
        _ = data.PlaySFX(PizzaSFXType.Waterdrop);
        await TextAnim(delay, true, token);

        txtCount.text = str[count++];
        _ = data.PlaySFX(PizzaSFXType.Waterdrop);
        await TextAnim(delay, true, token);

        txtCount.text = str[count];
        _ = data.PlaySFX(PizzaSFXType.Combo);
        await TextAnim(delay, false, token);
        FreezeAction.Invoke(false);
        CloseUI();
    }
}