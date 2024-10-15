using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : UIBase
{
    [SerializeField] private Button btnSingle;
    [SerializeField] private Button btnMulti;
    [SerializeField] private Button btnExit;
    [SerializeField] private Button btnSet;
    Animator anim;
    PizzaMenuBG bg;

    protected override void Init()
    {
        Setup();
    }

    protected override void AddListener()
    {
        btnSingle.onClick.AddListener(StartSingle);
        btnMulti.onClick.AddListener(StartMulti);
        btnExit.onClick.AddListener(ExitGame);
        btnSet.onClick.AddListener(OpenSet);
    }

    public void Setup()
    {
        //if (bg == null) bg = PizzaResources.Instance.BGMenu;

        //anim = bg.Chef;
        //anim.SetBool("Posing", true);
        //bg.gameObject.SetActive(true);
    }

    void StartSingle()
    {
        OpenUI<UISelectTheme>();
    }

    void StartMulti()
    {
        NetworkManager.Instance.Connect();
    }

    void OpenSet()
    {
        OpenUI<UIPopUpButton>().SetMessage("설정");
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