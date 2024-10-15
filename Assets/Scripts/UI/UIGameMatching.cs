using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameMatching : UIBase
{
    [SerializeField] Button btnCancel;
    [SerializeField] TMP_Text txtTitle;
    [SerializeField] TMP_Text txtCount;
    [SerializeField] RectTransform tr;

    private Tween spin;
    string solo = "개인전 참가 대기 중...";
    string team = "팀전 참가 대기 중...";
    int maxCount;

    protected override void Init()
    {
        OnCloseAction += (UIBase) => OnClose();
        spin = tr.DOJumpAnchorPos(Vector2.up * 360, 20, 1, 1)
                 .SetLoops(-1, LoopType.Restart)
                 .SetEase(Ease.InOutQuad);
        spin.Play();
    }

    void OnEnable()
    {
        if (spin != null && !spin.IsPlaying()) spin.Play();
    }

    void OnClose()
    {
        if (spin != null && spin.IsPlaying()) spin.Pause();
    }

    public UIGameMatching Setup(MultiMode type, int maxCount)
    {
        txtTitle.text = (type == MultiMode.Solo)? solo : team;
        this.maxCount = maxCount;
        return this;
    }

    protected override void AddListener()
    {
        btnCancel.onClick.AddListener(CancelMatching);
    }

    public void SetCount(int curCount)
    {
        txtCount.text = $"({curCount}/{maxCount})";
    }

    void CancelMatching()
    {
        var popup = OpenUI<UIPopUpButton>();
        popup
            .SetMessage("참가 신청을 취소하면\n60분간 참가 신청이 불가능합니다.\n그래도 취소하시겠습니까?")
            .AddConfirmAction(Cancel)
            .AddCancelAction(() => popup.SetButtonName("확인", "취소"))
            .SetButtonName("참가 취소", "참가 대기");
    }

    void Cancel()
    {
        NetworkManager.Instance.LeaveRoom();
        CloseUI();
    }
}