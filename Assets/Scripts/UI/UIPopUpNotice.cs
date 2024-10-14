using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using Cysharp.Threading.Tasks;

public class UIPopUpNotice : UIBase
{
    [SerializeField] private Image imgBG;
    [SerializeField] private TMP_Text txtMessage;
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private Color color;

    CancellationToken c;
    CancellationToken CancelToken => c = (c != null) ? c : this.GetCancellationTokenOnDestroy();


    public async UniTask SetMessage(string message, float duration = 0.8f)
    {
        canvas.alpha = 0;
        imgBG.color = color;
        txtMessage.text = message;

        await canvas.DOFade(1, 0.15f).ToUniTask(cancellationToken: CancelToken);
        await UniTask.Delay((int)(duration * 1000));
        await canvas.DOFade(0, 0.15f).ToUniTask(cancellationToken: CancelToken);
        CloseUI();
    }

    public async UniTask SetMessage(string message, Color color, float duration = 0.8f)
    {
        canvas.alpha = 0;
        imgBG.color = color;
        txtMessage.text = message;

        await canvas.DOFade(1, 0.15f).ToUniTask(cancellationToken: CancelToken);
        await UniTask.Delay((int)(duration * 1000));
        await canvas.DOFade(0, 0.15f).ToUniTask(cancellationToken: CancelToken);
        CloseUI();
    }
}