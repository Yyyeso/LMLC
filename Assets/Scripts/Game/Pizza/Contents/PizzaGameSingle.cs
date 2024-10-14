using Cysharp.Threading.Tasks;

public class PizzaGameSingle : IPizzaGameManager
{
    UIPizzaGameSingle uiGame;
    PizzaGameData data;


    public IPizzaGameManager IPizzaGameManager => this;

    public void LoadPlayer()
    {
        data = PizzaGameData.Instance;
        data.PlayerController = PizzaResources.Instance.Player;
        data.Player = data.PlayerController.GetComponent<IPizzaPlayerController>();
        PizzaGameManager.Instance.LoadGame();
    }

    public UIPizzaGameScene UIGame => UIManager.Instance.OpenUI<UIPizzaGameSingle>();

    public void SetGame()
    {
    }

    public void SetGameData()
    {
        data.CharacterIndex = UnityEngine.Random.Range(0, 10);
        data.Player.Setup(data.CharacterIndex);
        data.PlayerController.ResetPos();
    }
    public void ExitGame()
    {
        
    }

    public async UniTask OnReady()
    {
        await UniTask.Delay(100);
        PizzaGameManager.Instance.StartGame();
    }
    public void OnFailed()
    {
        PizzaGameManager.Instance.SetResult(false);
    }
    public void SetResult()
    {
        PizzaGameManager.Instance.SetResult(true);
    }
    public void StartGame()
    {
        PizzaGameManager.Instance.StartRound();
    }
}