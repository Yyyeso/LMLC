using UnityEngine;
using UnityEngine.UI;

public class UIPizzaGameGuide : UIPizzaBase
{
    [SerializeField] private Button btnClose;
    [SerializeField] private RectTransform parent;

    void Disable()
    {
        Time.timeScale = 1;
        SoundManager.Instance.PlayBGM();
    }

    protected override void AddListener()
    {
        btnClose.onClick.AddListener(Close);
    }

    void Close()
    {
        Disable();
        CloseUI();
    }

    public void SetGuide()
    {
        
    }
}