using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameOver : UIBase
{
    [SerializeField] Button btnRetry;
    [SerializeField] Button btnExit;


    protected override void AddListener()
    {
        btnRetry.onClick.AddListener(Retry);
        btnExit.onClick.AddListener(Exit);
    }

    private void Retry()
    {
        OpenUI<UIPopUpButton>().SetMessage("다시하기");
    }

    private void Exit()
    {
        SceneLoadManager.Instance.LoadScene(SceneType.Intro);
    }
}