using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIGameBase : UIBase
{
    [SerializeField] TMP_Text txtRound;
    [SerializeField] Slider silderProgress;

    [SerializeField] TMP_Text txtHP;
    [SerializeField] Slider silderHP;

    readonly string[] str = new string[3] { "1Round", "2Round", "3Round" };
    readonly float maxRound = 3;
    float roundValue;
    int maxHP = 50;


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
}