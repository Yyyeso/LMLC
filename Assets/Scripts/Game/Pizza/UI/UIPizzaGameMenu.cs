using UnityEngine;
using UnityEngine.UI;

public class UIPizzaGameMenu : UIPizzaBase
{
    [SerializeField] private Button btnSingle;
    [SerializeField] private Button btnMulti;
    [SerializeField] private Button btnExit;
    Animator anim;
    NetworkManager networkManager;
    PizzaGameData data;
    PizzaMenuBG bg;

    protected override void Init()
    {
        networkManager = NetworkManager.Instance;
        data = PizzaGameData.Instance;
    }

    protected override void AddListener()
    {
        btnSingle.onClick.AddListener(StartSingle);
        btnMulti.onClick.AddListener(StartMulti);
        btnExit.onClick.AddListener(ExitGame);
    }

    public UIPizzaGameMenu Setup()
    {
        if (bg == null) bg = PizzaResources.Instance.BGMenu;

        anim = bg.Chef;
        anim.SetBool("Posing", true);
        bg.gameObject.SetActive(true);
        return this;
    }

    void StartSingle()
    {
        data.OnLoading();
        data.IsMulti = false;
        anim.SetBool("Posing", false);
        bg.gameObject.SetActive(false);
        CloseUI();
        PizzaGameSingle single = new();
        PizzaGameManager.Instance.SetGame(single.IPizzaGameManager);
    }

    void StartMulti()
    {
        data.OnLoading();
        data.IsMulti = true;
        anim.SetBool("Posing", false);
        bg.gameObject.SetActive(false);
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
