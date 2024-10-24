using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;

public class MyPlayer : MonoBehaviour
{
    [SerializeField] private Image imgHP;
    [SerializeField] private RectTransform rectHP;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float dashDist = 1f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCoolDown = 10;
    [SerializeField] private GameObject objIsMine;
    [SerializeField] private GameObject objHpBar;
    [SerializeField] private Transform mysize;

    public Vector3 View { get; set; }
    Transform dirIndicator;
    public float directionOffset = 1.1f;
    Vector3 indicatorPos;
    public float DashCoolDown => dashCoolDown;
    private Vector2 moveDir = Vector2.zero;
    private Vector2 prevDir = Vector2.up;
    private float plSizeX = 0.5f;
    private bool isDashing = false;

    private LmlcInput input;

    private bool wasDiag = false;
    private bool canUpdateDir = false;
    private float diagCancelDelay = 0.05f;
    private DateTime diagCancelTime;
    bool isMine = true;
    bool myTeam = true;

    int maxHP = 180;
    int hp = 180;
    bool isLoaded;

    public bool OnDamage(int damage)
    {
        if (damage < 0) damage = 999;//maxHP;
        hp -= damage;
        if(hp < 0) hp = 0;
        SetHP(hp);
        var dt = Instantiate(dttt).GetComponent<DamageText>();
        dt.transform.position = transform.position+(Vector3)(Vector2.up * 1.5f);
        dt.transform.eulerAngles = View;
        dt.SetDamage(damage);
        return hp != 0;
    }
    private void OnValidate()
    {
        if (dashCoolDown < 0.1f) dashCoolDown = 0.1f;

        if (dashDuration > dashCoolDown)
        {
            dashDuration = dashCoolDown - 0.05f;
        }
    }
    #region Setup
    private async void Awake()
    {
        input = new();
        plSizeX = mysize.localScale.x;
        indicatorPos = new(7.49f, -1.84f);
        objIsMine.SetActive(isMine);
        objHpBar.SetActive(myTeam);
        SetHP(maxHP);
        if (isMine)
        {
            //dirIndicator = await ResourceLoadManager.Instance.Instantiate<Transform>("DashIndicator", position: indicatorPos);
        }

        dttt = await ResourceLoadManager.Instance.LoadAssetasync<GameObject>("DamageText");
        isLoaded = true;
    }

    private Tween hpTween;
    void SetHP(int value)
    {
        hp = value;

        if (!objHpBar.activeSelf) return;
        if (hpTween != null && hpTween.IsActive())
        {
            hpTween.Kill();
            //프로에선 삭제할 필요 없이 값을 바꿔서 사용할 수 있다고 함?
        }
        var size = hpSize.x * ((float)hp / maxHP);
        rectHP.DOSizeDelta(new(size, hpSize.y), 0.1f);
        hpTween = rectHP.DOSizeDelta(new(size, hpSize.y), 0.1f); //imgHP.DOFillAmount((float)hp / maxHP, 0.1f);
        hpTween.Play();
    }
    Vector2 hpSize = new(184, 24);
    GameObject dttt;
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

    #region InputAction
    bool CanUpdateDir(bool isNotDiag)
    {
        if (isNotDiag && wasDiag) // 대각 이동 취소 후
        {
            diagCancelTime = DateTime.Now;
            wasDiag = false;
            return false;
        }

        wasDiag = !isNotDiag;
        return true;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
        var isNotDiag = (moveDir.x == 0 || moveDir.y == 0);
        canUpdateDir = CanUpdateDir(isNotDiag);
        if (canUpdateDir) prevDir = moveDir;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context) => moveDir = Vector2.zero;
    #endregion

    #region Move
    private void FixedUpdate()
    {
        if (!isLoaded || !isMine) return;
        UpdateDirIndicator();

        if (isDashing || moveDir == Vector2.zero) return;

        if(!canUpdateDir) UpdateDiagCancel();
        transform.position += speed * Time.deltaTime * (Vector3)moveDir.normalized;
    }

    [SerializeField] private RectTransform dirIndicatorRect;
    [SerializeField] private RectTransform parentRectTransform;

    private void UpdateDirIndicator()
    {
        Vector2 offset = prevDir.normalized * (Vector2.one * 125);
        dirIndicatorRect.anchoredPosition = parentRectTransform.anchoredPosition + offset;
        float angle = Mathf.Atan2(prevDir.y, prevDir.x) * Mathf.Rad2Deg - 90f;
        dirIndicatorRect.localRotation = Quaternion.Euler(0, 0, angle);
    }

    private void UpdateDiagCancel()
    {
        var elapsed = (DateTime.Now - diagCancelTime).TotalSeconds;
        if (elapsed > diagCancelDelay)
        {
            canUpdateDir = true;
            prevDir = moveDir;
        }
    }
    #endregion

    public async void Dash()
    {
        isDashing = true;

        Vector3 dest = transform.position + dashDist * plSizeX * (Vector3)prevDir.normalized;
        await transform.DOMove(dest, dashDuration).SetEase(Ease.InOutQuad);

        isDashing = false;
    }
}
