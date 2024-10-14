using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPizzaGameSingle : UIPizzaGameScene
{
    [SerializeField] private Button btnGuide;
    [SerializeField] private Button btnPause;


    protected override void AddListener()
    {
        btnGuide.onClick.AddListener(OpenGuide);
        btnPause.onClick.AddListener(PauseGame);
    }

    void OpenGuide()
    {
        StopGame(); 
        OpenUI<UIPizzaGameGuide>();
    }

    void PauseGame()
    {
        StopGame(); 
        OpenUI<UIPizzaGamePause>();
    }

    void StopGame()
    {
        Time.timeScale = 0;
        SoundManager.Instance.PauseBGM();
    }
}