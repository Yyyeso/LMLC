using TMPro;
using DG.Tweening;
using UnityEngine;

public class UILoading : UIBase
{
    [SerializeField] Transform tr;
    [SerializeField] GameObject tip;
    [SerializeField] TMP_Text txtTipInfo;

    private Tween spin;


    protected override void Init()
    {
        OnCloseAction += (UIBase) => OnClose();
        spin = tr.DORotate(new Vector3(0, 0, -360), 2.5f, RotateMode.FastBeyond360)
                 .SetLoops(-1, LoopType.Restart)
                 .SetEase(Ease.Linear);
        spin.Play();
    }

    void OnEnable()
    {
        if(spin!= null && !spin.IsPlaying()) spin.Play();
    }

    void OnClose()
    {
        if (spin != null && spin.IsPlaying()) spin.Pause();
        tip.SetActive(false);
    }

    public void SetTip(string message)
    {
        tip.SetActive(true);
        txtTipInfo.text = message;
    }
}