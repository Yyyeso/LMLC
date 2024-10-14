using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPizzaGameResult : UIPizzaBase
{
    [SerializeField] private Button btnReplay;
    [SerializeField] private Button btnExit;
    [SerializeField] private TMP_Text txtResult;
    PizzaResultBG bg;


    protected override void AddListener()
    {
        btnReplay.onClick.AddListener(Replay);
        btnExit.onClick.AddListener(ExitGame);
    }

    void Replay()
    {
        SoundManager.Instance.PauseBGM();
        bg.gameObject.SetActive(false);
        CloseUI();
        PizzaGameManager.Instance.RestartGame();
    }

    void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    readonly string Clear = "Clear!";
    readonly string Fail = "Fail...";
    readonly string Win = "Win!";
    readonly string Lose = "Lose...";
    readonly string Tie = "Tie";

    public void SetResult(bool isClear)
    {
        if(bg == null) bg = PizzaResources.Instance.BGResult;
        bg.gameObject.SetActive(true);
        bg.SetAnim(isClear);
        if (PizzaGameData.Instance.IsMulti)
        {
            txtResult.text = isClear ? Win : Lose;
            btnReplay.gameObject.SetActive(false);
        }
        else
        {
            txtResult.text = isClear ? Clear : Fail;
            btnReplay.gameObject.SetActive(true);
        }
    }
    public void SetResult()
    {
        if (bg == null) bg = PizzaResources.Instance.BGResult;
        bg.gameObject.SetActive(true);
        bg.SetAnim();
        txtResult.text = Tie;
        btnReplay.gameObject.SetActive(false);
    }
}