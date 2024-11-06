using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    IGameManager gm;
    UIGameBase ui;
    UIPizzaGameCount uiCount;
    GameData data;
    CancellationTokenSource cancel;
    CancellationTokenSource linked;


    #region Setup
    public async UniTask Setup(IGameManager gm)
    {
        this.gm = gm;
        data = GameData.Instance;
        SetCam();
        await LoadUI();
        await LoadPlayer();
        await LoadStage();
        ResetGameData();
    }

    void SetCam()
    {
        var cam = Camera.main;
        cam.transform.position = Vector3.back * 1.63f;
        cam.transform.localEulerAngles = Vector3.left * 60;
        cam.orthographicSize = 3;
        cam.nearClipPlane = -50;
        cam.farClipPlane = 50;
    }

    async UniTask LoadUI()
    {
        {
            var uiManager = UIManager.Instance;
            ui = (GameData.Instance.IsMulti) ? uiManager.OpenUI<UIGameMulti>() : uiManager.OpenUI<UIGameSingle>();
            PoolManager.Instance.CreatePool(await ResourceLoadManager.Instance.LoadAssetasync<GameObject>(Const.DamageText));
        }
    }

    async UniTask LoadPlayer()
    {
        {
            data.Player = await ResourceLoadManager.Instance.Instantiate<MyPlayer>("TestPlayer");
            data.Player.Setup(true, ui);
        }
    }

    async UniTask LoadStage()
    {
        {
            data.Stage = (await ResourceLoadManager.Instance.Instantiate<PizzaStage>("PizzaStage")).Stage;
        }
    }

    void ResetGameData()
    {
        data.Player.transform.position = Vector3.zero;
        data.Player.ResetGameData();
    }

    public async UniTask OnCompleteLoading()
    {
        await SceneLoadManager.Instance.OnCompleteLoading();
    }
    #endregion

    public void StartGame()
    {
    }

    public async void RestartGame()
    {
        await SceneLoadManager.Instance.OnStartLoading();
        ResetGameData();
        await SceneLoadManager.Instance.OnCompleteLoading();
        StartGame();
    }

    #region Result
    public void GameOver()
    {
    }

    void SetResult()
    {
    }
    #endregion
}