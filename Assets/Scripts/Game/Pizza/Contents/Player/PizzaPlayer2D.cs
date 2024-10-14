using DG.Tweening;
using UnityEngine;
using Photon.Pun;
using Cysharp.Threading.Tasks;

public class PizzaPlayer2D : MonoBehaviour, IPizzaPlayerController
{
    [SerializeField] private PizzaSetCharacter characterSetter;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D col;
    float force = 2; // 1.5f

    #region Key
    readonly string Horizontal = "Horizontal";
    readonly string Vertical = "Vertical";
    readonly string Run = "Run";
    readonly string StartFallen = "Fallen";
    readonly string EndFallen = "Fallen2";
    #endregion

    Vector3 dir;
    bool run;
    bool onCollision;
    bool isFreeze = true;
    Sequence fallenSequence;
    PizzaGameData data;

    public bool IsFreeze 
    {
        get => isFreeze;
        set
        {
            isFreeze = value;
            if (value)
            {
                run = false;
                animator.SetBool(Run, false);
            }
        }
    }

    public bool OnCollision 
    {
        get => onCollision;
        set
        {
            if (onCollision == value) return;

            onCollision = value;
            if (onCollision) StopFallen();
        }
    }


    [PunRPC]
    public void SetCharacter(int idx)
    {
        characterSetter.SetCharacter(idx);
        if (PizzaGameData.Instance.IsMulti) col.enabled = (PV.IsMine);
    }

    public IPizzaPlayerController Setup(int idx)
    {
        if (data == null) data = PizzaGameData.Instance;
        if (data.IsMulti) PV.RPC(nameof(SetCharacter), RpcTarget.All, idx);
        else SetCharacter(idx);
        dir = Vector3.one;
        IsFreeze = true;
        onCollision = false;
        data.IsOutside = false;
        return this;
    }

    [PunRPC] public void PlayRun() => animator.SetBool(Run, true);
    [PunRPC] public void StopPlayRun() => animator.SetBool(Run, false);
    public Vector3 Move(float speed)
    {
        var data = PizzaGameData.Instance;
        float h = (data.IsMobile) ? UIManager.Instance.OpenUI<UIPizzaJoystick>().PizzaJoystick.Horizontal : Input.GetAxis(Horizontal);
        float v = (data.IsMobile) ? UIManager.Instance.OpenUI<UIPizzaJoystick>().PizzaJoystick.Vertical : Input.GetAxis(Vertical);

        bool run = (h != 0 || v != 0);
        if (this.run != run)
        {
            this.run = run;
            if (run)
            {
                if (data.IsMulti) PV.RPC(nameof(PlayRun), RpcTarget.All);
                else PlayRun();
            }
            else
            {
                if (data.IsMulti) PV.RPC(nameof(StopPlayRun), RpcTarget.All);
                else StopPlayRun();
            }
            //animator.SetBool(Run, run);
        }

        int d = (int)transform.localScale.x;
        if (h > 0) d = 1;
        else if (h < 0) d = -1;
        if (data.IsMulti) PV.RPC(nameof(SetDir), RpcTarget.All, d);
        else SetDir(d);

        var moveDirection = speed * Time.deltaTime * new Vector3(h, v, 0).normalized;
        moveDirection += transform.position;

        return moveDirection;
    }
    PhotonView pv;
    PhotonView PV => pv = (pv != null) ? pv : GetComponent<PhotonView>();
    [PunRPC] public void PlayFallen() => animator.SetTrigger(StartFallen);
    [PunRPC] public void StopPlayFallen() => animator.SetTrigger(EndFallen);
    [PunRPC] public void SetDir(int value)
    {
        dir.x = value;
        transform.localScale = new(value, 1, 1);
    }

    public void Fallen()
    {
        if (isOutside) { return; }
        if (data.GameOver || IsFreeze || OnCollision) return;

        StopFallen();

        Vector3 dest = Destination(force, Distance);
        void OnStart()
        {
            IsFreeze = true;
            int d = (int)transform.localScale.x;
            if (dest.x > 0) d = -1;
            else if (dest.x < 0) d = 1;
            if (data.IsMulti)
            {
                PV.RPC(nameof(SetDir), RpcTarget.All, d);
                PV.RPC(nameof(PlayFallen), RpcTarget.All);
            }
            else
            {
                SetDir(d);
                PlayFallen();
            }
                
            //animator.SetTrigger(StartFallen); 
            _ = PizzaGameData.Instance.PlaySFX(PizzaSFXType.Corn);
        }
        void OnComplete()
        {
            if (data.IsMulti) PV.RPC(nameof(StopPlayFallen), RpcTarget.All);
            else StopPlayFallen();
            //animator.SetTrigger(EndFallen);
            if (Vector2.Distance(data.Stage.position, transform.position) >= 3.05f) GameOver();
            IsFreeze = false;
        }

        fallenSequence = DOTween.Sequence()
            .OnStart(() => { OnStart(); })
            .Append(transform.DOMove(dest, 0.8f).SetEase(Ease.InOutQuart))
            .OnComplete(() => { OnComplete(); })
            .OnKill(() => { OnComplete(); })
            .Play();
    }

    void StopFallen()
    {
        if (fallenSequence != null && fallenSequence.IsActive() && !fallenSequence.IsComplete())
        {
            fallenSequence.Kill();
            fallenSequence = null;
        }
    }
    bool isOutside = false;
    public async void GameOver()
    {
        if(isOutside) { return; }
        if (data.GameOver && data.IsOutside) return;
        if (Vector2.Distance(data.Stage.position, transform.position) <= 3f)
        {
            StopFallen();
            Vector3 dest = Destination(3.05f);
            void OnStart()
            {
                IsFreeze = true;
                int d = (int)transform.localScale.x;
                if (dest.x > 0) d = -1;
                else if (dest.x < 0) d = 1;
                if (data.IsMulti)
                {
                    PV.RPC(nameof(SetDir), RpcTarget.All, d);
                    PV.RPC(nameof(PlayFallen), RpcTarget.All);
                }
                else
                {
                    SetDir(d);
                    PlayFallen();
                }
                    
                _ = PizzaGameData.Instance.PlaySFX(PizzaSFXType.Corn);
            }
            void OnComplete()
            {
                if (data.IsMulti) PV.RPC(nameof(StopPlayFallen), RpcTarget.All);
                else StopPlayFallen();
                IsFreeze = false;
            }
            await transform
                .DOMove(dest, 0.45f).SetEase(Ease.InOutQuart)
                .OnStart(OnStart)
                .OnComplete(OnComplete);
        }
        PizzaGameManager.Instance.SetOutside();
        col.transform.localScale = Vector3.one;
        data.IsOutside = isOutside = true;
        PizzaGameManager.Instance.GameOver();
    }

    private Vector3 Destination(float force, float dist = 0) => data.GetAnglePos(force + dist, Angle) + data.Stage.position;

    private float Angle => data.GetDegree(data.Stage.position, transform.position);

    private float Distance => Vector2.Distance(data.Stage.position, transform.position);
}