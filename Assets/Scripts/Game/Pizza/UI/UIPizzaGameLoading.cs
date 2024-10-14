using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class UIPizzaGameLoading : UIPizzaBase
{
    [SerializeField] private TMP_Text txtLoading;
    [SerializeField] private RectTransform[] ingredientGroup1;
    [SerializeField] private RectTransform[] ingredientGroup2;

    readonly string[] strs = new string[4] { "Loading", "Loading.","Loading..","Loading..." };
    WaitForSecondsRealtime wait;
    IEnumerator enumerator;
    Sequence sequence;
    int moveIdx = 0;

    protected override void Init()
    {
        base.Init();
        wait = new(0.5f);
        enumerator = LoadingCoroutine();
    }

    public void Setup()
    {
        StartCoroutine(enumerator);
        LoadingSequence().Play();
    }

    public void Stop()
    {
        StopCoroutine(enumerator); 
        if (sequence != null && sequence.IsActive() && !sequence.IsComplete())
        {
            sequence.Kill();
            sequence = null;
        }
    }

    IEnumerator LoadingCoroutine()
    {
        int idx = 0;

        while (true)
        {
            txtLoading.text = strs[idx];
            idx = (idx + 1) % strs.Length;
            yield return wait;
        }
    }

    Sequence LoadingSequence()
    {
        if (sequence != null && sequence.IsActive() && !sequence.IsComplete())
        {
            sequence.Kill();
            sequence = null;
        }

        float duration = 1.5f;

        sequence = DOTween.Sequence().AppendInterval(duration);

        for (int i = 0; i < ingredientGroup1.Length; i++)
        {
            int idx = i;
            float posX1 = ingredientGroup1[idx].anchoredPosition.x;
            float posX2 = ingredientGroup2[idx].anchoredPosition.x;
            sequence.Join(ingredientGroup1[idx].DOAnchorPosX(posX1 - 215, duration)).SetEase(Ease.OutQuart);
            sequence.Join(ingredientGroup2[idx].DOAnchorPosX(posX2 + 215, duration)).SetEase(Ease.OutQuart);
        }

        sequence
            .AppendInterval(0.5f)
            .OnComplete(SetMoveIdx);

        return sequence;
    }
    void SetMoveIdx()
    {
        ingredientGroup1[moveIdx].anchoredPosition = Vector3.right * 1072;
        ingredientGroup2[9 - moveIdx].anchoredPosition = Vector3.left * 1078;
        moveIdx = (moveIdx + 1) % ingredientGroup1.Length;
        LoadingSequence().Play();
    }
}