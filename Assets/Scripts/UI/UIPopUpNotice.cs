using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Threading;
using Cysharp.Threading.Tasks;

public class UIPopUpNotice : UIBase
{
    [SerializeField] private TMP_Text txtMessage;
    [SerializeField] private CanvasGroup canvas;

    CancellationToken c;
    CancellationToken CancelToken => c = (c != null) ? c : this.GetCancellationTokenOnDestroy();

    public async UniTask SetMessage(string message, Color color, float duration = 0.8f)
    {
        canvas.alpha = 0;
        txtMessage.text = message;
        txtMessage.color = color;
        
        await canvas.DOFade(1, 0.15f).ToUniTask(cancellationToken: CancelToken);
        await UniTask.Delay((int)(duration * 1000));
        await canvas.DOFade(0, 0.15f).ToUniTask(cancellationToken: CancelToken);
        CloseUI();
    }
}