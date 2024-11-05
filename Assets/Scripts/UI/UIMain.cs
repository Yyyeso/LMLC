using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : UIBase
{
    [SerializeField] private Button btnSingle;
    [SerializeField] private Button btnMulti;
    [SerializeField] private Button btnExit;
    [SerializeField] private Button btnSet;


    protected override void AddListener()
    {
        btnSingle.onClick.AddListener(StartSingle);
        btnMulti.onClick.AddListener(StartMulti);
        btnExit.onClick.AddListener(ExitGame);
        btnSet.onClick.AddListener(OpenSet);
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