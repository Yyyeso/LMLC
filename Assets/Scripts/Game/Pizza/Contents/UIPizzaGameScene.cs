using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIPizzaGameScene : UIPizzaBase
{
    [SerializeField] private Slider progress;
    [SerializeField] private TMP_Text txtRound;
    readonly string[] str = new string[3] { "1Round", "2Round", "3Round" };
    readonly float maxRound = 3;
    float roundValue;

    public void SetProgress(float value)
    {
        float progressValue = roundValue + (value / 3);
        progress.DOValue(progressValue, 0.3f);
    }

    public void SetRound(int round)
    {
        txtRound.text = str[round];
        roundValue = 1 / maxRound * round;
        SetProgress(0);
    }
}