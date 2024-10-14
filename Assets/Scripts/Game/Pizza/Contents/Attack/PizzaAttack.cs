using System;
using DG.Tweening;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public class PizzaAttack : MonoBehaviour
{
    public PizzaIngredient Type => type;
    [SerializeField] protected PizzaIngredient type;
    [SerializeField] protected bool previewType;
    public bool IsFirst { get; set; } = true;
    [SerializeField] private SpriteRenderer[] spr;
    protected Sequence sequence;
    protected float attackDelay = 6;
    protected Vector3 playerPos;
    protected bool isCompleted;
    //protected UIPizzaTimer ui;
    Action PushAction;
    protected CancellationToken token;

    public void SetPushAction(Action action) => PushAction = action;

    public PizzaAttack SetCancellationToken(CancellationToken token)
    {
        this.token = token;
        return this;
    }

    public async UniTask Play()
    {
        await SetSequence();
        OnComplete();
    }

    protected virtual async UniTask SetSequence()
    {
        if (previewType)
        {
            await SetShowPreview();
        }
        else
        {
            if (IsFirst)
            {
                IsFirst = false;
                await SetShowPreview();
            }
            else
            {
                await SetShowArea();
            }
        }
        OnStart();
    }

    async UniTask SetShowPreview()
    {
        float interval = 0.8f;

        SetTimer();
        await ShowArea(attackDelay - interval - (0.3f * 2), 0.3f, PizzaGameData.Instance.GetColor("#FF4B00"));
        await UniTask.Delay((int)(interval * 1000), cancellationToken: token);
        _ = ShowArea(0.1f, 0.1f, PizzaGameData.Instance.GetColor("#70FFEC"));
        Freeze();
    }

    async UniTask SetShowArea()
    {
        float show = 1.3f;
        int interval = 800;
        int attackDelay = (int)(this.attackDelay * 1000);

        SetTimer();
        await UniTask.Delay(attackDelay - (int)(show * 1000) - interval, cancellationToken: token);
        await ShowArea(show - (0.3f * 2), 0.3f, PizzaGameData.Instance.GetColor("#FF4B00"));
        await UniTask.Delay(interval, cancellationToken: token);
        _ = ShowArea(0.1f, 0.1f, PizzaGameData.Instance.GetColor("#70FFEC"));
        Freeze();
    }

    protected float stageDegree;

    void Freeze()
    {
        PizzaGameData data = PizzaGameData.Instance;
        var stage = data.Stage.position;
        var player = data.PlayerController.Pos;
        stageDegree = data.Stage.eulerAngles.z;
        float force = Vector2.Distance(stage, player);
        float degree = data.GetDegree(stage, player) - stageDegree;
        playerPos = data.GetAnglePos(force, degree);

        if (IsGameOver())
        {
            PizzaGameData.Instance.Player.GameOver();
        };
    }
    protected virtual void SetTimer()
    {
    }

    public virtual PizzaAttack Setup()
    {
        spr = GetComponentsInChildren<SpriteRenderer>();
        return this;
    }

    protected virtual void OnStart()
    {
        isCompleted = false;
        foreach (var sprite in spr)
        {
            sprite.DOKill();
            sprite.color = Color.white;
        }
    }

    protected virtual void OnComplete()
    {
        if (isCompleted) return;
        isCompleted = true;

        foreach (var sprite in spr)
        {
            sprite.DOFade(0.9f, 0.3f);
        }
        PushAction?.Invoke();
    }

    public async void Push()
    {
        float d = 0.4f;
        foreach (var sprite in spr)
        {
            _ = sprite.DOFade(0, d);
        }
        await UniTask.Delay((int)(d * 1000));
        gameObject.SetActive(false);
    }

    protected virtual async UniTask ShowArea(float duration, float fadeTime, Color color)
    {
        await UniTask.Delay(0, cancellationToken: token);
    }

    protected virtual bool IsGameOver()
    {
        return false;
    }

    protected void ResetSequence()
    {
    }

    public void ResetAttack()
    {
        OnStart();
    }
}
