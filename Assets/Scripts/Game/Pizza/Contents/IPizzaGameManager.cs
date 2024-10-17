using Cysharp.Threading.Tasks;

public interface IPizzaGameManager
{
    public UIPizzaGameScene UIGame { get; }
    public void StartGame();
    public void SetGame();
    public void SetGameData();
    public void LoadPlayer();
    public void ExitGame();
    public void OnReady();
    public void OnFailed();
    public void SetResult();
}