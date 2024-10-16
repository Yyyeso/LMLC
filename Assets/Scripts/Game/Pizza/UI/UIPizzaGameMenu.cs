using UnityEngine;
using UnityEngine.UI;

public class UIPizzaGameMenu : UIPizzaBase
{
    [SerializeField] private Button btnSingle;
    [SerializeField] private Button btnMulti;
    [SerializeField] private Button btnExit;
    PizzaNetworkManager networkManager;
    PizzaGameData data;

    protected override void Init()
    {
        networkManager = PizzaNetworkManager.Instance;
        data = PizzaGameData.Instance;
    }

    protected override void AddListener()
    {
        btnSingle.onClick.AddListener(StartSingle);
        btnMulti.onClick.AddListener(StartMulti);
        btnExit.onClick.AddListener(ExitGame);
    }

    void StartSingle()
    {
        data.OnLoading();
        data.IsMulti = false;
        CloseUI();
        PizzaGameSingle single = new();
        PizzaGameManager.Instance.SetGame(single.IPizzaGameManager);
    }

    void StartMulti()
    {
        data.OnLoading();
        data.IsMulti = true;
        CloseUI();
        networkManager.Connect(false);
    }

    void ExitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
