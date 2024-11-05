using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class UIGameBase : UIBase
{
    [SerializeField] TMP_Text txtRound;
    [SerializeField] Slider silderProgress;

    [SerializeField] TMP_Text txtHP;
    [SerializeField] Slider silderHP;

    [SerializeField] GameObject objDashIndicator;
    [SerializeField] RectTransform indicatorRect;
    [SerializeField] RectTransform parentRect;
    [SerializeField] Button btnDash;
    [SerializeField] Image imgDash;
    [SerializeField] float indicatorOffset = 125;

    readonly string[] str = new string[3] { "1Round", "2Round", "3Round" };
    readonly float maxRound = 3;
    float roundValue;
    int maxHP = 50;

    protected override void AddListener()
    {
        base.AddListener();
        btnDash.onClick.AddListener(Dash);
    }

    async void Dash()
    {
        btnDash.interactable = false;
        GameData.Instance.Player.Dash();
        imgDash.fillAmount = 1;
        await imgDash.DOFillAmount(0, CharacterSkill.dashCoolDown);
        btnDash.interactable = true;
    }

    public void Setup(int maxHP)
    {
        SetRound(0);
        this.maxHP = maxHP;
    }

    public void SetProgress(float value)
    {
        float progressValue = roundValue + (value / 3);
        silderProgress.DOValue(progressValue, 0.3f);
    }

    public void SetRound(int round)
    {
        txtRound.text = str[round];
        roundValue = 1 / maxRound * round;
        SetProgress(0);
    }

    public void SetHP(int hp)
    {
        txtHP.text = $"{hp}/{maxHP}";
        silderHP.value = (float)hp / maxHP;
    }

    public void SetIndicator(Vector2 offset, float angle)
    {
        indicatorRect.anchoredPosition = parentRect.anchoredPosition + offset * Vector2.one * indicatorOffset;
        indicatorRect.localRotation = Quaternion.Euler(0, 0, angle);
    }
}