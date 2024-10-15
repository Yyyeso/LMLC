using UnityEngine;
using UnityEngine.UI;

public class UIGameSingle : UIGameBase
{
    [SerializeField] private Button btnGuide;
    [SerializeField] private Button btnPause;


    protected override void Init()
    {
        OnCloseAction += (UIBase) => Time.timeScale = 0;
        OnCloseAction += (UIBase) => SoundManager.Instance.PauseBGM();
    }

    protected override void AddListener()
    {
        btnGuide.onClick.AddListener(() => OpenUI<UIGameGuide>());
        btnPause.onClick.AddListener(() => OpenUI<UIGamePause>());
    }
}