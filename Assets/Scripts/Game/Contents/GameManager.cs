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
    public async void Setup(IGameManager gm)
    {
        this.gm = gm;
        SetCam();
        LoadUI();
        await LoadPlayer();
        data = GameData.Instance;
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

    public async void LoadGame()
    {
        await LoadStage();
        ResetGameData();
        await SceneLoadManager.Instance.OnCompleteLoading();
    }

    async UniTask LoadPlayer()
    {
        {
            data.Player = await ResourceLoadManager.Instance.Instantiate<MyPlayer>("TestPlayer");
            await data.Player.Setup(true, ui);
            LoadGame();
        }
    }

    async UniTask LoadStage()
    {
        {
            data.Stage = (await ResourceLoadManager.Instance.Instantiate<PizzaStage>("PizzaStage")).Stage;
        }
    }

    void LoadUI()
    {
        {
            var uiManager = UIManager.Instance;
            ui = (GameData.Instance.IsMulti) ? uiManager.OpenUI<UIGameMulti>() : uiManager.OpenUI<UIGameSingle>();
        }
    }

    void ResetGameData()
    {
        data.Player.transform.position = Vector3.zero;
        data.Player.ResetGameData();
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