using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIGameGuide : UIBase
{
    [SerializeField] RectTransform scroll;
    [SerializeField] Button btnClose;

    protected override void Init()
    {
        SetGuide();
        OnCloseAction += (UIBase) => Time.timeScale = 1;
        OnCloseAction += (UIBase) => SoundManager.Instance.PlayBGM();
    }

    protected override void AddListener()
    {
        btnClose.onClick.AddListener(CloseUI);
    }
    public void SetGuide()
    {
        scroll.DOAnchorPosY(0, 0);
    }
}