using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    IGameManager gm;
    UIGameBase ui;
    UIPizzaGameCount uiCount;

    CancellationTokenSource cancel;
    CancellationTokenSource linked;


    #region Setup
    public void Setup(IGameManager gm)
    {
        this.gm = gm;
        LoadPlayer();
    }

    public async void LoadGame()
    {
        LoadStage();
        LoadUI();
        ResetGameData();
        await SceneLoadManager.Instance.OnCompleteLoading();
    }

    void LoadPlayer()
    { 

    }

    void LoadStage()
    {

    }

    void LoadUI()
    {

    }

    void ResetGameData()
    {
        
    }
    #endregion

    public void StartGame()
    {
    }

    public async void RestartGame()
    {
        await SceneLoadManager.Instance.OnStartLoading();

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