using UnityEngine;
using UnityEngine.UI;

public class UIPizzaGamePause : UIPizzaBase
{
    [SerializeField] private Button btnContinue;
    [SerializeField] private Button btnReplay;
    [SerializeField] private Button btnExit;


    protected override void AddListener()
    {
        btnContinue.onClick.AddListener(Continue);
        btnReplay.onClick.AddListener(Replay);
        btnExit.onClick.AddListener(Exit);
    }

    void Continue()
    {
        Time.timeScale = 1;
        SoundManager.Instance.PlayBGM();
        CloseUI();
    }

    void Replay()
    {
        Time.timeScale = 1;
        CloseUI();
        PizzaGameManager.Instance.RestartGame();
    }

    void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}