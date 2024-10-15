using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameResult : UIBase
{
    [SerializeField] private Button btnConfirm;
    [SerializeField] private Image[] imgStars;
    [SerializeField] Color colorStarEmpty;
    [SerializeField] Color colorStarFilled;

    [SerializeField] TMP_Text txtTitle;
    [SerializeField] TMP_Text txtXP;
    [SerializeField] TMP_Text txtFC;

    [SerializeField] Transform effect1;
    [SerializeField] Transform effect2;
    [SerializeField] Transform effectSpin;

    private Tween spin;
    readonly string[] result = new string[3] { "Good", "Great", "Perfect" };


    protected override void Init()
    {
        OnCloseAction += (UIBase) => OnClose();
        effect1.localScale = Vector3.zero; 
        effect2.localScale = Vector3.zero;
        spin = effectSpin.DORotate(new Vector3(0, 0, -360), 4, RotateMode.FastBeyond360)
                      .SetLoops(-1, LoopType.Restart)
                      .SetEase(Ease.Linear);
        spin.Play();
    }

    protected override void AddListener()
    {
        btnConfirm.onClick.AddListener(Confirm);
    }

    private void Confirm()
    {
        SceneLoadManager.Instance.LoadScene(SceneType.Intro);
    }

    public void SetReward(int xp, int fc)
    {
        txtXP.text = $"+{xp} XP";
        txtFC.text = $"+{fc} FC";
    }

    public void SetStar(int idx)
    {
        if (idx > 3) idx = 3;
        idx -= 1;
        txtTitle.text = result[idx];
        for (int i = 0; i < imgStars.Length; i++)
        {
            imgStars[i].color = (i < idx) ? colorStarFilled : colorStarEmpty;
        }
    }

    async void SetEffect()
    {
        await UniTask.Delay(400);
        _ = effect1.DOScale(Vector3.one * 1.2f, 0.6f).OnComplete(() => effect1.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InSine));
        await UniTask.Delay(200);
        _ = effect2.DOScale(Vector3.one, 0.6f).OnComplete(() => effect2.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InSine));
    }

    void OnEnable()
    {
        if (spin != null && !spin.IsPlaying()) spin.Play();
        SetEffect();
    }

    void OnClose()
    {
        if (spin != null && spin.IsPlaying()) spin.Pause();
    }
}