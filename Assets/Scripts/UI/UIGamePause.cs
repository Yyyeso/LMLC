using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class UIGamePause : UIBase
{
    [SerializeField] private Button btnContinue;
    [SerializeField] private Button btnReplay;
    [SerializeField] private Button btnExit;


    protected override void Init()
    {
        OnCloseAction += (UIBase) => Time.timeScale = 1;
    }

    protected override void AddListener()
    {
        btnContinue.onClick.AddListener(Continue);
        btnReplay.onClick.AddListener(Replay);
        btnExit.onClick.AddListener(Exit);
    }

    void Continue()
    {
        CloseUI();
        SoundManager.Instance.PlayBGM();
    }

    void Replay()
    {
        CloseUI();
        //RestartGame();
    }

    void Exit()
    {
        SceneLoadManager.Instance.LoadScene(SceneType.Intro);
    }
}