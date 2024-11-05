using System;
using DG.Tweening;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

public class MyPlayer : MonoBehaviour
{
    GameObject objDamageText;
    [SerializeField] bool isTestPlayer;
    #region Member
    [SerializeField] float speed = 1f;

    [SerializeField] float dashDist = 1f;
    [SerializeField] float dashDuration = 0.15f;
    [SerializeField] Ease  dashEase = Ease.InOutQuad;

    [SerializeField] Animator anim;
    [SerializeField] Transform trDir;
               const float dirSizeX = 0.4f;

    [SerializeField] GameObject objIsMine;
    [SerializeField] GameObject objHpBar;
    [SerializeField] RectTransform rectHP;
                     Tween hpTween;
                     Vector2 hpSize = new(184, 24);

    [SerializeField] float damageTextPos = 1.5f;
                     Vector3 view = new(-60, 0, 0);

    Vector2 moveDir = Vector2.zero;
    Vector2 dashDir = Vector2.up;
    Vector2 prevDir = Vector2.zero;

    bool isLoaded;
    bool isMine = true;
    bool isMyTeam = true;
    bool isDashing = false;
    bool wasDiag = false;
    bool canUpdateDashDir = false;

    DateTime    diagCancelTime;
    const float diagCancelDelay = 0.05f;

          int hp;
    const int maxHP = 180;

    CancellationTokenSource cancel;
    CancellationTokenSource linked;
    LmlcInput input;
    UIGameBase ui;
    #endregion


    private async void Awake()
    {
        if (isTestPlayer)
        {
            GameData.Instance.Player = this;
            var uiManager = UIManager.Instance;
            UIGameBase ui = (GameData.Instance.IsMulti) ? uiManager.OpenUI<UIGameMulti>() : uiManager.OpenUI<UIGameSingle>();
            await Setup(true, ui);
            ResetGameData();
        }
    }

    #region Setup
    public async UniTask Setup(bool isMine, UIGameBase ui)
    {
        isLoaded = false;
        input = new();
        this.isMine = isMine;
        objIsMine.SetActive(isMine);
        objHpBar.SetActive(isMyTeam);
        if (isMine)
        {
            isMyTeam = true;
        }
        this.ui = ui;
        objDamageText = await ResourceLoadManager.Instance.LoadAssetasync<GameObject>(Const.DamageText);
    }

    public void ResetGameData()
    {
        isLoaded = false;
        CreateCancelToken();
        SetHP(maxHP);
        moveDir = Vector2.zero;
        dashDir = Vector2.up;
        UpdateDashIndicator();
        isDashing = false;
        wasDiag = false;
        canUpdateDashDir = false;
        isLoaded = true;
    }

    private void OnEnable()
    {
        if (!isMine) return;
        input.Enable();
        input.PlayerAction.Move.performed += OnMove;
        input.PlayerAction.Move.canceled += OnMoveCanceled;
    }

    private void OnDisable()
    {
        if (!isMine) return;
        input.PlayerAction.Move.performed -= OnMove;
        input.PlayerAction.Move.canceled -= OnMoveCanceled;
        input.Disable();
    }
    #endregion

    #region SetHP
    public bool OnDamage(int damage)
    {
        if (damage < 0) damage = 999;//maxHP;
        hp -= damage;
        if (hp < 0) hp = 0;
        SetHP(hp);
        SetDamageText(damage);
        return hp != 0;
    }

    void SetHP(int value)
    {
        hp = value;
        if (!objHpBar.activeSelf) return;
        if (hpTween != null && hpTween.IsActive())
        {
            hpTween.Kill();
        }
        var size = hpSize.x * ((float)hp / maxHP);
        hpTween = rectHP.DOSizeDelta(new(size, hpSize.y), 0.1f); //imgHP.DOFillAmount((float)hp / maxHP, 0.1f);
        hpTween.Play();
    }

    void SetDamageText(int damage)
    {
        if(!isMine) return;
        var damageText = Instantiate(objDamageText).GetComponent<DamageText>();
        damageText.transform.position = transform.position + (Vector3.up * damageTextPos);
        damageText.transform.eulerAngles = view;
        damageText.SetDamage(damage);
    }
    #endregion

    #region InputAction
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveDir = Vector2.zero;
        SetRunAnim(false);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
        trDir.localScale = GetPlayerDir();
        SetRunAnim(true);
        var isDiag = !(moveDir.x == 0 || moveDir.y == 0);
        canUpdateDashDir = CanUpdateDashDir(isDiag);
        if (canUpdateDashDir) UpdateDashDir();
    }

    void UpdateDashDir() => dashDir = moveDir;

    void SetRunAnim(bool ismove)
    {
        if (anim.GetBool(Const.Run) == ismove) return;
        anim.SetBool(Const.Run, ismove);
    }

    Vector3 GetPlayerDir()
    {
        Vector3 dir = trDir.localScale;
        if      (moveDir.x > 0) dir.x =  dirSizeX;
        else if (moveDir.x < 0) dir.x = -dirSizeX;
        return dir;
    }

    bool CanUpdateDashDir(bool isDiag)
    {
        if (!isDiag && wasDiag) // 대각 이동 취소 후
        {
            diagCancelTime = DateTime.Now;
            wasDiag = false;
            return false;
        }

        wasDiag = isDiag;
        return true;
    }
    #endregion

    #region Move
    private void FixedUpdate()
    {
        if (!isLoaded || !isMine) return;
        if (prevDir != dashDir) UpdateDashIndicator();

        if (isDashing || moveDir == Vector2.zero) return;
        if(!canUpdateDashDir) UpdateDiagCancel();
        transform.position += speed * Time.deltaTime * (Vector3)moveDir.normalized;
    }

    private void UpdateDashIndicator()
    {
        prevDir = dashDir;
        float   angle  = Mathf.Atan2(dashDir.y, dashDir.x) * Mathf.Rad2Deg - 90f;
        ui.SetIndicator(dashDir.normalized, angle);
    }

    private void UpdateDiagCancel()
    {
        var elapsed = (DateTime.Now - diagCancelTime).TotalSeconds;
        if (elapsed > diagCancelDelay)
        {
            canUpdateDashDir = true;
            UpdateDashDir();
        }
    }
    #endregion

    #region Dash
    public async void Dash()
    {
        isDashing = true;
        anim.SetTrigger(Const.Dash_Start);
        await CharacterSkill.Dash(transform, dashDist, dashDir, dashDuration, dashEase, linked.Token); 
        // await CharacterSkill.Dash(transform, dashDist, dashDir, linked.Token);
        anim.SetTrigger(Const.Dash_End);
        isDashing = false;
    }

    public void CancleDash()
    {
        if (cancel != null)
        {
            cancel.Cancel();
            cancel.Dispose();
            linked.Dispose();
        }
        CreateCancelToken();
    }

    void CreateCancelToken()
    {
        cancel = new();
        linked = CancellationTokenSource.CreateLinkedTokenSource(cancel.Token, this.GetCancellationTokenOnDestroy());
    }
    #endregion
}